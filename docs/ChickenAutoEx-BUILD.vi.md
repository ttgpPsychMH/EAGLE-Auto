# ChickenAutoEx 107: bản build khôi phục và sửa startup

Đây là nhánh sửa `repair/chickenautoex-107-startup`, tiếp nối [điều tra tĩnh](../analysis/chickenautoex-107/REPORT.vi.md). Source điều tra được giữ nguyên; project build nằm tại `src/ChickenAutoEx`. Không chạy binary gốc để khôi phục dữ liệu.

## Thay đổi

- Target .NET Framework **4.8**, WinForms x86; CLR 4, không đổi kiến trúc hook/game.
- Newtonsoft.Json **13.0.4**: compiler, DLL cạnh executable và DLL nhúng đều dùng cùng một assembly. Bổ sung Zen.Barcode.Core 3.1.0.0 qua package pinned.
- Update endpoint mặc định: HTTPS metadata của fork `ttgpPsychMH/EAGLE-Auto`. Có thể đổi `UpdateMetadataUrl` trong `.exe.config` sang HTTPS do bạn kiểm soát. HTTP chỉ được chấp nhận cho loopback khi thử nghiệm.
- Khởi tạo cục bộ trước, kiểm tra cập nhật bất đồng bộ. Không có mạng, metadata sai hoặc phiên bản mới hơn đều không làm update check mở browser hay thoát chương trình. Điều kiện xác thực/licensing/VIP độc lập được giữ nguyên.
- Timeout toàn yêu cầu gồm cả đọc body, mặc định 5 giây; body tối đa 32 KiB, UTF-8, MIME `text/plain`, một dòng Version nguyên dương. Không theo redirect, không bỏ qua chứng chỉ TLS, không tự tải/cài binary.
- Hủy request khi đóng form. Các lỗi khởi tạo/resource/runtime có thông báo mã loại exception/HResult; không ghi nội dung account/config vào chẩn đoán mới.

Mã phụ bản, Game.cs, hook native, thuật toán config encryption, key/HWID và các nhánh VIP không được sửa. `Global` chỉ đổi UpdateURL. Phần khởi tạo cục bộ và mọi method sau nó trong FrmMain giống snapshot gốc. AntiDump vẫn được giữ nguyên, vì thuộc code cũ cần đánh giá runtime riêng.

## Build trên Windows hoặc Linux

Prerequisites: .NET SDK 8.x đã kiểm tra với **8.0.425**, Python 3.12, quyền đọc repository và tải package qua HTTPS từ NuGet. Không cần chạy game hoặc cung cấp credential. Dùng checkout hiện tại; không tạo worktree nếu không được yêu cầu.

Từ root repository, PowerShell trên Windows:

```powershell
python -m venv .build/build-venv
$buildPython = ".\.build\build-venv\Scripts\python.exe"
& $buildPython -m pip install -r analysis/chickenautoex-107/tools/requirements.txt
dotnet tool restore
dotnet restore ChickenAutoEx.sln --locked-mode
& $buildPython tools/prepare_build.py
dotnet build ChickenAutoEx.sln -c Release --no-restore
& $buildPython tools/verify_build.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
& $buildPython tools/package_build.py
```

Linux dùng `.build/build-venv/bin/python` thay đường dẫn Python trên. `dotnet` phải nằm trong PATH và SDK/ILSpy phải nhìn thấy runtime .NET 8. Nếu môi trường chặn ghi thư mục home, đặt `DOTNET_CLI_HOME` và `NUGET_PACKAGES` vào thư mục writable riêng trước khi restore. Không dùng shell tracing hoặc in biến môi trường chứa credential.

Lockfiles và tool manifest được commit. `PackageDownload` net35 chỉ cung cấp reference assemblies cho decompiler; **ứng dụng mới target net48**. Script `prepare_build.py`:

1. Kiểm tra ZIP/payload SHA-256 của bản Ex đã review, đọc PE như dữ liệu.
2. Trích tất cả 27 tài nguyên và icon; thay riêng resource Newtonsoft bằng DLL của package 13.0.4.
3. Dùng ILSpy để lấy lại sáu literal đã che, ghép vào bản sao của bốn file C# trong `.build/chickenautoex/hydrated`. Không sửa bất kỳ token khác trong bốn file đó. Game/auth/debug/config code vẫn có nội dung gốc.
4. Ghi manifest có hash; không in key/password. Native DLL không được thực thi.

**`.build`, `bin`, `obj` chứa dữ liệu nhạy cảm kế thừa và binary chưa qua Windows test, phải giữ ngoài Git.** Việc khôi phục constant chỉ nhằm giữ hành vi baseline; không thử dùng key API bên thứ ba. Gói build vẫn kế thừa các constant ấy trong binary, nên chỉ phục vụ review/test cô lập, chưa phải bản phát hành công khai. Không commit thư mục generated hoặc file account/game config của người dùng.

Output ứng dụng: `src/ChickenAutoEx/bin/Release/net48`. Gói review: `.build/artifacts/ChickenAutoEx-107-startup-net48.zip` cùng SHA256. Gói gồm EXE, `.exe.config`, Newtonsoft.Json.dll, Zen.Barcode.Core.dll và tài liệu. EasyHook native được nhúng và code gốc sẽ giải nén lúc khởi động. Không cần AutoUpdate.exe vì luồng mới chỉ thông báo; không thêm cơ chế tải/cài đặt tự động.

## Kết quả đã xác nhận

Build đầy đủ thành công trên Linux: **0 lỗi, 50 cảnh báo legacy**, có icon, manifest và đủ 27 embedded resource. Kiểm tra tĩnh xác nhận CLR 4/x86, identities phụ thuộc khớp, 26 resource gốc giữ nguyên từng byte; resource JSON được nâng cấp có chủ đích. Cả sáu binary stable/Ex trong repository và snapshot điều tra không đổi.

Bộ test net8 chạy cùng source của updater mới (không load assembly automation): **37 passed, 0 failed, 0 skipped**. Service giả lập Kestrel dùng loopback và socket HTTP/TLS thật; kiểm tra INI hiện hành/mới/cũ, malformed/duplicate/HTML, status lỗi, redirect không tới target, payload quá lớn, UTF-8 sai, timeout headers/body, cancellation và từ chối self-signed TLS. Có thêm kiểm tra schema BaiTrain khi nâng JSON. Fixture Python dùng cho test thủ công đã được kiểm tra tám response tương ứng.

Kiểm tra này **không xác nhận giao diện, CLR 4/WinForms, WMI, AntiDump, native hook hay phụ bản chạy được trên Windows 11**. Thực hiện [hướng dẫn Windows](ChickenAutoEx-WINDOWS11.vi.md) trước khi coi là đã sửa thành công. Reference assemblies chỉ giúp cross-compile; không cung cấp runtime Windows trong cloud.

Các log/hash không chứa key tại [evidence](evidence). Whitespace của source/resources gốc được giữ để đối chiếu, không format lại các module nhạy cảm.
