# Ác Bá — bản sửa và mô phỏng v0.6

Ngày 10-10-2026. Nhánh `repair/eagle-auto-ac-ba`, nối từ báo cáo `analysis/eagle-auto-ac-ba` và bản sửa v0.5. File chạy **EAGLE-Auto-v0.6.exe**. Đây là bản review để kiểm thử Windows/client riêng, chưa phải xác nhận vận hành trong game.

## Cách dùng

Trong **Nhiệm Vụ - Phụ Bản → Ác Bá**:

- **Tự nhận diện và bắt đầu**: chờ thông báo hệ thống mới có nội dung Ác Bá xuất hiện và đúng một môn phái. Khi đã biết đích, Auto điều phối tổ đội tới map ngoài của phái đó. `Nga Mi`/`Nga My` được xem là cùng phái; tên NPC có thể lặp lại tên phái trong câu.
- **Chọn Thiếu Lâm / … / Đường Môn**: chọn đích thủ công khi reader hoặc câu thông báo của client chưa tương thích. Chế độ này không cần hook nhận sự kiện. Nó vẫn tuân thủ điều kiện vào cửa cũ, không xác nhận sự kiện còn hoạt động.
- **Dừng Ác Bá**: hủy trạng thái nhiệm vụ. Tắt Auto hoặc đổi nhân vật/đội cũng hủy nhiệm vụ; không tự bật Auto của thành viên.

Nếu đội trưởng không cùng phái với đích, cần một thành viên đúng phái, online, khởi tạo xong, đang bật Auto, không có nhiệm vụ xung đột và ở ngoài phụ bản. Auto yêu cầu chuyển đội trưởng **một lần**, chỉ chuyển context Ác Bá sau khi đọc được trạng thái đội trưởng/key trên cả hai nhân vật. Không đủ điều kiện thì chờ có hạn rồi dừng. Điều kiện đội trưởng cùng phái được giữ từ baseline; chưa có bằng chứng để bỏ điều kiện này trên server riêng.

Thanh trạng thái cho biết đang chờ thông báo, chờ đội trưởng đúng phái, đi cửa, tuần tra, chờ boss hoặc chờ map ra. Không coi hết tuyến/không còn quái là đã hoàn thành hoặc đã nhận thưởng. Sau khi về map ngoài đúng phái, lượt kết thúc; chọn lại sự kiện mới thay vì lặp theo thông báo cũ.

## Các thay đổi và phần còn chưa xác minh

