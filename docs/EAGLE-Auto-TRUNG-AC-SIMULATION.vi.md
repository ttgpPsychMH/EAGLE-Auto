# Trừng Ác — sửa và mô phỏng v0.4

Ngày 10-10-2026 (Asia/Bangkok). Nhánh `repair/eagle-auto-trung-ac`, dựa trên báo cáo `c9ca4eb` và ứng dụng v0.3 `91a93f2`. [Báo cáo phân tích trước sửa](../analysis/trung-ac/REPORT.vi.md) được giữ nguyên như evidence lịch sử.

Đã sửa các lỗi managed có thể xác nhận trong TA01–TA12. Các giá trị và ý nghĩa phụ thuộc private client trong TA05/TA13 vẫn cần kiểm chứng; mô phỏng không chứng minh quest được server chấp nhận hay thưởng được cấp. Không chạy EXE automation hoặc native hook trên cloud.

## Thay đổi theo phát hiện

| Phát hiện | Xử lý v0.4 |
|---|---|
| TA01 — parser crash/sai tọa độ | Parser riêng TryParse, đúng một cặp tọa độ, không âm/overflow, giới hạn 0–4095; input rỗng/sai/map lạ thì chờ có hạn, không Move. Không đổi ParseInt/GetMapId chung. |
| TA02 — timer UI tự bỏ quest | Bỏ khối timer gửi yêu cầu bỏ quest khỏi TheoDoiCanhBao. Timeout chỉ dừng session, giữ quest để kiểm tra bằng tay; không gửi OK chung. |
| TA03 — state/timer/cờ xong cũ | Property IsTrungAc reset toàn bộ dữ liệu riêng khi bật/tắt; ClearMission/ClearNhiemVu/Auto OFF hủy session. Gắn owner nhân vật và quest ID. |
| TA04 — mất state khi dialog trễ | State machine riêng, mỗi bước giữ trạng thái chờ và có timeout. Unknown state dừng có lý do. |
| TA05 — nhận/trả thiếu xác nhận | Ràng buộc NPC Ngô Giới/map/vị trí/ID, option từ snapshot; nhận cần Task và lệnh xuất hiện; trả cần hai snapshot liên tiếp không còn Task sau lệnh Complete. Chưa xác nhận thưởng hoặc native read thành công. |
| TA06 — dùng lệnh/loot lặp | Mỗi bước Info/Spawn dùng lệnh một lần rồi chờ; không gọi ForcePickItem từ engine này để loot/FixKetMap lấn bước quest. Combat/loot chung ngoài engine không đổi. |
| TA07 — corpse và đổi target liên tục | Chỉ một mục tiêu sống, finite HP/tọa độ, phù hợp cấp/Menpai/bán kính; giữ ID, loại đối tượng đã có trước triệu hồi. Corpse/vắng quái không chứng minh thắng; chờ Task.Completed/Complete>=256. |
| TA08 — kẹt không timeout | 60s dialog/NPC/ack, 180s đổi map/tiến triển di chuyển/spawn/combat. Tiến gần/HP giảm làm mới timer tiến triển; pause/chuyển cảnh không tiêu thời gian bước. |
| TA09 — lịch và nhiệm vụ xung đột | IsBusy tính Trừng Ác ở map thường, ClearMission tắt cả ba module; kiểm tra các cờ quest xung đột trước dispatcher. |
| TA10 — UI leader, engine cá nhân | Menu điều khiển nhân vật đang chọn, có thể dùng ngoài đội; không toggle leader khác. Follow phải dừng trước quest cá nhân; không thêm quest chung đội hoặc copy kết quả member. |
| TA11 — AutoLogin bật lại sau OFF | Khởi tạo một lần theo owner/phiên online, ưu tiên tắt thủ công. Callback theo dõi cả sau 60s để nhận biết reconnect; không đổi login queue/captcha/auth. |
| TA12 — lỗi bị nuốt/race | Lock riêng cho engine/property/login; alarm không sửa state quest. Exception dừng riêng module, thông báo loại exception, không ghi message/dialog/account/token. Race native/UI và WinForms cần Windows kiểm chứng. |
| TA13 — dữ liệu client | Giữ baseline map/NPC/opcode/token/route/hook. Matcher riêng ưu tiên tên map cụ thể (Cao Xương Mê Cung, Thanh Nguyên Sơn Động), từ chối tên nhiều map/không biết; không đoán offsets mới. |

