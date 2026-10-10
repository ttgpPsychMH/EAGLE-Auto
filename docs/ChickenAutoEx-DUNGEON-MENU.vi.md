# ChickenAutoEx 107: sửa menu phó bản khi thiếu nhân vật/đội trưởng

Nhánh `repair/chickenautoex-107-dungeon-menu` kế thừa `e4161da` (startup net48 và hai lỗi MP/config). Đợt này sửa lớp UI trong FrmMain; engine Game.cs, scripts, offsets, native hook, licensing/VIP, format config và stable 116 giữ nguyên.

## Lỗi đã xác định

Người dùng mở được bản trước nhưng bấm Trừng Ác khi chưa thử game thì gặp NullReferenceException ở `ItemTrungAc_Click`. Nhánh `Leader == null` vẫn ghi `Leader.IsTrungAc = false`. Menu Trừng Ác không được vô hiệu hóa cùng nhóm menu khác, nên có thể bấm khi thiếu ngữ cảnh.

Rà soát còn thấy thông báo Trừng Ác/Kỳ Cuộc đọc cờ Lâu Lan; tiêu đề lỗi Trừng Ác là “Lỗi Thủy Lao”; menu Ác Tặc/Dừng Ác Tặc có đường truy cập Leader khi không có đội trưởng. Các click chọn map còn xóa IsTrieuTap của các game trước khi kiểm tra đội trưởng. Riêng Ác Bá reset cờ rồi đảo cờ nên luôn bật, không tắt được bằng click.

## Thay đổi

- Lấy nhân vật và đội trưởng vào biến cục bộ, kiểm tra metadata và việc lựa chọn bị đổi trong lúc lấy đội trưởng. Nếu thiếu ngữ cảnh thì cập nhật menu về trạng thái không khả dụng, hiển thị thông báo đúng nhiệm vụ và dừng trước khi thay đổi game.
- Cập nhật menu khi **DropDownOpening**, áp dụng cho cả chuột và bàn phím. Trừng Ác mặc định disabled; khi nhận diện được đội trưởng thì nhóm menu được cập nhật theo ngữ cảnh.
- Enabled/Checked của nhóm menu và dấu tick map Ác Tặc được dựng lại từ cờ của đội trưởng. Khi đổi nhân vật hoặc mất lựa chọn, UI không giữ dấu tick cũ. Mở menu chỉ đọc state, không đổi cờ game.
- Trừng Ác, Kỳ Cuộc và Lâu Lan dùng đúng cờ cho bật/tắt, dấu tick và thông báo. Ác Bá lưu trạng thái bật/tắt mong muốn trước khi reset, giữ chính sách reset các nhiệm vụ khác và map như cũ.
- Chọn map Ác Tặc chỉ thay đổi game sau khi có ngữ cảnh hợp lệ; giữ sáu map và cách chọn ngẫu nhiên cũ. Dừng Ác Tặc chỉ xóa map đó. Thuỷ Lao giữ cờ false và thông báo **“Not work”**; không kích hoạt implementation đang chưa hoạt động.

Không thêm cơ chế tạo đội, cấp VIP, mở khóa stub, đổi script/NPC/map offset hay thay thuật toán thực thi phó bản. Cờ tác vụ được thay đổi theo click UI hợp lệ; đây không phải xác nhận tác vụ đó chạy thành công trong game.

## Bằng chứng kiểm thử

- **38 test menu** dùng các method production với dependency giả. Cùng bộ test trên source trước sửa: **32 failed, 6 passed**. Sau sửa: **38 passed**.
- Toàn suite: **94 passed, 0 failed, 0 skipped**; gồm 38 menu, 19 UI/settings và 37 updater/schema. Updater vẫn được thử qua dịch vụ HTTP/TLS giả lập loopback.
- Build đầy đủ Release net48/x86 trên Linux: **0 lỗi, 50 cảnh báo legacy**. Không chạy executable automation hoặc game.
- Static verifier kiểm tra 27 resource, sáu binary gốc, dependencies và source engine/licensing. [Manifest review](../tools/dungeon_menu_review.json) ghi hash LF-normalized của 16 method UI thay đổi, 5 helper mới và hai delta designer. Verifier kiểm tra hash rồi khôi phục đúng các đoạn này để đối chiếu toàn bộ legacy tail, cùng hai sửa MP/config trước đó.

Harness mở rộng ở `UiSettingsRegressionTests.cs`; test mới ở `DungeonMenuRegressionTests.cs`. Roslyn chỉ biên dịch method UI/settings và helper thuần với fake game, checkbox/menu, notification, màu UI, key/map constants và I/O bộ nhớ. Harness mô phỏng CheckOnClick trước khi gọi handler; không load assembly automation, constructor thật, Win32, memory reader, Lua hoặc auth. Source trước sửa chỉ được thay vào input test đã ignore; source ứng dụng không bị rollback để test.

