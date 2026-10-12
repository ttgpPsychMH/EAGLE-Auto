# EAGLE Auto v0.10 — Aptos và logo đại bàng

Ngày 12-10-2026 (Asia/Bangkok), net48/x86, nhánh `branding/eagle-auto-aptos`. Tiếp nối giao diện compact v0.9; các engine/hook/adapter nhiệm vụ giữ nguyên.

## Thay đổi hiển thị

Dùng logo đầu đại bàng nhìn thẳng anh chọn sau cùng, thay icon ChickenAuto trong EXE, cửa sổ app và khay hệ thống. Asset PNG trong suốt được tạo từ ảnh tham chiếu; ICO chứa các cỡ 16/24/32/48/64/128/256. Bản PNG là asset đã xử lý, không phải bản sao từng byte của ảnh đầu vào. Ảnh VectorStock trước đó không dùng.

Theme áp dụng family **Aptos** cho mọi control, menu, context menu, 15 Form và UserControl gốc; control/menu thêm mới cũng được áp dụng. Giữ cỡ chữ, bold/italic/underline/strikeout để hạn chế thay đổi bố cục và hành vi. Menu chuột phải trên icon khay chuyển sang ContextMenuStrip để dùng Aptos, giữ hai handler Hiện Auto/Thoát Auto. Font được giữ/cached đến lúc đóng app.

**Về EAGLE Auto** là hộp thoại app có logo mới, version/attribution của tenkafuku và trạng thái font. Không thay các hộp thoại xác nhận của automation/licensing. Các ảnh chức năng OK/Cancel, cảnh báo, CAPTCHA và QR vẫn giữ ý nghĩa cũ; chúng không phải logo thương hiệu.

Font thanh tiêu đề, MessageBox, tooltip và hộp thoại file/folder chuẩn của Windows do OS quyết định. Đây không phải thay đổi font toàn hệ điều hành.

## Dùng Aptos

Theo [hướng dẫn font local](fonts/APTOS-LOCAL.vi.md): nếu máy có Aptos thì app tự chọn; hoặc chép đủ bốn face Aptos vào `Fonts` cạnh EXE. App nạp riêng qua GDI + GDI+, không cài vào hệ thống và không cần quyền admin riêng cho font. Thiếu font/face thì dùng Segoe UI để app vẫn mở được. Khởi động lại sau khi thêm file.

TTF/ZIP/EULA không được commit, nhúng vào EXE hay đóng gói artifact. EULA đi kèm bộ upload ghi không cấp quyền phân phối cho bên thứ ba. [Báo cáo metadata](analysis/EAGLE-Auto-APTOS-METADATA.vi.md) chỉ ghi hash/family/glyph coverage và trích dẫn cần thiết. Cả 28 TTF có đủ chữ Việt NFC/NFD kiểm tra; bốn face nền đều thuộc family Aptos.

## Bảo toàn và bằng chứng

Snapshot decompile và sáu binary gốc không sửa. Các Form phụ được tạo thành bản sao build với một hook ngay sau InitializeComponent; Debug hydrate được đảo hook trước so auth baseline. About/tray/project có delta chính xác để đảo ngược trong verifier. Main áp dụng theme sau dựng dashboard. Những resource cũ vẫn được giữ nguyên làm bằng chứng bảo toàn, nhưng icon hiển thị được thay bởi asset mới.

App có 29 embedded resources: 27 trước đó cộng PNG/ICO mới. Verifier kiểm tra cả icon Win32 của EXE lẫn icon/logo managed khớp asset; 26 resource gốc còn nguyên, resource JSON giữ nâng cấp đã duyệt. Không có font resource. Licensing/VIP/auth, updater protocol107, CLR assembly1.0, EasyHook.dll, stable116 và thuật toán phó bản không đổi. Thủy Lao vẫn chưa khả dụng.

Kiểm thử C# chạy policy chọn nguồn/font/style, các method traversal/event thật với control/font/GDI boundary giả, reader giới hạn với bytes giả và mapping hai handler tray. Không load app EXE, GDI font API hoặc DLL game. Các kiểm thử cũ tiếp tục chạy.

[Mẫu HTML](ui/EAGLE-Auto-v0.10-preview.html), [ảnh giao diện có nhân vật giả](ui/EAGLE-Auto-v0.10-selected.png), [bỏ chọn](ui/EAGLE-Auto-v0.10-empty.png) và [About](ui/EAGLE-Auto-v0.10-about.png) là mô phỏng HTML. Kiểm tra browser nạp bốn face Aptos từ thư mục local riêng trong bộ nhớ; xác nhận glyph Việt dùng custom font, không chỉ tên CSS. Không đưa TTF/base64 font vào HTML Git/artifact và không truy cập mạng. Browser rendering không xác nhận cách nạp font GDI/WinForms trên Windows.

## Kiểm tra Windows 11

Đóng v0.9, giải nén gói riêng; giữ EXE/config/DLL đi cùng. Chép Fonts như hướng dẫn nếu máy chưa có Aptos. Kiểm tra EXE/taskbar/title icon/tray đều là đại bàng mới; menu Hiện Auto/Thoát Auto hoạt động như cũ. Mở About, xác nhận font status Aptos và attribution đúng.

Kiểm tra tám tab, cửa sổ con (skills/items/settings/hotkeys/logs/alarms), popup/context menu và các dòng được thêm mới ở DPI100/125/150%. Chữ Việt đầy đủ dấu, chữ đậm/nghiêng đúng; nút/numeric/combo không bị cắt. Thử thiếu một face hoặc bỏ Fonts: fallback phải mở được và About ghi đúng. Đóng/mở nhiều lần, đóng cửa sổ phụ rồi app; không có lỗi font đã dispose.

Tiếp tục kiểm tra client trong đợt đã thống nhất. Cloud chưa thực thi WinForms/automation EXE/native game DLL; privatefont loading, icon cache, DPI và gameplay Windows chưa kiểm chứng.
