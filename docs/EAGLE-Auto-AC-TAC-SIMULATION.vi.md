# Ác Tặc — sửa và mô phỏng v0.5

Ngày 10-10-2026; baseline v0.4 `9ae98bd`, nhánh `repair/eagle-auto-ac-tac`. [Báo cáo trước sửa](../analysis/ac-tac/REPORT.vi.md) ghi AT01–AT12 và evidence source/IL. Người dùng đã được trình bày các phát hiện trước khi sửa. Build này vẫn cần kiểm thử Windows/private client sau đợt rà soát tất cả phụ bản.

## Các thay đổi

| Phát hiện | Cách sửa |
|---|---|
| AT01–03: map bị getter đổi, random thiếu map, lịch ghi đè | MapAcTac chỉ trả lựa chọn đã lưu, chỉ nhận 0 hoặc sáu map menu. UI/lịch dùng chung pool sáu map. IsBusy nhận biết Ác Tặc cả ngoài phụ bản; Auto OFF/ClearMission hủy session. |
| AT04, AT08: state/index/timer chung, thiếu giới hạn chờ | Partial riêng với lock, clock monotonic, owner/team/character ID, phase và state từng participant. Không dùng IsBossDie/MapATIndex chung. Guard và timeout được kiểm tra lại trước lệnh. |
| AT05: hết tuyến/vắng quái bị coi là boss chết | Giữ 14 điểm tuyến map170, đánh dấu điểm đã đi; sau combat nối tuyến từ điểm chưa đi gần nhất. Hết tuyến chỉ chờ boss. Cần thấy đúng boss `tacbinhdaumuc` cùng ID sống rồi HP=0 mới chuyển sang chờ rời map. |
| AT06: dialog chung/ClickAll | Helper riêng chỉ đọc NPC hợp lệ, bind ID, đóng dialog cũ một lần, Talk một lần rồi chờ. Chỉ click option `50013/-1` một lần, sau đó chờ map; không ClickAll. |
| AT07, AT09: điều khiển nhầm thành viên/đội, xung đột quest | Deduplicate Party; chỉ điều phối thành viên đúng đội, Auto ON, init/online và không xung đột. Không gửi follow toàn đội. Menu chỉ tắt triệu tập trong đội được chọn; reset nhiệm vụ xung đột của leader. Thành viên đổi ID/rời đội/tắt Auto trong phụ bản khiến session dừng. |
| AT10: lấy tọa độ phụ bản để đi map ngoài | Ngoài phụ bản dùng POINT của đúng một trong sáu map; trong170 giữ nguyên 14 điểm AcTacPoint. Không thay bằng route POINT170 chín điểm khác. |
| AT11: generic AcTac chạy khi flag OFF/cạnh tranh170 | Guard Global/control/pause/chuyển cảnh trước body generic, và không điều phối170 qua generic. Các nhánh generic của map/phụ bản khác giữ nguyên; default Global.IsAcTac vẫn false. |

Session có bốn phase: tìm/vào cửa → tuần tra → chờ boss → chờ rời map. Khi **tất cả participant đã từng ở170** thực sự chuyển về map ngoài đã chọn sau dấu hiệu boss chết, session mới reset để tìm cửa lượt kế tiếp. Đây chỉ là xác nhận chuyển map, **không phải xác nhận hoàn thành quest hoặc nhận thưởng**.

Chờ dialog sau Talk/đóng dialog cũ, map sau chọn option, boss sau hết tuyến và rời map có giới hạn 60 giây; tìm/đi cửa, thành viên chưa sẵn sàng và tuần tra không tiến triển có giới hạn 180 giây. Pause/chuyển cảnh của leader đóng băng phần thời gian chờ. Timeout dừng Ác Tặc và yêu cầu kiểm tra bằng tay, không tự abandon quest, force complete hay đoán packet cửa ra. Combat/loot chung vẫn do code hiện hữu thực hiện; thuật toán đánh quái/skill không đổi.

## Kiểm thử

- **66 ca Ác Tặc**, biên dịch selected method thực và partial mới bằng Roslyn với TLBB/Objects/dialog/clock/command giả; dùng MAP và sáu route POINT lấy từ snapshot. Không load EXE hoặc native DLL.
- **13 đối chứng** chạy trên mã v0.4 trước sửa: cả 13 thất bại đúng assertion hành vi, không có lỗi harness. Bao gồm getter, lịch, random, quiet completion, Auto OFF/pause, negative index, ClickAll, monster giả NPC, generic map272 và hai lỗi menu/đội.
- **375 test toàn suite passed**, 0 failed/skipped: Ác Tặc66, Trừng Ác85, Thủy Lao49, Kỳ Cuộc/pet78, menu41, updater37, UI/settings19.
- Bốn nhóm đối chứng cũ vẫn đạt yêu cầu: Thủy Lao4, Kỳ Cuộc/pet6, Trừng Ác11, rà soát v0.3 thêm9. Tổng **43 thất bại hành vi cũ được kiểm soát**; không tính chúng là lỗi của build sau sửa.
- Build net48/x86 thành công, 0 lỗi và 50 cảnh báo legacy. Verifier đảo các delta đã review rồi đối chiếu nguyên source/resource/binary; stable116, snapshot, licensing/VIP, HWID/config encryption, native hook/offsets và giao thức107 giữ nguyên.

Test kiểm tra chọn/clear/restart, guard owner/member/pause, recheck sau Talk/click/GoTo, dialog sai/stale/click một lần, đủ route14 sau reanchor, boss chưa từng thấy sống/ID khác/NaN/Infinity, member quan sát boss, map thực/exit timeout, member OFF khi chờ exit, exception chỉ báo loại và tọa độ/tuyến không hợp lệ. Các fake mô tả contract điều phối, không giả lập memory layout hay toàn bộ game server.

Chạy lại theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md); đối chứng mới dùng `python tools/check_ac_tac_baseline.py`. [Bằng chứng tổng hợp](evidence/EAGLE-Auto-v0.5-ac-tac-validation.json) không chứa source hydrate, key hoặc credential.

## Chưa thể xác minh — AT12

Giữ map170, tên NPC/boss và token50013/-1 theo baseline. Client riêng có thể đổi chúng; việc khớp tên/option không chứng minh quest đang đúng. Native reader không cung cấp success signal đáng tin cậy: giá trị hợp lệ về kiểu, kể cả HP=0, vẫn có thể sai khi đọc memory lỗi. Map ID không phân biệt được mọi instance server.

Chưa tìm thấy protocol cửa ra/nhận thưởng riêng đáng tin cậy cho menu Ác Tặc. Code mới **không tự đoán cửa ra**; nếu client không tự đưa đội ra map đã chọn, sau 60 giây session dừng để kiểm tra bằng tay. Nếu boss spawn chậm sau hết tuyến, cũng dừng khi hết hạn thay vì báo hoàn tất giả. Timeout/matching có thể cần điều chỉnh sau quan sát thực tế; chưa đổi memory offsets, packet hay quest/reward semantics.

Trong lượt kiểm thử Windows/game sau này, ghi riêng: lựa chọn map có giữ đúng; cả đội vào170; dialog/token đúng; tuần tra sau combat có tiếp nối; boss được quan sát sống/chết; map nào sau cửa ra; quest và thưởng có thực sự cập nhật. Tắt auto ở từng giai đoạn để kiểm tra dừng. Không gửi account, password, token hoặc file config chứa credential. Mở được ứng dụng và mô phỏng passed chưa xác nhận phụ bản chạy thành công.