| Phát hiện trong báo cáo | Xử lý trong v0.6 |
| --- | --- |
| AB01: marker NPC cũ, alias, sự kiện kết thúc/mơ hồ | Parser riêng, normalize không phụ thuộc locale, nhận xuất hiện/kết thúc; tên phái lặp cùng ID hợp lệ, nhiều phái bị từ chối. Không suy luận vai trò NPC khi câu mơ hồ. |
| AB02: buffer/offset/packet ghép | Kiểm `LParam`, `lpData`, `4 <= cbData <= 65536` trước cấp phát/copy; decoder chỉ nhận header hệ thống gốc tại offset 4, channel 4, body 15, tối đa 4096 byte. Private chat không điều khiển sự kiện. Packet ghép/cắt/format mới bị từ chối; **chưa có contract native để hỗ trợ reassembly**. |
| AB03: bật nhiệm vụ nhưng không có reader | Tách nhu cầu reader khỏi alarm; tự yêu cầu hook ở chế độ nhận diện. Kiểm địa chỉ recv và đọc đủ 10 byte trước gửi ABI cũ `-10`. Chưa có ACK native; `IsHooked` chỉ là trạng thái managed. Chọn thủ công không phụ thuộc reader. |
| AB04: cũ, trùng, sai nhân vật | Cache riêng theo Game/character; xóa khi quan sát offline/đổi nhân vật/ClearMission/OFF, nhận kết thúc, deduplicate cùng phái không gia hạn TTL. Không chia sẻ cache toàn ứng dụng. Chỉ truyền đích trong handoff tổ đội đã xác nhận. |
| AB05–06: đích chưa biết/handoff/member OFF | Chờ có hạn, chọn tay 11 phái; đích là event/selection. Guard Auto/init/online/role/character/team/state/scene/quest; participant trùng bị loại; recheck giữa lệnh. Pending/ACK/timeout của handoff; không bật member OFF. |
| AB07: nhận nhầm phụ bản | Đúng map Ác Bá của đích trên từng actor; phụ bản khác làm dừng nhiệm vụ. MapID chưa phân biệt được mọi instance/line. |
| AB08: route null/index/X,X | Parser số nguyên nghiêm ngặt, fallback các POINT gốc; tuyến ngoài riêng với tuyến trong. Index có hai bounds, tính khoảng cách X,Y và chọn điểm chưa đi gần nhất sau combat; không dùng shared MoveIndex/Next/MoveNext. |
| AB09: trạng thái chung/calendar | Engine/clock/boss/dialog/route riêng; reset khi OFF, busy cả lúc chờ/đi cửa. Chờ sự kiện tối đa 10 phút để không khóa lịch vô hạn. |
| AB10: HP0 giả và chờ vĩnh viễn | Cùng ID từng quan sát sống, đúng map, không NPC, HP hữu hạn; sau đó chờ map ra có hạn. Chuyển cảnh xóa dấu vết boss/dialog cũ. Không có reward ACK/exit opcode mới. |
| AB11: NPC/dialog sai | Chỉ NPC gần, tên baseline `Giang hồ tiểu tiểu`, bind ID sau Talk, Talk/option một lần, chỉ option baseline50013/-1, không ClickAll. Tên/token mới của client riêng chưa được xác minh. |
| AB12: pause/kẹt/lỗi read | Freeze hạn chờ khi pause/scene/combat-state; vẫn giữ hạn đi cửa còn lại. Hạn dialog/đi/tiến triển/thành viên/boss/map ra; exception chỉ báo loại, không báo message có thể nhạy cảm. |
| AB13: protocol/client riêng | Giữ native DLL/ABI/offsets và điều kiện server. Chưa biết nguyên văn thông báo, codec thật, NPC, reward/cửa ra, server ID/instance ID và tín hiệu memory-read thành công. |
| AB14: tuần tra chung cạnh tranh | `AcTac()` chung không tuần tra 11 map Ác Bá và không điều khiển đội do module Ác Bá sở hữu. Các helper legacy còn dùng bởi module khác được giữ; thuật toán attack/skill/pet không đổi. |

## Chính sách chờ và dữ liệu

Các hạn dưới đây là giới hạn quản lý lỗi, **không phải thời lượng sự kiện server**:

- Thông báo mới có hiệu lực 10 phút trước khi chọn đích; thông báo trùng không gia hạn. Một lượt đã chọn giữ nguyên đích; thông báo mới khác phái không đổi tuyến giữa lượt.
- Chờ thông báo tối đa 10 phút; chờ thành viên đúng phái/đi cửa/tiến triển hoặc thành viên sẵn sàng tối đa 3 phút.
- Chờ xác nhận chuyển đội trưởng, dialog/map vào, boss sau hết tuyến hoặc map ra tối đa 60 giây. Không tự thử option khác hoặc đoán cửa ra.
- Thời gian nhiệm vụ được đóng băng lúc pause/scene/map-change/combat-state7. Freshness thông báo dùng thời gian thật đã trôi của Stopwatch, không kéo dài theo pause.
- Route tùy chỉnh có 2–256 điểm, mỗi tọa độ nguyên 1–10000; thiếu/sai thì dùng POINT gốc theo phái. Không thay dữ liệu MapPath gốc. Đơn vị/tọa độ baseline vẫn cần kiểm client.

