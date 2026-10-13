# EAGLE Auto v0.15 — tổ đội và giao diện bốn tab

Bản này sửa C# của **Trân Long Kỳ Cuộc, Ác Tặc, Ác Bá và Thủy Lao**, trên nhánh `repair/dungeon-party-patrol-v015`, dựa trên v0.14. Build/mô phỏng trên Linux chưa chứng minh chạy đúng trên Windows hoặc client private. Stable 116, executable gốc, EasyHook.dll, offset, opcode, NPC/map/option và điều kiện licensing/VIP được giữ nguyên.

## Những gì thay đổi

Trong source 107, `PlayerState == 2` được hiển thị là **Di chuyển**, `7` là **Đánh quái**. Một số guard phục hồi trước đó nhầm 2 với trạng thái không sử dụng được và nhầm 7 với chuyển cảnh. Bốn mô phỏng đối chứng trên source v0.14 tái hiện việc hủy phụ bản ngay khi đội trưởng bắt đầu di chuyển. V0.15 quan sát riêng đứng (0), di chuyển (2), đánh (7), thú cưỡi và Theo Sau. Trạng thái 9 vẫn bị chặn theo guard có sẵn; chưa xác định mọi mã trạng thái của client private.

Trong map phó bản, vòng điều phối chung thực hiện:

1. Sau hai giây yên quái, hủy theo ở đội trưởng; lên thú cưỡi với nhân vật có thú cưỡi và chờ trạng thái quan sát xác nhận. Nhân vật không có thú cưỡi được đi bộ.
2. Yêu cầu Theo Sau khi mọi nhân vật đủ điều kiện, cùng map, không có cửa sổ hội thoại và không có cửa sổ cùng đội đã tắt Auto. Chờ tối đa năm giây xác nhận. Thiếu xác nhận thì dùng Move cùng map cho thành viên tụt lại, tối đa một lần mỗi hai giây.
3. Đội trưởng dẫn theo tuyến. Thành viên đã theo không nhận Move cạnh tranh. Khi gặp quái hoặc quan sát thấy trạng thái đánh: hủy theo → chờ xác nhận → xuống thú cưỡi → chờ xác nhận → dừng di chuyển → chờ xác nhận.
4. Hàm đánh và dùng skill hiện có tiếp tục chạy theo cấu hình từng nhân vật. Sau giao tranh, nối vào điểm **chưa đi gần nhất**, không quay lại điểm A/C theo chỉ số cũ.

Các bước cưỡi/hủy theo/xuống thú cưỡi/dừng được giới hạn chờ mười giây cho từng bước. Không có ACK riêng trong DLL: đã gửi lệnh không được coi là thao tác thành công. C# quan sát trạng thái ở lần đọc sau; không xác nhận được thì dừng có lý do. Giới hạn này cần hiệu chỉnh bằng bằng chứng Windows thực tế nếu client chậm.

Đi bãi/nhiệm vụ thường và FollowKey thường không cạnh tranh với coordinator. Lời gọi **SkillDo** được tách khỏi block đi bãi; chỉ chạy trong pha combat đã chuẩn bị xong đối với nhân vật do coordinator quản lý. Nội dung chọn target/skill, cooldown, hồi phục và kiểm tra VIP bên trong không đổi. Không tự bật Auto hoặc Đánh quái/skill của người dùng; trạng thái báo rõ nếu một số nhân vật chưa bật Đánh quái.

NPC vào/ra và điều kiện boss/quest vẫn xử lý theo từng nhiệm vụ. Ác Tặc giữ lựa chọn map và tìm NPC riêng khi đội trưởng đã vào. Ác Bá giữ cơ chế thông báo/môn phái và handoff trước đây, đồng thời xóa bằng chứng boss cũ ngay khi bắt đầu chuyển cảnh. Thủy Lao dùng đúng 12 điểm legacy với visited/reanchor riêng; đi hết tuyến không được coi là hoàn thành quest. **Menu và tab Thủy Lao được mở để kiểm thử**, thay cho thông báo “Not work”.

Lệnh dừng dùng hợp đồng tọa độ Move hiện có: đích là tọa độ hiện tại, opcode 50/51/105. `Game.Move` có nhánh bỏ qua đích quá gần nên không dùng được trực tiếp cho việc này. Đây là phương án C# cần xác minh riêng trên Windows; không thay DLL hay đoán opcode mới.

## Giao diện

Cửa sổ chính mặc định khoảng **350 × 660 phần nội dung** (khoảng 350 × 700 kể cả viền/title bar), hai menu **Tệp tin / Giúp đỡ**, danh sách nhân vật/chỉ số phía trên, bốn tab **Cơ bản / Kỹ năng / Tiện ích / Nhiệm vụ**, nhật ký và trạng thái phía dưới. Font nội dung 9pt, Aptos theo cơ chế chọn font cục bộ trước đây, logo EAGLE giữ nguyên. Cỡ chữ được kiểm thử bằng dependency giả ở 100/125/150/200% DPI; hình vẽ thật phải kiểm tra trên Windows.

