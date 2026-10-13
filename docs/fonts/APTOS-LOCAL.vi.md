# Dùng Aptos trong EAGLE Auto

Gói EAGLE Auto không kèm font TTF. Bộ ZIP anh cung cấp có điều khoản **No Distribution Rights**; việc đóng gói giữ các file font ở máy người dùng.

Từ v0.13, cách tiện nhất: **Tiện Ích → Về EAGLE Auto → Chọn font…**, chọn thư mục chứa bốn tệp đã giải nén rồi mở lại Auto. Xem [hướng dẫn v0.13](../EAGLE-Auto-LAYOUT-APTOS.vi.md). Không cần command hay quyền admin.

Các cách cũ vẫn dùng được:

- Nếu Windows đã nhận diện family **Aptos**, app tự dùng font đó.
- Để dùng riêng trong EAGLE Auto, tạo thư mục `Fonts` cạnh EXE, chép bốn file từ bộ ZIP đã có:

```text
EAGLE-Auto-v0.13.exe
EAGLE-Auto-v0.13.exe.config
Fonts/
  Aptos.ttf
  Aptos-Bold.ttf
  Aptos-Italic.ttf
  Aptos-Bold-Italic.ttf
```

Sau khi chép, đóng app và mở lại. Không cần cài font vào Windows hay quyền admin để nạp riêng. Không đổi tên thành Aptos Display/Narrow/Serif. Có thể giữ những file khác trong thư mục, nhưng app chỉ nạp các face Aptos nền đã liệt kê.

Mở **Tiện ích → Về EAGLE Auto** để xem font đang dùng. Thiếu font, kiểu chữ hoặc nạp lỗi sẽ dùng Segoe UI dự phòng. Chỉ chép `Aptos.ttf` có thể khiến các chữ đậm/nghiêng phải dùng font khác; chép đủ bốn file để giữ đồng nhất.

App giữ font đến lúc đóng, không sửa registry, không cài đặt font toàn hệ thống và không tải font qua mạng. Thanh tiêu đề, MessageBox, tooltip và hộp thoại file/folder chuẩn của Windows vẫn dùng font do Windows chọn.
