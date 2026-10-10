# EAGLE Auto v0.3 — mô phỏng và sửa Kỳ Cuộc

Ngày: 10-10-2026 (Asia/Bangkok). Nhánh `repair/eagle-auto-ky-cuoc-simulation`; baseline code ứng dụng `6d83466` (v0.2), tiếp nối [báo cáo tĩnh Kỳ Cuộc](../analysis/tran-long-ky-cuoc/REPORT.vi.md). Gói chạy bằng **EAGLE-Auto-v0.3.exe** và config cùng tên.

**207 test qua, 0 thất bại, 0 bỏ qua:** 135 test cũ và 72 ca Kỳ Cuộc/pet AOE mới. Sáu đối chứng trên source gốc tái hiện đúng lỗi; bản sửa qua cùng sáu assertion. Không load/run executable automation hoặc DLL native. Chưa kiểm thử Windows 11 hay client game trong lượt này; gói là bản review.

## Hành vi đã thay đổi

| Vấn đề | Sửa trong v0.3 |
|---|---|
| Bật lại sau lượt trước bị tắt ngay | Session Kỳ Cuộc riêng; bắt đầu lượt mới reset cờ hoàn thành và trạng thái do module sở hữu. ClearMission và Auto OFF hủy session. |
| Tắt menu nhưng vẫn đi NPC sau boss chết | Gỡ nhánh kết thúc không có guard trong Auto chung; chỉ session đang active được điều phối boss/cửa ra. Không dùng `IsP` chung để giữ luồng chạy sau khi tắt menu. |
| Member đã tắt Auto vẫn nhận lệnh từ leader | Kỳ Cuộc dùng điều phối riêng, không gọi TrieuTap chung. Kiểm tra Auto, Init, online, chết/chuyển cảnh, đúng đội/leader và thời gian sau chuyển cảnh trước lệnh cho từng participant. Leader mất quyền/đổi đội thì hủy session. |
| Leader vào/tuần tra trước member | Chờ các nhân vật cùng đội đang được điều khiển đến cửa và quan sát họ vào map. Member đã vào trước được giữ ở trong; leader đã vào chờ member ngoài. Không ra lệnh cho nhân vật Auto OFF. |
| Chọn dialog mù/lặp lại | Kiểm tra NPC ID và đúng cặp option trong dialog trước khi chọn. Entry `401001/-1`, exit `44000/0` gửi một lần rồi chờ map cập nhật, không tự Accept hay giả trạng thái quest. |
| Đánh dấu xong ngay khi phát GoTo | Đi tới NPC chưa là hoàn thành. Chỉ kết thúc session sau khi các participant đang được điều khiển đều được quan sát rời map 61 về Lạc Dương sau bước chọn cửa ra. Đây là xác nhận chuyển map, **chưa chứng minh nhận thưởng**. |
| Quay về điểm tuần tra cũ sau combat | Dùng tuyến/index/đánh dấu đã đi riêng. Sau khoảng vắng quái, chọn điểm gần chưa đi qua; giữ đích đến lần gián đoạn tiếp theo. Không khởi động lại từ điểm 1 chỉ vì combat. |
| Bỏ sót các điểm do chọn lại tuyến | Giữ điểm chưa đi qua, quay lại quét chúng trong phần còn lại của vòng. Đi hết tuyến chỉ bắt đầu vòng mới, không đánh dấu xong phụ bản. |
| Timer chung ảnh hưởng tuần tra | Clock riêng; chờ 4 giây từ lúc tới map hoặc lần thấy quái gần nhất. Combat được xử lý trước thời gian chờ; loot dài không bị tính là kẹt tuyến. |
| Countdown sai | Cùng hằng chờ 30 giây và remaining không âm. Chỉ ra cửa khi không còn quái trong các vùng quan sát. |
| Skill pet với list quái rỗng | Helper AOE chung không truy cập `Monter[0]`; chọn quái sống gần nhất trong bán kính 20, bỏ null/HP hoặc tọa độ không hợp lệ, giữ điều kiện skill/tick và quyền điều khiển. |
| Lỗi bị nuốt/nhầm hoàn thành | Lỗi trong session dừng Kỳ Cuộc, chỉ báo loại exception; không ghi Exception.Message, account hoặc memory dump. Map sai/timeout không được coi là hoàn thành. |

Kỳ Cuộc đang chạy được xem là bận với lịch, kể cả khi đi tới cửa. Nếu nhiệm vụ khác đang active khi bật Kỳ Cuộc, module này dừng và yêu cầu người dùng dừng nhiệm vụ kia; không tự xóa cờ module khác. Không ép tắt HP/MP, pet hoặc combat thường.

## Cách nối lại tuyến

Sáu tọa độ giữ theo 107: `(42,42), (84,42), (81,81), (42,85), (50,49), (71,50)`.

