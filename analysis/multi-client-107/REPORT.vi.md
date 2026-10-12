# MultiAcc và quản lý nhiều client — rà soát, mô phỏng

Ngày 12-10-2026, Asia/Bangkok. Base `56b4d899f4eca6ee2c46a9b2fe4ec1e9d13ec30a`, EAGLE Auto **v0.10**. Nhánh `analysis/multi-client-investigation`.

## Phạm vi và kết quả

Đã rà soát hai đường xử lý `MultiAcc` của 107, các đường mở game và bằng chứng assembly 116. Bộ mô phỏng chạy **46 kiểm tra: 46 đạt, 0 thất bại**, trong đó **9 kiểm tra tái hiện hành vi có rủi ro của legacy**, không phải chín lỗi đã sửa trong ứng dụng. Mô hình giữ chỗ được thử với 2, 3, 4, 6 client, gồm 200 lượt stress mở đồng thời. Không tiến trình game thật nào được tạo.

Đã build công cụ đo Windows riêng `EAGLE-MultiClient-Monitor.exe`, net48/x86, không dependency bên ngoài .NET Framework 4.8. Công cụ này chỉ quan sát các PID người dùng chỉ định. **Chưa chạy công cụ trên Windows, chưa chứng minh mở được sáu cửa sổ.** Không thay đổi EAGLE Auto, giới hạn hiện tại, licensing/VIP, DLL, offsets, stable 116 hay các phó bản. Không tạo bản v0.11.

## Bằng chứng cơ chế cũ

`src/ChickenAutoEx/FrmMain.cs`, `Monitor()` dòng 1396, nhánh hộp thoại dòng 1488–1497: tìm cửa sổ `#32770` của PID game, ghi hai lần bốn byte NOP tại `Address.MultiAcc`, chờ 500 ms, gửi `WM_CLOSE`. `Game.Init()` trong recovered snapshot dòng 3787–3793 có đường tương tự. `Memory.Write(int,uint,int)` dòng 482–485 gọi `WriteProcessMemory` nhưng bỏ qua BOOL/số byte ghi. `Address.GetOffset()` dòng 516 lấy `MultiAcc` từ bộ offsets; địa chỉ phụ thuộc client.

Đường `Monitor()` nằm dưới điều kiện `Global.IsFull != 0`. Đây là điều kiện được giữ nguyên, không bị mô hình giả thay thế. Mutex trong `Program.Main()` ngăn chạy hai phiên auto; không phải bằng chứng về cơ chế giới hạn hai cửa sổ game.

Ở 116, objdump của artifact đã phân tích trước đây cho thấy nhận diện `#32770`, dựng hai nhóm byte NOP rồi gọi helper dùng `WriteProcessMemory`. [Trích assembly mới](evidence/116-dialog-write-excerpt.txt) và bảng import được trích tĩnh từ binary SHA256 `34e4440dcef979de67a69478fc4165a6818d4d41e4a1b23a83ea11d49ed4ffea`. Những địa chỉ là VA của artifact auto 116, **không phải offsets để dùng cho client khác**. Đoạn này củng cố nhận định về sửa mã trong bộ nhớ; chưa có binary client của người dùng để xác nhận instruction và nhánh thoát bị thay thế.

## Cảnh báo người dùng cung cấp

Ảnh ngày 12-10-2026 hiển thị tiêu đề `Thien Long Bat Bo`; nội dung đọc được:

```text
因为您运行的客户端过多,导致资源不足,无法启动!
```

Dịch: “Vì bạn đang chạy quá nhiều client, dẫn đến thiếu tài nguyên, không thể khởi động!” Đã đưa văn bản này vào classifier **offline**. Classifier chỉ bỏ whitespace và chuẩn hóa dấu phẩy/dấu chấm than toàn chiều rộng; không bỏ từ, không match bằng một từ “client”. Cần đúng tiêu đề và xác nhận cửa sổ thuộc game được chọn. Các thông báo khác bị từ chối.

