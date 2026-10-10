# Trừng Ác — rà soát ChickenAutoEx 107 / EAGLE Auto v0.3

Ngày: 10-10-2026 (Asia/Bangkok). Baseline ứng dụng: commit `91a93f2`, đã có các sửa hẹp Thủy Lao và Kỳ Cuộc/pet AOE. Nhánh báo cáo: `analysis/eagle-auto-trung-ac`.

**Lượt này chỉ phân tích: không sửa automation, không nâng version, không build/phát hành EXE mới.** Việc kiểm thử trong game được để lại sau đợt rà soát/sửa các phụ bản theo yêu cầu của người dùng. Chưa kết luận Trừng Ác chạy đúng trên private client.

## Kết luận

107 có engine Trừng Ác thực sự. Đây là luồng quest dùng **Trừng Ác Lệnh**, không phải engine vào một map phụ bản cố định như Kỳ Cuộc. Điểm cần ưu tiên nhất là **timer tự hủy quest vẫn chạy ngoài session** và **parser tọa độ có thể ném exception hoặc nhận dữ liệu sai**. Vòng đời, dialog bất đồng bộ và nhận diện mục tiêu cũng cần mô phỏng trước khi sửa.

Lỗi menu NullReferenceException cũ đã được sửa ở đợt trước: handler có kiểm tra context, trạng thái tick và thông báo đúng cờ Trừng Ác. Các sửa đó chưa thay đổi `Game.TrungAc()` hoặc khối timer của `TheoDoiCanhBao()`. Bản v0.3 đã sửa AOE rỗng dùng chung; không ghi lại lỗi đó như lỗi Trừng Ác chưa được sửa.

## Phạm vi và bằng chứng

Đọc toàn bộ engine, các tham chiếu cờ/task/timer, caller Auto, menu và AutoLogin, alarm/refresh UI, reset/load setting, combat/loot/di chuyển và bộ đọc quest/task/item. File/hash, dòng tham chiếu và hash method có trong [source-inventory.json](evidence/source-inventory.json). Các method/hướng dữ liệu chính:

