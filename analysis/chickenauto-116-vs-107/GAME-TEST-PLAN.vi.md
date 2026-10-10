# Thử từng tính năng thường trước phó bản

Dùng cùng [báo cáo hiện tại](REGULAR-FEATURES.vi.md), [CSV kết quả](GAME-TEST-RESULTS.template.csv) và [hướng dẫn Windows](../../docs/ChickenAutoEx-WINDOWS11.vi.md). Không có kết quả game được tạo bởi cloud; ứng dụng giữ nguyên build menu `9c59d17`. Gói tải ở [Actions đã thành công](https://github.com/ttgpPsychMH/EAGLE-Auto/actions/runs/38017970647#artifacts).

## Cách thử đối chứng

- Server hiện tại được người dùng xác nhận là private, không phải game4you.us; version/build client chưa rõ. Không cần đoán từ nhãn cửa sổ. Dùng cùng thư mục client cho hai lượt; để nhận diện về sau, ghi SHA256 của **EXE game thực** (không chỉ launcher) bằng `Get-FileHash -Algorithm SHA256 -Path 'D:\duong-dan-client\Game.exe'`, thay đường dẫn cho đúng file. Nếu Properties → Details có FileVersion/ProductVersion thì ghi thêm; những trường đó không luôn xác định engine chính xác. Không cần gửi toàn bộ client hoặc credential để lập fingerprint.
- Ghi phiên client/version và SHA256 EXE/ZIP của cả hai auto. Dùng PowerShell `Get-FileHash -Algorithm SHA256 -Path .\ChickenAutoEx.exe` hoặc đường dẫn stable tương ứng; không chạy file mới chỉ để đo hash. Giữ mỗi bản trong thư mục/config riêng, mỗi lượt chỉ một auto gắn vào cùng game; giữ nguyên binary stable.
- Cùng server/client, map, nhân vật/đội và item/skill thích hợp. Lượt 116 xong đóng auto đó trước lượt 107. Nếu đổi class/role, ghi vào cột `settings_and_context`; không coi test Thiên Long chứng minh buff NM.
- Mỗi lượt chỉ bật một tính năng đang đo, tắt cờ nhiệm vụ/lịch tự động không liên quan. Source cũ có tương tác chéo; R04 cần được thử riêng vì master Auto có thể chưa chặn hồi pet. Không coi “Auto OFF” là cam kết chỉ đọc memory hoặc không gắn hook.
- Ghi giá trị HP/MP/pet trước và sau, số lượng item trước/sau, action quan sát, độ trễ và điều kiện chặn. Một checkbox đúng hoặc balloon đúng chưa đủ PASS cho thuốc/skill. Khi không thấy action, trước hết ghi guard như follow/cưỡi/busy/dead/tick thay vì đổi offset/protocol.
- Cho source 107 thời gian qua nhiều lượt tick phù hợp; tick không phải thời gian cố định khi nhiều game/caller bị chậm. Lưu khoảng quan sát thực tế, không suy ra timeout từ số tick.
- Với việc vứt đồ, chỉ lập ca dữ liệu giả/quyết định phân loại trước. Không thử khám phá quy tắc bằng cách xóa vật phẩm thật. Tiện ích liên quan reset giờ chơi/quyền server không nằm trong các hành động cần nâng cấp ở đây.

## Ý nghĩa trạng thái trong CSV

CSV có một dòng cho mỗi **ca × phiên bản**. `NOT_RUN` là chưa chạy, không phải thất bại. `USER_REPORTED_STARTS` là người dùng báo mở được; `USER_REPORTED_STARTS_HISTORICAL` là quan sát 116 từ lượt trước; `PARTIAL_USER_OBSERVATION` chỉ ghi phần hiện trên ảnh. `PLANNED_OFFLINE` là ca phân loại đồ giả chưa thực hiện. `BLOCKED_PREREQUISITE` là phần phó bản đang chờ baseline. `NOT_AVAILABLE_UI`/`NOT_AVAILABLE_STUB` mô tả giới hạn 107 đã xác định tĩnh, không phải test Windows đã chạy. `UNKNOWN_NATIVE_IMPLEMENTATION` cho 116 nghĩa là chưa khôi phục code tương ứng.

Khi thử, dùng `PASS`, `FAIL`, `BLOCKED` hoặc `N/A`, thêm actual và lý do. N/A chỉ dùng khi class/option/tính năng không áp dụng, không dùng để che lỗi. Ghi `tested_at`, `client_build`, `auto_sha256`, `settings_and_context`, `actual`, `evidence_reference`. CSV không chứa account/password/token; không đính kèm file config thật. Screenshot/log được chia sẻ cần che dữ liệu tài khoản và tên nhân vật nếu không muốn công khai.

## Bốn nhóm kiểm thử

| Nhóm | Ca | Mục tiêu |
|---|---|---|
| Đọc và dừng | R00–04 | Xác nhận startup, metadata thay đổi, chọn đúng instance và master stop; ảnh mới là một phần R01 của 107. |
| Tính năng thường cốt lõi | R05–26 | Hồi phục độc lập, pet, skill/target/radius, follow, NM khi áp dụng, loot; mỗi test đối chứng riêng. |
| Bổ sung theo nhu cầu | R27–35 | Đồ giả, death/cảnh báo, team/scene, persist/scope config, hotkeys/tray, tiện ích, ngôn ngữ và hiệu năng. |
| Nhiệm vụ/phó bản | D00–07 | Chỉ chạy sau cốt lõi dùng trong nhiệm vụ đã đạt; ghi từng bước và giới hạn module. |

## Cổng bắt đầu phó bản

Chưa điền PASS cho phó bản từ ảnh hiện tại. Trước D01–05, phải có đọc map/nhân vật/HP/MP chính xác qua thay đổi trạng thái, dừng được hành động đang thử, và các tính năng cần dùng như combat/skill, hồi phục, pet, follow/đội đã đạt hoặc được xác định không áp dụng. Cần app nhận diện đúng đội trưởng; `Game.Leader` chỉ tìm trong game instances app theo dõi nên không tìm được leader chưa chứng minh game không có tổ đội.

| Ca | Thứ tự đề xuất / điểm cần ghi |
|---|---|
| D00 Menu/ngữ cảnh | Không lựa chọn, không nhận diện leader, leader hợp lệ, đổi/mất lựa chọn. Menu không crash và tick đúng mới là PASS UI. |
| D01 Trừng Ác | Bắt đầu/đọc nội dung nhiệm vụ → xác định map/NPC/mục tiêu → thực thi → trả/kết thúc; ghi **bước đầu tiên thất bại**, dialog text đã che và cờ bật/tắt. Chưa đổi parser/NPC/script. |
| D02 Ác Bá | Đặt đội/role → nhận/entry → map/combat/boss → kết thúc. Ác Bá vẫn reset các cờ nhiệm vụ khác theo chính sách cũ; thử riêng. |
| D03 Ác Tặc | Map cụ thể trước ngẫu nhiên → đặt đội/di chuyển → nhận/entry → combat/nhặt → kết thúc/dừng. Dừng map Ác Tặc không đồng nghĩa dừng mọi tác vụ. |
| D04 Trân Long Kỳ Cuộc | Nhận/entry → các bước trong map → nhận diện boss chết/chờ → kết thúc/ra; ghi map/state/thời gian tại điểm kẹt. |
| D05 Lâu Lan Tam Bảo | Đội/entry → chuyển map/NPC/dialog → combat/boss → kết thúc; giữ khác biệt giữa menu Tam Bảo và module Q123 Lâu Lan. |
| D06 Thủy Lao | UI hiện giữ false và “Not work”. Ghi không khả dụng; chưa bật engine hoặc tìm cách đi vòng qua handler. |
| D07 TKC/module có guard VIP | `IsTKC` false, `TKC()` rỗng; các module có guard vẫn yêu cầu quyền hợp lệ. Ghi stub/không có quyền/không áp dụng; không sửa licensing hoặc bật stub để vượt bước. |

Với 116, chưa biết implementation phó bản đầy đủ; không bắt buộc bật tab Quest để làm đối chứng. So sánh tính năng thường trước, kiểm thử phó bản dựa trên module có source của 107. PASS một phó bản không đại diện cho mọi phó bản/server.

## Mẫu báo lại để chọn lỗi

```text
Ca: R06 / 107
Client build: ...; auto SHA256: ...; map: ...
Thiết lập: HP OFF, MP ON, ngưỡng ..., đang đứng/không follow/không cưỡi
Trước: HP ..., MP ..., thuốc ...
Sau khoảng ... giây: ...
116 cùng điều kiện: ... / chưa chạy
Kết quả: PASS/FAIL/BLOCKED/N/A; bước đầu tiên sai: ...
```

Nếu đọc trạng thái sai, dừng nhóm sau để chẩn đoán metadata/client. Nếu master stop chưa dừng hồi pet, đóng auto để kết thúc lượt và ưu tiên gate hồi pet. Sau đó chọn một lỗi có ca tái hiện, bổ sung mock regression và sửa đúng phạm vi; chưa gom việc này thành thay toàn bộ automation/phó bản.