Ảnh không cho biết ngưỡng thực tế, nguyên nhân thiếu tài nguyên, phiên bản client, bytes trước patch hoặc việc server có giới hạn riêng. Thông báo có thể dùng cho một điều kiện giới hạn cứng; cần đo để phân biệt. Văn bản được chép từ ảnh, chưa trích bằng API từ client thật. Không thể coi việc classifier đạt là đã kiểm tra đúng HWND/control trên Windows.

## Các vấn đề tìm thấy

| Mã | Bằng chứng và ảnh hưởng | Kiểm tra/hướng xử lý |
|---|---|---|
| MC01 | Legacy chỉ kiểm class `#32770`, không title/text. Có thể đóng hộp thoại lỗi khác của game. | Fake tái hiện; classifier offline nhận cảnh báo từ ảnh, từ chối thông báo khác. Chưa tích hợp. |
| MC02 | Hai lần ghi không kiểm thành công, số byte hoặc readback. Vẫn đóng dialog khi ghi thất bại hoặc chỉ một nửa thành công. | Fake chạy chính nhánh C# và helper ghi gốc, tái hiện cả fail lần 1/lần 2. Cần contract kết quả và xác minh client trước sửa runtime. |
| MC03 | Không kiểm bytes gốc/biên instruction tại vị trí patch; nguồn MD5 ở Monitor là trường offset hiện có, không phải hash EXE đo tại nhánh đó. | Static audit. Client lạ có thể bị ghi sai; cần nhận diện build và giải mã instruction. Không đoán offsets. |
| MC04 | Không kiểm kết quả `PostMessage`; gửi thành công cũng không chứng minh game khởi động tiếp. | Fake tái hiện post thất bại bị bỏ qua; runtime cần trạng thái riêng “đã gửi”, “đã có cửa sổ”, “đã vào game”. |
| MC05 | Cảnh báo còn xuất hiện thì nhánh Monitor có thể ghi lại; không lưu trạng thái patch/bytes để cleanup. | Fake tái hiện gọi lặp; cần lifecycle và chính sách phục hồi được kiểm chứng trước tích hợp. |
| MC06 | Bốn đường kiểm `GameCount() > 3`: Login, double-click, Enter và Ctrl+O. Khi count=3 vẫn cho mở thêm; menu mở game không có cùng guard. | Legacy probe lấy điều kiện thật, tái hiện count 3 vẫn được chấp nhận. Ý đồ ngưỡng chưa có tài liệu; không tự coi là entitlement hay tự sửa thành 6. |
| MC07 | Số đếm từ Monitor có cadence 2 giây và còn được UI cập nhật, không có reservation chung cho các yêu cầu đang mở. | Probe tái hiện nhiều yêu cầu dùng cùng count. Mô hình mới thử admission atomic + pending slots. |
| MC08 | Nhiều catch rỗng trong Monitor; lỗi quyền/offset/process biến mất dễ im lặng. Có truy cập dictionary/UI qua các thread. | Static audit, chưa mô phỏng scheduler đầy đủ. Sáu client có thể làm tăng tần suất race, chưa chứng minh crash thực tế. |

Không suy ra nguyên nhân giới hạn mặc định của mọi TLBB/auto khác từ kết quả này. Chưa chứng minh chính đoạn tám byte trên client riêng là toàn bộ kiểm tra multi-client. Không sửa native reader để giải quyết MC01–MC08 trong lượt này.

## Mô hình quản lý mở game được kiểm thử

`simulation/LaunchModel.cs` là code có source kiểm soát được, chỉ dùng PID/start-time giả, không có API mở game hoặc ghi process. Không được compile/reference vào EAGLE Auto.

- Giới hạn tính bằng `running + pending`, giữ chỗ trước yêu cầu khởi động; lock bảo vệ thao tác admission.
- Yêu cầu trùng bị từ chối; nhận diện tiến trình bằng PID **và** thời điểm khởi động.
- Timeout giữ chỗ dưới trạng thái chưa xác nhận, không tự giải phóng rồi mở thêm. ACK đến muộn vẫn được đối chiếu.
- Chỉ giải phóng khi xác nhận không có process hoặc process đã thoát. Observer phải phát hiện cả process mở ngoài model; không bỏ qua client vượt trần đã tồn tại.
- Gate quyền và gate tài nguyên do caller cung cấp. Kiểm tra gate giả bị từ chối không phải kiểm thử licensing thật.
- “Có PID”, “có cửa sổ” và “đã đăng nhập” là các mức khác nhau. Model mới thử hai mức đầu; không chứa tài khoản/game login.