| Thành phần | Vai trò |
|---|---|
| [Game.TrungAc](../chickenautoex-107/recovered/TinhKiemAuto/Game.cs#L2641) | Nhận thông tin, nhận/trả quest, di chuyển, dùng lệnh, chọn quái và đánh. |
| [STATE](../chickenautoex-107/recovered/TinhKiemAuto/STATE.cs) | State là `int`, None=0 mặc định, Null=-1; nhiều hằng dùng chung trong bản legacy. |
| [Game.Auto](../chickenautoex-107/recovered/TinhKiemAuto/Game.cs#L5517) | Guard Auto/Init/chuyển cảnh/online; gọi TrungAc cho từng Game khi MapAcTac=0, không nằm trong nhánh leader-only. |
| [Menu hiện tại](../../src/ChickenAutoEx/FrmMain.cs#L4306) | Toggle `leader.IsTrungAc`; có guard context, không reset session. |
| [AutoLogin](../../src/ChickenAutoEx/FrmMain.cs#L2883) | Trong 60 giây đầu online, Account.IsTrungAc có thể bật Game.IsTrungAc và IsBTDByLogin. |
| [SetInfo](../../src/ChickenAutoEx/FrmMain.cs#L1261), [TheoDoiCanhBao](../chickenautoex-107/recovered/TinhKiemAuto/Game.cs#L10402) | Alarm UI, thông báo xong, timer lỗi và yêu cầu hủy quest. |
| [Task](../chickenautoex-107/recovered/TinhKiemAuto/Task.cs), [TaskInfo](../chickenautoex-107/recovered/TinhKiemAuto/TaskInfo.cs), [QuestFrame](../chickenautoex-107/recovered/TinhKiemAuto/QuestFrame.cs) | Đọc quest đang có, trạng thái >=256 và các chuỗi/option dialog từ memory. |
| [TINHKIEM](../chickenautoex-107/recovered/TinhKiemAuto/TINHKIEM.cs#L1331) | Map theo tên, tọa độ theo ParseInt, lựa chọn xa phu theo GetTruyen. |
| [PacketItem](../chickenautoex-107/recovered/TinhKiemAuto/PacketItem.cs), [GameObjects](../chickenautoex-107/recovered/TinhKiemAuto/GameObjects.cs#L200), [GameObject](../chickenautoex-107/recovered/TinhKiemAuto/GameObject.cs) | Túi đồ và snapshot đối tượng; Near20m lọc từ All, không chỉ quái đang sống. |
| [FindPath](../chickenautoex-107/recovered/FindPath.cs), Game.TimDuong/TrongPhamVi/DaDenNoi | Di chuyển theo dữ liệu runtime 16.dat/17.dat, kiểm tra bán kính 3 hoặc 15. |
| [Script XML](../chickenautoex-107/recovered/TinhKiemAuto.Scripts.xml#L82) | Metadata “Trừng Giới Hung Đồ”, Ngô Giới tại (224,226,map1), Level31; không phải guard level trực tiếp của engine. |

Dòng Game trong báo cáo dẫn tới snapshot recovered bất biến. Game được build hiện tại là bản hydrate + hai allowlist đã review nên số dòng có thể lệch. Collector đối chiếu source hiện tại sau áp dụng Thủy Lao rồi Kỳ Cuộc: `TrungAc`, `TheoDoiCanhBao` và các helper khảo sát giữ nguyên; `Auto`/`ClearMission` có delta của đợt trước, đã được tính vào baseline v0.3.

Disassemble payload gốc bằng ILSpyCmd tin cậy, chỉ đọc target như dữ liệu: [TrungAc.il.txt](evidence/TrungAc.il.txt), [khối timer hủy quest](evidence/Alarm-trung-ac-timer.il.txt), [ClearNhiemVu.il.txt](evidence/ClearNhiemVu.il.txt). Không commit full IL/hydrated source vì có literal gốc đã được che trong snapshot.

## Luồng hiện tại

1. Menu bật cờ trên leader, hoặc AutoLogin bật trên từng Game được cấu hình. Auto chung gọi TrungAc khi Auto ON, đã init/online và qua guard chuyển cảnh.
2. Engine chạy khi tick chia hết 12, không pause, không ở Giám Ngục và PlayerState=0. `ForcePickItem()` được gọi trong guard trước kiểm tra PlayerState nên loot có thể được ưu tiên trước task/combat.
3. Đọc Task tên chứa `#{CXDT_090304_01}`; nếu Completed thì chuyển `Done` để đi trả quest. `Done` ở đây là bước nội bộ, **không phải** `IsXongTrungAc`/đã nhận thưởng.
4. Null/None tìm “trungaclenh” trong túi. Có lệnh → dùng → GetInfo → đọc toàn bộ text dialog, map/tọa độ → Do. Null không có lệnh → tới Ngô Giới, map1/Tô Châu, (224,226) → nhận quest. None không có lệnh → hỏi thông tin qua hook/Lua.
5. Do đi tới tọa độ và bắt đầu `comeTime`. Come giữ trong bán kính 15; còn lệnh thì dùng lại; hết lệnh thì tìm đối tượng Menpai28 trong 20 đơn vị và gửi BaseSkill. Thảo Nguyên còn đòi Title khác rỗng.
6. Gặp đối tượng HP<=0 và đạt điều kiện timer/map → Done → Ngô Giới → chọn tên quest → gửi Continue rồi Complete → Null. CheckComplete có hai token `CXDY_090423_01/02` mới đặt `IsXongTrungAc=true` và tắt cờ.
7. Ngoài engine, alarm UI: comeTime >150s đặt IsHong; IsHong thêm >300s sẽ chạy Lua mở yêu cầu bỏ quest, đặt HuyQ. Engine HuyQ gửi nút OK chung. Nếu IsHong bắt đầu do comeTime, tổng thời gian xấp xỉ 450s cộng nhịp refresh; IsHong có thể bắt đầu sớm hơn từ None.

## Các phát hiện và đề xuất

### TA01 — Parser tọa độ có thể crash hoặc cho kết quả sai — cao, source/IL xác nhận

`GetInfo:2780–2793` dùng IndexOf rồi Substring **trước** khi kiểm tra MissionMap. Dialog chưa đến/rỗng/thiếu ngoặc hoặc ngoặc sai thứ tự gây ArgumentOutOfRangeException. Guard `Split(',').Length != 0` không bảo đảm hai phần; text `[123]` ở map nhận diện được vẫn đọc `[1]`, gây IndexOutOfRangeException. IL xác nhận `Substring`, `ldlen/brfalse` rồi lấy phần tử 1; không phải lỗi decompile.

`ParseInt:2409` bỏ dấu và chữ, không giữ dấu âm, không có TryParse/giới hạn tọa độ: `abc,xyz` có thể thành (0,0), `-5` thành 5, số quá lớn có thể OverflowException. Chưa chạy các input này trong engine; đây là đường lỗi suy ra từ C# và IL. Đề xuất parser riêng TryParse, đúng hai giá trị, map hợp lệ, giới hạn phù hợp dữ liệu map; dialog chưa sẵn sàng thì chờ có hạn, không gọi Move hay tự coi là xong. Không đổi ParseInt chung ngay vì module khác dùng nó.

### TA02 — Timer hủy quest không bị ràng buộc bởi module/Auto/pause — cao, source/IL xác nhận

`TheoDoiCanhBao:10506–10525` không kiểm tra IsTrungAc, IsAuto, online hoặc Global.Paused trước khối hủy. `FrmMain.SetInfo:1266` vẫn gọi method này cho các dòng nhân vật, ngoài Game.Auto và không guard Auto ON. Menu OFF/Auto OFF không clear comeTime/IsHong/HongTrungAcTime. Timer còn từ lượt cũ có đường tiếp tục tới Lua `Mission_Abnegate_Popup` khi nhiệm vụ đã tắt. **Mở yêu cầu hủy không đồng nghĩa server đã hủy:** bước OK nằm trong engine và có thể không chạy khi menu OFF.

Đề xuất alarm chỉ quan sát/thông báo; timeout/hủy do session đang active sở hữu, gắn character/quest/lượt. Dừng/pause cần chính sách rõ và không tự gửi yêu cầu bỏ quest từ timer mồ côi. Kiểm tra đúng popup/quest trước OK; không gửi nút OK chung cho dialog khác. Đối chứng đầu tiên nên là menu OFF, Auto OFF, pause và reload nhân vật với timer cũ.

### TA03 — Bật lại không reset state/timer/cờ xong — cao, source xác nhận

Menu chỉ toggle bool. State, TrungAcInfo, MissionMap/X/Y, doneTime/comeTime, IsHong và IsXongTrungAc không reset cùng một thao tác bắt đầu/hủy. Reset IsXong chỉ có ở GetMissionInfo khi Lua trả “Xong”/“Chua”; nếu lượt mới đi trực tiếp qua lệnh/GetInfo, cờ hoàn thành cũ có thể vẫn true. Alarm/AutoLogin cũng đọc cờ này, nên không thể coi tick menu là một lượt mới sạch.

`ClearNhiemVu:9493` đặt IsTrungAc=false rồi mới `if (IsTrungAc) State=Null`; nhánh đó không tới được trong thứ tự code bình thường (ngoài trường hợp thread khác bật lại giữa hai dòng). Method được `LoadSetting:10183` gọi; nó cũng không clear toàn bộ timer/cờ xong Trừng Ác. Đề xuất lifecycle riêng, reset dữ liệu do module sở hữu, hủy khi đổi character; không dùng sửa thứ tự if này như giải pháp hoàn chỉnh. Giữ các cờ combat/HP/MP độc lập.

### TA04 — Fall-through làm mất trạng thái khi dialog chưa sẵn sàng — cao, source/IL xác nhận

Cuối TrungAc có `State=Null`. TalkToAcceptMission/TalkToCompleteMission không tìm thấy đúng option và DongY không tìm thấy “đồng ý” đều rơi xuống cuối method. GetInfo map không nhận diện cũng rơi xuống đó nếu Substring chưa ném lỗi. Vì PostMessage/Lua bất đồng bộ, một tick dialog trễ có thể làm engine quên bước đang chờ, nói lại NPC hoặc đọc lại nhiệm vụ thay vì chờ phản hồi.

Đề xuất mỗi state có return/transition rõ, giữ nguyên khi chưa có phản hồi, timeout riêng và lý do dừng. Unknown state cần dừng/đồng bộ lại có kiểm soát, tránh tự chuyển Null và phát thêm lệnh nhận quest. Guard đọc Task.Completed đầu method còn có thể đổi HuyQ thành Done trước khi xử lý OK; cần định nghĩa ưu tiên hủy với phản hồi quest.

### TA05 — Hoàn thành/nhận-trả quest dựa vào dialog và lệnh gửi, thiếu xác nhận — cao, thiết kế xác nhận; hậu quả cần client

Nhánh nhận/trả có kiểm tra **tên option** từ QuestFrame.Enum; không phải mọi thao tác đều click mù. Tuy vậy không ràng buộc NPC/dialog identity và không chờ xác nhận cho `PostMessage(14,105)` → `QuestFrameMissionComplete()` → State.Null. PostMessage wrapper trả void và không kiểm tra kết quả native; gửi lệnh không chứng minh quest được nhận/trả hay thưởng được cấp.

CheckComplete đọc `QuestFrame.All` hai lần riêng rồi ghép điều kiện hai token. Nội dung có thể đổi giữa hai lần đọc; chưa có bản dịch/ý nghĩa token từ client thật nên **chưa kết luận hai token luôn có nghĩa “đã làm đủ lượt”**. Đề xuất snapshot một lần kèm context, kiểm tra task/item đổi sau lệnh, phân biệt nhận thành công/hết lượt/từ chối/lỗi đọc; tuyệt đối không ép cờ complete hay sửa dữ liệu quest client để coi là hoàn thành.

### TA06 — Dùng lệnh nhiều lần và loot có thể chặn bước task — vừa, source xác nhận; tác động cần client

Trong Come, thấy item tên chứa “trungaclenh” là UseItem rồi return, không đổi state, không ghi item/lần dùng, không chờ nó mất hoặc quái xuất hiện. Nếu item còn hiển thị do lag/không dùng được/còn nhiều count, lời gọi có thể lặp mỗi nhịp engine. Các nhánh Null/None đổi GetInfo sau UseItem nhưng không chờ dialog, liên quan TA01/04.

ForcePickItem trong guard và trong Come có thể ưu tiên loot trước kiểm tra trạng thái quest/quái; helper còn gọi FixKetMap nếu đứng lâu. Đề xuất bước “đã gửi dùng lệnh, đang chờ phản hồi”, giới hạn retry theo snapshot và nguyên nhân từ chối; chọn ưu tiên combat/quest/loot rõ. Không kết luận item bị tiêu hao nhiều lần trên mọi server vì chưa kiểm thử inventory/server ack.

### TA07 — Mục tiêu và detector Done thiếu liên hệ quest, phụ thuộc thứ tự list — cao, source/IL xác nhận đường lỗi

Ngoài Thảo Nguyên, lọc Near20m theo Menpai28 và chênh cấp<=5. Near20m là đối tượng từ All trong bán kính, không tự lọc chỉ quái sống hay target của quest. Không lưu ID đã triệu hồi/đã thấy sống, không đối chiếu quest trước khi chuyển Done từ HP<=0. Map<=2 thì chuyển Done ngay khi gặp ứng viên chết; ngoài thành, corpse + timer>10s cũng đủ dù quest chưa báo hoàn tất.

Nhánh thường không break sau SelectTarget/SendKey cho quái sống, có thể phát nhiều lệnh đổi target một tick. Trường hợp timer corpse cũ >10s, list có corpse trước rồi live target sau: state có thể được đặt Done trước, sau đó vẫn gửi skill cho live target và không khôi phục state. Thảo Nguyên break ở live target nên khác nhánh thường, nhưng vẫn dựa Title/Menpai/corpse. Không khẳng định thứ tự/spawn đó đã xảy ra trên private client.

Đề xuất một target ổn định gắn session, sống/hợp lệ, tránh đổi target nhiều lần; Task.Completed là phản hồi ưu tiên. Nếu cần detector corpse dự phòng thì phải gắn observed-live ID và kiểm tra snapshot, không dùng một corpse bất kỳ hay chỉ vắng quái làm bằng chứng thắng. `Task.SetComplete`/`TaskInfo.SetTrangThai` tồn tại trong recovered source nhưng **TrungAc không gọi**; không đưa chúng vào chiến lược sửa để giả thành công server.

### TA08 — Thiếu timeout đi tới mục tiêu/Ngô Giới và thiếu đường phục hồi phân loại — vừa, source xác nhận

comeTime chỉ khởi động khi đã tới tọa độ trong Do; đi sai map/không tìm được đường/kẹt xa NPC có thể chờ vô hạn trước đó. TimDuong bắt lỗi FindPath và báo “Không thể tìm đường” rồi return; engine không nhận kết quả bước đi để quyết định chờ/dừng. Trong Come, nếu quái không xuất hiện/corpse biến mất trước đọc thì Done có thể không lên, sau đó đi vào luồng bỏ quest TA02.

Đề xuất timer từng bước, theo dõi tiến triển, lý do riêng cho không có route/không NPC/không lệnh/chưa spawn/quest đã hoàn tất/mất kết nối, giới hạn retry. Nhánh TalkToXaPhu chỉ được **so sánh** trong engine; không tìm thấy `State=STATE.TalkToXaPhu` trong source managed khảo sát. Nhánh này chưa có đường vào rõ, Extra1 có thể cũ trên map không hỗ trợ và GetTruyen có thể -1; không “sửa” bằng cách tự kích hoạt nó trước khi hiểu protocol.

### TA09 — Module/lịch có thể cùng phát lệnh — cao, source xác nhận

Menu Trừng Ác không gọi ResetDungeonActions hay kiểm tra module khác. IsBusy hiện tại là IsMapPhuBan || IsThuyLao || IsKyCuoc, thiếu IsTrungAc ở map thường. Lịch có thể gọi ClearMission rồi bật module khác; **ClearMission không tắt Trừng Ác**. Nếu bật Ác Tặc MapAcTac!=0, caller không gọi TrungAc nữa nhưng timer UI vẫn tồn tại. Nếu bật module khác ngoài điều kiện này, có nhiều đường di chuyển/dialog/combat trong một vòng Auto. Kỳ Cuộc v0.3 có guard xung đột riêng nên nó có thể tự dừng khi thấy Trừng Ác, không có nghĩa Trừng Ác đã có guard tương ứng.

Đề xuất một lựa chọn nhiệm vụ có hiệu lực, session Trừng Ác được lịch coi là bận, hủy/chuyển việc có quy tắc. Sửa ClearMission chỉ khi đã có hồi quy các phụ bản và lịch vì đây là helper chung; không chỉ thêm IsBusy rồi bỏ qua cờ mồ côi.

### TA10 — UI yêu cầu leader nhưng engine là từng nhân vật, không điều phối đội — vừa, xác nhận thiếu tính năng; không tự coi là lỗi protocol

Menu dùng RequireDungeonContext và toggle leader, kể cả khi chọn member. Engine TrungAc không gọi Party/TrieuTap/AskTeamFollow và không có nhánh member nhận/trả quest hay xác nhận đội. Caller Auto có thể chạy engine cho member nếu cờ được bật qua AutoLogin. Khi đổi leader, cờ trên Game leader cũ không tự chuyển/hủy; menu lần sau điều khiển leader mới.

Cần chốt contract: quest cá nhân hay leader dẫn đội. Với quest cá nhân, menu có thể nên điều khiển selectedGame; với đội, cần session leader/member và mỗi người xác nhận quest riêng. Chưa biết server có quest/kết quả chung đội hay solo, nên không tự copy IsXong hoặc phát UseItem cho cả đội. Không ghi nhận lỗi “leader ra lệnh member Auto OFF” trong TrungAc như Kỳ Cuộc cũ: module này chưa có dispatcher đội đó.

### TA11 — AutoLogin có thể bật lại sau menu OFF — vừa, source xác nhận đường bật lại

Refresh timer có `if (item.IsTrungAc && OnlineTimeSec<60) game.IsTrungAc=true`. Nó không phân biệt “mới online cần khởi tạo một lần” với “người dùng vừa chủ động tắt”. Nếu account tương ứng có IsTrungAc, tắt trong cửa sổ 60s đầu có thể bị bật lại ở refresh tiếp theo. Menu leader và account Game còn có thể khác nhau.

Đề xuất init tự động một lần mỗi phiên online và ưu tiên thao tác dừng thủ công. Không sửa login/captcha/auth trong lượt phân tích; cần mock timer riêng để chứng minh sửa không thay đổi cách đăng nhập hoặc queue account khác.

### TA12 — Lỗi bị nuốt, UI và worker cùng sửa state — vừa, source xác nhận; race cần Windows

FrmMain.Auto catch rỗng quanh từng Game.Auto; parser/đọc memory exception có thể khiến engine đứng mà không nêu nguyên nhân. Refresh cũng catch rỗng quanh SetInfo; một lỗi của một nhân vật có thể làm phần còn lại của lượt cập nhật bỏ qua. Worker chạy TrungAc trong khi UI alarm/menu/AutoLogin cùng sửa cờ/State/timer, không có session lock riêng. Chưa tái hiện race bằng Windows.

Đề xuất log giới hạn tần suất gồm module/state/map/lý do chờ/loại exception; không ghi tài khoản, password, token, dialog chứa dữ liệu cá nhân hay dump memory. Một nơi sở hữu state/timer, lỗi không phục hồi dừng riêng module; giữ khả năng Auto OFF. Snapshot được mock hợp lệ không kiểm chứng native read thành công.

### TA13 — Map/NPC/tên/token và dữ liệu đường đi còn phụ thuộc client — cần xác minh

Engine hardcode Ngô Giới (224,226,map1), tên normalized “ngogioi”, quest token CXDT_090304_01, hoàn tất theo hai CXDY token, lệnh “trungaclenh”, Menpai28 và nhánh Thảo Nguyên. Script XML khớp Ngô Giới/toạ độ, nhưng Level31 chỉ là metadata script; engine chưa kiểm tra level/daily limits/team requirements riêng. Server vẫn có quyền từ chối; không thêm cách vượt các điều kiện đó.

GetMapId dùng chuỗi contains theo thứ tự, không lấy map ID có cấu trúc. Ví dụ “caoxuong” trước “caoxuongmecung”, “thanhnguyen” trước “thanhnguyensondong”, và “lacduong” map0 trước nhánh cùng tên map242: các nhánh cụ thể có thể bị che. Tên bản đồ nhiều lần trong toàn dialog cũng có nguy cơ lấy nhầm tên đầu được matcher ưu tiên. Đây là helper dùng chung, chưa xác nhận quest Trừng Ác của private hiện tại trỏ các map đó.

Đề xuất parser riêng cho nội dung lệnh/quest và cấu hình baseline được kiểm chứng, không đổi cả GetMapId/TIMDUONG/offsets theo suy đoán. Runtime đường đi dùng 16.dat/17.dat; script dùng 20.dat, nên resource recovered chưa chứng minh dữ liệu trên máy người dùng. Native hook/opcodes/offsets chưa được kiểm chứng đầy đủ.

## Kiểm chứng trong lượt này

- **19 mẫu tĩnh PASS:** engine/alarm không đổi, các vị trí lỗi, reversal chính xác hai allowlist và payload hash. Đây là assertion về source, **không phải 19 test engine/game**. Collector lúc đầu dùng mẫu whitespace quá chặt cho nhánh combat; đã sửa regex, không thay đổi engine hay điều kiện phát hiện lỗi.
- ILSpyCmd 9.1.0.7988 disassemble payload gốc có SHA256 `11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573`; đối chiếu parser, timer và reset. Target EXE/native DLL không được chạy.
- Test menu có sẵn Release chạy lại: **38 passed, 0 failed, 0 skipped**. Test dùng method handler/context thật + fake UI/game, xác nhận sửa menu trước đây; không mô phỏng Game.TrungAc.
- `tools/verify_build.py` trên output v0.3 có sẵn: **PASS**, CLR4/x86, 27 resources, dependency identity khớp, binary gốc/stable116 và licensing/VIP/HWID/config/auth được bảo toàn. Có hai cảnh báo dnfile compressed-int legacy; checks vẫn thành công, không tắt verification.
- Không sửa ứng dụng/version/CI/dependency, không rebuild app, không tạo gói tải mới. [Validation](evidence/validation.json) ghi chính xác các checks đã chạy. Chưa có mô phỏng engine Trừng Ác trong lượt này, chưa Windows/game test, chưa chứng minh nhận/trả thưởng hoặc daily limits trên server.

## Đề xuất triển khai tiếp theo

1. **Mô phỏng trước, ưu tiên TA01–04:** biên dịch selected method thực với fake quest/dialog/item/TLBB/commands và clock. Đối chứng source gốc phải tái hiện từng lỗi rồi cùng assertion qua sau sửa. Case dialog rỗng/trễ/sai ngoặc/thiếu Y/tọa độ sai/overflow/map lạ; OFF/pause/reload khi timer cũ còn; lượt 2; dialog trễ giữ state.
2. **Sửa hẹp lifecycle và parser:** adapter riêng Trừng Ác, khối timer ra khỏi side effect UI, trạng thái có owner/lượt và hủy đúng. Kiểm tra Auto ON/online/snapshot ở ranh giới lệnh; không sửa auth hoặc nhiệm vụ khác để giải quyết session.
3. **Mô phỏng và sửa dialog/target TA05–08:** UseItem một lần rồi chờ ack; quest chưa đổi không xong; corpse-first/live-next, nhiều quái, không spawn, loot dài, mất NPC, kẹt đường, map sai, task completed từ server, thưởng chưa nhận. Không đổi quest token/offset/map theo guess hoặc ép task complete.
4. **Hồi quy orchestration TA09–12:** lịch/module cùng bật, đổi leader, AutoLogin bật lại sau OFF, reconnect và UI/worker. Chốt semantics cá nhân/đội từ baseline hiện có, ghi rõ trường hợp chưa biết client; không thêm dispatcher đội mù.
5. Sau các sửa có đối chứng, tăng version chung và build gói review như các đợt trước; giữ stable116 và các sửa Thủy Lao/Kỳ Cuộc. Tiếp tục rà soát phụ bản còn lại. Đến lượt kiểm thử tổng hợp anh yêu cầu, vẫn kiểm tra **từng bước/từng phụ bản có giám sát**, ưu tiên OFF/dừng đúng và xác minh map/NPC/quest private trước lệnh tự động.

Chưa cần yêu cầu người dùng thử game ngay trong lượt này. Các sai khác client được ghi thành điều kiện chưa xác minh; phần phụ thuộc chúng chưa thể được gọi là “fix xong” chỉ từ mô phỏng.

## Thu lại evidence

Từ root repo, dùng môi trường đã chuẩn bị theo [hướng dẫn build](../../docs/ChickenAutoEx-BUILD.vi.md):

```bash
python analysis/trung-ac/tools/collect_evidence.py
# Target chỉ được đọc; payload có hash nêu trên, file nằm trong .build ignored.
dotnet tool run ilspycmd -- -il -t TinhKiemAuto.Game .build/ChickenAutoEx-original.exe > .build/trung-ac-original.il
python analysis/trung-ac/tools/collect_evidence.py --il .build/trung-ac-original.il
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore --filter FullyQualifiedName~DungeonMenuRegressionTests
# Python venv đã cài dnfile; kiểm tra build hiện có, không chạy app.
python tools/verify_build.py
```

Nếu payload chưa có, trích đúng entry ChickenAutoEx.exe từ ChickenAutoEx-new.zip vào .build và kiểm hash trước ILSpy; collector không chạy target. --il đối chiếu payload/mẫu chỉ thị và ghi hash đầu vào, không attestation độc lập của mọi file IL tùy ý truyền vào. Không commit full IL, hydrated source hoặc config/account người dùng.