Ca kiểm tra: không có nhân vật; không có đội trưởng; mất lựa chọn sau khi mở menu; đổi lựa chọn trong lúc lấy đội trưởng; metadata null; đổi nhân vật; bật/tắt, thông báo và dấu tick; Ác Bá reset; Thủy Lao vẫn không hoạt động; map cụ thể/ngẫu nhiên và dừng Ác Tặc; binding DropDownOpening trong designer. Test event binding là kiểm tra source, không phải đã chạy WinForms trên Windows.

Kết quả local và hash gói ở `docs/evidence/ChickenAutoEx-dungeon-menu-validation.json` trong repository. Hash gói CI nằm trong artifact của chính lần chạy; không mặc định trùng hash local.

## Tải và kiểm thử Windows 11

Trong [GitHub Actions](https://github.com/ttgpPsychMH/EAGLE-Auto/actions/workflows/chickenautoex-review-build.yml), chọn lần chạy xanh của nhánh **repair/chickenautoex-107-dungeon-menu**, tải **ChickenAutoEx-107-net48-review**. Giải nén artifact, rồi ZIP ứng dụng bên trong. Tên ZIP giữ `ChickenAutoEx-107-startup-net48.zip`; chọn đúng nhánh/commit, kiểm tra file SHA256 và dùng thư mục thử riêng.

| Ca | Kết quả cần quan sát trên Windows |
|---|---|
| Chưa mở/chọn game; mở menu bằng chuột hoặc bàn phím | Nhóm phó bản disabled, Trừng Ác không bấm được, không có dialog exception. |
| Đã chọn nhân vật nhưng chưa nhận diện được đội trưởng | Menu unavailable, trạng thái báo chưa nhận diện đội trưởng, không giữ dấu tick cũ. |
| Có nhân vật và đội trưởng được app nhận diện | Menu phản ánh cờ của đúng đội trưởng. Chưa kết luận thuật toán phó bản chạy được. |
| Mất lựa chọn trong lúc menu đang mở | Click còn sót không gây exception hoặc thay đổi game; mở lại menu hiển thị trạng thái mới. |
| Đổi nhân vật/đội trưởng rồi mở lại menu | Dấu tick lấy từ nhóm mới; không sửa các cờ nhóm cũ chỉ vì đổi lựa chọn. |
| Bật/tắt Trừng Ác, Kỳ Cuộc, Lâu Lan, Ác Bá | Thông báo và dấu tick khớp trạng thái. Ác Bá vẫn reset các cờ nhiệm vụ khác theo chính sách cũ. |
| Chọn/dừng map Ác Tặc | Một map thực được đánh dấu; dừng thì các tick map được xóa. |
| Thủy Lao | Giữ thông báo “Not work”, không giữ dấu tick bật. |

Làm ca đầu tiên **không có game** trước. Các ca có đội trưởng chỉ làm sau khi app nhận diện đúng client/server, nhân vật, map, HP/MP. Game.Leader hiện được xác định từ các game mà app theo dõi; UI thiếu đội trưởng chưa chứng minh nhân vật trong game thực sự không có tổ đội. Chưa có kết quả Windows của bản mới, test file config thật hoặc hook/game trong cloud.

Không coi tắt checkbox Auto là bảo đảm không gắn hook/ghi memory; cơ chế cũ vẫn được giữ. Nếu cần chế độ chỉ quan sát đúng nghĩa, phải thiết kế và kiểm chứng riêng, chưa có trong patch này. Ghi build SHA và từng kết quả thực tế; che account/password/token khi gửi log, không commit config thật.

## Việc tiếp theo

1. Xác nhận ca mở menu không có game trên Windows hết lỗi đã báo.
2. Xác nhận app nhận diện client/server và đọc đúng nhân vật/map/HP/MP, rồi đội trưởng. Nếu sai hoặc không nhận diện, ưu tiên chẩn đoán đường nhận diện/đọc trạng thái trước khi sửa phó bản.
3. Khi baseline đạt, thử từng tính năng thông thường HP/MP, pet, skill, radius, follow; đối chiếu 116–107 trên cùng client để chọn lỗi hoặc thiếu hụt cụ thể.
4. Sau đó mới thử từng phó bản có quyền hợp lệ, ghi map/đội/điểm dừng. Giữ các guard VIP và các module/stub hiện có cho tới khi có bằng chứng runtime và một phạm vi sửa riêng.
