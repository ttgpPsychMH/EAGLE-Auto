# EAGLE Auto v0.12 — Quy Cách Viết Và Cỡ Chữ

Ngày: 13-10-2026 (Asia/Bangkok). Nhánh `branding/vietnamese-typography`, kế thừa v0.11. Đây là thay đổi giao diện, chưa xác nhận bố cục WinForms trên Windows 11.

| Thành phần | Quy cách | Ví dụ | Cỡ chữ tối thiểu |
|---|---|---|---|
| Heading 1: tiêu đề khu vực | Viết HOA | NHÂN VẬT | 11 pt, đậm |
| Heading 2: tab, nhóm cài đặt, menu có nhóm con | Hoa Từng Từ | Kỹ Năng; Hồi Phục | 10,5 pt |
| Heading 3, nút, nhãn và mô tả | Viết theo câu | Lên bãi; Chọn bản đồ | 10 pt |

Tên NPC, map, phó bản, nhân vật, kỹ năng và vật phẩm giữ nguyên. Ví dụ Trân Long Kỳ Cuộc, Thủy Lao, Cộng Sinh, Huyết Tế và Thổ Linh Châu không bị chuyển thành chữ thường hoặc chữ HOA. Các viết tắt HP/MP/NPC/NM và tên phím tắt giữ nguyên. Tên EAGLE Auto cũng giữ nguyên cách viết của thương hiệu.

Cỡ chữ thông thường tăng từ khoảng 9 lên 10 pt; chữ vốn lớn hơn được giữ nguyên, không cộng thêm mỗi lần cập nhật giao diện. Vẫn dùng Aptos hoặc Segoe UI dự phòng; gói không kèm font có hạn chế phân phối. Cửa sổ mặc định tăng từ 560 × 700 lên 600 × 720 và hàng tab cao hơn để có chỗ cho chữ.

Một số câu được sửa rõ hơn: “Phân Cứng” thành “Phần cứng”, “Rao Chát” thành “Rao chat”, “Bặt Tắt AutoTrain” thành “Bật/tắt tự luyện”; câu xác nhận thoát thành “Bạn có chắc chắn muốn thoát Auto không?”. Nhãn dài được rút gọn khi vẫn giữ ý nghĩa, ví dụ “Đặt kỹ năng vào F1”.

## Phạm Vi Áp Dụng

`UiCopyCatalog` chỉ liệt kê các câu giao diện cố định đã rà soát. `UiCopyPolicy` áp dụng ba cấp viết; `UiTypography` xử lý nhãn, nút, tab, nhóm và menu. Các cửa sổ cài đặt kế thừa nhận cùng quy tắc qua hook giao diện có sẵn. Nhãn thay đổi theo trạng thái của nút cũng được cập nhật qua sự kiện TextChanged.

Không áp dụng đổi hoa/thường hàng loạt cho dữ liệu game hoặc log: ô nhập, danh sách/combobox, tên và giá trị trong dòng ListView, nhãn động và thanh trạng thái giữ nguyên nội dung. Chỉ tiêu đề cột cố định được sửa. Nhãn Label phải thuộc danh sách nhãn tĩnh đã kiểm tra; nhãn cảnh báo chứa tên game không bị sửa. Lệnh debug `Atk` vẫn giữ nguyên vì handler cũ so sánh trực tiếp chữ này. Riêng lựa chọn mặc định của pet dùng sự kiện Format để hiển thị “Không xuất”; Text, Value và ToString của item vẫn là giá trị gốc “Không Xuất” mà handler pet sử dụng. Tên pet khác không đổi.

Nút/nhãn có thể tăng kích thước trong khoảng trống sẵn có, có giới hạn theo cạnh container và các control lân cận. Không đổi vị trí, parent, Checked, Value, lựa chọn hay handler. Nhãn cố định và nút có kích thước cố định dùng dấu ba chấm khi thiếu chỗ. Các tab có vùng cuộn; cần kiểm tra Windows thực tế để xác nhận tất cả nhãn ở các mức DPI.

Không sửa engine nhiệm vụ/phó bản, skills, chế đồ, đăng nhập, licensing/VIP, DLL native hoặc stable 116. `typography_review.json` khai báo các thay đổi literal trực tiếp để verifier đảo lại trước khi kiểm tra source cũ. Các source giao diện mới có hash; source engine/hook và binary gốc vẫn chịu kiểm tra bảo toàn trước khi đóng gói.

## Kiểm Tra Và Cách Tải

Build thành công: 0 lỗi, 49 cảnh báo legacy; 747 test .NET passed, 26 tình huống C++ và 51 đối chứng lịch sử. Mẫu HTML/font có 79 kiểm tra passed. [Bằng chứng](evidence/EAGLE-Auto-v0.12-typography-validation.json); [ảnh dữ liệu giả](ui/EAGLE-Auto-v0.12-selected.png).

Build net48/x86 và kiểm thử chạy trong cloud không mở EXE automation, không nạp DLL game. Bộ kiểm thử thực thi source định dạng chữ và code typography với control/GDI giả, kiểm tra chữ theo cấp, tên riêng, phím tắt, cập nhật muộn, áp dụng lặp lại, khoảng trống và bảo toàn ô nhập/giá trị. Các kiểm thử updater, logger, adapter và nhiệm vụ cũ tiếp tục chạy.

[Mẫu HTML v0.12](ui/EAGLE-Auto-v0.12-preview.html) chỉ dùng dữ liệu giả. Chromium kiểm tra font Aptos người dùng cung cấp riêng, dấu tiếng Việt, tab và bố cục mẫu; HTML không thay cho kiểm thử WinForms/Windows DPI. Font chỉ nạp trong bộ nhớ trình duyệt, không ghi vào artifact hoặc Git.

Tải từ [GitHub Actions](https://github.com/ttgpPsychMH/EAGLE-Auto/actions/workflows/chickenautoex-review-build.yml): chọn lần chạy thành công của nhánh `branding/vietnamese-typography`, tải artifact **EAGLE-Auto-v0.12-net48-review**, giải nén hai lớp rồi chạy **EAGLE-Auto-v0.12.exe** cùng config/DLL đúng gói. Không cần merge để tải gói review; artifact được giữ 7 ngày.

## Kiểm Tra Windows 11

1. Mở bản mới trong thư mục thử riêng. Kiểm tra tiêu đề v0.12, logo và font Aptos/Segoe UI. Title bar Windows, hộp MessageBox và hộp thoại chọn file dùng font do hệ điều hành quản lý.
2. Thử DPI 100%, 125%, 150% và cửa sổ thu nhỏ; đi qua đủ tám tab. Kiểm tra nhãn, nút, ô số, dòng nhóm, thanh menu và cuộn trang.
3. Mở các cửa sổ kỹ năng, vật phẩm, buff, chat, phím tắt, thiết lập, shutdown, bán/hủy đồ và Về EAGLE Auto; kiểm tra chữ có bị cắt hay chồng nhau không. Chỉ mở và xem, không cần bật automation để kiểm tra giao diện.
4. Khi kết nối client thử, xác nhận tên nhân vật/NPC/map/kỹ năng và các lựa chọn vẫn giống trước. Nút, trạng thái bật/tắt, ngưỡng HP/MP và phím tắt giữ hành vi cũ.
5. Nếu chữ bị cắt, gửi ảnh kèm DPI, tab/cửa sổ và kích thước cửa sổ. Chưa kết luận bản này đã qua kiểm thử Windows hoặc trong game.