Source chính: [engine](../src/ChickenAutoEx/TrungAc/Game.TrungAc.cs), [parser](../src/ChickenAutoEx/TrungAc/Game.TrungAcParser.cs), [allowlist Game/FrmMain](../tools/trung_ac_review.json). Hydrated Game chỉ nằm trong `.build` ignored; snapshot recovered, stable 116, licensing/VIP/HWID/config encryption và Global.Version=107 giữ nguyên.

## Kiểm thử và đối chứng

85 ca Trừng Ác dùng actual C# engine/partial/parser và các khối ClearMission/ClearNhiemVu/Auto OFF/AutoLogin chọn từ source, biên dịch Roslyn với fake TLBB, inventory, task, dialog, đối tượng, clock và command sink. Không load ứng dụng hoặc native DLL. 41 ca menu kiểm tra contract cá nhân/đội và selection thay đổi. Toàn suite v0.4: **309 passed, 0 failed, 0 skipped**.

11 assertion chạy lại trên selected method của snapshot bất biến phải thất bại đúng loại lỗi: dialog rỗng/thiếu Y; timer bỏ quest sau OFF; completion cũ; dialog nhận trễ; corpse; nhiều target; Use lặp; lịch không bận; ClearMission; AutoLogin sau OFF. [Checker](../tools/check_trung_ac_baseline.py) kiểm tra đủ tên, outcome, message và lỗi parser thật trong Game.TrungAc, từ chối lỗi compile/fixture/runtime khác. Đây là **11 thất bại mong đợi trong lượt riêng**, không cộng vào test PASS của bản sửa.

Sau đó [rà soát cả ba nhiệm vụ](EAGLE-Auto-DUNGEON-REAUDIT.vi.md), gồm 9 đối chứng bổ sung trên partial v0.3. CI chạy đủ mọi checker và cùng suite trước khi đóng gói.

```bash
python tools/prepare_build.py
dotnet build ChickenAutoEx.sln -c Release --no-restore
python tools/check_trung_ac_baseline.py
python tools/check_dungeon_reaudit_baseline.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
python tools/verify_build.py
```

Dùng Python có dnfile và toolchain theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md). CI upload summary đối chứng/TRX/verified.json; không upload fixture/hydrated source hoặc full IL.

## Giới hạn và kiểm thử tổng hợp sau này

- Task.Enum hiện không trả cờ “native read thành công”; hai snapshot vắng Task hạn chế lag nhưng không loại trừ hai lần đọc thất bại trả list rỗng. Không dùng Task.SetComplete/TaskInfo.SetTrangThai để giả kết quả.
- Continue/Complete giữ opcode 14/15; dialog phải còn token quest. Nếu private client đổi text/cấu trúc, engine dừng có hạn thay vì click theo suy đoán. Không có bảo đảm reward ack chỉ từ việc Task biến mất.
- Hai token CXDY legacy chưa có bản dịch chính xác. Cờ IsXongTrungAc chỉ có thể lên sau đã quan sát ack trả và dialog giới hạn, vẫn không chứng minh hết lượt/ngày hoặc đã nhận thưởng. Dialog giới hạn trước lần trả chỉ dừng và yêu cầu kiểm tra bằng tay.
- Bật lại khi quest active nhưng thiếu lệnh/thông tin sẽ dừng; chưa khôi phục qua Lua/hook vì phản hồi chưa được xác minh. Không tự bỏ quest để lấy lượt mới.
- Tọa độ 0–4095 là giới hạn bảo thủ, chưa chứng minh mọi private map nằm trong miền này. Tên Lạc Dương trùng map ID giữ lựa chọn legacy; input map không xác định không tự sửa chung route.
- Việc loại đối tượng đã có trước Use và Menpai/cấp/bán kính chỉ là lọc bảo thủ, chưa chứng minh target thuộc quest nếu server thiếu ID quest/spawn. Server khác có thể tái dùng ID; hiện dừng sau timeout.
- Clock/fake command không kiểm chứng WinForms, native offsets, đường 16.dat/17.dat, hook, quyền game, yêu cầu level/đội/VIP hoặc vận hành server.

Theo quyết định của người dùng, **chưa yêu cầu thử game ngay**. Sau khi rà soát/sửa các phụ bản còn lại, kiểm thử tổng hợp có giám sát: OFF/pause/restart, owner/đội thay đổi, nhận quest và item, thông tin lệnh, tới điểm/triệu hồi, một target, Task hoàn tất, NPC trả và thưởng thật; lưu bản đồ/bước/loại lỗi đã che thông tin riêng. Giữ stable 116 riêng và dừng ngay khi dữ liệu không khớp baseline. Xem [checklist Windows 11](ChickenAutoEx-WINDOWS11.vi.md).
