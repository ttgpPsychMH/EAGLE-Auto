# EAGLE Auto v0.7 — chẩn đoán hook và mô phỏng native reader

Ngày: 12-10-2026. Nhánh `repair/eagle-auto-hook-diagnostics`, kế thừa v0.6 và [phân tích EasyHook bằng REA](../analysis/easyhook-107-rea/REPORT.vi.md). Đây là bước chuẩn bị trước khi sửa DLL. **EasyHook.dll gốc chưa thay đổi; chưa kiểm thử Windows 11 hoặc trong game.**

## Chẩn đoán C# đã tích hợp

Chọn một cửa sổ game trong danh sách rồi mở **Tiện Ích → Chẩn đoán hook**. Hộp thoại hiển thị PID, trạng thái, bộ đếm và tối đa 32 quan sát gần nhất theo UTC. Có thể nhấn Ctrl+C trong hộp thoại Windows để sao chép báo cáo. Khi chưa chọn game, menu chỉ nhắc chọn cửa sổ; không khởi chạy game, không bật automation hoặc tự cài reader.

Báo cáo giữ trong RAM của từng đối tượng Game, không tự ghi file. Chỉ ghi tên sự kiện cho phép sẵn, số byte, mã lỗi Win32/HResult và thời gian. Không lưu payload, chat, tên/tài khoản nhân vật, mật khẩu, đường dẫn hay nội dung exception. Quan sát trùng liên tiếp không tăng lịch sử; bộ đếm vẫn tăng đầy đủ. Khóa bảo vệ trạng thái khi thread Auto và WndProc cùng cập nhật.

| Quan sát | Điều đã biết | Điều chưa được xác nhận |
|---|---|---|
| Window hook accepted | `SetHook(HWND)` trả giá trị khác 0 | Callback hoạt động đúng ABI, recv detour được cài |
| Receiver queued | `PostMessage(-5)` được Windows chấp nhận | DLL đã xử lý yêu cầu hoặc lưu receiver |
| `RequestQueuedNoAck` | Đã đọc đủ 10 byte recv và `PostMessage(-10)` thành công | DLL cũ không có ACK cài reader |
| `DataObserved` | Nhận WM_COPYDATA đúng PID được tra trong danh sách và hình dạng DLL 107 | Nguồn gửi được xác thực, gói TLBB đầy đủ, codec/framing đúng |
| `RestoreFailed` | WriteProcessMemory bị từ chối hoặc không ghi đủ 10 byte | Trạng thái detour thực tế; không được gửi yêu cầu cài mới |
| `RestoreWrittenNoAck` | API báo ghi đủ 10 byte | Yêu cầu `-10` đang chờ có thể chạy sau đó; chưa xác nhận cleanup/lifecycle native |

Các thay đổi hẹp trong luồng C#:

- Ghi nhận BOOL của SetHook và hai lệnh điều khiển `-5`, `-10`. Giữ nguyên declaration và calling convention export DLL. Không dùng GetLastWin32Error cho SetHook vì export không cung cấp hợp đồng lỗi đó.
- Chỉ lệnh hook dùng wrapper PostMessage/ReadProcessMemory/WriteProcessMemory có `SetLastError=true`; các opcode/Lua/command khác vẫn dùng code cũ. HookMessage phải thuộc vùng RegisterWindowMessage `0xC000..0xFFFF`.
- Không yêu cầu recv khi hook Windows/receiver chưa được chấp nhận, địa chỉ recv thiếu hoặc đọc không đủ 10 byte. Chỉ đặt `IsHooked=true` khi PostMessage nhận yêu cầu; đây vẫn là cờ legacy “đã gửi yêu cầu”, không phải xác nhận native.
- Khóa riêng chống hai thread C# đồng thời gửi `-10`. Khi đã gửi, chưa nhận dữ liệu cũng không tự gửi lại, tránh kích hoạt installer cũ nhiều lần.
- Kiểm tra BOOL và số byte của thao tác khôi phục recv. Nếu ghi lỗi/thiếu, giữ cờ bảo vệ và báo lỗi. Đường thoát qua menu dùng cùng wrapper thay vì ghi bộ nhớ không kiểm tra. Không đổi độ dài khôi phục legacy 10 byte.
- WndProc chỉ bổ sung quan sát; không nới parser, không đổi codec, không đưa envelope mới vào ứng dụng. Hình dạng native 107 là `dwData=0`, `wParam=cbData`, `15<cbData<1000`. PID prefix chiếm 4 byte.

