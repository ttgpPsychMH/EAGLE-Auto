#pragma once
#include <cstdint>
#include <deque>
#include <functional>
#include <mutex>
#include <vector>

namespace eagle_sim {
using Bytes = std::vector<std::uint8_t>;
enum class State { idle, installed, install_rejected, cleanup_rejected, stopped };
enum class Delivery { delivered, timeout, failed };

// Portable behavioral prototype. No sockets, Windows APIs, process memory or executable loading.
class Reader {
    mutable std::mutex mutex_;
    std::mutex dispatcher_;
    std::deque<Bytes> queue_;
    State state_ = State::idle;
    std::uint32_t pid_, sequence_ = 0;
    std::size_t bytes_ = 0, dropped_ = 0, delivered_ = 0, timed_out_ = 0;
public:
    static constexpr std::size_t header_size = 24, max_payload = 65536;
    static constexpr std::size_t max_frames = 8, max_queued_bytes = 131072;
    static constexpr unsigned delivery_budget_ms = 100;
    explicit Reader(std::uint32_t pid) : pid_(pid) {}
    bool start(const std::function<bool()>& fake_install) {
        std::lock_guard<std::mutex> lock(mutex_);
        if (state_ == State::installed) return true;
        if (state_ != State::idle || pid_ == 0) return false;
        state_ = fake_install() ? State::installed : State::install_rejected;
        return state_ == State::installed;
    }
    bool stop(const std::function<bool()>& fake_remove) {
        std::lock_guard<std::mutex> dispatcher(dispatcher_);
        std::lock_guard<std::mutex> lock(mutex_);
        if (state_ == State::stopped) return true;
        if (state_ != State::installed && state_ != State::cleanup_rejected) return false;
        if (!fake_remove()) { state_ = State::cleanup_rejected; return false; }
        dropped_ += queue_.size(); queue_.clear(); bytes_ = 0; state_ = State::stopped;
        return true;
    }
    // Return the original recv result untouched, including EOF and SOCKET_ERROR.
    int receive(const Bytes& original, int result) {
        std::lock_guard<std::mutex> lock(mutex_);
        if (result <= 0) return result;
        ++sequence_; // A dropped chunk leaves a detectable gap in subsequent envelopes.
        if (state_ != State::installed || static_cast<std::size_t>(result) > original.size()
            || static_cast<std::size_t>(result) > max_payload || queue_.size() >= max_frames
            || bytes_ + header_size + static_cast<std::size_t>(result) > max_queued_bytes) {
            ++dropped_; return result;
        }
        Bytes frame{'E', 'G', 'R', 'D', 1, 0, 24, 0};
        auto put32 = [&frame](std::uint32_t n) {
            for (unsigned i = 0; i < 4; ++i) frame.push_back(static_cast<std::uint8_t>(n >> (8 * i)));
        };
        put32(pid_); put32(sequence_); put32(static_cast<std::uint32_t>(result)); put32(0);
        frame.insert(frame.end(), original.begin(), original.begin() + result);
        bytes_ += frame.size(); queue_.push_back(std::move(frame));
        return result;
    }
    bool drain_one(const std::function<Delivery(const Bytes&, unsigned)>& fake_delivery) {
        // Model a dedicated dispatcher. The producer never calls or waits for the UI.
        // Serialize lifecycle with delivery, but release the producer lock before calling the sink.
        // Fake delivery must honor its budget; an actual Windows implementation is still required.
        std::lock_guard<std::mutex> dispatcher(dispatcher_);
        Bytes frame;
        {
            std::lock_guard<std::mutex> lock(mutex_);
            if (state_ != State::installed || queue_.empty()) return false;
            frame = std::move(queue_.front()); queue_.pop_front(); bytes_ -= frame.size();
        }
        Delivery outcome = fake_delivery(frame, delivery_budget_ms);
        std::lock_guard<std::mutex> lock(mutex_);
        if (outcome == Delivery::delivered) ++delivered_;
        else { ++dropped_; if (outcome == Delivery::timeout) ++timed_out_; }
        return true;
    }
    State state() const { std::lock_guard<std::mutex> lock(mutex_); return state_; }
    std::size_t dropped() const { std::lock_guard<std::mutex> lock(mutex_); return dropped_; }
    std::size_t delivered() const { std::lock_guard<std::mutex> lock(mutex_); return delivered_; }
    std::size_t timed_out() const { std::lock_guard<std::mutex> lock(mutex_); return timed_out_; }
    std::size_t queued_bytes() const { std::lock_guard<std::mutex> lock(mutex_); return bytes_; }
};

inline Bytes legacy_copy(std::uint32_t pid, const Bytes& data, int result, bool receiver) {
    if (!receiver || result <= 15 || result >= 1000 || static_cast<std::size_t>(result) > data.size()) return {};
    Bytes forwarded;
    for (unsigned i = 0; i < 4; ++i) forwarded.push_back(static_cast<std::uint8_t>(pid >> (8 * i)));
    // Native 107 copies n bytes into buffer+4 but sets COPYDATA.cbData=n.
    forwarded.insert(forwarded.end(), data.begin(), data.begin() + result - 4);
    return forwarded;
}
} // namespace eagle_sim
