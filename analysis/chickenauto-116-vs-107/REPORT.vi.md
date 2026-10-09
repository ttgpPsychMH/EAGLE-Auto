# ChickenAuto 116 và ChickenAutoEx 107: đối chiếu kỹ thuật và tính năng

Ngày: 09/10/2026 UTC. Base: commit merge `5385725` trên `ttgpPsychMH/EAGLE-Auto/master`. Đây là báo cáo phân tích tĩnh; không sửa automation, hook, licensing/VIP hoặc stable 116. Không chạy executable, DLL hay game. “107 hiện tại” là project .NET Framework 4.8 đã merge; khi đối chiếu engine/phó bản dùng snapshot C# của binary Ex gốc, được bản build hiện tại giữ nguyên.

## Kết luận để quyết định hướng phát triển

**Giữ 107 làm nền có source và chọn cải tiến theo hành vi của 116 là hướng hợp lý. Tuy nhiên, chưa có source C++ đầy đủ của 116 để chuyển trực tiếp sang C#.** Bản 116 là native x86/Qt; bản 107 là C# WinForms. Đây không phải hai phiên bản của cùng một project .NET mà có thể merge method tương ứng.

Nhiều chức năng thông thường thấy trong giao diện 116 đã có implementation ở 107: hồi HP/MP, pet, skill, đánh quanh, nhặt/vứt đồ, danh sách nhận buff, tổ đội và phím tắt. Cần kiểm thử hành vi để biết 107 thiếu chức năng, có lỗi, hay chỉ trình bày khác; không mặc định viết lại engine.

Những điểm đáng học từ 116 có bằng chứng rõ nhất là **giao diện chuyển ngôn ngữ** và **các tên thiết lập rõ nghĩa**. Chưa có benchmark để kết luận 116 nhẹ hơn, ổn định hơn hoặc tương thích server tốt hơn. Mã phó bản của 107 là tài sản cần bảo toàn; không có đủ bằng chứng để coi tab phó bản của 116 là một implementation tương đương.

Người dùng báo cáo bản 107 net48 tải từ Actions **đã mở được trên máy của mình**, nhưng **chưa test trong game**. Đây là xác nhận startup từ người dùng, chưa phải xác nhận hook, offsets, quyền VIP hay phó bản. Báo cáo điều tra trước và log build cũ giữ nguyên như hồ sơ lịch sử.

## 1. Nguồn và mức độ bằng chứng

| Mức | Ý nghĩa trong báo cáo |
|---|---|
| C# | Có thân method trong source decompile 107; xác nhận implementation tồn tại, không bảo đảm chạy đúng trong game. |
| Native | Đọc PE, resource, Qt reflection hoặc vài lệnh máy 116; không có source C++ gốc. |
| UI | Nhãn, tooltip, tên cấu hình hoặc Qt slot của 116; thể hiện ý định thiết kế, chưa chứng minh thuật toán hoạt động. |
| Người dùng | Người dùng báo cáo quan sát thực tế; không phải kiểm thử độc lập của môi trường cloud. |
| Chưa biết | Cần phân tích native sâu hơn hoặc thử cùng game/server để kết luận. |

Chỉ so sánh hai payload có provenance trong repository này. Không khảo sát tất cả release từng phát hành, không dùng chương trình từ website cũ, không gửi request tới dịch vụ game/licensing. Mọi địa chỉ trong evidence là vị trí byte/VA của binary phân tích, không phải đề xuất thay offsets game.

## 2. Khác biệt kiến trúc và khả năng khôi phục