SetHook lỗi vẫn không thay đổi điều kiện IsInit, licensing hoặc luồng Init legacy. Exception gốc vẫn được rethrow tại các điểm tương ứng; nhánh bắt lỗi Ác Bá hiện hữu vẫn giữ lựa chọn môn phái thủ công. Không suy luận lỗi reader từ việc im lặng: có thể client chưa phát sinh lưu lượng. Native không trả ACK nên C# chưa giải quyết được race yêu cầu cài đang chờ so với thao tác khôi phục, callback lặp do PeekMessage, trampoline hoặc calling convention.

## Mô hình C++ độc lập

Source tại [tools/native_reader_simulation](../tools/native_reader_simulation). Đây là mô hình hành vi portable C++17 do project viết, không phải source C++ đã khôi phục của EasyHook. Executable mô phỏng chỉ chạy dữ liệu giả lập: không socket, không API Windows, không đọc/ghi process, không nạp DLL hoặc EXE automation.

Mô hình đối chiếu hai đường:

1. **Legacy** tái hiện gate `15<n<1000` và `cbData=n` dù buffer chứa PID4+recv n byte. Với 20 byte `abcdefghijklmnopqrst`, C# chỉ nhận PID4+`abcdefghijklmnop`: mất `qrst` trên bản sao thông báo. Buffer và kết quả recv của game không bị mô hình thay đổi.
2. **Reader đề xuất** chép đủ `n` byte sang bộ đệm do mỗi envelope sở hữu, xếp hàng có giới hạn, giữ nguyên return recv kể cả EOF/error, rồi dispatcher riêng giao qua fake sink. Producer không giữ khóa hàng đợi trong lúc sink xử lý; có ca fake sink gọi producer để kiểm tra điều này.

Envelope đề xuất (chưa được áp dụng cho DLL/WndProc):

| Offset | Nội dung little-endian |
|---|---|
| 0 | Magic ASCII `EGRD`, 4 byte |
| 4 | Version uint16 = 1 |
| 6 | Header size uint16 = 24 |
| 8 | PID uint32 |
| 12 | Sequence uint32, bắt đầu từ 1 |
| 16 | Payload length uint32 |
| 20 | Reserved flags uint32 = 0 |
| 24 | Payload đủ số byte nhận |

Payload tối đa 65,536 byte; hàng đợi tối đa 8 frame và 131,072 byte tính cả header. Khi đầy hoặc dữ liệu không hợp lệ, bỏ thông báo, tăng bộ đếm và giữ nguyên return recv. Sequence có thể phát hiện gap; không phải cơ chế khôi phục byte đã mất. EOF/error không sinh envelope. Timeout/failure giao dữ liệu bỏ một frame, không tự cài lại detour. Fake install/remove trả lỗi được giữ thành trạng thái riêng; start lặp không gọi installer lần nữa, cleanup lỗi chặn restart/delivery.

Fake sink nhận budget 100 ms và trả `Delivered/Timeout/Failed`; mô phỏng kiểm tra kết quả và budget, **không chứng minh API Windows hoặc sink bất kỳ tuân thủ deadline thực tế**. Fake installer không patch code, không giải mã/relocate instruction và không thử VirtualProtect/VirtualAlloc thật. Trước khi dùng cần thiết kế handshake/ACK, token session chống dữ liệu cũ, xử lý sequence wrap, callback lifecycle và deadline với API Windows thật.

