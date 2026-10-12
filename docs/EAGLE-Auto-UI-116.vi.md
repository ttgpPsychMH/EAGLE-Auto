# EAGLE Auto v0.8 — giao diện theo bố cục 116

Ngày: 12-10-2026 (Asia/Bangkok). Bản review .NET Framework 4.8/x86, nền engine 107 đã sửa đến v0.7. Giao diện được dựng bằng WinForms trên source khôi phục 107; không ghép DLL/Qt hoặc mở khóa tab của binary 116.

## Thay đổi

- Danh sách nhân vật nằm bên trái, thông tin HP/MP/pet và thao tác chung ở trên, các tab cấu hình bên phải, nhật ký phía dưới. Cửa sổ có thể đổi kích thước; tab cũ có thanh cuộn khi thiếu chỗ.
- Giữ bảy tab cấu hình cũ và các handler: Tổng Hợp, Kỹ Năng, Vật Phẩm, Pet, Tiện Ích, Chế Đồ, AutoLogin. Tên tab đầu vẫn theo nhân vật đang chọn như trước. Bảng mới được **thêm cuối**, giữ index 5/6 mà handler Chế Đồ/AutoLogin đang dùng.
- Thêm tab **Nhiệm Vụ – Phó Bản**: chọn Trừng Ác, Trân Long Kỳ Cuộc, Ác Tặc, Ác Bá hoặc xem tình trạng Thủy Lao. Các nhiệm vụ chưa được rà soát vẫn có trong menu cũ.
- Ác Tặc: chọn một trong sáu map cũ hoặc map ngẫu nhiên theo cơ chế cũ. Ác Bá: nhận diện thông báo hệ thống hoặc chọn một trong 11 môn phái thủ công. Đây là các entry point đã có từ v0.5/v0.6.
- Có nút Bắt đầu/Dừng nhiệm vụ, nhân vật hoặc đội trưởng thực hiện, bước engine và lý do chưa sẵn sàng. Dừng chỉ tắt nhiệm vụ đang xem; không tắt Auto hay xóa quest/cấu hình.
- Nút chung cũ đổi nhãn thành **Bật/tắt tất cả** để thể hiện đúng phạm vi. Các thao tác chung khác vẫn dùng handler cũ.

## Cách dùng

1. Giải nén `EAGLE-Auto-v0.8-net48.zip` vào thư mục thử riêng; giữ `EAGLE-Auto-v0.8.exe.config` và các DLL đi cùng. Mở `EAGLE-Auto-v0.8.exe` trên Windows khi đến lượt kiểm tra giao diện.
2. Chọn **một dòng nhân vật**; ô tick là bật Auto, không phải chọn phạm vi cho bảng mới. Không có dòng được chọn thì bảng không gửi lệnh tới tất cả nhân vật.
3. Mở tab Nhiệm Vụ – Phó Bản, chọn nhiệm vụ và map/môn phái nếu có. Trừng Ác tác động nhân vật được chọn; các nhiệm vụ tổ đội tác động đội trưởng đã nhận diện của đúng đội.
4. Bảng yêu cầu online/khởi tạo/Auto/sẵn sàng trước khi bắt đầu. Nếu có nhiệm vụ khác, dừng nhiệm vụ đó qua bảng hoặc menu tương ứng. Bảng không tự reset nhiệm vụ đang chạy. Trừng Ác cá nhân cũng bị chặn nếu đội trưởng đang điều khiển nhiệm vụ tổ đội.
5. Dừng vẫn được phép khi tạm dừng, chết hoặc mất kết nối **nếu danh tính/tổ đội vẫn xác định được**. Đổi nhân vật/đội trưởng/đăng nhập giữa lúc hiển thị và bấm nút sẽ hủy thao tác và yêu cầu xem lại trạng thái.

Thủy Lao vẫn **chưa khả dụng** trong bảng và handler menu cũ vẫn giữ thông báo chưa hoạt động. Không có thay đổi DLL, offsets, licensing/VIP, giao thức version `107`, CLR AssemblyVersion `1.0.0.0`, thuật toán phụ bản, pet AOE hay stable 116.

## Ý nghĩa trạng thái và giới hạn

Bảng đọc phase dưới lock của từng engine; Ác Bá dùng trạng thái đã có. Đọc trạng thái không gọi tick, parser, hook hoặc gửi lệnh di chuyển. Refresh mỗi 500 ms khi cửa sổ hiện, chỉ đọc chi tiết nhiệm vụ khi mở tab mới.

