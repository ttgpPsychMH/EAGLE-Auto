# So sánh tính năng thường 107–116 và kế hoạch thử game

Ngày 10/10/2026 UTC. Baseline ứng dụng 107: commit `9c59d17`, gồm startup net48, sửa MP/config và menu phó bản. Nhánh báo cáo `analysis/chickenauto-107-116-feature-baseline` không sửa ứng dụng, automation, licensing/VIP, hook, offset, dependencies hoặc stable 116; không tạo build ứng dụng mới.

**107 đã có phần lớn nhóm chức năng thường thấy trên UI 116. Việc cần làm tiếp là kiểm tra điều kiện hoạt động và khả năng dừng trên đúng client, rồi sửa sai lệch có bằng chứng.** Không có source C++ đầy đủ hoặc kết quả game đối chứng của 116 để khẳng định 116 thực thi tốt hơn ở từng nhóm.

## Những gì đã được xác nhận

Người dùng báo mở được bản sửa menu. Ảnh mới cho thấy một nhân vật được liệt kê, thông tin phái/cấp hiển thị, map Đại Lý và tọa độ `(218:152)` trùng với phần hiển thị game; header auto báo HP 99%, MP 99%, PET 0%. Không lưu tên nhân vật hoặc ảnh vào repository. Đây là **quan sát một thời điểm**, chưa xác nhận độ chính xác HP/MP, pet chưa xuất hay lỗi đọc pet, cập nhật khi đổi map, đọc đội trưởng, gửi action, sử dụng thuốc/skill hoặc hoàn thành phó bản. Người dùng xác nhận đây là server private, **không phải game4you.us**, và không biết version/build client. Tên trong tiêu đề cửa sổ không xác định server/engine. SHA gói auto và fingerprint EXE client chưa được cung cấp; ghi phiên bản chưa rõ, không chặn việc đối chứng hai auto trên cùng client.

Với 116, đã tái lập phân tích ZIP/7z/wrapper: payload giống nhau, SHA256 `34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea`, native x86, Qt 5.7.1; ILSpy từ chối vì không có CLR metadata. 13 resource Qt, 256 mục dịch và 16 metaobject được chọn khớp từng byte với evidence cũ. Không chạy executable/DLL. [Báo cáo kiến trúc lịch sử](REPORT.vi.md) mô tả giới hạn phân tích native.

Mức **C#** dưới đây nghĩa là có implementation trong 107; mức **UI116** là literal/tooltip/Qt slot, chỉ xác nhận tùy chọn hoặc ý định thiết kế. Hai mức này không phải PASS trong game. [Evidence hiện tại](evidence/regular-feature-baseline.json) lưu vị trí/hash method 107 và offset literal/class 116 cho từng ID.

## Đối chiếu từng nhóm tính năng thường