C# decoder thử nghiệm kiểm tra magic/version/header/PID/length/flags/sequence trước cấp phát, từ chối frame thiếu/thừa byte và sao chép payload sang buffer riêng. Decoder chỉ nằm trong test project, **không nằm trong EXE EAGLE Auto**. Fixture C++ sinh ra được C# đối chiếu từng byte, có chuỗi UTF-8 giả `AảB` bị chia giữa các ký tự. Ghép lại giữ đủ bytes; decode riêng từng fragment thất bại đúng kỳ vọng. Đây không phải xác nhận client TLBB dùng UTF-8 hay đã biết framing TLBB. Một recv chunk có thể chứa một phần hoặc nhiều gói; reader mới chỉ giải quyết vận chuyển byte.

## Chạy lại kiểm thử

Prerequisites: workflow .NET/Python hiện có và compiler C++17 `g++` hoặc `clang++`. Từ root checkout:

```bash
python tools/run_native_reader_simulation.py
# Hoặc: python tools/run_native_reader_simulation.py --compiler clang++
# Sau restore theo hướng dẫn build:
python tools/prepare_build.py
dotnet build ChickenAutoEx.sln -c Release --no-restore
python tools/verify_build.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
```

prepare/verify cần Python venv có dnfile theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md). Script C++ compile với `-Wall -Wextra -Werror -pthread`, có timeout build/run; xóa kết quả cũ trước chạy. Fixture tổng hợp ở `.build/native-reader-simulation/fixtures.json`; không commit và không đưa vào gói ứng dụng. Test C# yêu cầu fixture thật được sinh, không dùng fallback khiến thiếu mô phỏng vẫn pass. GitHub Actions chạy C++ trước build/test và upload summary cùng bằng chứng .NET.

Kết quả local: **552 test .NET passed, 0 failed/skipped** (488 cũ + 38 hook diagnostics + 26 envelope), **26 tình huống C++ passed**, **51 đối chứng lịch sử** tái hiện đúng lỗi cũ, không lỗi harness. Build net48/x86: **0 lỗi, 49 cảnh báo legacy**. Verifier đảo chính xác delta hook rồi các delta phụ bản để đối chiếu toàn bộ phần còn lại với snapshot; 26 resource gốc, 6 binary release, licensing/VIP và engine phụ bản không đổi. Bằng chứng: [EAGLE-Auto-v0.7-hook-validation.json](evidence/EAGLE-Auto-v0.7-hook-validation.json).

## Bước tiếp theo trước khi sửa DLL

Chuẩn bị harness Windows x86 với cửa sổ/message loop và fake receiver do project sở hữu. Xác minh calling convention/callback stack, registered-message flow, deadline giao dữ liệu, lifecycle/ACK và cleanup trên Windows 11; chưa attach game và chưa nạp DLL gốc. Nếu viết detour mới, dùng thư viện có giải mã/relocate instruction phù hợp thay vì chép cố định 5 byte. Giữ bridge Lua/opcode và licensing riêng, chưa thay chúng bằng reader thử nghiệm.

Sau khi harness native đạt, mới đề xuất thay DLL hoặc thêm module reader trong nhánh riêng, có lựa chọn quay về bản cũ. Kiểm thử game/phụ bản tiếp tục đợi hoàn tất các đợt rà soát như đã thống nhất; báo cáo hiện tại không yêu cầu anh chơi thử ngay.

Gói review: **EAGLE-Auto-v0.7-net48-review**, file chạy **EAGLE-Auto-v0.7.exe**. V0.7 kế thừa mọi sửa nhiệm vụ của v0.6; menu Thủy Lao vẫn chưa khả dụng. Không tự merge, không tạo Release công khai, không tuyên bố DLL hoặc automation đã sửa thành công trên Windows.