**Tệp tin → Nâng cao** mở Vật phẩm, Pet, Chế đồ, Đăng nhập; “Di chuyển / lên bãi” mở nhóm di chuyển trong Cơ bản. “Điều khiển chung” và “Hiện nhật ký” nằm trong Tệp tin. Menu nhiệm vụ cũ vẫn có trong Tệp tin. Mọi trang legacy và chỉ số 0–6 được giữ, trang nhiệm vụ vẫn là 7; tab phía trước chỉ chọn trang tương ứng, không tạo cấu hình/handler mới.

## Chẩn đoán và cách thử Windows 11

Không cần gõ command. Giải nén `EAGLE-Auto-v0.15-net48.zip` vào thư mục riêng, chạy **EAGLE-Auto-v0.15.exe** cùng config/DLL. Mở **Tệp tin → Chẩn đoán tự động**, đọc thông báo và chọn Đồng ý nếu muốn ghi CSV cục bộ. Không gửi dữ liệu tự động. V0.15 thêm `TeamPatrolState/Ready/Following/Mounted/Moving`: chỉ phase, số lượng, bật/tắt, mã lý do và alias cửa sổ; không thêm tên/ID/đội/tọa độ/chat.

1. Kiểm tra cửa sổ không bị mất chữ ở scale đang dùng; chuyển bốn tab, mở từng trang Nâng cao, kiểm tra chọn nhân vật/cấu hình/Đăng nhập/Chế đồ vẫn đúng. Kiểm tra Aptos trong Về EAGLE Auto. Đổi lựa chọn không được tự bật Auto.
2. Với ba nhân vật được nhận diện, chọn từng nhân vật và bật **Đánh quái** cùng các skill muốn dùng, rồi bật Auto bằng checkbox danh sách. Chỉ bật nhiệm vụ ở đội trưởng.
3. Thử **Ác Tặc trước**, cho cả đội vào map bằng tay nếu NPC/đường vào của client chưa tương thích. Quan sát từng nhân vật: lên thú cưỡi, nhận Theo Sau, đến điểm, hủy theo, xuống thú cưỡi, đánh; hết quái có đi tiếp từ điểm gần không. Nếu dừng chờ xác nhận, ghi lại thao tác nào chưa thực hiện và thời điểm.
4. Sau một vòng Ác Tặc, thử Kỳ Cuộc, Ác Bá chọn phái thủ công, cuối cùng Thủy Lao thử nghiệm. Giữ thử nhận thông báo tự động Ác Bá riêng để phân biệt lỗi framing/codec với di chuyển.
5. Tắt Auto một thành viên hoặc dừng nhiệm vụ: kiểm tra không còn lệnh cho nhân vật đó, không tự báo hoàn thành/nhận thưởng. Gửi CSV cùng mô tả ngắn hành vi và giờ xảy ra lỗi; không gửi mật khẩu.

Nếu cưỡi/Theo Sau/Move không thực hiện dù trạng thái báo đã gửi, dùng thao tác game bằng tay và kiểm tra hook/reader. Nếu map/NPC/option khác legacy, đi/vào bằng tay và thử vòng bên trong trước. Không đổi offset/DLL khi chưa có bằng chứng ABI và client. Cửa ra Ác Tặc vẫn cần xác nhận boss/map; bản này không thêm giao thức ra cửa chưa biết.

## Tham khảo Micro 6.9

ZIP tải lại có SHA256 `a56c478aff98e36ae544fecec62e85f4f93eb411bab6c29c6dbf5403e2bea107`; 26 file C# trùng bản đã xem. Project khai báo .NET Framework 2.0/WinForms, nhiều tên đã bị làm rối (`GClass0`, `method_N`). Chỉ dùng để tham khảo luồng/UI/Win32; không chạy chương trình, không sao chép offset, DLL hoặc suy diễn `method_39()==2` là PlayerState của ChickenAuto. Quyền sử dụng công khai do người dùng cung cấp; không thay giấy phép các thành phần khác.

## Bằng chứng kiểm thử

Xem báo cáo [dungeon-party-v015](../analysis/dungeon-party-v015/REPORT.vi.md), `startup-tests.trx`, `dungeon-party-baseline-summary.json`, `verified.json` và kết quả native simulation trong artifact. Các hướng dẫn v0.14 trở về trước là lịch sử; bước cưỡi/tuần tra và việc mở Thủy Lao trong tài liệu này thay thế các mô tả cũ tương ứng.
