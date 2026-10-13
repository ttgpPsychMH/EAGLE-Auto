# EAGLE Auto v0.14 — điều phối Ác Tặc sau kiểm thử Vô Lượng Sơn

Ngày 13-10-2026. Kế thừa v0.13 `f948e01`; nhánh `repair/ac-tac-team-flow-v014`.
Chạy **EAGLE-Auto-v0.14.exe** cùng file `.exe.config` trong gói **EAGLE-Auto-v0.14-net48-review** trên GitHub Actions.
Đây là bản kiểm thử: build và mô phỏng không xác nhận phụ bản chạy thành công trên Windows/client private.

## Những lỗi C# được sửa

| Vấn đề | Hành vi v0.14 |
|---|---|
| Một thành viên chưa sẵn sàng chặn mọi người ở bước đi/vào cửa | Các nhân vật đủ điều kiện tiếp tục đi và thao tác cửa riêng. Khi tuần tra, đội trưởng chờ thành viên chưa sẵn sàng; thành viên đủ điều kiện vẫn được tập hợp. |
| Nhân vật vào trước gặp quái làm thành viên ngoài cửa không được điều khiển | Đánh/nhặt của nhân vật đã vào không chặn lệnh vào cửa của các nhân vật còn ngoài. Chỉ bắt đầu tuần tra khi tất cả participant được nhận diện đã vào và sẵn sàng. |
| Đội trưởng đã vào thì thành viên ngoài không tìm NPC | Mỗi nhân vật có index tìm cửa riêng, nối từ điểm ngoài gần nhất. Không còn phụ thuộc đội trưởng vẫn đứng ở map ngoài. |
| Không yêu cầu Theo Sau | Gọi lệnh yêu cầu Theo Sau hiện hữu, tối đa một lần/5 giây hoạt động, khi ít nhất hai cửa sổ đúng đội, cùng map, đủ điều kiện và không thao tác hội thoại. Không broadcast khi có cửa sổ đã nhận cùng đội tắt Auto/xung đột. Không tự đổi lựa chọn của người dùng. |
| Lệnh đi chung gây lên thú cưỡi trong phó bản | Thành viên cùng map dùng Move trực tiếp; xuống thú cưỡi trước khi tự tập hợp. Nếu đã quan sát thấy Theo Sau thì giữ theo, không gửi GoTo cạnh tranh. |
| Theo Key/đi bãi thường ghi đè điều phối | Coordinator nhận quyền điều phối trong thời gian Ác Tặc chạy; chặn FollowKey và block đi bãi/nhiệm vụ thường cho những thành viên do coordinator quản lý. Đánh/skill/hồi phục hiện hữu vẫn chạy. Hủy hoặc reset session trả quyền ngay. |
| Mất online/identity/init/leader tạm thời xóa nhiệm vụ | Chuyển cảnh/chuyển map tạm dừng, không xét dữ liệu chưa ổn định để xóa nhiệm vụ. Ngoài chuyển cảnh, dữ liệu thiếu được chờ tối đa 5 giây thực, không gửi lệnh. Mã nhân vật/đội đã xác nhận thay đổi vẫn dừng ngay. |
| Thành viên tạm mất dữ liệu biến mất khỏi hàng rào vào cửa | Giữ thành viên đã theo dõi trong session và chờ tối đa 180 giây hoạt động. Tắt Auto, đóng cửa sổ, rời đội hoặc đổi nhân vật dừng session; không tiếp tục với đội bị thu nhỏ giả. |
| Một cửa sổ không liên quan chưa có TLBB làm Party getter ném lỗi | Ác Tặc chụp danh sách cửa sổ riêng, bỏ qua phần tử null và cửa sổ không cùng đội; không đổi getter Party chung. Snapshot bị thay đổi đồng thời được chờ có giới hạn 5 giây. |
| Đi đường vẫn tiến triển nhưng bị timeout tổng chuyến đi | Watchdog đi đến map theo thời gian không có thay đổi vị trí quan sát được, thay vì tổng thời gian chuyến đi. Tìm NPC/hội thoại vẫn có hạn chờ riêng. |
| Đánh/nhặt có thể giữ tuần tra vô hạn | Giữ tuyến khi đang đánh/nhặt; sau yên quái 2 giây nối điểm chưa đi gần nhất. Watchdog 180 giây hoạt động khi số mục tiêu và HP quan sát không tiến triển sẽ dừng có lý do, không bỏ qua quái hay báo hoàn thành. Thời gian chờ boss sau hết tuyến được gia hạn sau giao tranh quan sát được. |
| Talk gửi một lần nhưng không mở dialog | Thử mở dialog tối đa ba lần, cách nhau ít nhất 5 giây, trong hạn 60 giây ban đầu. Chọn option vào cửa vẫn chỉ một lần; không đoán/retry thao tác có thể tiêu vật phẩm hoặc quyền vào. Dừng theo/xuống thú cưỡi trước khi nói chuyện. |
| Dừng không có lý do trong CSV | Giữ phase trước khi dừng và mã lý do; thêm số participant, sẵn sàng, đã vào, đang giữ bởi quái và thay đổi vị trí quan sát. |
| Exception không liên quan tiêu hết budget logger | Lọc framework/context bị loại trước khi lấy budget. Tách số mẫu bỏ do queue đầy và do giới hạn lỗi. |

