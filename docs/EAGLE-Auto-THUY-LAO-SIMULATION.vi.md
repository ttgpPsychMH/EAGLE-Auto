# EAGLE Auto v0.2 — mô phỏng và sửa hẹp Thủy Lao

Ngày: 10-10-2026 (Asia/Bangkok). Nhánh `repair/eagle-auto-thuy-lao-simulation`, baseline `c17c67303e6ce8a80d360a7cd56f186a50775ab8`.

## Kết quả và phạm vi

Gói mới chạy bằng **EAGLE-Auto-v0.2.exe** và config cùng tên. **135 test qua, 0 thất bại, 0 bỏ qua**: 94 test updater/UI/settings/menu trước đây và 41 ca mô phỏng Thủy Lao. Cùng bốn assertion quan trọng được chạy trên method gốc: cả bốn thất bại đúng lỗi hành vi, không có lỗi biên dịch/harness. Đây là đối chứng chủ đích, không phải lỗi test của bản sửa.

Menu Thủy Lao vẫn giữ false và thông báo `Not work`. Không mở khóa licensing/VIP, không đổi stable 116, hook native, offsets, dialog ID, NPC/tọa độ, combat/skill, resource tuyến đường hay snapshot recovered gốc. Bản này không tuyên bố hoàn thành phụ bản hoặc chạy được trong game private.

## Các sửa đã thực hiện

| Phát hiện trong báo cáo trước | Sửa và kiểm chứng |
|---|---|
| TL02: map đích khác map NPC | Dùng map của Hô Diên Khánh (4) thay Tô Châu (1). Đối chứng trên code gốc nhận map 1; bản sửa nhận 4. |
| TL03–04: thiếu guard thành viên/người dẫn | Kiểm tra Auto, online, metadata, chết/chuyển cảnh, đúng đội và leader hiện tại trước nhánh Thủy Lao; người dẫn mất điều kiện thì dừng module. Thành viên không hợp lệ không nhận lệnh. |
| TL05: member vào trước bị kéo ra | Member ở map 66 chờ leader vào. `DiThuyLao` ở 66 không đi về NPC. |
| TL06: hết tuyến bị coi hoàn thành | Tách cờ hết tuyến khỏi `IsXongThuyLao`; chỉ tới điểm kết thúc 94,94 khi đọc thấy quest hoàn thành và không còn quái trong vùng quan sát. Không giả trạng thái quest, không tự thiết kế protocol thoát/trả quest. |
| TL07: reset thiếu, lịch chồng nhiệm vụ | Reset session khi đổi map/đội, tắt Auto, đổi cờ module hoặc `ClearMission`. Lịch xem module Thủy Lao đang bật là bận; các map/module khác giữ hành vi trước đây. |
| TL08: thiếu xác nhận và timeout | Chờ cửa sổ mission được quan sát mở; đọc task để xác nhận đã nhận; không vào lượt mới nếu task đã hoàn thành/chờ trả. Kiểm tra NPC ID và option hiện tại trước chọn/xác nhận dialog. Accept chỉ gửi một lần rồi chờ quest/map cập nhật. Quá 60 giây không có tiến triển thì dừng, không tự thử mãi. |
| TL09: timer/index dùng chung | Dùng clock đơn điệu và timer Thủy Lao riêng; chờ hơn 2 giây sau khi vào map/vắng quái. Lưu/khôi phục `MoveIndex` dùng chung khi tuần tra bằng index riêng. Có quái thì ưu tiên để combat cũ xử lý. |
| TL11: lỗi bị nuốt | Bắt lỗi trong module/nhánh dẫn member, dừng module và báo loại exception một lần; không hiện `Exception.Message`, account, token hoặc dump memory. Không đổi xử lý exception của module khác. |

## Bộ mô phỏng thực sự chạy gì

`ThuyLaoSimulationTests.cs` dùng Roslyn của SDK đã pin để chọn đúng method/property từ source Game đã áp dụng delta, cộng toàn bộ `Game.ThuyLao.cs` đang được build. Các phụ thuộc đọc game/quest/dialog, clock, tổ đội và lệnh hook được thay bằng dữ liệu giả. Lệnh `Move/Talk/Accept/GoTo` chỉ ghi vào danh sách; không có P/Invoke, không load hoặc chạy EXE/DLL game automation.

