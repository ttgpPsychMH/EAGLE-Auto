# Chẩn đoán tự động tích hợp — EAGLE Auto v0.11

Ngày 13-10-2026, Asia/Bangkok. Nhánh `repair/integrated-diagnostics`.

## Cách dùng, không cần command

1. Tải artifact **EAGLE-Auto-v0.11-net48-review** ở GitHub Actions, giải nén artifact rồi giải nén ZIP ứng dụng bên trong. Đóng phiên auto cũ, mở `EAGLE-Auto-v0.11.exe` cùng config/DLL trong gói.
2. Lần đầu, hộp thoại **Cho phép chẩn đoán EAGLE Auto** giải thích dữ liệu được ghi và dữ liệu không ghi. Chọn **Đồng ý** để tự bắt đầu, hoặc **Không** để tiếp tục dùng Auto mà không bật chẩn đoán. Enter/Escape mặc định Không; đóng hộp thoại cũng không đồng ý.
3. Lựa chọn được ghi nhớ. Các lần mở sau tự ghi nếu đã đồng ý với cùng chính sách; không hỏi lại nếu đã từ chối. Khi thay đổi chính sách thu thập, cần version policy mới để hỏi lại.
4. Dùng Auto như bình thường. Menu **Tiện ích → Chẩn đoán tự động** cho phép xem trạng thái, **Bật ghi nhận…**, **Tắt ghi nhận**, **Mở thư mục log**, **Dữ liệu được ghi**. Bật lại sau khi tắt luôn hỏi đồng ý.
5. Khi có lỗi, mở thư mục log bằng menu, chọn các file `eagle-diagnostics-….csv` gần nhất và gửi kèm mô tả thao tác/mốc thời gian. Không cần chạy sampler hay nhập PID. Auto **không tự gửi file cho ai**.

Thư mục mặc định: `%LOCALAPPDATA%\EAGLE Auto\Diagnostics`. Lựa chọn consent ở `consent.txt`; CSV là file chẩn đoán. Tối đa **4 file, mỗi file 2 MiB**, tổng xấp xỉ 8 MiB. Khi tạo/đổi file log, các CSV cũ do module này quản lý có thể bị xóa để giữ giới hạn. Tắt ghi nhận dừng thu thập, bỏ hàng đợi còn chờ; file đã có vẫn giữ lại. Có thể xóa CSV bằng Explorer. Không xóa file game/config.

Nếu không ghi được file, module tự dừng và menu báo lỗi; Auto vẫn giữ hành vi hiện có. Nếu không lưu được consent, có thông báo rằng lựa chọn chỉ áp dụng phiên hiện tại; kiểm tra trạng thái ở lần mở sau. Không có CSV trước consent. Một file consent ghi nhận lựa chọn Không không phải file thu thập lỗi.

## Nội dung file CSV

| Dữ liệu | Phạm vi |
|---|---|
| Thời gian UTC, version | Để đối chiếu phiên và thao tác. |
| CPU, working set, private bytes, handles | Auto và tối đa 32 cửa sổ game **đã xuất hiện trong danh sách Auto**. CPU chuẩn hóa theo số logical processors của tiến trình Auto. |
| Độ trễ giao diện | Dựa trên heartbeat UI 1 giây, quan sát nền mỗi 5 giây; chỉ là chỉ dấu UI bị chậm, không phải FPS/ping server. |
| Số cửa sổ Auto đã nhận | Không phải toàn bộ process game chưa được Auto nhận; client bị từ chối lúc khởi động có thể không xuất hiện. |
| Nhóm exception, HRESULT | Nhận exception ở code ứng dụng, kể cả một số lỗi sau đó được legacy catch xử lý; lỗi chưa xử lý là event riêng. |
| Cảnh báo và lý do chuẩn hóa | Từ cơ chế CanhBao hiện có, chỉ lưu nhóm tính năng và code hữu hạn, không lưu nội dung cảnh báo. |
| Trạng thái và phase | Snapshot bật/tắt từ cached fields của automation, combat, cấu hình skill đã nạp, chế đồ, pet, nhặt, hồi phục, Trừng Ác, Thủy Lao, Kỳ Cuộc, Ác Tặc, Ác Bá và reader hook. |

Window alias dạng `window_0001` được tạo trong bộ nhớ, không phải tên nhân vật/tài khoản/PID. Alias mô tả đối tượng cửa sổ Auto đã nhận, không khẳng định danh tính nhân vật. Dòng ProcessSample không có alias là Auto. PID/start-time chỉ dùng tạm trong bộ nhớ để đo đúng process, không ghi ra CSV. Khi PID được dùng lại, mẫu CPU đầu tiên của process mới không lấy delta từ process cũ.

**Không ghi vào CSV:** tài khoản, mật khẩu, token, tên nhân vật, HWID, địa chỉ server/IP, tọa độ, tên/vị trí skill/item/NPC, chat, gói tin, ảnh màn hình, command line, đường dẫn file, nội dung exception/Data/ToString hoặc stack trace đầy đủ. Chỉ dùng enums/giá trị số, version được kiểm tra và alias tự tạo. Không bám vào file log UI cũ để sao chép nội dung; các file `.dat` legacy và hành vi hiện có được giữ nguyên, nằm ngoài module CSV này.

## Giải thích các event

