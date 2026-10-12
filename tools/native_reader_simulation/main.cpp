#include "reader_model.hpp"
#include <algorithm>
#include <iomanip>
#include <iostream>
#include <sstream>
#include <stdexcept>
#include <string>
#include <thread>
using namespace eagle_sim;

static void require(bool ok, const char* message) { if (!ok) throw std::runtime_error(message); }
static std::string hex(const Bytes& b) {
    std::ostringstream s;
    for (auto n : b) s << std::hex << std::setw(2) << std::setfill('0') << static_cast<unsigned>(n);
    return s.str();
}
static Bytes pattern(std::size_t size) {
    Bytes b(size); for (std::size_t i = 0; i < size; ++i) b[i] = static_cast<std::uint8_t>(i * 17 + 3);
    return b;
}
struct Fixture { std::string id; Bytes original, legacy, envelope; };
static std::vector<Fixture> fixtures;
static std::vector<std::string> passed;
template<class F> static void test(const std::string& name, F f) {
    f(); passed.push_back(name);
}
int main() {
    try {
        for (int length : {1, 15, 16, 20, 999, 1000, 4096, 65536}) {
            test("recv_length_" + std::to_string(length), [length] {
                Bytes original = pattern(length), source = original, sent;
                Reader r(4242); require(r.start([]{return true;}), "start");
                require(r.receive(source, length) == length, "recv return changed");
                source.assign(source.size(), 0); // Queue must own a snapshot, not the recv buffer.
                require(r.drain_one([&](const Bytes& frame, unsigned budget) {
                    require(budget == 100, "budget"); sent = frame; return Delivery::delivered;
                }), "no delivery");
                require(sent.size() == Reader::header_size + original.size(), "length loss");
                require(std::equal(original.begin(), original.end(), sent.begin() + Reader::header_size), "payload altered");
                Bytes legacy = legacy_copy(4242, original, length, true);
                bool eligible = length > 15 && length < 1000;
                require(legacy.size() == (eligible ? static_cast<std::size_t>(length) : 0), "legacy gate");
                if (eligible) require(std::equal(original.begin(), original.end() - 4, legacy.begin() + 4), "legacy tail");
                fixtures.push_back({"length_" + std::to_string(length), original, legacy, sent});
            });
        }
        test("legacy_twenty_bytes_loses_four", [] {
            Bytes data{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t'};
            auto b=legacy_copy(4242,data,20,true); require(b.size()==20 && b.back()=='p', "legacy truncation");
        });
        test("legacy_receiver_missing", []{require(legacy_copy(4242,pattern(20),20,false).empty(),"receiver");});
        test("eof_and_socket_error_untouched", [] {
            Reader r(4242); r.start([]{return true;});
            require(r.receive({},0)==0 && r.receive({},-1)==-1 && r.queued_bytes()==0,"EOF/error");
        });
        test("partial_recv_uses_result_not_buffer_capacity", [] {
            Reader r(4242); r.start([]{return true;}); r.receive(pattern(80),16);
            require(r.queued_bytes()==40,"capacity copied");
        });
        test("invalid_result_dropped_without_changing_return", [] {
            Reader r(4242); r.start([]{return true;});
            require(r.receive(pattern(10),20)==20 && r.dropped()==1 && r.queued_bytes()==0,"invalid result");
        });
        test("oversize_chunk_is_explicit_drop", [] {
            Reader r(4242); r.start([]{return true;});
            require(r.receive(pattern(65537),65537)==65537 && r.dropped()==1,"oversize");
        });
        test("installation_failure_has_no_data_delivery", [] {
            Reader r(4242); int calls=0; require(!r.start([&]{++calls;return false;}),"failed install accepted");
            require(!r.start([&]{++calls;return true;}) && calls==1,"implicit retry");
            r.receive(pattern(20),20); require(r.state()==State::install_rejected && r.dropped()==1,"failed state");
        });
        test("repeated_start_does_not_reinstall", [] {
            Reader r(4242);int calls=0; auto install=[&]{++calls;return true;};
            require(r.start(install) && r.start(install) && calls==1,"duplicate install");
        });
        test("invalid_pid_does_not_install", [] {
            Reader r(0);int calls=0; require(!r.start([&]{++calls;return true;}) && calls==0,"zero PID");
        });
        test("frame_queue_limit_and_gap", [] {
            Reader r(4242);r.start([]{return true;});
            for(int i=0;i<9;++i) r.receive(pattern(20),20);
            require(r.dropped()==1 && r.queued_bytes()==8*44,"frame bound");
            r.drain_one([](const Bytes&,unsigned){return Delivery::delivered;});
            r.receive(pattern(20),20);std::uint32_t last=0;
            while(r.drain_one([&](const Bytes& b,unsigned){last=b[12]|b[13]<<8|b[14]<<16|b[15]<<24;return Delivery::delivered;})){}
            require(last==10,"loss not visible in sequence");
        });
        test("queued_byte_budget_is_bounded", [] {
            Reader r(4242);r.start([]{return true;});r.receive(pattern(65536),65536);r.receive(pattern(65536),65536);
            require(r.dropped()==1 && r.queued_bytes()==65560,"byte bound");
        });
        test("delivery_timeout_drops_once_without_reinstall", [] {
            Reader r(4242);int calls=0;r.start([&]{++calls;return true;});r.receive(pattern(20),20);
            require(r.drain_one([](const Bytes&,unsigned b){require(b==100,"timeout budget");return Delivery::timeout;}),"timeout");
            require(r.timed_out()==1 && r.dropped()==1 && calls==1 && r.state()==State::installed,"timeout state");
            require(!r.drain_one([](const Bytes&,unsigned){return Delivery::delivered;}),"automatic retry");
        });
        test("delivery_failure_does_not_change_game_result", [] {
            Reader r(4242);r.start([]{return true;});require(r.receive(pattern(20),20)==20,"result");
            r.drain_one([](const Bytes&,unsigned){return Delivery::failed;});require(r.dropped()==1 && r.timed_out()==0,"failure");
        });
        test("producer_lock_is_released_during_delivery", [] {
            Reader r(4242);r.start([]{return true;});r.receive(pattern(20),20);
            r.drain_one([&](const Bytes&,unsigned){require(r.receive(pattern(30),30)==30,"producer");return Delivery::delivered;});
            require(r.queued_bytes()==54,"reentrant producer snapshot");
        });
        test("concurrent_producers_own_buffers_and_respect_bound", [] {
            Reader r(4242);r.start([]{return true;});std::vector<std::thread> threads;
            for(int i=0;i<64;++i) threads.emplace_back([&r,i]{Bytes b(20,static_cast<std::uint8_t>(i));r.receive(b,20);b.assign(20,255);});
            for(auto& t:threads) t.join();
            require(r.dropped()==56 && r.queued_bytes()==8*44,"concurrent bound");
            std::vector<unsigned> identities;
            while(r.drain_one([&](const Bytes& b,unsigned){auto id=b[24];require(id<64 && std::all_of(b.begin()+24,b.end(),[id](auto n){return n==id;}),"mixed buffer");identities.push_back(id);return Delivery::delivered;})){}
            std::sort(identities.begin(),identities.end());require(std::adjacent_find(identities.begin(),identities.end())==identities.end(),"duplicate ownership");
        });
        test("cleanup_failure_blocks_reinstall_and_delivery", [] {
            Reader r(4242);r.start([]{return true;});r.receive(pattern(20),20);
            require(!r.stop([]{return false;}) && r.state()==State::cleanup_rejected,"cleanup failure");
            require(!r.start([]{return true;}) && !r.drain_one([](const Bytes&,unsigned){return Delivery::delivered;}),"unsafe restart");
            require(r.stop([]{return true;}) && r.state()==State::stopped && r.queued_bytes()==0,"cleanup retry");
        });
        test("stop_flushes_queue_and_is_idempotent", [] {
            Reader r(4242);r.start([]{return true;});r.receive(pattern(20),20);int calls=0;
            auto remove=[&]{++calls;return true;};require(r.stop(remove) && r.stop(remove) && calls==1,"double cleanup");
            require(r.dropped()==1 && !r.drain_one([](const Bytes&,unsigned){return Delivery::delivered;}),"stale delivery");
            require(r.receive(pattern(20),20)==20 && r.dropped()==2,"stopped result");
        });
        test("fragmented_utf8_preserved_as_raw_chunks", [] {
            Bytes text{0x41,0xe1,0xba,0xa3,0x42};Reader r(4242);r.start([]{return true;});
            Bytes merged;int index=0;
            for(const auto& b : {Bytes{0x41,0xe1},Bytes{0xba},Bytes{0xa3,0x42}}) {
                r.receive(b,static_cast<int>(b.size()));
                r.drain_one([&](const Bytes& frame,unsigned){merged.insert(merged.end(),frame.begin()+24,frame.end());fixtures.push_back({"utf8_fragment_"+std::to_string(index++),b,{},frame});return Delivery::delivered;});
            }
            require(merged==text,"fragment bytes lost"); // No claim about TLBB packet framing or codec.
        });
        std::cout << "{\"executed_untrusted_binaries\":false,\"windows_hook_tested\":false,\"passed\":" << passed.size() << ",\"failed\":0,\"tests\":[";
        for(std::size_t i=0;i<passed.size();++i) std::cout << (i?",":"") << '"' << passed[i] << '"';
        std::cout << "],\"fixtures\":[";
        for(std::size_t i=0;i<fixtures.size();++i) {const auto& f=fixtures[i];std::cout << (i?",":"") << "{\"id\":\"" << f.id << "\",\"pid\":4242,\"payload_hex\":\"" << hex(f.original) << "\",\"legacy_hex\":\"" << hex(f.legacy) << "\",\"envelope_hex\":\"" << hex(f.envelope) << "\"}";}
        std::cout << "]}\n";
    } catch(const std::exception& e) {std::cerr << "Simulation failed: " << e.what() << '\n';return 1;}
}