Các ca bao gồm: sai map; Auto OFF; offline; chết; đang chuyển cảnh; đổi leader/đội; metadata null; member vào trước; thiếu/sai NPC và option; cửa sổ quest chưa mở; quest chưa xuất hiện sau Accept; quest đã completed; entry không đổi map; còn quái sau điểm cuối; thiếu tín hiệu hoàn thành; stuck trên tuyến; exception từ leader/member; tắt/bật lại session; bảo toàn nhánh triệu tập thường khi Thủy Lao OFF. Clock được tăng bằng test, không sleep để giả timeout.

`tools/check_thuy_lao_baseline.py` đòi đúng bốn lỗi assertion trên source gốc và từ chối lỗi harness/runtime hoặc thiếu test. CI chạy cả đối chứng lẫn toàn bộ suite sửa. Baseline summary, TRX suite sửa, `verified.json` và checksum được tải cùng artifact.

## Bảo toàn source và cách build

Ba method Thủy Lao chuyển vào partial `Game.ThuyLao.cs`. `tools/thuy_lao_review.json` khai báo chính xác method bị thay và các delta ở property/nhánh Thủy Lao/scene/Auto OFF/lịch/route. `prepare_build.py` áp dụng chúng lên bản sao hydrate trong `.build`; recovered snapshot không bị chỉnh. `verify_build.py` đảo ngược từng delta và redaction rồi so toàn bộ Game với snapshot gốc, đồng thời kiểm tra hash file partial mới. Mọi code auth và automation ngoài delta được bảo vệ; không bỏ qua cả Game hoặc cả vùng phụ bản.

Sau restore, luôn chạy `tools/prepare_build.py`, build solution, `tools/verify_build.py`, `tools/check_thuy_lao_baseline.py`, `dotnet test ... --no-build --no-restore`, rồi `tools/package_build.py` theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md). Test harness cần source đã chuẩn bị; không chỉ build test project khi `.build/thuy-lao/ReviewedGame.cs` còn thiếu hoặc cũ. File chứa literal hydrate tiếp tục ignored và không được upload/commit.

## Giới hạn còn lại và kiểm tra Windows

Mô phỏng kiểm tra quyết định C# với dữ liệu hợp lệ được cung cấp, không xác nhận native hook, memory reader, NPC/dialog/quest của client private, độ trễ thật, UI thread hay race giữa click UI và loop nền. `Task.Enum` gốc vẫn mở mission tracking khi đọc; chưa thay API này. Phạm vi kiểm tra completion là trạng thái task đọc từ client, chưa chứng minh server xác nhận kết thúc theo protocol nào. Điều kiện option yêu cầu đúng cặp legacy `232000/-1` hoặc `232002/-1`; nếu client dùng dialog khác, module sẽ chờ rồi dừng. Timeout 60 giây là giới hạn bảo thủ cho bản chưa bật; boss lâu hoặc lag có thể cần hiệu chỉnh bằng bằng chứng thực tế. Các lỗi nearest-point dùng chung TL12 chưa sửa.

1. Tải artifact xanh của đúng nhánh/commit, giải nén vào thư mục riêng và kiểm tra EXE/config v0.2. Mở app, kiểm tra tiêu đề/About, menu phó bản và các chức năng thường đang dùng.
2. Thủy Lao vẫn phải báo chưa khả dụng. Không sửa config/source hoặc dùng công cụ để ép bật nhằm thử engine này.
3. Thực hiện một lượt Thủy Lao bằng tay trên private client; ghi mapID trước/sau cửa, NPC/dialog, tên và trạng thái quest, thứ tự thành viên vào, tọa độ/chặng và cách thoát/trả quest. Có thể gửi ảnh đã che thông tin tài khoản; không gửi password/token.
4. Đợt sau đối chiếu dữ liệu đó với mock, bổ sung mô phỏng độ trễ/chuyển dialog và kiểm thử adapter trên Windows có giám sát. Chỉ cân nhắc bật menu khi entry/completion/thoát đã có evidence; việc mở app thành công không đủ để xác nhận sửa xong phụ bản.

[Báo cáo tĩnh trước sửa](../analysis/thuy-lao/REPORT.vi.md) tiếp tục được lưu nguyên để đối chiếu; các dòng source trong đó thuộc baseline cũ.