`AcBaSystemEncoding` trong file `.exe.config` mặc định `VISCII` như binary107. Chỉ đổi sang `UTF-8` sau khi xác minh client; UTF-8 được giải mã nghiêm ngặt. Không tự đoán codec. Không lưu raw packet, chat, credential hoặc văn bản thông báo vào bằng chứng Git.

Dữ liệu unmanaged vẫn cần đúng pointer/contract của native sender; kiểm size không chứng minh pointer bất kỳ là an toàn. Không có source native nên không sửa packet fragmentation hay thêm opcode/offset mới.

## Bằng chứng và tái lập

**488 test passed, 0 failed, 0 skipped**: 109 Ác Bá, 45 menu, 78 Kỳ Cuộc, 49 Thủy Lao, 85 Trừng Ác, 66 Ác Tặc, 37 updater và 19 UI/settings. Cả 6 nhóm đối chứng tái hiện **51 lỗi hành vi cũ**, 0 lỗi harness; 8 thuộc Ác Bá v0.5. Build net48/x86: 0 lỗi, 50 cảnh báo legacy.

Bộ mô phỏng biên dịch các method thật đã review cùng partial Ác Bá bằng Roslyn SDK 8, nhưng dùng process, clock, memory, QuestFrame và commands giả. MAP/POINT/normalizer/codec được lấy từ source snapshot. Không load hoặc chạy EXE/native DLL. Xem [evidence v0.6](evidence/EAGLE-Auto-v0.6-ac-ba-validation.json).

```bash
dotnet build ChickenAutoEx.sln -c Release --no-restore
python tools/check_ac_ba_baseline.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
python tools/verify_build.py
python tools/package_build.py
```

Chạy prepare/restore theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md) trước. Đối chứng v0.5 là8 assertion thất bại đúng lỗi cũ, tách khỏi suite bản sửa; runner từ chối lỗi compiler/fixture. Bằng chứng phân tích 27 probes trước sửa giữ nguyên như hồ sơ lịch sử.

Verifier kiểm exact-delta đảo ngược Game/FrmMain và hash partial, toàn bộ source licensing/VIP/auth/config ngoài delta, stable116, snapshot,26 resource gốc, CLR4/x86/assembly version và tên/version sản phẩm.

## Kiểm thử Windows/game sau khi hoàn tất các phó bản

Theo [hướng dẫn Windows11](ChickenAutoEx-WINDOWS11.vi.md), dùng thư mục/config thử nghiệm riêng. Kiểm menu không có character/leader rồi chọn từng chế độ, OFF/pause, đổi character/đội và restart. Khi đến lượt kiểm game:

1. Ghi **nguyên văn thông báo hệ thống** cùng phái và lúc xuất hiện/kết thúc; không gửi chat riêng, packet thô hoặc tài khoản. Đối chiếu việc tự nhận diện; nếu chưa có thì dùng chọn tay.
2. Xác nhận riêng header/channel/codec, tên NPC/map và option thật trước đổi profile/protocol. Kiểm nhận thông báo khi alarm OFF và việc dừng reader khi không còn nhu cầu; simulation không chứng minh hook native hoạt động.
3. Tổ đội có member OFF/khác đội/phái không hợp lệ: không có lệnh gửi tới họ. Kiểm chuyển đội trưởng có ACK, không có ACK, người nhận rời đội/đổi character và việc đổi lịch trong lúc chờ.
4. Giám sát vào cửa, combat → reanchor, hết tuyến chưa có boss, corpse chưa từng sống, chuyển cảnh cùng map, boss chết → map ra. Kiểm timeout dừng và không tự nhận thưởng/repeat event.
5. So regression Ác Tặc, Kỳ Cuộc, Trừng Ác; Thủy Lao vẫn chưa bật. Licensing/VIP và stable116 không đổi.

Chưa yêu cầu thử trong game ngay; giữ kế hoạch kiểm một lượt sau các đợt sửa phó bản.
