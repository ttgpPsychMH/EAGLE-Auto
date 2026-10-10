# EAGLE Auto v0.1

Bản hiển thị mới của project khôi phục Chicken Auto 107. Nhánh `branding/eagle-auto-v0.1` kế thừa sửa startup net48, MP/config, menu phó bản và hồ sơ so sánh 107–116. Thông tin giới thiệu:

> Được sửa lại dựa trên Chicken Auto 107, vibe coding bằng Codex bởi tenkafuku.

## Thay đổi hiển thị

- Tiêu đề application và tooltip tray: **EAGLE Auto v0.1**.
- Menu **Về EAGLE Auto**: hộp giới thiệu có tên/version và đúng câu trên, thay màn QR/hardware ID trước đây.
- Màn **Thông tin cập nhật EAGLE Auto**: ghi các sửa của fork v0.1; không trình bày changelog ChickenAuto cũ như lịch sử của EAGLE.
- Caption thông báo đường dẫn game/xóa account/lỗi khởi động và thông báo đã có Auto khác đang chạy dùng tên mới. Hành vi xác nhận và mutex cũ giữ nguyên.
- File Properties: Product/File description EAGLE Auto, Company tenkafuku, FileVersion `0.1.0.0`, ProductVersion `v0.1`. Copyright gốc và LICENSE vẫn được giữ; tên người sửa không thay quyền tác giả của source gốc.

`v0.1` là phiên bản hiển thị của fork. `Global.Version = "107"` vẫn là mã legacy dùng trong metadata cập nhật/báo cáo/giao thức; không gán `v0.1` vào trường số này. Log cập nhật gọi rõ là dữ liệu **bản gốc**, không coi remote 107/108 là phiên bản EAGLE. Endpoint, parser, timeout và logic kiểm tra cập nhật không đổi. Assembly identity, namespace/resource name, native hook, format config, HWID/licensing/VIP, thuật toán và script game/phó bản giữ nguyên. Lớp QR cũ vẫn tồn tại; chỉ menu giới thiệu chuyển sang nội dung mới.

Icon gốc được giữ. Tên file chạy vẫn là **ChickenAutoEx.exe**, đi kèm `ChickenAutoEx.exe.config`; tên hiển thị và tên ZIP đã đổi. Không đổi tên hai file này riêng lẻ vì cần giữ config đi kèm và khả năng tương thích hiện tại.

## Tải bản mới

Mở [GitHub Actions](https://github.com/ttgpPsychMH/EAGLE-Auto/actions/workflows/chickenautoex-review-build.yml), chọn lần chạy xanh của nhánh **branding/eagle-auto-v0.1**, tải artifact **EAGLE-Auto-v0.1-net48-review**. Giải nén artifact để lấy ZIP **EAGLE-Auto-v0.1-net48.zip**, SHA256, `verified.json` và TRX; giải nén ZIP ứng dụng vào thư mục thử riêng. GitHub yêu cầu đăng nhập để tải artifact; không dùng Code → Download ZIP làm bản build mới.

## Kiểm tra Windows

1. Mở application, xác nhận tiêu đề và tooltip tray là **EAGLE Auto v0.1**.
2. Vào Tùy Chọn → **Về EAGLE Auto**, xác nhận đúng câu giới thiệu; đóng dialog rồi tiếp tục dùng form.
3. Vào Thông Tin Cập Nhật: xác nhận nội dung fork v0.1 và title mới.
4. Xem Properties → Details của `ChickenAutoEx.exe`: Product Name/Company/FileVersion/ProductVersion đúng như trên. Có thể đọc bằng `(Get-Item .\ChickenAutoEx.exe).VersionInfo` trong PowerShell mà không chạy game.
5. Kiểm tra lại menu phó bản khi không có lựa chọn và ca HP/MP trước đó. Không coi lần đổi tên này là xác nhận automation/game đã được sửa hoặc kiểm thử thành công.

## Kiểm tra bảo toàn

[Manifest branding](../tools/branding_review.json) ghi chính xác các delta text cùng hash nội dung trước sửa và hash lớp constant. `verify_build.py` đảo các delta này, đối chiếu source, rồi kiểm tra các sửa MP/config/menu được review trước đó, resources và binary gốc. Metadata tên/version được đọc từ PE như dữ liệu; không thực thi EXE/DLL. Các snapshot/hồ sơ điều tra trước giữ nguyên và mô tả build tại thời điểm của chúng.

Build net48/x86, bộ regression updater/UI/settings/menu và kiểm tra gói được ghi trong evidence của lần Actions tương ứng. Windows/game cần kiểm thử trên máy người dùng; bản v0.1 vẫn đang hoàn thiện.
