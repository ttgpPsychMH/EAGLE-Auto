# Ác Tặc — rà soát trước sửa trên v0.4

Baseline `9ae98bd`; nhánh `repair/eagle-auto-ac-tac`. Báo cáo này ghi các phát hiện trước khi sửa. Người dùng đã được trình bày kết quả và đã yêu cầu sửa trong cùng lượt; không cần vòng phê duyệt mới. Target EXE/native DLL không được chạy.

## Phạm vi

Menu chọn/random/dừng, MapAcTac, AcTacPoint, DatDoiAcTac, helper TalkNPCPhuBan, TrieuTap/Party, Auto/reset/chuyển cảnh, SetCalendar/IsBusy/RandomAcTac, AcTac generic, combat/loot/route và các reader GameObjects/QuestFrame. [Inventory](evidence/source-inventory.json) ghi hash và dòng selected method trong snapshot; IL selected method xác nhận luồng chứ không thực thi binary.

Có hai luồng khác nhau: **menu Ác Tặc** dùng MapAcTac/DatDoiAcTac và map Tặc Khấu Doanh Địa=170; method tên **AcTac** là automation generic của nhiều map/phụ bản theo Global.IsAcTac (mặc định false, không tìm thấy managed setter). Khối map272 chạy trước guard Global nên vẫn có side effect khi flag OFF. Không coi map272/Thanh Thú Sơn hoặc boss Ác Bá là engine của menu Ác Tặc.

| ID | Phát hiện source/IL | Chiến lược sửa |
|---|---|---|
| AT01 | Getter MapAcTac trả map hiện tại nếu đứng trong năm map được liệt kê; chọn map khác không giữ được. | Getter thuần, setter quản lý session/reset; chỉ sáu map menu hoặc 0 hợp lệ. |
| AT02 | RandomAcTac dùng Next(0,4), nhánh num==4 Đôn Hoàng không tới được; khác sáu map menu. | Một pool sáu map, RNG có lifetime riêng; lịch và UI cùng pool. |
| AT03 | IsBusy v0.4 chưa có MapAcTac ngoài map phụ bản. Lịch ClearMission có thể ghi đè đang đi/tìm cửa. | Thêm module vào IsBusy, Auto OFF/ClearMission hủy session. |
| AT04 | MapATIndex/CurMapATIndex/ClearTime/IsBossDie dùng chung với module khác; bật/tắt không reset riêng. Index chỉ kiểm upper bound có đường negative index<-1. | State/index/timer riêng, không sử dụng legacy index; vòng đời gắn owner/team/map, bounds kiểm đủ. |
| AT05 | Đi hết tuyến, vắng quái 10s đặt IsBossDie; 40s reset tuyến, không xác nhận boss hay ra cửa/thưởng. | Boss cụ thể từng ID phải từng thấy sống rồi chết, finite HP; hết tuyến không báo hoàn tất, chờ có hạn; map departure có kiểm chứng riêng. |
| AT06 | TalkNPCPhuBan không kiểm IsNPC/null name/title, chọn theo các tên nhiều nhiệm vụ; fallback QuestFrame.ClickAll. | Helper riêng Ác Tặc, NPC/name/range/ID, option50013/-1, không ClickAll, click một lần rồi chờ map. Không sửa toàn bộ helper chung. |
| AT07 | TrieuTap không chặn member Auto OFF/init/offline; AskTeamFollow là lệnh toàn đội không giới hạn member Auto ON. Party có thể thêm leader hai lần. | Dispatcher riêng deduplicate, chỉ participant đúng đội/Auto/init/online; recheck giữa lệnh, không phát follow toàn đội. |
| AT08 | Không timeout route/NPC/dialog, FixKetMap/MoveNext/Use/dialog có thể lặp; timer shared biến đổi theo module khác. | Clock monotonic, pause không tiêu timeout; tiến triển route refresh, dialog/map/boss/exit có hạn. |
| AT09 | Menu chọn/random tắt IsTrieuTap trên mọi dicGame, cả đội khác; không xử lý xung đột các cờ nhiệm vụ. | Giới hạn đội được chọn, ResetDungeonActions của leader khi chọn; engine từ chối xung đột còn lại. |
| AT10 | Route AcTacPoint là 14 điểm map170 nhưng dùng điểm đầu làm đích đi sang map ngoài. POINT còn có route 9 điểm map170 khác và route ngoài sáu map. | Bảo toàn route14 cho menu trong170; ngoài dùng POINT đúng map, index riêng. Không tự thay hai route bằng một bộ mới. |
| AT11 | AcTac generic map272 có side effect trước Global OFF; generic có thể cạnh tranh với menu trong170 nếu bật Global. | Guard control/Global trước body; menu session sở hữu170, generic không điều khiển170. Các nhánh generic phụ bản khác giữ nguyên. |
| AT12 | Map/NPC/token và reader memory không có success signal; không tìm thấy nhận/trả quest/reward ack riêng cho menu. | Giữ baseline, ghi rõ giới hạn; không force quest complete hoặc suy đoán opcode thoát. |

Combat SkillDo/Atk, HP/MP/pet, offsets/hook/path resources, Global/version protocol, licensing/VIP và stable116 phải được giữ nguyên. Core mới chỉ điều phối di chuyển/dialog/control; không đổi thuật toán combat chung.

Sau triển khai xem [báo cáo mô phỏng/sửa](../../docs/EAGLE-Auto-AC-TAC-SIMULATION.vi.md), bao gồm đối chứng mã cũ, test sau sửa và phần phải xác minh trên private client. Kiểm thử Windows/game tiếp tục để sau đợt rà soát tất cả phụ bản như người dùng đã chọn.
