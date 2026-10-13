# Rà soát v0.15: trạng thái và vòng tổ đội

Ngày: 13-10-2026. Baseline: `f5dc6a9` (v0.14). Nguồn bằng chứng: recovered 107, partial C# hiện tại, phản hồi Windows của người dùng và mô phỏng độc lập; không chạy auto/game/DLL native trong cloud.

## Nguyên nhân xác nhận ở source

- `analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs`, getter TrangThai: state 2 → “Di Chuyển”; state 7 → “Đánh Quái”. Các guard phục hồi của cả bốn dungeon đã chặn/hủy 2; bộ mô phỏng cũ cũng chứa giả định sai 2 = chết và 7 = chuyển cảnh. Các ca đó được sửa thành guard 9/scene flag; pet AOE vẫn giữ chặn casting khi đang di chuyển/đánh như logic có sẵn.
- V0.14 Ác Tặc có ownership guard bọc chung SkillDo và đi bãi. V0.15 mở rộng ownership cho cả bốn nhưng tách lời gọi skill ra, chỉ cho nhân vật được điều phối dùng skill khi pha combat đã chuẩn bị xong. Nội dung hàm skill, tấn công, hồi phục, licensing giữ nguyên.
- Coordinator riêng trước đây gửi hủy theo/xuống thú cưỡi cùng lượt hoặc coi gửi lệnh là thành công. Thành viên vào cửa và tuần tra thiếu một vòng quan sát chung. V0.15 có state machine chung với chờ/cooldown/fallback, giữ cấu hình Auto/skill từng nhân vật.
- Ác Bá cần xóa live-boss evidence ngay khi pause chuyển cảnh; Thủy Lao cần visited/reanchor riêng và kiểm tra combat trước watchdog tuần tra. Menu Thủy Lao trước đây vẫn trả “Not work”; nay được nối vào engine để thử nghiệm.

## Phạm vi và kiểm chứng

`tools/dungeon_party_review.json` lưu delta đảo ngược chính xác, SHA256 trước/sau cho bốn partial, dashboard/form/typography, và ba hook trong Game. Baseline partial bất biến lưu trong `baselines`; verifier đảo lớp v0.15 trước toàn bộ delta lịch sử, rồi so với recovered source. Generated constants/key material chỉ nằm trong `.build` bị ignore. Binary original/stable 116, resource gốc, native DLL và entitlement code được đối chiếu hash/reversal như trước.

Mô phỏng mới chạy actual C# với giả lập reader/clock/commands: bốn state-2 cancellation đối chứng trên v0.14; pha mount/follow có ACK trễ, mất ACK và retry giới hạn; chuỗi 0→2→7→0; hủy giữa các native write; Auto-off/khác map; giữ lựa chọn không có thú cưỡi/không bật đánh; owner release; resume waypoint. Chẩn đoán đọc cache và ghi phase/count hữu hạn. Layout giả lập chạy phương thức sắp xếp thật với các DPI khác nhau, kiểm tra ô threshold không chồng và không ghi giá trị cấu hình.

**872 test .NET passed, 0 failed/0 skipped; 63 đối chứng expected failures; 26 kịch bản C++ và 11 fixture chéo C#/C++. Build 0 lỗi/49 cảnh báo legacy.** Kết quả cuối cùng được ghi trong `VALIDATION.json` và artifact CI của đúng commit. Các đối chứng lịch sử là expected failures, không được tính vào test passed.

## Giới hạn thực tế

Không suy ra offset/native reader của client private từ việc thấy tên/HP/map; queue thành công không phải ACK. Dừng bằng tọa độ hiện tại, xác nhận cưỡi/theo, map/NPC/options, hành vi ba cửa sổ và render WinForms thật đều cần thử Windows. Broadcast chỉ kiểm soát được các cửa sổ Auto biết, không có roster server đầy đủ. Thủy Lao được mở để thử nghiệm, chưa xác nhận reward/exit thật. Tài liệu [TEAM-PATROL](../../docs/EAGLE-Auto-TEAM-PATROL.vi.md) có thứ tự thử và phương án thao tác tay.
