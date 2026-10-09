# Ma trận kiểm thử 116–107: điền kết quả thực tế

Đây là kế hoạch, **chưa chạy game**. 107 net48 đã được người dùng báo cáo mở được giao diện; các ô còn lại chưa có kết quả. Thực hiện các lượt riêng cho từng bản, giữ config riêng, theo [hướng dẫn Windows](../../docs/ChickenAutoEx-WINDOWS11.vi.md). Chỉ tiếp tục sang hành động game sau khi đọc trạng thái chính xác và có quyền hợp lệ trên server thử nghiệm.

Thông tin phiên thử: ngày giờ, OS/.NET, SHA EXE hoặc ZIP, version client/server, số instance, map, cờ bật và ngưỡng. Không ghi account/password/token. Chỉ dùng tên nhân vật đã che nếu chia sẻ kết quả. Không bật hai auto lên cùng process, không đổi hook/offsets/quyền để vượt một bước thất bại.

| ID | Kiểm tra / điều kiện | Tiêu chí xác nhận | 116 | 107 net48 |
|---|---|---|---|---|
| A01 | Startup chưa có game | Giao diện mở, không crash hoặc bắt mở website cũ | Người dùng từng báo hoạt động; chưa đo cùng điều kiện | Người dùng báo đã mở; chưa test game |
| A02 | Update offline/timeout/HTML | Ghi thời gian, thông báo và UI còn dùng được; dùng endpoint giả chỉ cho 107 | Chưa thử; chưa có cơ chế cấu hình endpoint giả được xác minh | Chưa thử Windows; updater cô lập đã test trên net8 |
| B01 | Một process game, automation tắt | Đúng process/nhân vật; không thao tác tự động | Chưa thử | Chưa thử |
| B02 | Đọc trạng thái | Level, phái, map, tọa độ, HP/MP và pet khớp game | Chưa thử | Chưa thử |
| B03 | Chuyển map / đăng xuất game | UI cập nhật trạng thái, không stale hoặc crash | Chưa thử | Chưa thử |
| B04 | Hai instance, automation tắt | Chọn đúng instance; setting/trạng thái không lẫn nhau | Chưa thử | Chưa thử |
| C01 | HP và MP: OFF/OFF, ON/OFF, OFF/ON, ON/ON | Lưu/đọc và trạng thái engine độc lập; kiểm tra trước bằng trạng thái giả | Chưa thử | Có lỗi tĩnh MP đọc HP; chưa sửa/chưa xác nhận runtime |
| C02 | Pet / hồi phục | Chỉ dùng khi option và ngưỡng phù hợp; ghi skill/item tiêu thụ | Chưa thử | Chưa thử |
| C03 | Một skill / attack radius / quái bỏ qua | Đúng mục tiêu/phạm vi; dừng được bằng option/phím tắt | Chưa thử | Chưa thử |
| C04 | Theo key / buff danh sách | Đúng leader/người nhận; ghi tính năng bị giới hạn theo client | UI 116 có cảnh báo một số game không hỗ trợ attack with leader | Chưa thử |
| C05 | Nhặt đồ | Đúng phạm vi/loại; không kẹt hoặc gián đoạn hành động ngoài dự kiến | Chưa thử | Chưa thử |
| C06 | Danh sách vứt rác | Trước hết dùng item giả để so quyết định; chưa xóa vật phẩm thật để khám phá quy tắc | Có tooltip bảo vệ theo sao/cấp; chưa xác minh code native | Có guard khác trong C#; chưa coi tương đương |
| C07 | Sau tử vong | Đúng option logout/return; ghi thời gian và điều kiện map | Chưa thử | Chưa thử |
| C08 | Phím tắt, tray, đổi instance | Chỉ tác động đúng phạm vi; bật/tắt đăng ký và dừng auto rõ ràng | Chưa thử | Chưa thử |
| C09 | Lưu/đọc config không có credential | Option giữ nguyên qua mở lại; không đọc format của bản khác trực tiếp | Chưa thử | Nhánh config 62 phần tử có lỗi tĩnh cần sửa riêng |
| D01 | Ác Bá / Ác Tặc | Ghi từng bước nhận/đặt đội/vào map/combat/kết thúc | Chưa có bằng chứng native đủ để thử theo source | Có code; chưa thử |
| D02 | Kỳ Cuộc | Ghi chuyển trạng thái/nhận boss chết/chờ/kết thúc | Chưa biết | Có code; chưa thử |
| D03 | Thủy Lao / Lâu Lan Tam Bảo | Ghi NPC/dialog/map và bước dừng | Chưa biết | Có code; chưa thử |
| D04 | Module có guard VIP / đang tắt | Chỉ test điều kiện hợp lệ; ghi không khả dụng thay vì chỉnh quyền | Chưa đối chiếu xác thực native | Giữ guard VIP; `IsTKC` false và `TKC()` rỗng; không bật để thử |
| E01 | Đo hiệu năng ở cùng số instance | CPU/RAM, thời gian UI đáp ứng, crash/hang; cùng thời lượng và tính năng | Chưa đo | Chưa đo |

Nếu B01/B02 thất bại, dừng phần C/D và ghi dữ liệu chẩn đoán đã che. Nếu C01/C09 lộ lỗi đúng như source, giữ baseline và đưa vào lượt sửa nhỏ riêng. Một phó bản PASS không đại diện cho tất cả phó bản hoặc mọi server.
