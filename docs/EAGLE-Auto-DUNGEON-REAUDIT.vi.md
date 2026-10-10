# Rà soát lại Trừng Ác, Thủy Lao và Trân Long Kỳ Cuộc — v0.4

Ngày 10-10-2026 (Asia/Bangkok). Tự động rà soát sau sửa Trừng Ác, trên nhánh `repair/eagle-auto-trung-ac`. Soát lại control/owner/reset/timeout/dialog/target/route/exit, cùng caller Auto, ClearMission, lịch và UI/AutoLogin. Không thực thi EXE automation; không đổi stable 116 hoặc licensing/VIP.

## Kết quả bổ sung

| Module | Lỗi tái hiện trên v0.3 | Sửa hẹp v0.4 |
|---|---|---|
| Thủy Lao | Global.Paused vẫn gửi PostMessage nhận quest | Kiểm tra pause ở ranh giới lệnh và đóng băng thời gian bước. |
| Thủy Lao | IsInit=false vẫn phát lệnh | Kiểm tra init trước thao tác leader/member. |
| Thủy Lao | Tắt sau GoTo NPC vẫn chọn option | Kiểm tra lại quyền điều khiển sau GoTo/read và trước Talk/Select/Accept/Close. |
| Thủy Lao | Leader OFF giữa lúc member tới NPC, member vẫn Select | Controller có phạm vi try/finally; guard member yêu cầu controller vẫn active/leader/đúng đội. |
| Thủy Lao | Member đang làm Trừng Ác vẫn bị điều khiển | Chặn member có Trừng Ác/Kỳ Cuộc active. Leader xung đột dừng module. |
| Kỳ Cuộc | Global.Paused vẫn GoTo/Dialog | Pause guard và clock hiệu dụng; timeout tiếp tục từ thời gian còn lại. |
| Kỳ Cuộc | Member Trừng Ác active vẫn nhận lệnh | Không điều khiển member xung đột; chờ sẵn sàng có timeout. |
| Kỳ Cuộc | PlayerState=9 không hủy session | Dừng session trước command như trạng thái chết 2. |
| Kỳ Cuộc | Instance Game đổi nhân vật, session đội vẫn tiếp tục | Bind CharacterId từng participant, dừng khi đổi ID. |

9 case trên đều thất bại đúng assertion khi dùng partial v0.3 phục hồi theo hash, rồi PASS với partial mới. [Checker](../tools/check_dungeon_reaudit_baseline.py) từ chối thiếu case, PASS nhầm, hoặc lỗi fixture. [Allowlist bổ sung](../tools/dungeon_reaudit_review.json) đảo chính xác từng delta về hash v0.3 trước khi verifier đối chiếu baseline cũ.

Các chốt bổ sung khác: Thủy Lao dùng dispatcher riêng trong DatDoiThuyLao, generic TrieuTap giữ lựa chọn dispatcher lúc bắt đầu để tránh rơi sang summon thường khi bị OFF giữa vòng; member không cần bật cờ riêng khi được leader điều phối nhưng vẫn phải Auto/init/online/đúng đội. Kỳ Cuộc bỏ HP NaN/Infinity khỏi quan sát boss sống. Trừng Ác callback AutoLogin theo dõi phiên dài hơn 60s để khởi tạo lại đúng sau reconnect; đọc task ném lỗi không thể báo hoàn tất.

## Kiểm thử hồi quy

**309 PASS, 0 FAIL, 0 SKIP**, gồm:

| Nhóm | Số ca |
|---|---:|
| Trừng Ác | 85 |
| Thủy Lao | 49 |
| Kỳ Cuộc/pet AOE | 78 |
| Menu | 41 |
| Updater/schema và UI/settings | 56 |

Các lượt đối chứng riêng: **4** lỗi original Thủy Lao, **6** original Kỳ Cuộc/pet AOE, **11** original Trừng Ác, **9** partial v0.3 — tổng **30 thất bại đúng hành vi cũ**, không có lỗi harness. Chạy lại toàn bộ suite sau sửa, không chỉ case mới. Build net48/x86 và verifier kiểm tra PE/resources/dependency, đảo delta Game/FrmMain/partial rồi so source; không coi mock PASS là Windows/game PASS.

## Phần vẫn cần private client

- Thủy Lao: giữ menu chưa khả dụng theo baseline trước; không tự bật sau mô phỏng. IsXongThuyLao phản ánh Task completed đã đọc, chưa chứng minh ra cửa hoặc nhận thưởng. Không đoán dialog thoát mới.
- Kỳ Cuộc: giữ sáu waypoint, chọn điểm chưa thăm gần vị trí hiện tại sau combat; timeout/map/NPC/options và quan sát boss sống→chết được kiểm tra lại. Map script 550 so với runtime 61, cửa ra Lạc Dương, NPC và offsets vẫn phải xác minh. HP đọc sai nhưng có vẻ hợp lệ vẫn có thể làm detector sai; không có native success bit mới.
- Trừng Ác: quest/Ngô Giới/item/token/cấp/Menpai/opcodes còn phụ thuộc client. Vắng task hai snapshot không đủ chứng minh nhận thưởng. Không thêm reset quest, force-complete, bypass VIP hoặc dispatcher quest chung đội. [Chi tiết giới hạn](EAGLE-Auto-TRUNG-AC-SIMULATION.vi.md).
- Global pause mới chặn các engine được sửa; các module combat/HP/MP/hook chung giữ nguyên. Không tuyên bố pause dừng mọi loại input của toàn ứng dụng nếu caller/native có hành vi khác.

Tiếp theo tiếp tục rà soát các nhiệm vụ/phụ bản còn lại bằng cùng phương pháp source → mô phỏng lỗi → sửa hẹp → hồi quy. Sau đợt đó thực hiện kiểm thử Windows 11 và trong game từng bước, như người dùng đã yêu cầu; hiện chưa cần đổi client hoặc nhập credential để chạy mô phỏng.
