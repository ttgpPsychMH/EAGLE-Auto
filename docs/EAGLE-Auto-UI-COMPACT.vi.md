# EAGLE Auto v0.9 — giao diện gọn

Ngày 12-10-2026 (Asia/Bangkok). Bản review net48/x86, kế thừa v0.8 và các engine đã sửa đến v0.7. Nhánh `repair/eagle-auto-compact-ui`; file chạy `EAGLE-Auto-v0.9.exe`.

## Cách bố trí

Cửa sổ mặc định có vùng client **560 × 700** tại kích thước thiết kế, thay cho 880 × 760 của v0.8. Danh sách nhân vật và các chỉ số nằm cạnh nhau ở trên; các tab cấu hình dùng toàn bộ chiều ngang bên dưới. Cửa sổ vẫn đổi kích thước được; khi mở, kích thước/vị trí được giới hạn trong vùng làm việc của màn hình.

Tám tab: **Cơ bản, Kỹ năng, Vật phẩm, Pet, Tiện ích, Chế đồ, Đăng nhập, Nhiệm vụ**. Tab Cơ bản chia thành Đánh quái, Di chuyển, Hồi phục; nhóm được giữ nguyên để bảo toàn các radio/handler. Combo map/bãi giãn theo chiều rộng, các nút hành động ở bên phải. Các tab cũ có thanh cuộn khi thiếu chỗ. Nhãn ngắn, nền trắng/xám và màu nút chuẩn của Windows thay cho nhiều nút màu nổi.

**Điều khiển chung** mặc định gập. Bấm để mở các thao tác dùng chung; không kích hoạt automation chỉ vì mở panel. **Nhật ký** có thể gập/mở. Khi cửa sổ thấp hoặc panel chung đang mở, phần nhật ký tự thu để chừa vùng tab; nó hiện lại khi có chỗ nếu nút Nhật ký còn được chọn. Nút hiện “Nhật ký (thiếu chỗ)” trong trường hợp đó. Nội dung log vẫn được lưu trong control, không bị xóa khi gập.

## Nhân vật và trạng thái trống

Chọn **một dòng** để cấu hình; ô tick là bật/tắt Auto. Nút **Bỏ chọn** chỉ bỏ lựa chọn cấu hình, không tắt Auto.

Khi không có dòng được chọn, chọn nhiều dòng, actor đã rời danh sách hoặc đang chờ nhận diện/online, HP/MP/Pet/vị trí/online hiển thị **—**. Header và status dùng nhãn riêng, không lấy giá trị mẫu 10% hoặc `CurGame` còn giữ từ lần chọn trước. Giá trị phần trăm/tọa độ không hợp lệ cũng hiện —; thời gian online được định dạng giờ:phút:giây.

Các control cấu hình nhân vật trong sáu tab đầu bị khóa khi không có một nhân vật online đã khởi tạo và còn thuộc danh sách. **Đăng nhập và Nhiệm vụ vẫn mở được**; bảng nhiệm vụ tiếp tục tự kiểm tra actor/leader bằng adapter v0.8. Nhân vật online có thể cấu hình khi Auto đang tắt. Việc khóa/mở chỉ đổi `Enabled`, không xóa hoặc reset checkbox, numeric hay cấu hình đã lưu. Nếu đọc header lỗi hoặc danh tính/init thay đổi giữa lúc đọc, header khóa lại và chờ refresh thành công.

Tooltip cho tên/vị trí/status giúp đọc nội dung bị rút gọn trên cửa sổ hẹp.

## Phạm vi điều khiển chung

| Thao tác | Phạm vi giữ từ engine/handler cũ |
|---|---|
| Nhặt | Các nhân vật được quản lý bởi Auto |
| Pet / Theo / Skill | Cài đặt dùng chung |
| Đảo bật/tắt Auto | Đảo từng nhân vật: bật → tắt, tắt → bật |
| Triệu tập các đội | Đưa các tổ đội về từng đội trưởng |

Đây không phải các nút chỉ tác động nhân vật đang chọn. Tooltip/nhãn được sửa để thể hiện rõ phạm vi. Nút Triệu tập bị khóa khi chưa có nhân vật được chọn hợp lệ. Các handler không được viết lại.

## Bảo toàn và kiểm thử

Giữ nguyên thuật toán phó bản, adapter nhiệm vụ v0.8, chẩn đoán hook, native DLL, licensing/VIP/auth, stable116, protocol107 và CLR AssemblyVersion1.0. Thủy Lao vẫn chưa khả dụng. Tab 0–6 giữ đúng thứ tự để handler Chế đồ/Đăng nhập không bị đổi index; chỉ caption tab đầu đổi cố định thành Cơ bản. Verifier đảo ba delta FrmMain rồi so toàn bộ baseline; hash engine/hook v0.7 và adapter v0.8 được giữ riêng.

Kiểm thử C# chạy model hiển thị/budget vùng phụ và trích các method UI thật để compile với Game/control giả. Có tripwire nếu method refresh ghi checkbox/value của settings. Nó kiểm tra bỏ chọn, multi-select, offline, khôi phục, mất registration/identity/init và giữ quyền truy cập Đăng nhập/Nhiệm vụ. Các bộ updater/dungeon/hook cũ tiếp tục chạy.

[Mẫu HTML tương tác](ui/EAGLE-Auto-v0.9-preview.html), [ảnh có nhân vật giả](ui/EAGLE-Auto-v0.9-selected.png) và [ảnh bỏ chọn](ui/EAGLE-Auto-v0.9-empty.png) là **thiết kế với dữ liệu giả**, không phải ảnh WinForms chạy trên Windows. Gói có `UI-PREVIEW-FAKE-DATA.html`; mở trong trình duyệt để xem thử. HTML không kết nối game và không dùng trong EXE. Playwright/Chromium đã kiểm tra 45 tương tác; đây là kiểm tra tùy chọn, không cần để build/test C#.

## Kiểm tra Windows 11 sau này

Giải nén gói review vào thư mục riêng; giữ EXE/config/DLL cùng nhau. Kiểm tra ở DPI100/125/150%: mở, thu nhỏ/khôi phục, kéo về kích thước nhỏ, mở/gập điều khiển chung và nhật ký, chuyển qua tám tab. Nội dung có thể cuộn; cửa sổ không vượt vùng làm việc; không có JIT dialog. Khi game đóng, header phải hiện —, sáu tab cấu hình bị khóa, Đăng nhập/Nhiệm vụ vẫn truy cập được.

Trong lượt kiểm tra client đã thống nhất sau khi rà soát phụ bản, đối chiếu tên/chỉ số/vị trí với game; thử chọn/bỏ chọn/multi-select/đóng cửa sổ game; xác nhận checkbox/numeric vẫn lưu như trước. Bỏ chọn không được tắt Auto. Kiểm tra phạm vi các thao tác chung với vài nhân vật có trạng thái Auto khác nhau. Tiếp tục checklist nhiệm vụ v0.8 và hook v0.7; không suy ra kết quả game từ “đã gửi yêu cầu”. Không đưa credential/config game vào báo cáo.

Cloud chưa thực thi automation EXE, native DLL hoặc WinForms. Build/mô phỏng không xác nhận rendering, DPI hoặc gameplay trên Windows; đây chưa phải bản phát hành đã kiểm chứng.
