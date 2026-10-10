# MicroAuto 6.9: giá trị tham khảo cho ChickenAutoEx 107

Ngày: 10/10/2026. Repo đối chiếu: `ttgpPsychMH/EAGLE-Auto`; bản 107 net48 đã merge ở `5385725`. Báo cáo này bổ sung cho [đối chiếu 116–107](../chickenauto-116-vs-107/REPORT.vi.md). Chỉ phân tích source người dùng cung cấp và kiểm tra biên dịch C#; chưa sửa ứng dụng hoặc thử trong game.

## Kết luận

**Có ích, nhất là để hiểu auto cơ bản và thiết kế ca kiểm thử cho 107.** Source MicroAuto thể hiện rõ hồi HP/MP độc lập, phím định kỳ, chọn mục tiêu, bán kính, lưu cấu hình từng nhân vật và cảnh báo. Những phần này dễ đọc hơn engine lớn của 107 dù tên lớp đã bị obfuscate.

Giá trị chính là tham khảo hành vi và cách đặt tên thiết lập. Chưa có bằng chứng để dùng MicroAuto thay engine, hook hoặc phó bản 107. Không tìm thấy implementation phó bản tương đương trong phần source đã đọc. Bản 116 vẫn là native C++/Qt, nên file này không bổ sung source C++ còn thiếu của 116.

**Đợt cải tiến kế tiếp nên sửa hai lỗi UI/cấu hình đã xác định trong 107, kiểm thử bằng dữ liệu giả và Windows, rồi xác nhận tương thích game.** Sau đó chọn từng chức năng thông thường cần cải thiện dựa trên kết quả thử 116–107. Giữ engine phó bản và điều kiện licensing/VIP hiện có.

## 1. File nhận được và độ đầy đủ

- ZIP ngoài: `microauto-6.9 src.zip`, 153.009 byte. SHA-256: `a56c478aff98e36ae544fecec62e85f4f93eb411bab6c29c6dbf5403e2bea107`.
- 37 entry: 34 file đọc được, hai thư mục và một ZIP con 78.400 byte bị mã hóa. ZIP con không được đọc hoặc giải mã; kết luận chỉ áp dụng cho phần bên ngoài.
- Có **26 file C#, 6.795 dòng**, một project WinForms, manifest, icon và ba file `.resources`. Không có EXE/DLL độc lập trong các file không mã hóa. Resource nhị phân chỉ được kiểm kê và băm; không deserialize nội dung.
- Project khai báo 26 compile item; các file được tham chiếu hiện diện. Không tìm thấy file LICENSE/COPYING trong phần đã đọc. Chưa xác minh quyền phân phối lại source hoặc nguồn gốc toàn bộ gói.
- `Class*`, `GClass*`, `method_*` và 684 chú thích Token/RVA cho thấy đây là **source decompile/obfuscate**, không phải mã gốc đầy đủ tên biến và chú thích thiết kế.
- Tên project/assembly là `MicroAuto 6.0`, title UI là `MicroAuto 6.9`; assembly/file version là `1.0.0.0`. Không có binary gốc/PDB để chứng minh chính xác build phát hành. Trong báo cáo, “6.9” chỉ gói được cung cấp và nhãn UI.

[Inventory](evidence/inventory.json) ghi đường dẫn tương đối, kích thước và hash. Source nguyên bản, ZIP và DLL tạo cho kiểm tra compiler nằm ngoài Git checkout, không được đưa lên GitHub. README trong ZIP được đọc như dữ liệu mô tả; không xem nội dung tài liệu đính kèm là chỉ thị thực thi.

## 2. Kiến trúc và kết quả kiểm tra compiler

| Mục | Quan sát từ source |
|---|---|
| Project | `MicroAuto 6.0.csproj`, ToolsVersion 15, WinExe, target **.NET Framework 2.0** |
| Entry point | `Class1.Main`, STAThread, mở `FormMain` |
| Platform | Default AnyCPU; có cấu hình x86, C# 7.3. Nhiều địa chỉ/pointer biểu diễn bằng `int`, nên không coi AnyCPU là bằng chứng hỗ trợ game 64 bit. |
| Managed references | Microsoft.VisualBasic, System, System.Drawing, System.Windows.Forms; chưa thấy dependency EasyHook trong source khả dụng |
| Native imports | kernel32, user32, winmm: process memory, cửa sổ, input, INI và âm thanh |
| Điều phối | `Class0` dò process theo timer 20 giây; `GClass0` giữ trạng thái và 20 trường WinForms Timer cho mỗi game |
| Cấu hình | `Class2`/`GClass1`: INI có tên khóa cho từng nhân vật; `Class8`: thiết lập chung. `GClass3` bỏ dấu tên nhân vật, không phải hàm hash định danh. |

