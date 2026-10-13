# EAGLE Auto v0.13 — Căn Ô, Nút Và Aptos

Ngày: 13-10-2026 (Asia/Bangkok). Nhánh `branding/heading-layout-aptos`, kế thừa v0.12.

Các tab chính và tab kỹ năng con dùng Heading 1: chữ HOA, 11 pt, đậm. “ĐIỀU KHIỂN CHUNG” ở thanh công cụ và nhóm điều khiển cũng dùng Heading 1. Font header được gán cho TabControl; nội dung TabPage giữ font thường để không khiến toàn trang bị đậm. Tên riêng trong game giữ nguyên.

## Căn Chỉnh

Các nhóm cài đặt kế thừa trên các tab chính được đăng ký vào `UiFixedLayout`. Nó ghi lại hàng/cột gốc, đo chữ sau khi áp dụng font, rồi dùng `RowLayoutPolicy` để chừa khoảng trống giữa nhãn, ký hiệu, ô nhập và nút. Các cột cùng loại ở nhiều hàng giữ thẳng nhau; một nhãn dài có thể dùng khoảng trống của các cột không có ô nhập ở hàng đó. Không lấy kích thước đã tăng làm đầu vào cho lần sau, tránh ô/nút tiếp tục lớn lên mỗi lần refresh.

Nhóm cơ bản dùng hàng AutoSize, không giữ chiều cao cố định của bản cũ. Chiều cao nhóm được tính theo nội dung và legend. Các ô số tối thiểu 60 px ở DPI ban đầu; nút có khoảng đệm riêng. Nút di chuyển ở cột phải và combobox giữa có thể giãn. Cửa sổ mặc định 660 × 800, có giới hạn theo vùng màn hình. Khi thiếu diện tích, nội dung có thanh cuộn; không dùng dấu ba chấm để giấu caption của các nút đã được căn theo hàng. Phần điều khiển chung cuộn bên trong vùng được cấp, không ép chiều cao tối thiểu che lên vùng tab.

Đối tượng control, parent của radio, thứ tự tab, handler, Checked, Value và SelectedIndex giữ nguyên. Chỉ thay vị trí, kích thước, anchor và vùng cuộn. Không thay engine nhiệm vụ/phó bản, kỹ năng, chế đồ, licensing/VIP, hook native hoặc stable 116.

## Dùng Aptos Không Cần Command

1. Giải nén bộ **Microsoft Aptos Fonts.zip** đã có trên máy của anh.
2. Trong Auto, mở **Tiện Ích → Về EAGLE Auto → Chọn font…**.
3. Chọn thư mục chứa `Aptos.ttf`, `Aptos-Bold.ttf`, `Aptos-Italic.ttf`, `Aptos-Bold-Italic.ttf`.
4. Khi thuận tiện, đóng và mở lại Auto. Auto không tự khởi động lại để tránh ngắt phiên đang dùng.
5. Mở Về EAGLE Auto, kiểm tra dòng **Aptos: 4/4 kiểu đã nạp**. Nếu đang dùng Aptos cài trên Windows, trạng thái ghi rõ nguồn Windows.

Lựa chọn thư mục được lưu ở `%LOCALAPPDATA%\EAGLE Auto\Appearance\font-directory.txt`, chỉ trên máy người dùng; không đưa đường dẫn vào CSV chẩn đoán, không gửi dữ liệu. Không sao chép/cài font hệ thống, không ghi registry, không cần admin. Nguồn ưu tiên: thư mục đã chọn hợp lệ → `Fonts` cạnh EXE → Aptos đã cài trên Windows → Segoe UI dự phòng theo từng kiểu chữ. Kiểm tra trước khi chọn gồm tên file, chữ ký SFNT và giới hạn dung lượng; native GDI/GDI+ kiểm tra khả năng nạp và style khi app mở lại. Chữ ký SFNT riêng không chứng minh toàn bộ TTF hợp lệ.

Font thường được nạp cho cả GDI và GDI+ bằng cơ chế có sẵn; tài nguyên font được giữ đến khi đóng app và giải phóng lúc thoát. Số kiểu chữ được lọc theo khả năng style thật của family Aptos. Thanh tiêu đề, MessageBox và hộp chọn thư mục chuẩn vẫn dùng font hệ điều hành.

### Vì Sao Gói GitHub Không Nhúng TTF

Bộ font được tải miễn phí vẫn có [EULA đã kiểm tra](analysis/EAGLE-Auto-APTOS-METADATA.vi.md), mục **No Distribution Rights**:

> This agreement does not grant you a license to distribute nor sublicense all or part of the software to any third party.

Cờ Editable embedding (`fsType=0x0008`) không tự cấp quyền đóng font vào một executable để phân phối. Bản này dùng font riêng do người dùng cung cấp; không nhúng hoặc phát hành TTF qua EXE, ZIP hay GitHub. Nếu có giấy phép riêng cho phép phân phối trong ứng dụng, có thể đánh giá bản nhúng sau.

## Bằng Chứng Và Kiểm Tra Windows

Build net48/x86: 0 lỗi, 49 cảnh báo legacy. **773 test .NET passed**, gồm 26 ca mới về layout và chọn font; verifier xác nhận bảo toàn source/binary, 29 resource và không nhúng font. **83 kiểm tra HTML/font/bố cục mẫu passed**, có nạp bốn file Aptos thật trong bộ nhớ Chromium và kiểm tra glyph tiếng Việt của bốn face.

[Mẫu HTML](ui/EAGLE-Auto-v0.13-preview.html) và [ảnh](ui/EAGLE-Auto-v0.13-selected.png) dùng dữ liệu giả. Mô hình control/GDI của test là dependency giả; không chạy EXE Auto, DLL game hay font native trên Linux. Windows 11, WinForms, DPI thật và game chưa được kiểm thử trong lượt này. Không tuyên bố mọi bố cục đã qua Windows test chỉ dựa vào mẫu HTML.

Trên Windows: kiểm tra tám tab chính, hai tab kỹ năng con, điều khiển chung, Về EAGLE Auto; chọn thư mục font rồi xác nhận trạng thái sau khi mở lại. Thử DPI 100/125/150/200%, thay đổi kích thước, cuộn ngang/dọc và đổi trạng thái nút Trị liệu. Chữ không được chồng lên ô số/nút; các nhóm radio và thông số cũ phải giữ giá trị. Nếu có vị trí còn lỗi, gửi ảnh cùng DPI và tên tab/cửa sổ để sửa đúng vị trí.