Các trường hợp gồm pending/running ở mỗi ngưỡng, mở đồng thời, yêu cầu trùng, chậm/timeout/ACK muộn, thất bại, discovery trước ACK, process thoát, PID tái sử dụng, process đã vượt trần, denied permission/resource và classifier từ ảnh. `summary.json` ghi từng case và phân biệt `unsafe_legacy_reproduced`, `legacy_control`, `proposed_model`, `dialog_candidate`, `measurement_math`.

Một giới hạn thiết kế: `ConfirmNoProcess` chỉ an toàn nếu adapter sau này thực sự có bằng chứng không khởi động; không được gọi chỉ vì chờ quá hạn. Model không đọc process table để xác nhận. Admission không kiểm soát việc người dùng tự mở game ngoài auto. Mô hình không mô phỏng đầy đủ IPC/GUI, game/server, licensing, instruction patch hoặc tài nguyên thật.

## Công cụ đo và kiểm thử tiếp theo

Xem [hướng dẫn Windows](WINDOWS-TEST.vi.md). Sampler ghi CPU chuẩn hóa theo toàn máy, working set/private bytes, handles/GDI/USER, RAM vật lý còn trống và phản hồi message loop bằng `WM_NULL` có timeout 200 ms. Không đọc memory game, không log title/command-line/tài khoản, không mở/đóng game và không thay đổi cửa sổ. Handle phục vụ GDI chỉ xin `PROCESS_QUERY_INFORMATION`.

Độ trễ WM_NULL là thời gian phản hồi UI trong lượt đo, không phải FPS hoặc ping server. CPU các process chỉ định không phải tổng CPU máy. GDI/USER trả 0 hoặc bị từ chối chưa đủ để kết luận không có resource; CSV để trống khi không mở được query handle. Cần Task Manager để đánh giá GPU/VRAM/tổng CPU. PID là process người dùng chỉ định, không tự chứng minh đó là client đã đăng nhập. Math helper đã kiểm bằng .NET 8; đường Windows API và CSV collector mới build, chưa runtime test.

Sau dữ liệu Windows, chọn một sửa hẹp: nhận diện đúng cảnh báo + kết quả ghi/đọc lại trên build client đã xác minh, hoặc thống nhất bộ quản lý khởi động. Chỉ nối giới hạn 6 vào ứng dụng sau khi phân loại guard và giữ đầy đủ entitlement/server controls. Không hứa “không giới hạn”: cần trần vận hành theo tài nguyên và kết quả đo.

## Tái lập

Từ repository, SDK .NET `8.0.425`, Python 3.12, không cần game/REA runtime:

```bash
dotnet restore analysis/multi-client-107/simulation/MultiClientSimulation.csproj --locked-mode
dotnet run --project analysis/multi-client-107/simulation/MultiClientSimulation.csproj -c Release --no-restore -- "$PWD" "$PWD/.build/multi-client-107/summary.json"
dotnet restore analysis/multi-client-107/windows/EagleMultiClientMonitor.csproj --locked-mode
dotnet build analysis/multi-client-107/windows/EagleMultiClientMonitor.csproj -c Release --no-restore
python analysis/multi-client-107/tools/verify_preservation.py
python analysis/multi-client-107/tools/package_review.py
```

`LegacyProbe.cs` dùng Roslyn để lấy đúng hai nhánh và helper ghi, compile chúng với fake APIs; không load assembly automation gốc/hydrated, DLL native hoặc logic licensing. Dữ liệu giả không có credentials. Toàn bộ file tracked cũ phải không thay đổi so với base; sáu release gốc được kiểm SHA256. Evidence công khai gồm summary và preservation. Công cụ Windows/ZIP là artifact build, không commit binary.
