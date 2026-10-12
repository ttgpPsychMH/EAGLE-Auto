# Kiểm thử nhiều client trên Windows 11

Đây là gói **chẩn đoán riêng**, không phải bản auto mới. Auto vẫn v0.10 với cơ chế và giới hạn hiện có. Licensing/VIP, game/server controls được giữ nguyên. Không cần cài .NET 8; sampler dùng .NET Framework 4.8 đã có trên máy chạy EAGLE Auto.

## Chuẩn bị

1. Giải nén `EAGLE-MultiClient-Diagnostics.zip` vào thư mục riêng. Giữ file game/auto đang dùng và cấu hình gốc. Không thay DLL.
2. Ghi CPU, RAM, GPU, bản Windows, scale màn hình, độ phân giải game; ghi SHA256 Game.exe bằng `Get-FileHash` nếu tiện. Không cần gửi file game, account, password hay command line.
3. Mở một phiên EAGLE Auto v0.10. Mở client thủ công bằng cách anh đang sử dụng. Tool chẩn đoán không giúp vượt giới hạn; nếu auto/client từ chối, ghi nhận và dừng mức đó.
4. Tìm PID trong Task Manager → Details. Hoặc PowerShell:

```powershell
Get-Process -Name Game,GameOLD -ErrorAction SilentlyContinue | Select-Object Id,ProcessName
```

PID thay đổi sau mỗi lần mở lại. Không dùng PID của ví dụ. Ghi PID auto bằng Task Manager; nếu không cần đo auto thì bỏ `--auto`.

## Kiểm tra sampler trước

Chạy `EAGLE-MultiClient-Monitor.exe --help`. Sau đó dùng PID **một** client đang mở, đo 15 giây. Ví dụ, thay các số bằng PID thật:

```powershell
.\EAGLE-MultiClient-Monitor.exe --games 1234 --auto 5678 --seconds 15 --interval 5 --out smoke.csv
```

File phải có dòng `status="sampled"`, working/private memory hợp lý. CPU lượt đầu trống vì chưa có mẫu trước. `responsive` xác nhận cửa sổ chính trả lời WM_NULL; không xác nhận gameplay. Nếu mọi dòng đều query failed/missing, kiểm lại PID và quyền đọc trước khi dùng kết quả. Không nâng quyền hoặc thay game để làm mất cảnh báo; ghi rõ điều kiện kiểm thử.

Sampler không ghi đè CSV có sẵn; dùng tên file mới. Ctrl+C dừng đo. Nếu CSV có `pid_reused_not_sampled`, đóng sampler và lấy lại PID. Nếu `timeout_or_denied`, cần phân biệt cửa sổ treo và quyền UIPI; không tự coi là game crash. Sampler chưa được chạy trên Windows trong cloud; bước smoke này là xác minh runtime đầu tiên.

## Các mức 2 → 3 → 4 → 6

Chỉ tăng mức khi mức trước đã ổn định. Ở mỗi mức thực hiện theo thứ tự:

| Pha | Thời gian đề nghị | Việc cần xác nhận |
|---|---:|---|
| Khởi động tuần tự | Chờ từng client có cửa sổ | Số process/cửa sổ, cảnh báo, client tự tắt, vị trí chờ login. |
| Đứng yên, chưa bật automation | 180 giây | Độ phản hồi, RAM/CPU, tổng GPU/VRAM bằng Task Manager. |
| Đã vào game, chưa bật phó bản | 180 giây | Đăng nhập được tất cả client, chuyển focus/chuyển tab auto vẫn bình thường. |
| Tính năng thường đã quen dùng | 180 giây | Trạng thái từng nhân vật, UI auto có lag/đứng; giữ cấu hình giống giữa các mức. |

Ví dụ cho hai client, thay PID thực tế:

```powershell
.\EAGLE-MultiClient-Monitor.exe --games 1234,2345 --auto 5678 --seconds 180 --interval 5 --out clients-2-idle.csv
```

Tương tự cho 3, 4, 6 client: danh sách đúng PID đang sống, tên file khác cho từng mức/pha. Không đặt `--games` gồm sáu số giả để coi là đã kiểm sáu cửa sổ.

Sau khi các mức tuần tự ổn định, thử yêu cầu mở thêm sát nhau **trong phạm vi số lượng anh định thử**, qua thao tác auto đang hỗ trợ. Ghi mỗi lần bấm và số PID thực xuất hiện. Nếu auto từ chối thì ghi `blocked_auto_policy`; nếu game hiện cảnh báo thì ghi `blocked_client_warning`. Không bỏ điều kiện licensing/VIP, không sửa binary và không tiếp tục ép mở khi bị từ chối. Các mức 4/6 có thể không đạt với auto/client hiện tại; đó là kết quả cần giữ lại.

Kiểm thêm tình huống đóng **thủ công** một client rồi mở lại: danh sách auto có xóa PID cũ không, PID mới có được nhận đúng không. Với khởi động chậm, ghi thời gian chờ; không thay config/auth hoặc gây treo game để tạo lỗi. Có thể ghi một trường hợp game tự mở thất bại nếu đã xảy ra tự nhiên. Các ca thất bại nhân tạo/race mạnh đã được kiểm trong mô phỏng, không cần cố gây trên game thật.

Không test phó bản trong đợt đo này để tránh trộn lỗi mở client và lỗi automation. Chỉ tiếp tục kiểm phó bản khi quản lý cửa sổ ổn định.

## Dừng và báo kết quả

Dừng mức thử nếu game tự tắt, UI không phản hồi kéo dài, RAM/VRAM liên tục gần hết hoặc xuất hiện cảnh báo. Giảm mức bằng cách đóng game thủ công bình thường. Không coi việc đóng được dialog là mở client thành công: cần cửa sổ tồn tại ổn định và đăng nhập được.

Điền `results-template.csv`: mỗi mức/pha một dòng. Gửi CSV đo và ghi ngắn số cửa sổ thực tế, bước gây cảnh báo/lỗi, nội dung thông báo. CSV sampler không chứa tên nhân vật/tài khoản. Nếu gửi ảnh khác, che thông tin tài khoản. Chưa có ngưỡng CPU/RAM đạt cố định; chọn trần sau khi so tốc độ phản hồi và tài nguyên của chính máy anh.

Kết quả cần báo riêng: “mở được cửa sổ”, “vào được game”, “automation thường hoạt động”. Không dùng một trong ba để thay thế hai kết quả còn lại. Sáu là mục tiêu kiểm thử; chưa có kết luận sáu client đã được hỗ trợ hay không giới hạn.