| ID / tính năng | 107: implementation và điều kiện đáng chú ý | 116: bằng chứng UI/native | Khác biệt và ca cần thử |
|---|---|---|---|
| F01 Nhận diện và trạng thái | `TLBB.Read`, `FrmMain.SetInfo`; đọc process/memory và hiển thị nhân vật/map/HP/MP | Worker detect/UI và danh sách game | Ảnh 107 xác nhận hiển thị một thời điểm. R01–03: đổi trạng thái, map, nhân vật; chưa kết luận offsets đúng hoàn toàn. |
| F02 Hồi HP | `Game.Buff`: `IsHP`, HP **<=** ngưỡng `Global.BuffHPPercent`; tick %66, không follow/cưỡi/người chết. Duyệt các loại thuốc đã nhận diện, có nhánh skill pet | `autoRecoverHp`, ngưỡng riêng; tooltip tìm vật phẩm hồi HP | R05, R07–10: thuốc thực dùng, ngưỡng và điều kiện chặn. Có thể dùng nhiều item phù hợp trong một lần duyệt; không giả định chỉ dùng một thuốc. |
| F03 Hồi MP | `IsMP`, MP **<=** `Global.BuffMPPercent`; điều kiện chung của `Buff` như HP. Checkbox MP đã sửa đúng, 19 test UI/settings là evidence từ đợt trước | `autoRecoverMp`, `autoRecoverMpPercent` | R06–10: bốn tổ hợp HP/MP và tình huống HP bình thường/MP thấp. Test mock chưa chứng minh thuốc trên client. |
| F04 Hồi HP/Hoan Hỉ pet | `BuffPet`: HP <=50%, hoặc <=85% và thiếu ít nhất 10.000 HP; Hoan Hỉ <=81 và >0 với item tương ứng. Tick %9/%66, `IsPet`, không follow/cưỡi; không kiểm tra `IsAuto` | `autoRecoverPetHp`, `autoRecoverPetHpPercent` và slot đổi ngưỡng | 116 có ngưỡng pet trên UI; 107 dùng số cố định. R04, R11–13 kiểm tra hành vi và dừng; PET 0% trong ảnh chưa đủ kết luận. |
| F05 Cộng Sinh/Huyết Tế | `Game.Buff`: cờ/ngưỡng riêng, yêu cầu pet skill type đúng; vẫn chịu điều kiện chặn chung của `Buff` | Hai cờ/ngưỡng pet skill riêng; tooltip nhắc cả skill thường/cao | R15–16: nhận diện pet/skill, trigger, tiêu hao và tắt. Chưa biết ID/skill type client này có khớp 107. |
| F06 Xuất/thu pet | `XuatPet` chọn PetId và điều kiện online, không cưỡi, pet chưa xuất; có nhánh thu pet theo level trong `Auto` | `enableAutoSummonPet`, `autoSummonPetId`, combo chọn pet | Xuất tự động có ở cả hai; chưa chứng minh 116 có thu theo level tương đương. R14: chọn đúng pet, đổi lựa chọn, dừng. |
| F07 Đánh thường/F1 | `Attack`, cờ `IsAttack/IsAuto`, phím cơ bản; nhiều guard theo nhiệm vụ/map/state. Trừng Ác đang bật có thể chặn nhánh đánh thường | Tự đánh và `enableUseF1`, tooltip yêu cầu đặt skill tại F1 | R17: một mục tiêu, F1 cụ thể, tắt và đổi mục tiêu. Tắt nhiệm vụ khi thử combat thường để không lẫn điều kiện. |
| F08 Danh sách skill | `SkillDo/DoSkill`, `SaveSkill`; tick %12, cooldown, MP, target/type/range; nhánh PK/buff | `autoSkillList`, packetId/target và tab Train/PK, nút Save | R18: một skill trước, cooldown/MP/target, rồi nhiều skill. Nhánh skill 448 có xử lý phó bản riêng; chưa sửa/chuyển logic đó. |
| F09 Đánh quanh/gom/lọc quái | `GetBestTarget`, `IsRadius` gắn với map; radius nội/ngoại, gom, danh sách bỏ qua. Có nhánh map phó bản | Tọa độ/radius, ignore list, KS, nhãn Lùa Quái | R19–20: giới hạn vị trí, quái bỏ qua, chuyển map, 1:1/gom. Không coi gom của 107 đồng nghĩa KS của 116. |
| F10 Theo key | `FollowKey`, cờ/global radius, object key và guard state/map; follow tổ đội là đường khác | `enableAttackFollowLeader`; tooltip nói một số game chỉ hỗ trợ theo sau, không đánh cùng key | R21–22 tách **theo sau** và **đánh theo mục tiêu key**; cần đội trưởng/client được nhận diện. |
| F11 Buff Nga My | Nhánh trong `Game.Auto`, yêu cầu Menpai==5, MP, cooldown/party/state; ngoài phó bản và trong phó bản có nhánh khác. UI ghi đúng `IsNM` nhưng thông báo đọc nhầm checkbox Huyết Tế | Cờ/ngưỡng, danh sách người nhận buff | R23 là lỗi thông báo cụ thể; R24 cần nhân vật NM, ưu tiên/self/team. Nhân vật trong ảnh là Thiên Long nên chưa thử engine NM. |
| F12 Nhặt đồ | `PickItem/ForcePickItem`, radius, blacklist/kẹt, đầy túi, busy/follow/map; nhặt có thể được ưu tiên trước attack | `enableAutoPickupLootPackage`, shortcut và trạng thái di chuyển tới đồ | R25–26: trong/ngoài radius, tắt nhặt, hồi phục/attack/follow có bị gián đoạn. `ForcePickItem` trong nhiệm vụ không mặc định cùng quy tắc UI nhặt thường. |
| F13 Vứt rác/bảo vệ đồ | `DropItem`, danh sách tên/loại, `IsCanDelete`, ngọc/Long Văn; nhiều nhánh, gồm nhánh nhiệm vụ không dùng cùng bộ guard | Tooltip giữ >=5 sao hoặc >=4 sao và cấp >=50; danh sách/delay | Quy tắc khác nhau; tooltip chưa xác nhận implementation native. R27 dành cho dữ liệu giả và rà soát nhánh trước; chưa thử bằng việc xóa đồ thật. |
| F14 Sau tử vong/cảnh báo | `Auto`, `TheoDoiCanhBao`, các cờ comeback/hồi sinh; nhánh quay lại thường loại trừ map phó bản và Trừng Ác | Action sau chết, logout theo giây, quay lại ưu tiên phù, cảnh báo HP | R28: map thường, từng lựa chọn riêng. Không port quy tắc này sang phó bản khi chưa có kết quả. |
| F15 Đồng ý tổ đội/chuyển cảnh | `Accept/AcceptAll` và caller có sẵn cho tổ đội; có xử lý chuyển map nhưng chưa xác minh tùy chọn accept-scene độc lập tương đương 116 | `enableAutoAcceptTeamRequest`, `enableAutoAcceptSceneTransfer` | R29 tách hai hành vi; ghi thiếu option/không áp dụng thay vì coi mọi nhánh chuyển map là accept-scene. |
| F16 Cấu hình | Global `.dat`/CSV index và cấu hình nhân vật/danh sách; hai index map tùy chọn đã sửa. Ngưỡng HP/MP thuộc Global, cờ HP/MP thuộc từng Game | `AutoSettings.json`, tên field rõ nghĩa | R30, R32: reopen, đổi nhân vật, scope option. Chưa biết đầy đủ scope/default/schema của JSON 116; không import trực tiếp qua 107. |
| F17 Phím tắt/tray/nhiều game | `SetHotKey`, NotifyIcon, list Game; checkbox nhân vật ghi `IsAuto` | Shortcut manager có 9 method, tray/global shortcuts, game list | R03–04, R31–32: phạm vi một/tất cả game, mất lựa chọn, dừng. Tên worker 116 không chứng minh UI mượt hơn. |
| F18 Tiện ích level/exp | `UpLvl`, `AutoX2` dùng item/buff identifier đã có | Levelup tới ngưỡng và x2.5 exp | R33 khi server hỗ trợ hợp lệ; không đồng nhất “x2”/“x2.5” hoặc thay cơ chế reset thời gian/quyền máy chủ. Các chức năng nâng cao AutoLogin/chế đồ/train cần phạm vi riêng. |
| F19 Ngôn ngữ | Nhãn Việt trong designer/source; chưa thấy selector tương đương trong UI đã rà | Selector Language và QM Anh/Việt | R34: 116 có thiết kế localization rõ hơn. Ưu tiên sau baseline, không cần đổi WinForms sang Qt. |
| F20 Hiệu năng/chẩn đoán | Vòng duyệt nhiều Game, catch rỗng; một lỗi có thể làm bỏ qua phần còn lại của lượt xử lý mà không báo | Tên worker detect/UI, log path; chưa khôi phục toàn bộ scheduling/catch | R35: cùng số instance/thời lượng/tính năng, CPU/RAM/độ trễ; chưa có số đo nên không xếp hạng nhẹ/ổn định. |