1. Combat/loot và các điều kiện điều khiển được xử lý trước hành trình.
2. Sau combat và tối thiểu 4 giây vắng quái, chọn điểm chưa đi qua gần vị trí leader nhất. Nếu khoảng cách gần bằng nhau, ưu tiên thứ tự tiến trong tuyến đang lưu.
3. Khóa lựa chọn đó cho tới khi tới điểm hoặc có combat mới; không tính lại “gần nhất” mọi tick.
4. Khi tới điểm trong bán kính 5, tiếp tục theo chiều tuyến qua các điểm chưa đi. Đủ sáu điểm thì bắt đầu vòng tiếp theo.

Case mô phỏng: đang chờ điểm 3 `(81,81)`, đã qua điểm 1/2, combat kéo leader tới `(82,50)`; sau combat chọn điểm 6 `(71,50)` thay vì quay ngay về điểm 3. Điểm 3/4/5 vẫn được giữ lại và được quét tiếp. Đây không phải thuật toán tìm đường tránh vật cản mới; native Move vẫn theo cơ chế gốc.

Chỉ số `MoveIndex` chung không bị sửa bởi tuần tra Kỳ Cuộc. Không sửa tuyến/helper của Thủy Lao, Tàng Kinh Các hoặc các phụ bản khác. Chọn một điểm gần theo khoảng cách hình học chưa bảo đảm đường đi ngắn nhất qua vật cản; phải kiểm chứng trên map thực tế.

## Mô phỏng và đối chứng

[KyCuocSimulationTests.cs](../tests/ChickenAutoEx.Startup.Tests/KyCuocSimulationTests.cs) dùng Roslyn của SDK pin để chọn property/caller thực từ Game đã áp dụng allowlist, cộng toàn bộ `Game.KyCuoc.cs` và `Game.PetAoe.cs` được build. Fake thay TLBB/objects/dialog/đội, clock và lệnh hook; lệnh chỉ ghi vào danh sách. Không chạy app, process game, P/Invoke hoặc kết nối server game.

Các ca kiểm tra: tắt/bật và lượt 2; caller Auto OFF/ClearMission; đổi leader/đội; member Auto OFF/offline/chết/chuyển cảnh/chưa Init; hủy trong lúc GoTo; cửa vào/cửa ra sai option/NPC; entry không chuyển map; member vào trước/đến cửa muộn; boss đã thấy sống rồi chết; chỉ thấy corpse chưa đủ; member nhìn thấy boss thay leader; còn quái; leader ra trước member; map thoát sai; member tắt Auto/rời đội trong lúc ra; timeout một lần; countdown; chọn lại tuyến và giữ đích; bảo toàn đầy đủ điểm chưa đi và index chung; chờ sau chuyển map, kẹt tuyến, loot dài, thời gian hồi phục member từ lúc mất điều kiện; skill pet rỗng/không có skill/sai điều kiện/mục tiêu chết/ngoài bán kính và map thường.

`tools/check_ky_cuoc_baseline.py` yêu cầu đúng sáu lỗi trên snapshot gốc: reset, menu OFF, member OFF, hoàn thành sớm, quay về điểm cũ và AOE rỗng. Lỗi AOE phải là ArgumentOutOfRangeException trong method Game.AOE của fixture biên dịch từ source thật; lỗi biên dịch/harness không được tính là đối chứng. Các thử nghiệm đầu có lỗi fixture đã được sửa; chỉ lượt đối chứng hoàn chỉnh được ghi trong summary. Bốn đối chứng Thủy Lao và toàn bộ 135 test cũ vẫn qua điều kiện kiểm tra tương ứng.

Clock tăng bằng test; không sleep để giả timeout. Test định nghĩa snapshot hợp lệ, không kiểm chứng kết quả read memory/native hook hay race Windows thực tế. Việc đổi trạng thái Auto được kiểm tra tại ranh giới phát lệnh; lệnh đã gửi vào hàng đợi Windows/server không thể thu hồi bằng cách tắt menu.

## Bảo toàn và build

- Target net48/x86, Newtonsoft.Json 13.0.4, 27 embedded resources; build 0 lỗi, 50 cảnh báo legacy.
- Cả sáu binary stable/Ex và snapshot recovered được bảo toàn. Stable 116, licensing/VIP, HWID/key/config encryption và `Global.Version = "107"` không đổi.
- `Game.ThuyLao.cs` và allowlist Thủy Lao không đổi; menu vẫn báo `Not work`. Helper AOE chung có sửa chủ đích nên các map khác cũng nhận guard/chọn mục tiêu pet mới; không gọi đây là toàn bộ automation ngoài Kỳ Cuộc giữ nguyên.
- `tools/ky_cuoc_review.json` khai báo chính xác các thay đổi property/engine/caller/boss/lịch/AOE, cùng hash hai file partial mới. Prepare áp dụng Thủy Lao trước, Kỳ Cuộc sau. Verify đảo Kỳ Cuộc trước, Thủy Lao sau rồi đối chiếu toàn bộ Game gốc và các file auth/debug. Không bỏ qua cả Game hoặc cả vùng phụ bản.
- Harness Thủy Lao tiếp tục dùng source riêng chỉ có delta Thủy Lao; harness Kỳ Cuộc dùng `.build/ky-cuoc/ReviewedGame.cs` đã redacted. Hydrated source/full IL có literal gốc chỉ nằm trong `.build`, không commit/upload.
- Chỉ sửa `EagleAuto.Version.props` thành 0.3; tên EXE/config/UI/metadata/ZIP/artifact được sinh theo version chung.

Theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md), sau prepare và build chạy:

```bash
python tools/check_thuy_lao_baseline.py
python tools/check_ky_cuoc_baseline.py
# Dùng Python venv có dnfile cho verify/package.
python tools/verify_build.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
python tools/package_build.py
```

[Evidence v0.3](evidence/EAGLE-Auto-v0.3-ky-cuoc-validation.json) ghi số test, đối chứng, hash ZIP local và metadata kiểm tra tĩnh. CI chạy lại cùng checks trong môi trường sạch và upload ZIP/checksum/TRX/summary/verified.json.

## Giới hạn client và timeout

Giữ nguyên map 61, Vương Tích Tân ID142 map0 và Tế Thánh ID12349 map61, dialog legacy và tên boss `viencokyhon`. Chưa hỗ trợ biến thể map550/Bốc Hối Kỳ. Cửa ra đang đòi về map0 (Lạc Dương); nếu server private cho ra map khác, module dừng bảo thủ, không tự đổi mapping theo suy đoán.

Boss phải được một participant đang được điều khiển quan sát cùng ID còn HP dương rồi HP0; chỉ thấy corpse lần đầu không đủ. Snapshot gốc chưa cung cấp cờ xác nhận read thành công, nên không khẳng định đã loại mọi lỗi đọc memory hoặc corpse bị mất giữa các lần đọc. Chưa kiểm tra quest “Ván Cờ Sinh Tử”/nhận thưởng vì chưa có baseline server; không gửi thêm lệnh nhận quest hoặc giả hoàn thành.

Chờ entry/travel và kẹt tuyến tối đa 180 giây; sau chọn entry hoặc bước ra cửa chờ tối đa 60 giây; chờ member khôi phục điều kiện tối đa 180 giây kể từ lúc mất điều kiện. Combat không bị tính thành kẹt tuyến. Lag/client khác có thể cần điều chỉnh bằng dữ liệu thực tế. Người ngoài danh sách Game được quản lý hoặc member Auto OFF không được điều khiển tự động. Đội thay đổi trong bước ra cửa gây hủy session để tránh đánh dấu hoàn thành thiếu xác nhận.

## Checklist Windows 11 / private client

1. Tải artifact xanh **EAGLE-Auto-v0.3-net48-review** của đúng nhánh/commit; giải nén wrapper rồi ZIP app vào thư mục riêng. Giữ `EAGLE-Auto-v0.3.exe.config` cạnh EXE. Không ghi đè bản 116/v0.2 đang dùng.
2. Mở app không cần game; kiểm tra v0.3/About/menu, cập nhật mạng, đóng/mở lại. Không bật JIT để coi là cách sửa exception. Thủy Lao vẫn chưa khả dụng.
3. Trước khi bật Kỳ Cuộc, đi tay xác minh map/NPC/dialog/quest của client, cách đội vào và map sau cửa ra. Nếu khác baseline trên, dừng thử tự động và gửi ảnh/ghi chú đã che tài khoản; không gửi password/token hoặc dùng cách vượt điều kiện server.
4. Khi client khớp, dùng một đội nhỏ có giám sát, Auto ON trên các nhân vật muốn điều khiển, tắt nhiệm vụ phụ bản/lịch khác. Bật Kỳ Cuộc từ menu; xác minh tới NPC → entry một lần → cùng map mới tuần tra.
5. Kiểm tra menu OFF trong lúc đang tuần tra và trong lúc chờ sau boss: không có lệnh Kỳ Cuộc mới. Auto OFF trên member phải chặn lệnh do Kỳ Cuộc điều phối cho member đó; các hành động Auto thường khác cần đối chiếu riêng.
6. Quan sát một combat kéo leader xa điểm đang tới. Ghi vị trí trước/sau, đích đi tiếp, có quay lại điểm cũ ngay không, có đổi đích liên tục không, và các vùng chưa quét có được quay lại trong vòng đó không. Cấu hình loot/follow/skill thường và quái spawn thật có thể ảnh hưởng kết quả.
7. Sau boss, xác minh chờ 30 giây không hiện số âm, tới NPC, đúng dialog, từng member ra map; chỉ sau xác nhận mới tắt module. Kiểm tra thưởng bằng tay; phiên bản này chưa tự chứng minh thưởng. Tới lượt hợp lệ tiếp theo, bật lại và kiểm tra không bị cờ xong cũ chặn.
8. Nếu kẹt/timeout/map khác hoặc exception, dừng Auto, ghi bước/map/leader-member/NPC-dialog/ảnh lỗi. Không sửa offsets hoặc ép cờ hoàn thành. Lỗi cụ thể được đưa thành case mock trước lần sửa tiếp theo.