Không tự bật Auto của thành viên. Không bật nhiệm vụ Ác Tặc riêng trên từng thành viên: chọn nhiệm vụ ở đội trưởng và bật Auto cho các cửa sổ cần tham gia.
Broadcast Theo Sau dùng lệnh cũ; Auto chỉ biết các cửa sổ nó nhận diện, không có roster server đầy đủ. Thành viên chơi trên máy khác hoặc không được nhận diện nằm ngoài khả năng kiểm soát này.

## Những điểm phụ thuộc client được giữ nguyên và phương án thay thế

| Chưa thể xác nhận | Phương án thay thế |
|---|---|
| Tên/type NPC, mã map, tọa độ ngoài và option hội thoại | Giữ dữ liệu 107: Vô Lượng Sơn map6, Tặc Khấu Doanh Địa map170, tuyến trong14 điểm, option50013/-1 và bộ nhận diện NPC cũ. Nếu không tìm/nói đúng NPC, **dẫn đội và vào cửa bằng tay** trong thời gian chờ; khi mọi participant được nhận diện ở170 thì Auto có thể chuyển sang tuần tra. Có thể chọn nhiệm vụ khi cả đội đã ở trong170. |
| Client không thực hiện yêu cầu Theo Sau | Dùng Theo Đội/Theo Sau trong game bằng tay. Nếu Auto chưa quan sát thấy đang theo, nó có lệnh di chuyển riêng làm fallback; fallback vẫn cần Move/native hook hoạt động. |
| Mã đội/online/trạng thái đọc sai | Không nới guard hay ghép đội bằng tên. Kiểm tra đội trong game và số participant trong CSV. Có thể chạy từng nhân vật bằng tay; không khẳng định automatic party hoạt động nếu reader chưa nhận đúng đội. |
| Tuyến bị tường/địa hình sửa hoặc GoTo không tương thích | Dẫn qua đoạn đó bằng tay. Chỉ sửa route sau khi có tọa độ/map xác nhận trên client thực; không đoán offset/packet. |
| Move/Talk đã gọi nhưng DLL/client không xử lý | Gửi CSV cùng quan sát thao tác trong game để phân biệt guard với native channel. Queue acceptance không phải ACK. Giữ EasyHook.dll và ABI/offsets; chưa sửa DLL. |
| Boss/cửa ra/nhận thưởng khác bản107 | Giữ bằng chứng boss cùng ID đã sống rồi HP=0. Ra cửa bằng tay nếu cần; chờ xác nhận map thật. Không tự ClickAll, abandon, nhận thưởng hoặc báo hoàn tất giả. |

Vào cửa bằng tay chỉ thay thế bước tìm/thoại NPC; **không sửa được reader sai map, đội, trạng thái hoặc DLL không thực thi Move**. Hết timeout cần kiểm tra tình trạng đội rồi chọn lại nhiệm vụ. Không tự tăng thời gian chờ vô hạn.

## CSV chẩn đoán

Chỉ ghi khi đã đồng ý bật chẩn đoán. Không upload, không thêm tên/ID nhân vật, tọa độ, account, mật khẩu, chat hoặc packet vào CSV; consent và giới hạn file cũ giữ nguyên.