## Ba phát hiện mới và thứ tự ưu tiên sửa

### 1. Kiểm soát dừng hồi pet — ưu tiên đầu tiên cần xác nhận

`FrmMain.Auto()` gọi `value.Auto();` rồi, khi tick phù hợp, vẫn gọi `value.BuffPet();`. `Game.Auto()` có `if (!IsAuto) return;`, nhưng `BuffPet()` không kiểm tra `IsAuto` hoặc `Global.Paused`. Khi vòng ngoài đang được phép chạy, metadata pet hợp lệ và `IsPet` còn bật, đường gửi lệnh hồi pet vẫn có thể đạt tới sau khi Auto nhân vật đã tắt. Đây là kết luận về control flow, chưa phải quan sát tiêu thụ vật phẩm trên client.

Chọn **R04** làm ca đầu tiên để kiểm chứng sau đọc trạng thái. Nếu tắt Auto mà vẫn dùng thuốc pet, đó là lỗi ưu tiên sửa riêng trước khi thử phó bản. Phạm vi dự kiến: gate hồi pet và kiểm thử giả cho ON/OFF/pause, giữ caller/engine nhiệm vụ và licensing. Cần xác định rõ ý nghĩa “Ngừng/Chạy” so với checkbox từng nhân vật trước khi quyết định gate; không mặc định UI nào cũng cùng state. Nếu còn hành động ngoài dự kiến, đóng application auto để kết thúc lượt thử; chỉ bỏ tick Auto chưa được chứng minh là chế độ chỉ quan sát hoặc không gắn hook.

### 2. Ngưỡng hồi pet chưa có parity với 116