**Đã bật** chỉ xác nhận cờ yêu cầu của engine đang bật. **Đã gửi yêu cầu** không phải ACK của game, không xác nhận đã vào phụ bản, đánh boss, nhận thưởng hay hoàn tất. Nếu nhân vật bận, bước đang đọc có thể vẫn là combat; engine quyết định hành động thực tế.

Các engine v0.7 không lưu riêng nguyên nhân dừng cuối cùng. Khi engine tự tắt, bảng ghi “Chưa bật / đã dừng” và hướng đến cảnh báo/nhật ký; không suy đoán thành công hoặc hiển thị lại nguyên nhân của nhân vật khác. Tự động bàn giao đội trưởng Ác Bá giữ cơ chế xác nhận cũ; bảng sẽ theo đội trưởng hiện tại khi client phản ánh thay đổi.

Adapter kiểm tra lại lúc bấm và ngay trước đổi cờ. Game có thể thay đổi trên thread khác sau snapshot cuối; các guard owner/team của engine cũ vẫn là lớp kiểm tra khi thực thi. Không tuyên bố snapshot là giao dịch nguyên tử với game.

## Mẫu giao diện và kiểm thử cloud

[Ảnh mẫu (HTML, dữ liệu giả)](ui/EAGLE-Auto-v0.8-preview.png) và [HTML mẫu tương tác](ui/EAGLE-Auto-v0.8-preview.html) mở được trên trình duyệt thông thường, không cần server. Gói ZIP có `UI-PREVIEW-FAKE-DATA.html`. Mẫu cho phép thử chọn nhân vật/task, Bắt đầu/Dừng, tạm dừng, thiếu đội trưởng, offline và Thủy Lao chưa khả dụng. Các nút thao tác chung bị vô hiệu hóa trong mẫu. Đây là **HTML mô phỏng**, không phải ảnh chụp WinForms hay kiểm thử trong game; JavaScript không được sử dụng trong EXE.

Kiểm thử C# gồm policy với dữ liệu giả, binding bị đổi trước click/giữa snapshot, double-start, dừng khi không sẵn sàng, lựa chọn không hợp lệ, nhiệm vụ cá nhân/tổ đội, dispatch của **port C# thật** với Game/TLBB giả và projection phase chỉ đọc. Nó không load automation EXE hoặc DLL native. Toàn bộ suite trước đó cũng tiếp tục chạy.

Kiểm tra HTML bằng Chromium/Playwright là tùy chọn:

```bash
# Với Playwright đã có; chỉ định module nếu chưa nằm trong NODE_PATH.
PLAYWRIGHT_MODULE_PATH=/path/to/playwright node tools/check_dashboard_preview.cjs
```

Có thể đặt `CHROMIUM_EXECUTABLE` đến Chromium phù hợp. Script render HTML do dự án cung cấp trong bộ nhớ, không mở website hoặc localhost; kết quả/ảnh chỉ ghi vào `.build/ui-preview`. Không phải điều kiện để build/test C# trên CI.

Verifier đảo đúng hai delta FrmMain (partial + gọi initializer), so lại toàn bộ FrmMain v0.7, kiểm tra hash các partial engine/hook v0.7 và tiếp tục kiểm tra toàn bộ baseline/resources/licensing. Source mới được pin hash riêng trong `tools/dashboard_review.json`.

## Kiểm tra Windows 11 sau này

Trước tiên kiểm tra UI với game đóng: mở app, đổi kích thước; DPI 100/125/150%; chuyển qua tám tab; thiếu nhân vật thì Bắt đầu/Dừng bị chặn, không còn JIT dialog; Thủy Lao không bật được; menu About/diagnostics còn dùng được. Kiểm tra nút và nội dung không bị che, tab có thanh cuộn, đóng/mở và thu nhỏ/khôi phục hoạt động bình thường. Mẫu HTML dùng dữ liệu giả có thể xem trước, không cần chạy automation.

Sau khi xong lượt rà soát phụ bản mới kiểm tra với client riêng: chọn đúng actor/leader; cấu hình hồi HP/MP, skill, pet, items, follow và AutoLogin vẫn lưu/đọc như cũ (không gửi credential vào báo cáo); bật/dừng từng nhiệm vụ; thử đổi selection trong khi đang chạy; đối chiếu phase với game và các checklist mô phỏng từng phụ bản. Kiểm tra riêng Ác Bá bàn giao đội trưởng và thông báo reader. Báo kết quả thiếu bằng chứng thay vì coi thao tác gửi là thành công.

Cloud chưa thực thi EXE, native DLL hoặc WinForms. Mẫu trình duyệt và build Linux **không xác nhận bố cục/DPI hoặc hành vi Windows/game**. Chưa gọi đây là bản phát hành đã kiểm chứng.