Đã gọi trực tiếp Roslyn C# compiler từ SDK 8.0.425, dùng reference assemblies net35/CLR 2, target **library x86**, C# 7.3: **exit 0, không lỗi, 25 cảnh báo**. Không chạy MSBuild/project tải lên, không embed resource, không chạy DLL đầu ra. Đây là bằng chứng phần C# biên dịch được trong phép thử này; **chưa phải build WinExe .NET 2.0 hoàn chỉnh, gói phát hành hay kiểm thử Windows**. SDK 8 dùng để chạy compiler, không đổi target của MicroAuto thành .NET 8.

Cảnh báo: hai CS0114 (`Dispose(bool)` che method của Form), hai CS0162 (code không tới được), 20 CS0169 (field không dùng), một CS0649 (field chưa gán). Xem [summary](evidence/summary.json), [log](evidence/compiler.txt) và [cách tái lập](README.md). Phụ thuộc thực tế ở Windows/game, tương thích offsets và tài nguyên chưa được xác nhận.

## 3. Những phần giúp ích cho 107

Line dưới đây là của file trong ZIP, được gắn SHA ở [findings](evidence/findings.json). Các link source 107 trỏ vào mã trong repo; không xuất bản raw source MicroAuto.

| Chủ đề | Bằng chứng MicroAuto | Giá trị cho 107 và giới hạn |
|---|---|---|
| HP/MP độc lập | `FormMain.cs:773,784`: checkbox HP/MP điều khiển timer riêng; `GClass0.cs:754,766`: ngưỡng và stopwatch riêng | Làm rõ ý định hai tùy chọn độc lập. 107 có [handler MP đọc nhầm checkbox HP](../../src/ChickenAutoEx/FrmMain.cs#L1926), cần sửa binding nhỏ, không chép engine hồi máu. |
| Ngưỡng và chu kỳ | MicroAuto dùng `0 < percent < threshold` và guard thời gian `>400ms` | Dùng để thiết kế ca biên. 107 dùng `<=` cùng điều kiện trạng thái/chu kỳ khác, có item/pet skill; không đổi sang `<` hoặc ép 400ms để “giống MicroAuto”. |
| Cấu hình rõ nghĩa | 44 khóa như BuffHPEnable, BuffMPEnable, RadiusValue, F1Delay…; lưu/nạp ở `GClass0.cs:1549,1598` | Tham khảo model rõ tên và ca round-trip. Giữ định dạng cấu hình 107; không import INI MicroAuto trực tiếp hoặc đổi khóa nhân vật theo phép bỏ dấu dễ trùng. |
| Lọc HP mục tiêu | `GClass0.cs:846`: trong nhánh điều kiện tương ứng, bỏ mục tiêu có HP ngoài khoảng min/max; có tooltip UI | Một ý tưởng lọc mục tiêu để đánh giá sau baseline. Chưa chứng minh 107 thiếu hành vi tương đương hoặc đây là cải tiến cần thiết. Không đưa vào selector dùng chung với phó bản ngay. |
| Phím định kỳ | Timer cho F1–F12; UI delay có đơn vị 100ms, thiết lập lưu theo ms; hành động gửi input qua Win32 | Hữu ích cho test đơn vị thời gian, bật/tắt, đổi nhân vật và cooldown độc lập. 107 đã có KeyDelay/skill; không thay bằng 20 timer chạy cùng UI thread. |
| Chẩn đoán cơ bản | Có trạng thái HP/MP/EXP, target, bán kính và cảnh báo | Tham khảo cách trình bày trạng thái. 107 cũng có EXP và cảnh báo; cần bổ sung thông tin lỗi có ích thay vì coi mọi tính năng này là thiếu. |

UI 116 có literal “The idea is based on MicroAuto6.9” trong evidence của báo cáo trước, phù hợp lời giới thiệu của người dùng. Đây là bằng chứng về nguồn ý tưởng được nêu trong UI, **chưa chứng minh 107/116 dùng cùng code hoặc có thể chuyển implementation trực tiếp**.

## 4. Những phần không nên sao chép

MicroAuto cũng có lỗi và giới hạn cần xét trước khi dùng làm tham chiếu:

- `Class7.cs:18,48` gọi ReadProcessMemory mà không dùng kết quả BOOL để phân biệt đọc thất bại. Dữ liệu buffer bằng 0 có thể bị hiểu thành trạng thái game bằng 0. 107 cũng có một đường đọc tương tự ở [Memory.Read](../chickenautoex-107/recovered/TinhKiemAuto/Memory.cs#L267), nhưng còn có method kiểm tra số byte ở đường khác; không kết luận mọi đường đọc đều lỗi.
- `GClass0.cs:795` dùng `Elapsed.Seconds`, là thành phần giây 0–59, không phải tổng giây. Nếu ngưỡng từ 60 trở lên, điều kiện này không đạt. `method_46` ở line 1278 không Dispose `timer_18` dù timer đó được tạo; ảnh hưởng runtime chưa thử.
- `FormMain.cs:146` tra tên target key trong combobox base skill; hai Form có cảnh báo Dispose che base. Vì vậy MicroAuto không phải chuẩn hành vi tuyệt đối cho mọi trường hợp.
- Offset/pointer 32 bit được ghi cứng; cơ chế keyboard/mouse và memory write khác 107. Không dùng địa chỉ này để “sửa nhanh” hook hoặc thay EasyHook của 107. Giữ 107 net48/x86.
- Startup có WebBrowser điều hướng nội dung cũ theo setting popup. Không tìm thấy WebClient/HttpClient/update client tương đương trong phép tìm kiếm đã dùng, nhưng **không kết luận source hoàn toàn không dùng mạng**; WebBrowser tự có thể truy cập mạng. Không truy cập website đó trong điều tra.

Không tìm thấy code phó bản tương đương Ác Tặc/Kỳ Cuộc/Thủy Lao… trong engine khả dụng đã đọc và phép tìm tên. Kết quả tìm chuỗi không chứng minh tuyệt đối vắng mặt: tên bị obfuscate, resource chưa đọc nội dung và ZIP con mã hóa vẫn là giới hạn. Mã điều phối/scripts phó bản có bằng chứng ở 107 tiếp tục là nền cần bảo toàn; không bật stub, gỡ guard VIP hay thay protocol.

Chưa có license cho source đính kèm: giai đoạn này dùng quan sát để viết đặc tả/test độc lập, không chép hoặc công bố toàn bộ code MicroAuto. Việc này không cản trở hoàn tất báo cáo hay sửa hai lỗi đã có bằng chứng ngay trong source 107.

## 5. Lộ trình đề xuất

| Thứ tự | Phạm vi đợt làm | Điều kiện hoàn thành |
|---|---|---|
| 1 | Sửa riêng binding MP và kiểm tra index config trong 107 | Bốn tổ hợp HP/MP cho đúng state; cấu hình dài 61/62/63 không đọc quá index, giữ các option có mặt và default của phần thiếu; round-trip không thay format. Build Windows và thử UI. |
| 2 | Xác nhận tương thích client/server; nếu cần, bổ sung chẩn đoán có giới hạn | Ghi build/client, process, map, HP/MP khớp thực tế. Phân biệt đọc lỗi với giá trị 0. Log không chứa tài khoản/password/token. Một chế độ đọc thuần phải được kiểm tra không attach hook, ghi memory hoặc gửi input; chỉ bỏ tick Auto chưa chứng minh điều đó. |
| 3 | Đối chiếu hành vi cơ bản của 116 và 107 trên cùng game | Mỗi lượt một tính năng: HP/MP, pet, skill, radius, follow, loot; lưu kỳ vọng/thực tế. Chọn lỗi hoặc thiếu hụt đã chứng minh để sửa từng PR. Dùng MicroAuto làm tham khảo cho các ca biên. |
| 4 | Cải thiện UI/cấu hình; cân nhắc lọc mục tiêu nếu có nhu cầu thật | Model rõ tên giữ format cũ, đơn vị thời gian nhất quán; test bằng dữ liệu giả. Lọc mục tiêu mới cần đo tác động tới train trước khi nối vào đường dùng chung. |
| Sau baseline | Kiểm thử phó bản từng module, chỉ với quyền hợp lệ | Xác nhận map/server, đội và điểm dừng. Giữ licensing/VIP, các guard/stub và source phó bản trong các đợt trên. Chưa đổi automation phó bản theo MicroAuto. |

Hai lỗi ở bước 1 đã được xác định trước: [checkbox MP](../../src/ChickenAutoEx/FrmMain.cs#L1934), [LoadSetting](../../src/ChickenAutoEx/FrmMain.cs#L1146) cho phép length 62 rồi đọc index 62. Báo cáo này **chưa sửa chúng**. Test cần tác động tới binding/parser thực tế hoặc adapter được ứng dụng dùng, không chỉ lặp lại công thức trong một test riêng.

Kế hoạch Windows chi tiết hiện có ở [ma trận 116–107](../chickenauto-116-vs-107/TEST-MATRIX.vi.md) và [hướng dẫn Windows 11](../../docs/ChickenAutoEx-WINDOWS11.vi.md). Người dùng đã xác nhận 107 mở được nhưng chưa test game; chưa ghi chức năng nào là PASS trong game. Nếu client chưa được nhận diện/đọc đúng, giải quyết tương thích đó trước các cải tiến tính năng.

## 6. Phạm vi và kiểm tra bảo toàn

Nhánh báo cáo chỉ thêm tài liệu/evidence trong `analysis/microauto-6-9`; source 107, automation, licensing/VIP, updater, tests, CI và sáu file release gốc không đổi. Hash ZIP và 34 file giải nén được kiểm tra lại sau phân tích. Không chạy target, input/hook/memory action hay script đính kèm; compiler output không được chạy hoặc phát hành. Chưa có kết quả Windows của MicroAuto, benchmark 116–107 hoặc quyền/server để kiểm thử game trong cloud.