| Thuộc tính | Stable được phân phối là 116 | Ex 107 gốc / project 107 hiện tại |
|---|---|---|
| Payload | `ChickenAuto.exe`, **13,757,952 byte** | `ChickenAutoEx.exe` gốc, **2,913,280 byte** |
| SHA-256 payload gốc | `34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea` | `11c98b8eff545fbafa5fd6e9d3d281af62aa7fac4234eb546fcc2a4d671df573` |
| Gói phân phối | ZIP, 7z và overlay wrapper chứa payload giống nhau từng byte | Đã xác nhận tương tự ở điều tra Ex |
| Wrapper `.exe` root | Native x64 SFX, overlay 7z tại byte 400896 | Native x64 SFX; không phải assembly cần decompile |
| Payload thực | PE32 native x86; COM descriptor RVA/size đều 0, không có CLR metadata | PE32 managed x86, CLR 2/.NET Framework 3.5; project sửa target net48/CLR 4 |
| GUI | Dấu vết native C++/Qt, `.qtmetad`, QObject metaobjects; có chuỗi **Qt 5.7.1** ở offset `0x99d768` | WinForms, source C# khôi phục |
| Phụ thuộc | Import Win32; Qt được đưa vào executable, không có import DLL Qt trong import table đã đọc | .NET Framework, JSON, Zen Barcode và native hook; xem tài liệu build hiện tại |
| Resource ứng dụng đã đọc | **13 file** từ một bộ Qt rcc v1 đã xác định | **27 embedded resource**, gồm dữ liệu/scripts và DLL nhúng |
| Source/project | Không có source C++ gốc, project Qt hoặc symbol chức năng đầy đủ; chưa có project 116 build lại được | 331 file C# snapshot, project build net48 đã có |

Chữ “116” lấy từ `PatchInfo.ini` và tên gói stable trong repository. Version resource PE vẫn là `1.0.0.0`, giống cách Ex dùng assembly version không trùng version sản phẩm. Không dùng trường file version để chứng minh số release.

Đã thử **ILSpyCmd 9.1.0.7988** với payload 116: exit code **70**, `MetadataFileNotSupportedException: PE file does not contain any managed metadata.` Đây là giới hạn đúng của công cụ .NET, không phải lỗi cần “sửa runtime” của stable. Phân tích tiếp bằng parser PE/Qt và `objdump`, không thực thi ứng dụng. Không cài hoặc dùng native decompiler như Ghidra trong lượt này, nên không tuyên bố đã khôi phục toàn bộ thuật toán C++ của 116.

Evidence: [inventory](evidence/inventory.json), [kết quả ILSpy](evidence/ilspy-116.txt), [Qt metaobjects](evidence/qt-metaobjects-116.json). Thư viện Qt lớn và lượng resource khác nhau làm kích thước hai executable không phải thước đo số tính năng hay mức sử dụng RAM/CPU.

## 3. Ma trận tính năng

Các tên literal của 116 và offset tương ứng nằm trong [selected-literals](evidence/selected-literals-116.json). Các nhãn/tooltip Anh–Việt nằm trong [256 mục dịch UI](evidence/ui-translations-116.json). Cột 116 phần lớn có mức **UI**, không phải C# implementation.