`BuffPetPercent` chỉ xuất hiện ba lần trong `Game.cs`: khai báo, load và save; `BuffPet()` không đọc field đó. UI 107 được rà chưa có numeric selector cho ngưỡng hồi pet tương đương 116. Vì vậy đây là khác biệt chức năng có bằng chứng, **không phải đã xác nhận một slider hiện có bị hỏng**. Sau R11–13, có thể thiết kế option/adapter giữ cấu hình cũ và default chính sách cũ; chưa đổi số cố định hoặc tác động vào phó bản trong đợt này.

### 3. Thông báo bật/tắt buff Nga My đọc sai cờ

`checkisNM_CheckedChanged` ghi `CurGame.IsNM = checkisNM.Checked`, nhưng balloon dùng `checkhuyette.Checked` để chọn “Bật/Tắt”. Khi hai checkbox khác nhau, thông báo không khớp lựa chọn NM. Đây là lỗi UI tĩnh cụ thể, khác với việc skill NM có hoạt động không. Phạm vi sửa sau: đúng một binding thông báo và mock hai trạng thái checkbox; chưa sửa target/buff trong `Game.Auto`.

Không ghi `GetBestTarget` truy cập `list2[0]` là lỗi runtime đã chứng minh: nhánh fallback đáng rà nhưng một số tổ hợp list có thể không đạt được trong trạng thái nhất quán. Cũng không coi guard follow/cưỡi, nhánh riêng phó bản, hoặc ngưỡng dùng `<=` là lỗi chỉ vì UI ghi “<”. Cần đối chiếu hành vi mong muốn trước.

## Trình tự kiểm thử và cổng chuyển sang phó bản

[Ma trận và cách ghi kết quả](GAME-TEST-PLAN.vi.md) cùng [CSV điền kết quả](GAME-TEST-RESULTS.template.csv) có mã ca R00–R35, D00–D07. CSV là **kế hoạch**, không phải kết quả game đã chạy. Những ô source/mock từ đợt trước cũng không được điền PASS cho action game.

1. R00–04: startup, đọc trạng thái thay đổi, đổi nhân vật/map và xác minh dừng. Ảnh hiện tại chỉ hoàn thành một phần R01. Nếu đọc sai hoặc không dừng được thì dừng nhóm sau và chọn lỗi tương ứng.
2. R05–26: HP/MP, pet, combat/skill, radius/lọc, follow, buff NM nếu có nhân vật phù hợp, nhặt đồ. Thử 116 và 107 riêng trên cùng client/map/thiết lập có thể đối chiếu. Với option không tương đương, ghi **khác biệt**, không ép cùng một value/schema.
3. R27–35: xử lý đồ bằng giả lập trước, cấu hình/phím tắt/cảnh báo và các mục tùy nhu cầu. Hiệu năng/localization không phải điều kiện bắt buộc để vào phó bản; các chức năng dùng trong phó bản phải đạt hoặc được xác định không áp dụng.
4. Sau baseline cốt lõi, thử D00–05 từng nhiệm vụ có quyền hợp lệ. Ghi đội trưởng, entry/map, mục tiêu, điểm dừng và kết thúc; không chỉ ghi “menu bật được”. D06 Thủy Lao vẫn “Not work”, D07 stub/VIP không mở khóa để thử.
5. Chọn lỗi theo ca thất bại + evidence source. Ưu tiên dừng/đọc trạng thái, HP/MP/pet, sau đó mới NPC/dialog/map/boss transition cụ thể của phó bản. Mỗi đợt sửa một nhóm nhỏ và có kiểm thử giả lập trước Windows.

## Tái lập và giới hạn

Sau khi chạy `compare_static.py` theo [README](README.md) với một thư mục `--work` mới, từ root repository chạy:

```bash
python analysis/chickenauto-116-vs-107/tools/regular_feature_evidence.py \
  --stable-evidence /workspace/chickenauto-compare-output/evidence \
  --output /workspace/chickenauto-regular-feature-evidence.json
```

Script kiểm tra evidence native mới khớp evidence lịch sử, anchor/hash method cho 20 nhóm, các đường source đã nêu và việc application/release/test/CI không đổi so với `9c59d17`. Output local trong repository được lưu tại [regular-feature-baseline.json](evidence/regular-feature-baseline.json). Tên/hash method không phục hồi thuật toán C++ của 116 hoặc xác nhận feature parity.

Không chạy lại build/94 regression test cho commit chỉ có báo cáo và công cụ phân tích. Lần build menu trước đã đạt 94 test; đó là bằng chứng lịch sử cho updater/settings/menu, không phải 94 test game hoặc test native 116. Máy cloud Linux không có phiên Windows/client game/tài khoản hợp lệ để thực hiện R/D; phần live cần người dùng chạy theo ma trận và gửi kết quả đã che thông tin riêng tư.