- `SessionStarted`: phiên ghi đã bắt đầu sau đồng ý.
- `ExceptionObserved`: quan sát exception, **chưa có nghĩa ứng dụng chết**. Một số exception có thể được engine xử lý.
- `UnhandledException`: CLR báo exception chưa được xử lý. Không thay cơ chế WinForms/JIT hiện có; cố ghi một dòng hữu hạn trước khi process có thể kết thúc.
- `Warning`: thông báo cảnh báo/lỗi hiện có được chuẩn hóa, hoặc API đo process bị thất bại. `reason` gồm `WaitingTimeout`, `MissingTarget`, `InvalidState`, `TeamChanged`, `OtherWarning`.
- `FeatureState`: có thay đổi trong snapshot trạng thái/phase. Không khẳng định thao tác game thành công, hoàn tất quest hoặc nhận thưởng.
- `ProcessSample` / `InterfaceSample`: số đo nền. CPU dòng đầu còn trống vì chưa có mẫu trước để lấy delta.
- `SamplesDropped`: tổng mẫu bị bỏ do hàng đợi/rate limit. Không được coi một file thiếu exception là bằng chứng không có lỗi.

`count` của FeatureState xác định cached field: 0 IsAuto, 1 IsAttack, 2 có cấu hình Skills, 3 IsCheDo, 4 IsPet, 5 IsPickItem, 6 có ít nhất một toggle hồi phục HP/MP/NM, 7 Trừng Ác, 8 Thủy Lao, 9 Kỳ Cuộc, 10 Ác Tặc, 11 Ác Bá, 12 IsHooked. **Skills On nghĩa có cấu hình được nạp**, không xác nhận skill đã được dùng. **Hook On là cờ managed**, không xác nhận DLL đã trả ACK.

Phase là ordinal của enum engine hiện có: Trừng Ác Scan/Info/TravelTarget/Spawn/Combat/AcceptNpc/ReturnNpc/AcceptDialog/ReturnDialog/AcceptAck/ReturnContinue/ReturnComplete/ReturnAck; Kỳ Cuộc Entry/Patrol/WaitingExit/Exit; Ác Tặc Entry/Patrol/AwaitBoss/AwaitExit; Ác Bá Entry/Patrol/AwaitBoss/AwaitExit. Thủy Lao phase0 là không theo dõi phase và tính năng vẫn chưa bật. Reader hook dùng ReaderObservation: NotRequested/RequestQueuedNoAck/DataObserved/RestoreFailed/RestoreWrittenNoAck. Chỉ lưu trạng thái reader; không lưu bytes/payload/chat.

## Phạm vi và giới hạn

Module dùng FirstChanceException và UnhandledException, không thêm ThreadException handler để thay hộp thoại lỗi mặc định. Chỉ đọc một số frame của assembly ứng dụng để phân loại, không lưu frame names/path; loại ngữ cảnh Debug/auth và helper crypto/config được chỉ định. Lỗi trước khi form/module khởi tạo hoặc trước khi người dùng đồng ý sẽ không được ghi. Không thể bảo đảm ghi kịp khi OS kill, mất điện hoặc crash native nghiêm trọng.

Đây là chẩn đoán **nhiều nhóm tính năng**, không phải bộ kiểm chứng mọi lỗi. Lỗi logic không throw/không phát cảnh báo, API native trả false bị legacy bỏ qua, tương thích offsets/server và thay đổi trạng thái quá nhanh giữa hai mẫu vẫn có thể không được nhận biết. Snapshot 1 giây/đo 5 giây có thể bỏ qua chuyển trạng thái ngắn; cached fields được đọc độc lập, không phải transaction của engine. Không thu thập mọi hành động game.

Hàng đợi tối đa 256 dòng, giới hạn exception/cảnh báo quan sát 60 lượt mỗi phút để tránh error storm. Số bị bỏ được báo khi sampler còn hoạt động. Writer chạy nền; tắt ghi nhận chờ một thao tác ghi đang diễn ra kết thúc rồi bỏ hàng đợi. Không chỉnh flow/catch của automation để nuốt lỗi, không khởi động thêm game, không thay DLL/ABI/offsets, giới hạn client, licensing/VIP hoặc stable116. Các phó bản được bảo toàn so với v0.10.

## Kiểm thử Windows ngắn

1. Chạy lần đầu chọn Không: Auto mở bình thường, menu báo Đang tắt, không tạo CSV mới.
2. Bật qua menu, chọn Đồng ý: sau khoảng 5–10 giây mở thư mục log và thấy SessionStarted/ProcessSample/InterfaceSample.
3. Khi có game, kiểm tra số cửa sổ đã nhận và nhãn alias; bật/tắt một tính năng thường đã quen dùng để đối chiếu FeatureState.
4. Nếu gặp lỗi tự nhiên, giữ mốc thời gian và file CSV; không cố gây crash, không dùng tài khoản/credentials giả để ép lỗi.
5. Tắt ghi nhận: kích thước/mtime CSV dừng thay đổi; đóng/mở Auto vẫn Đang tắt. Bật lại thì hỏi consent lại.
6. Chọn Đồng ý, đóng/mở Auto: tự ghi phiên mới theo lựa chọn được lưu. Có thể tắt bất cứ lúc nào.
7. Nếu thông báo lỗi/JIT xuất hiện, logger không được tự loại bỏ nó. Thao tác đóng/tiếp tục vẫn theo ứng dụng hiện có.

Các kiểm thử trong cloud chạy core logger, file rotation, consent và các đoạn UI/cached projection với fake dependencies. **Không chạy EXE automation, DLL game hoặc WinForms trên Linux; Windows runtime chưa kiểm thử.** Dùng bản review trong thư mục riêng.
