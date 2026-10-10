# ChickenAutoEx 107: sửa binding MP và index cấu hình

Nhánh `repair/chickenautoex-107-ui-settings`, base `5385725` đã merge phần startup/net48. Đợt này chỉ có hai thay đổi hành vi ứng dụng trong `FrmMain.cs`; không dùng hoặc chép source MicroAuto.

## Hành vi trước và sau

1. Handler `checkrengenmp_CheckedChanged` trước ghi `CurGame.IsMP = checkregenhp.Checked`. Nay đọc `checkrengenmp.Checked`, đồng nhất với checkbox và thông báo MP. HP và MP bật/tắt độc lập; khi không có nhân vật được chọn, cả hai handler vẫn bỏ qua như trước.
2. `LoadSetting` trước đọc index 61 và 62 trong cùng guard `Length > 61`. Với 62 phần tử, đọc index 62 gây IndexOutOfRangeException rồi bị catch rỗng che đi. Nay mỗi index có guard riêng: length 62 vẫn áp dụng MapBanDoIndex ở index 61; MaptriLieuIndex chỉ áp dụng khi length >=63. Phần thiếu giữ giá trị đang có; ở khởi động sạch hai mặc định đều bằng 0.

Không đổi format CSV, tên `General.dat`, thứ tự trường, encryption hoặc serializer. Đây không phải thay đổi schema hay sửa mọi trường cấu hình legacy. SaveSetting giữ nguyên 64 trường và vị trí hai map ở 61/62. Không đổi ngưỡng/chu kỳ HP/MP trong Game.Buff, offsets, hook, scheduling, VIP hoặc mã phó bản. Stable 116 và snapshot source điều tra không đổi.

## Bằng chứng kiểm thử

- Bộ regression mới: **19 ca**. Trước sửa: **8 failed, 11 passed**, bắt được lỗi MP và exception bị che ở length 62. Sau sửa: **19 passed**.
- Toàn bộ suite: **56 passed, 0 failed, 0 skipped**, gồm 37 test updater/schema đã có. Updater vẫn được kiểm thử bằng service HTTP/TLS giả lập local.
- Build solution Release net48/x86 thành công trên Linux: **0 lỗi, 50 cảnh báo legacy**. Không chạy executable hoặc game.
- Static verifier xác nhận 27 embedded resource, identity dependencies, 26 resource gốc, sáu binary gốc và source engine/licensing giữ nguyên. Verifier chỉ chấp nhận đúng hai delta của FrmMain rồi đối chiếu phần còn lại, không bỏ qua toàn bộ method/file.

Kết quả build, số test và hash gói local được lưu ở [validation JSON](evidence/ChickenAutoEx-ui-settings-validation.json). Hash gói do GitHub Actions tạo nằm trong artifact của chính lần chạy đó; không mặc định bằng hash local.

### Test thực sự chạy gì?

Roslyn của SDK 8.0.425 đọc source FrmMain rồi lấy các method `checkregenhp_CheckedChanged`, `checkrengenmp_CheckedChanged`, `LoadSetting`, `SaveSetting`, `Bool2Int` nguyên thân method. Cùng các helper thuần `String2Arr`/`String2Int` và chuyển key, chúng được biên dịch vào harness test net8 với checkbox, numeric control, notification, game và I/O bộ nhớ giả. Chỉ các field/property scalar cần thiết của Global/Option được đưa vào, không đưa startup, constructor, auth, native API hoặc automation vào harness. Không load EXE/DLL ứng dụng khôi phục.

Ca kiểm tra: bốn tổ hợp HP/MP và đổi từng checkbox; không chọn nhân vật; thông báo MP khớp state; length 61/62/63/64; trường trước tail vẫn áp dụng; trường thiếu giữ mặc định hoặc giá trị đang có; config null; map options save/load qua đúng vị trí CSV. FirstChanceException observer bắt cả exception bị catch che, vì chỉ so final state có thể bỏ sót lỗi cũ.

Round-trip ở đây kiểm tra **hai map option qua CSV trong bộ nhớ**, không khẳng định mọi trường config legacy round-trip đúng hoặc encryption/file I/O đã chạy. Các control giả cũng không thay thế WinForms event, NumericUpDown validation hoặc runtime CLR 4. Source test ở `tests/ChickenAutoEx.Startup.Tests/UiSettingsRegressionTests.cs`; không thêm package, lockfile hoặc dependency ứng dụng.

## Tải và kiểm thử trên Windows 11

Với gói EAGLE mới, dùng [hướng dẫn tải/build hiện tại](ChickenAutoEx-BUILD.vi.md) và chạy EXE/config theo version ghi trong `BUILD-INFO.json`. Nhánh `repair/chickenautoex-107-ui-settings` và artifact `ChickenAutoEx-107-net48-review` là hồ sơ đợt sửa UI trước. Giải nén artifact, rồi ZIP ứng dụng bên trong; chọn đúng nhánh/commit và so SHA256.

1. Thử trong thư mục riêng theo `WINDOWS11.vi.md`, với config giả lập updater. Xác nhận mở/đóng form và updater vẫn hoạt động; không chép config/tài khoản từ stable 116 vào gói thử.
2. Trước test UI trên game, xác nhận bản build đọc đúng nhân vật, map và HP/MP trên client/server hợp lệ. Cloud chưa làm được bước này. Chưa coi việc tắt Auto là chế độ đọc thuần đã được kiểm chứng: code attach/hook cũ vẫn giữ nguyên.
3. Khi đã có nhân vật để thử, chọn lần lượt bốn tổ hợp checkbox bên dưới. Quan sát độc lập HP/MP, thông báo và trạng thái sau đổi nhân vật/đọc lại thiết lập. Ngưỡng và thuật toán hồi HP/MP vẫn là của 107.

| HP checkbox | MP checkbox | State HP mong đợi | State MP mong đợi |
|---|---|---|---|
| Tắt | Tắt | Tắt | Tắt |
| Tắt | Bật | Tắt | Bật |
| Bật | Tắt | Bật | Tắt |
| Bật | Bật | Bật | Bật |

4. Với config thử do chính bản build tạo, lưu hai lựa chọn map rồi đóng/mở và kiểm tra chúng giữ nguyên. `General.dat` có encryption, không sửa số phần tử bằng trình soạn thảo text. Các length 61/62/63 được kiểm chứng bằng harness; kiểm tra file legacy trên Windows cần fixture được tạo đúng định dạng, không cần gửi hoặc commit config thật của người dùng.
5. Ghi commit/build SHA, Windows/.NET version, client/server và kết quả từng ca. Không ghi password, token hoặc account config vào Git. Bản này chưa có kết quả chạy Windows/game và chưa phát hành như bản đã xác nhận tương thích.

## Nên làm gì tiếp?

Ưu tiên xác nhận **tương thích game và trạng thái nhân vật** trên bản mới. Nếu không nhận diện hoặc đọc sai dữ liệu, đợt tiếp theo nên thêm chẩn đoán có giới hạn cho đường đọc hiện có, phân biệt lỗi đọc với giá trị 0; chưa thay offsets/hook hoặc thuật toán phó bản trước khi biết nguyên nhân.

Khi baseline đạt, đối chiếu 116–107 từng tính năng thông thường: hồi HP/MP, pet, skill, bán kính, follow, loot. Ghi chỗ khác biệt thực tế rồi chọn một nhóm nhỏ để sửa. MicroAuto hữu ích để tham khảo các ca biên và cách đặt tên thiết lập. Mã phó bản và điều kiện VIP giữ nguyên trong các đợt này; kiểm thử từng phó bản là bước sau, với quyền/map/server phù hợp.