- `TaskState`: `count` là số participant coordinator nhận diện, `phase` 0=vào cửa, 1=tuần tra, 2=chờ boss, 3=chờ rời map; `state` On/Off; `reason` là mã hữu hạn.
- `TaskReady`, `TaskInside`, `TaskCombatHold`: `count` lần lượt là số actor sẵn sàng, đã quan sát ở170, có quái gần. Không phải số lần lỗi.
- `TaskProgress`: số actor đổi vị trí ít nhất một đơn vị ở lượt điều phối được lấy mẫu, **không phải ACK của DLL hoặc xác nhận một lệnh cụ thể**.
- `SessionCleared`, `AutoDisabled`, `TeamChanged`, `NotOnline`, `MissingIdentity`, `LeaderUnconfirmed`, `CharacterUnavailable`, `OtherTask`, `ReadFailure`, `WaitingTimeout`, `NoProgress` giúp xác định lý do chờ/dừng. `MissingTarget`, `DialogPending`, `OptionMissing` giúp khoanh vùng bước NPC.
- `SamplesDropped` vẫn là tổng lũy kế; `QueueSamplesDropped` và `ErrorSamplesDropped` là các thành phần lũy kế riêng. Không cộng từng dòng thành tổng lỗi, không coi mẫu bị bỏ là packet mạng bị mất.

Lấy mẫu khoảng 5 giây có thể bỏ lỡ chuyển trạng thái ngắn; số participant không chứng minh roster server đầy đủ. Watchdog HP là quan sát tiến triển, không chứng minh reader native chính xác.

## Kiểm thử Windows không cần command

1. Giải nén gói mới vào thư mục riêng; đóng Auto cũ và mở v0.14. Đồng ý ghi chẩn đoán nếu muốn cung cấp CSV.
2. Bắt đầu với hai nhân vật cùng đội. Chọn đội trưởng, bật Auto cho cả hai, chọn Ác Tặc → Vô Lượng Sơn. Xác nhận số participant bằng hai, không chỉ số cửa sổ game bằng hai.
3. Quan sát riêng: yêu cầu Theo Sau; đi map ngoài của từng nhân vật; tìm/nói NPC; chọn vào; cả hai vào170. Nếu cửa chưa tương thích, dùng phương án vào bằng tay ở trên.
4. Trong170, quan sát thành viên theo/tập hợp, xuống thú cưỡi để đánh, và tiếp tục tuyến sau hết quái. Thử một nhân vật vào trước để xác nhận thành viên còn ngoài vẫn được điều phối.
5. Tắt Auto của một thành viên, hủy nhiệm vụ hoặc rời đội để kiểm tra dừng. Không để chạy không giám sát. Sau khi hai nhân vật đạt từng bước mới tăng lên ba.
6. Nếu đứng yên, ghi lại bước đang thực hiện và thời điểm; lấy CSV qua Tiện ích → Chẩn đoán tự động → Mở thư mục log. Không cần bật JIT hoặc chạy command.

## Phạm vi kiểm thử cloud

Mô phỏng dùng code C# thực với reader/dialog/clock/command giả; không chạy EXE automation hoặc load DLL native. Tám ca điều phối mới có đối chứng trên partial v0.13 được kiểm tra SHA-256: chặn thành viên sẵn sàng, combat chặn vào cửa, tìm NPC khi leader đã vào, thiếu Theo Sau, dùng GoTo khi tập hợp, FollowKey cạnh tranh, timeout chuyến đi và giữ combat vô hạn. Đối chứng phải thất bại đúng hành vi, không chấp nhận lỗi harness.

Các ca mới còn kiểm tra pause/identity tạm mất, member null, vào đủ đội, không broadcast tới cửa sổ tắt Auto/xung đột/dialog, retry Talk có giới hạn, không retry chọn cửa, reanchor, HP có tiến triển, hủy giữa lệnh Theo Sau, lý do dừng, dữ liệu ẩn danh và logger bị error storm.

Stable116, snapshot điều tra, native DLL/ABI/offsets, licensing/VIP/auth và các partial Thủy Lao/Kỳ Cuộc/Trừng Ác/Ác Bá không đổi. Ownership hooks trong Game được khai báo và đảo riêng trước verifier bảo toàn toàn bộ phần ngoài delta; không bỏ kiểm tra cả Game hoặc cập nhật checksum binary gốc.

Kết quả cloud: **811 test passed, 0 failed/skipped**, gồm **101 ca Ác Tặc** và **53 ca local diagnostics**. Tám đối chứng v0.13 và 51 đối chứng lịch sử thất bại đúng hành vi cũ, 0 lỗi harness. Mô hình C++ có 26 ca passed. Build 0 lỗi/49 cảnh báo legacy; verifier xác nhận CLR4/x86, 29 resource và bảo toàn source/binary ngoài delta. [Bằng chứng v0.14](evidence/EAGLE-Auto-v0.14-ac-tac-runtime-validation.json).