| Nhóm | Bằng chứng 116 | Bằng chứng 107 | Nhận định / việc cần xác nhận |
|---|---|---|---|
| Ngôn ngữ | `Language`, resource `AutoTLBB_vi.qm` và `AutoTLBB_en.qm`; 256 mục dịch Việt, tiếng Anh dùng chuỗi source | Nhãn tiếng Việt viết trong WinForms designer/source; chưa thấy cơ chế chọn ngôn ngữ tương đương trong các màn đã đối chiếu | 116 có thiết kế localization rõ hơn. Có thể học cách tách nhãn UI sau khi ổn định baseline. |
| Cấu hình | `AutoSettings.json` và tên như `autoRecoverMpPercent`, `enableAttackRadius`, `autoThrowTrashItemList` | [Setting.cs](../chickenautoex-107/recovered/TinhKiemAuto/Setting.cs): INI, `.dat` mã hóa, chuỗi CSV theo index; một số danh sách dùng JSON | Có thể thêm model/schema rõ nghĩa quanh config của 107. Chưa biết đầy đủ layout/kiểu/default của JSON 116; không import trực tiếp. |
| Nhiều game / trạng thái UI | `WorkerDetectGameInstance`, `WorkerUpdateUi`, `GameInstanceListWidget`, slot mở thêm game | `FrmMain.dicGame`, loop `FrmMain.Auto()`, list nhân vật và menu mở thêm game | Cả hai có dấu vết hỗ trợ nhiều instance. Tên worker chưa chứng minh 116 phân luồng hiệu quả hơn. Cần đo độ trễ UI/CPU với cùng số instance. |
| Tray, phím tắt | `enableMinimizeToSystemTray`, bật/tắt shortcut; 9 method trong `ShortcutKeysManager` | `NotifyIcon`, [SetHotKey](../../src/ChickenAutoEx/FrmMain.cs#L2153), [HotKey.cs](../chickenautoex-107/recovered/TinhKiemAuto/HotKey.cs) cũng có nút bật/tắt | Đã có ở 107. Đối chiếu tổ hợp phím, phạm vi game được chọn và lỗi đăng ký, không làm mới cả cơ chế. |
| Hồi HP/MP, pet | Tùy chọn bật riêng, ngưỡng riêng, pet skill HP/MP; BasicSettings có slot tương ứng | [Game.Buff](../chickenautoex-107/recovered/TinhKiemAuto/Game.cs#L7903), `BuffPet`, `CongSinh`, `HuyetTe`, UI ngưỡng HP/MP | Implementation 107 đã tồn tại. Có lỗi checkbox MP cụ thể ở mục 6; chưa xác nhận ngưỡng/chu kỳ tương đương 116. |
| Skill / đánh quanh / KS | `enableAutoUseSkill`, `autoSkillList`, `skill.packetId`, `skill.target`; attack radius, KS, lọc quái | `Attack`, `DoSkill`, `GetBestTarget`, `SaveSkill`, `IsRadius`, [BoQua.cs](../chickenautoex-107/recovered/TinhKiemAuto/BoQua.cs) | So sánh trên cùng quái/map và cấu hình; danh sách option không chứng minh thuật toán chọn mục tiêu giống nhau. |
| Theo key / tổ đội | `enableAttackFollowLeader`, request/abort team follow, accept team request. Tooltip cảnh báo một số game chỉ hỗ trợ follow, không hỗ trợ attack with leader | `AskTeamFollow`, `FollowKey`, `SetTeam`, `AcceptAll` và các nhánh theo đội/phó bản | Không coi tính năng này của 116 luôn hoạt động trên mọi server. Cần xác nhận điều kiện game cụ thể, không chép protocol/offsets. |
| Buff NM theo danh sách | `autoLotusBuffPlayerList`, màn `SettingListPlayerGetLotusBuffHp` | [Buff.cs](../chickenautoex-107/recovered/TinhKiemAuto/Buff.cs), `Setting.BuffValue`, `CheckBuff`; [GameObjects](../chickenautoex-107/recovered/TinhKiemAuto/GameObjects.cs#L414) chọn mục tiêu | 107 cũng có, không phải tính năng cần bổ sung từ đầu. Cần thử thứ tự ưu tiên và ngưỡng, với cùng danh sách. |
| Tử vong / quay lại / cảnh báo | `enableAutoActionWhenDead`, logout sau N giây, tooltip quay lại vị trí chết và ưu tiên phù; cảnh báo HP | `Global.AutoComeBack`, `IsDead`, nhánh `Game.Auto()`/`TheoDoiCanhBao`, cảnh báo HP/PK | Cùng nhóm chức năng, khác chi tiết chưa rõ. Không thay hành vi logout/return khi chưa biết ảnh hưởng phó bản. |
| Nhặt đồ | `enableAutoPickupLootPackage`, shortcut bật/tắt loot, trạng thái đi tới loot | `PickItem`, `ForcePickItem`, `PickAll`, `Global.PickRadius` | Đã có mã 107. Cần kiểm tra bán kính, thời điểm nhặt và việc gián đoạn đội/phó bản. |
| Vứt rác / bảo vệ đồ | Tooltip 116 nêu giữ đồ ≥5 sao hoặc ≥4 sao và cấp ≥50; có danh sách vứt và delay | [DropItem](../chickenautoex-107/recovered/TinhKiemAuto/Game.cs#L9726), `IsDropEx`, `IsCanDelete`, guard ngọc/Long Văn và danh sách tên/loại | Quy tắc bảo vệ không được coi là tương đương. Tooltip chưa chứng minh code native 116 áp dụng chính xác. Nếu cải thiện 107 sau này, thiết kế bộ phân loại bằng dữ liệu giả trước khi nối vào hành động xóa. |
| Tiện ích | x2.5 exp, lên cấp tới ngưỡng, chọn xuất pet, reset giờ chơi, chấp nhận chuyển cảnh | `AutoX2`, `UpLvl`, `XuatPet`, `ResetTime`, các cờ/caller trong `Game.Auto()` | Có code cùng nhóm; x2/x2.5, loại item, buff và điều kiện có thể khác. Chỉ ghi nhận hiện trạng; không sửa cơ chế thời gian/quyền máy chủ. |
| Khởi động / cập nhật | Có `WorkerDetectGameInstance::updateAuto`, `Version`, `NewAutoUrl` và 4 tên fallback; UI download và restart | Bản gốc chặn startup bằng update HTTP. Bản hiện tại kiểm tra metadata HTTPS bất đồng bộ, timeout, không tự download/install | Chưa khôi phục control flow/URL thực của 116, nên chưa kết luận lý do stable mở được. 107 hiện tại có chủ đích chỉ thông báo cập nhật; không chuyển updater 116 vào ngay. |
| Train / hoạt động mở rộng | Có tab skill/train, nhưng chưa khôi phục đầy đủ engine native cho các hoạt động này | `SelectBaitrain`, `ThucThiAutoTrain`, `TimDuong`, `TrongTrot`, `CheDoFree`; FrmMain có tab AutoLogin và Chế Đồ | 107 chứa nhiều code ngoài phó bản và chiến đấu cơ bản. Chưa đủ bằng chứng để nói các tính năng này vắng mặt ở 116 hoặc hoạt động tốt trong 107. |
| Phó bản / nhiệm vụ | Qt tab `QuestSettings`, nhãn “Tự chạy Phó bản / Nhiệm vụ”; xem mục 4 | Thân method `DatDoiAcBa`, `DatDoiAcTac`, `DatDoiKyCuoc`, `DatDoiThuyLao`, `DatDoiLauLanTamBao` và nhánh map trong `P()` | 107 có implementation phía client để bảo toàn. Chưa có chứng cứ tương đương ở 116; không dùng tên tab làm tiêu chí thay engine. |
| Licensing/VIP | Không khôi phục/đối chiếu đầy đủ nhánh xác thực C++ | `Global.IsVIP` và các guard trong module; được giữ nguyên | Không kết luận quyền giữa hai bản tương đương, không port để mở khóa, không dùng credential/hằng số nhúng để truy cập dịch vụ. |

## 4. Phó bản: bằng chứng và giới hạn quan trọng

**116:** `setting_tabs::QuestSettings` có metadata revision 7, **0 method** được khai báo qua Qt reflection, `static_metacall` ở VA `0x408270` chỉ có lệnh `ret`. Context `QuestSettings` trong QM có hai mục: `Auto Quest / Event` → “Tự chạy Phó bản / Nhiệm vụ”, và `Form` với bản dịch rỗng. Bộ 13 resource Qt ứng dụng đã đọc gồm icon, âm thanh, hai QM và hook; không thấy một bộ script phó bản như Ex trong bộ resource này.

Những điểm trên cho thấy **bằng chứng về phó bản 116 hiện rất hạn chế**, không chứng minh chức năng đã hoàn chỉnh. Chúng cũng **không chứng minh toàn bộ 116 không có phó bản**: C++ method thường, callback ngoài Qt reflection, code trong hook hoặc dữ liệu khác vẫn có thể triển khai logic. Chưa decompile engine native/constructor QuestSettings đầy đủ. [Disassembly](evidence/quest-metacall-116.txt) chỉ là dispatch stub, không phải toàn bộ lớp.

**107:** mã điều phối phó bản và 213 định nghĩa script đã được khôi phục ở [điều tra Ex](../chickenautoex-107/REPORT.vi.md). Tuy nhiên có các giới hạn thực sự trong source:

- `Game.IsTKC` luôn trả `false`; `Game.TKC()` rỗng. `DatDoiTKC()` có thân xử lý khác và guard VIP, không được coi là đã bật/chạy được chỉ vì tồn tại.
- `DatDoiQ123LauLan()` có guard `Global.IsVIP`. Giữ điều kiện đó khi lập kế hoạch và khi test.
- `NhiemVuCoBan()` chỉ kiểm tra điều kiện và tính modulo, không có tiến trình làm nhiệm vụ trong chính method đó; những nhánh khác của `Game.Auto()` vẫn có thao tác liên quan nhiệm vụ.
- `P()` và các method đi đội có logic, nhưng map/NPC/skill/offsets phụ thuộc game/server. Chưa chạy game nên không ghi bất kỳ phó bản nào là “PASS”.

Do đó “giữ phó bản” ở giai đoạn này nghĩa là **giữ nguyên code và điều kiện quyền hiện có, rồi xác định module nào thực sự chạy trên server của bạn**. Không đồng nghĩa kích hoạt mọi method đang tắt hoặc điền phần stub.

## 5. Hook và tương thích game: không thay ngang

| Thuộc tính native DLL nhúng | 116 | 107 |
|---|---|---|
| Resource | `:/Resources/EasyHook.dll`, Qt rcc | `TinhKiemAuto.EasyHook.dll`, .NET resource |
| Kích thước | **583,168 byte** | **202,240 byte** |
| Machine | x86 native | x86 native |
| SHA-256 | `f22b7f349f20f94327c365d47a56484092c45dba3fa6b4fa76231f4f49336cef` | `1017f117193687302d3817d66cb9e7914c026d78aced4a02517a5c940efe5d79` |
| Export quan sát | `GetMSG`, `SetHook`, `UnHook` | `GetMSG`, `SetHook`, `UnHook` |

Đã đọc cây resource Qt để xác định chính xác DLL 116, không chỉ tìm một chuỗi tên. Hai DLL **khác byte**. Tên export trùng không chứng minh ABI, calling convention, giao thức message, layout tham số hoặc Lua bridge tương thích. Native source của cả hai DLL chưa có; không thay DLL, không suy ra offsets 107 từ DLL 116. Các chuỗi `LuaPlus.dll` và `UI_CEGUI.dll` ở 116 xác nhận tên module được nhắc tới, chưa chứng minh mọi game đều cung cấp đúng phiên bản ấy.

Ưu tiên kiểm thử nhận diện game và đọc trạng thái ở bản 107 hiện tại trước khi bật từng chức năng. 107 có thể mở được UI nhưng chưa attach/đọc game đúng; đây là điều chưa kiểm chứng, không phải lỗi đã được xác nhận.

## 6. Các vấn đề 107 đã thấy trong source — chưa sửa

### Checkbox MP đọc nhầm checkbox HP

Trong [FrmMain.cs hiện tại](../../src/ChickenAutoEx/FrmMain.cs#L1926), `checkrengenmp_CheckedChanged` ghi:

```csharp
CurGame.IsMP = checkregenhp.Checked;
```

Handler được nối với `checkrengenmp.CheckedChanged`. Vì vậy khi người dùng đổi MP, trạng thái engine nhận giá trị HP thay vì giá trị MP. Ví dụ HP bật/MP tắt thì handler có thể giữ `IsMP = true`. Dòng này có trong snapshot gốc ở line 1921 và hiện tại ở line 1934; không phải lỗi do nâng net48. Đây là sai liên kết UI có bằng chứng tĩnh, chưa đo hành vi runtime trên máy người dùng và **không sửa trong nhánh báo cáo**.

### Thiết lập cũ có đúng 62 phần tử có thể đọc quá index

[LoadSetting](../../src/ChickenAutoEx/FrmMain.cs#L1146) kiểm tra `array.Length > 61`, rồi đọc cả `array[61]` và `array[62]`. Với length = 62, index 62 không tồn tại. Catch ngoài cùng rỗng sẽ che lỗi và có thể để một phần option chưa được áp dụng. Chưa biết người dùng có file config đúng trường hợp này; đây là nhánh lỗi có thể chỉ ra bằng source, không phải kết quả test file thật.

### Lỗi trong loop dễ bị che

`FrmMain.Auto()` có catch rỗng quanh từng game và vòng duyệt. Vì vậy khi kiểm thử game, một chức năng không hoạt động có thể không tạo thông báo. Nếu làm đợt cải tiến sau, chẩn đoán có giới hạn và che thông tin riêng tư nên được thiết kế trước khi sửa thuật toán. Không thay scheduling/catch trong lượt này.

## 7. Ưu tiên đề xuất cho lượt sửa tiếp theo

| Ưu tiên | Công việc | Cổng xác nhận trước khi tiến tiếp |
|---|---|---|
| P0 | Test baseline 107 trên đúng client/server; ghi nhận nhận diện process, thông tin nhân vật, map/HP/MP, trạng thái bật/tắt | Đọc trạng thái khớp game; ghi SHA build và phiên bản client. Nếu chưa khớp, chưa bật automation/phó bản. |
| P1 | Sửa lỗi liên kết checkbox MP và giới hạn index config bằng thay đổi nhỏ riêng; thêm chẩn đoán phù hợp nếu cần | User phê duyệt phạm vi lượt sửa mới; test tổ hợp HP/MP bằng trạng thái giả và kiểm tra lại UI Windows. Giữ nguyên engine/phó bản/licensing. |
| P2 | Cải thiện config, nhóm option, giải thích ngưỡng và localization theo nhu cầu sử dụng | Adapter giữ format cũ và mapping từng cờ; round-trip bằng config giả. Không sao chép account/password giữa hai bản. |
| P3 | Đối chiếu hành vi 116–107 cho skill, loot, buff, quay lại sau chết | Cùng client/map/cấu hình; mỗi lần một tính năng. Chỉ triển khai phần thiếu/lỗi đã chứng minh. |
| Sau baseline | Kiểm thử từng phó bản có quyền hợp lệ, ghi module chạy được và điểm dừng | Quyền/map/server phù hợp, đã xác nhận chức năng thông thường. Không gỡ guard VIP hoặc bật `IsTKC` để thử. |

Quy tắc bảo vệ đồ của 116 là ý tưởng đáng đánh giá, nhưng thay đổi hành động vứt/xóa có ảnh hưởng trực tiếp trong game: trước hết cần phân loại item giả và hiển thị quyết định dự kiến, chưa nối vào thao tác xóa. Không tự áp threshold của tooltip 116 lên 107.

Không đề xuất thay UI bằng Qt, đổi kiến trúc x86, thay hook, nâng tất cả dependency hoặc port engine 116 trước baseline. Các việc này không giải quyết một thiếu hụt chức năng đã được chứng minh trong báo cáo này.

## 8. Kiểm thử tiếp theo và thông tin cần ghi

Có [ma trận kiểm thử](TEST-MATRIX.vi.md) để dùng cùng [hướng dẫn Windows 11](../../docs/ChickenAutoEx-WINDOWS11.vi.md). Ma trận còn trống kết quả game, không phải bộ test đã chạy. Máy cloud không có client/server/game hợp lệ nên không thể hoàn thành phần này thay người dùng.

Ghi version client/server, tên module/map, SHA build, số instance, thiết lập và kết quả kỳ vọng/thực tế. Nếu cần gửi log/screenshot, che tên đăng nhập, password, token và dữ liệu tài khoản; không commit file config game. Thử 116 và 107 ở các lượt riêng, không dùng chung thư mục config hoặc bật cả hai auto trên cùng nhân vật. Không cần chạy lại binary gốc trong cloud để tạo báo cáo này.

## 9. Tái lập, phạm vi commit và kiểm tra bảo toàn

[README](README.md) có lệnh tái lập. Script [compare_static.py](tools/compare_static.py) kiểm tra SHA sáu release gốc, xác nhận ba payload stable giống nhau, parse PE/Qt resource/reflection/QM, kiểm tra ILSpy từ chối native metadata và đọc dispatch stub bằng `objdump`. Tool không chạy target. Raw payload/DLL giải nén chỉ ở workspace ngoài checkout; chỉ evidence dạng text được đưa vào nhánh.

Kết quả: **13 resource Qt, 256 mục dịch, 16 metaobject được chọn để đối chiếu**, hook có hash khác nhau; ILSpy không khôi phục C# từ native 116. Không phải danh sách toàn bộ lớp C++/toàn bộ resource của Qt runtime, không phải decompilation đầy đủ. Sáu binary gốc không đổi, source/project/tests/lockfiles/CI của 107 giữ nguyên so với base merge. Không chạy lại test ứng dụng cho một commit chỉ chứa báo cáo/phân tích; 37 test updater đã PASS ở lượt build trước không đại diện cho game/native 116.

Nhánh này không có build sửa mới, không bổ sung mật khẩu/API key, không merge hoặc phát hành binary. Điều còn thiếu để kết luận ưu điểm thực tế là native algorithm đầy đủ của 116 và kết quả thử cùng game/server.
