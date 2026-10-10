# ChickenAutoEx 107: bản build khôi phục và sửa startup

**Bản hiện tại: [EAGLE Auto v0.5 — mô phỏng Ác Tặc](EAGLE-Auto-AC-TAC-SIMULATION.vi.md)** trên nhánh `repair/eagle-auto-ac-tac`. Gói là `EAGLE-Auto-v0.5-net48.zip`, artifact **EAGLE-Auto-v0.5-net48-review**; chạy **EAGLE-Auto-v0.5.exe** cùng `EAGLE-Auto-v0.5.exe.config`. Sửa `EagleAuto.Version.props` để tự đồng bộ tên file, giao diện, metadata, ZIP và artifact khi nâng version. Các tên nhánh/gói ChickenAuto bên dưới là hồ sơ các đợt sửa trước.

Project build nằm tại `src/ChickenAutoEx`, tiếp nối [điều tra tĩnh](../analysis/chickenautoex-107/REPORT.vi.md). Phần sửa startup đã merge vào master; nhánh `repair/chickenautoex-107-dungeon-menu` kế thừa [hai lỗi UI/cấu hình](ChickenAutoEx-UI-SETTINGS.vi.md) và sửa [menu phó bản](ChickenAutoEx-DUNGEON-MENU.vi.md). Source điều tra được giữ nguyên. Không chạy binary gốc để khôi phục dữ liệu.

## Thay đổi

- Target .NET Framework **4.8**, WinForms x86; CLR 4, không đổi kiến trúc hook/game.
- Newtonsoft.Json **13.0.4**: compiler, DLL cạnh executable và DLL nhúng đều dùng cùng một assembly. Bổ sung Zen.Barcode.Core 3.1.0.0 qua package pinned.
- Update endpoint mặc định: HTTPS metadata của fork `ttgpPsychMH/EAGLE-Auto`. Có thể đổi `UpdateMetadataUrl` trong `.exe.config` sang HTTPS do bạn kiểm soát. HTTP chỉ được chấp nhận cho loopback khi thử nghiệm.
- Khởi tạo cục bộ trước, kiểm tra cập nhật bất đồng bộ. Không có mạng, metadata sai hoặc phiên bản mới hơn đều không làm update check mở browser hay thoát chương trình. Điều kiện xác thực/licensing/VIP độc lập được giữ nguyên.
- Timeout toàn yêu cầu gồm cả đọc body, mặc định 5 giây; body tối đa 32 KiB, UTF-8, MIME `text/plain`, một dòng Version nguyên dương. Không theo redirect, không bỏ qua chứng chỉ TLS, không tự tải/cài binary.
- Hủy request khi đóng form. Các lỗi khởi tạo/resource/runtime có thông báo mã loại exception/HResult; không ghi nội dung account/config vào chẩn đoán mới.

Các delta automation được khai báo và review theo từng đợt: Thủy Lao v0.2, Kỳ Cuộc/pet AOE v0.3, Trừng Ác/rà soát ba nhiệm vụ v0.4, Ác Tặc v0.5. Scripts, hook native, thuật toán config encryption, key/HWID và các nhánh VIP giữ nguyên; helper AOE chung có sửa chủ đích. `Global` chỉ đổi UpdateURL. `verify_build.py` kiểm tra hash các partial và handler được review, đảo đúng từng delta Game/FrmMain rồi đối chiếu toàn bộ phần còn lại với snapshot; không bỏ qua cả file hoặc cả vùng menu. AntiDump vẫn được giữ nguyên, vì thuộc code cũ cần đánh giá runtime riêng.

## Build trên Windows hoặc Linux

### Tải gói kiểm thử từ GitHub

Source hiện tại nằm trên nhánh [`repair/eagle-auto-ac-tac`](https://github.com/ttgpPsychMH/EAGLE-Auto/tree/repair/eagle-auto-ac-tac). Mở [EAGLE Auto review build](https://github.com/ttgpPsychMH/EAGLE-Auto/actions/workflows/chickenautoex-review-build.yml), chọn lần chạy có dấu xanh của **đúng nhánh và commit**, rồi tải **EAGLE-Auto-v0.5-net48-review** (hoặc version mới hơn của nhánh đó) ở mục **Artifacts**. GitHub yêu cầu đăng nhập để tải artifact. Giải nén file tải về để lấy ZIP ứng dụng, SHA256 và bằng chứng kiểm thử; tiếp tục giải nén ZIP ứng dụng vào thư mục kiểm thử riêng.

Workflow tự chạy khi `master` hoặc các nhánh `repair/**`, `branding/**` có thay đổi liên quan, bao gồm `EagleAuto.Version.props`; không cần sửa workflow cho từng nhánh/version mới. Artifact được giữ **7 ngày**; hết hạn thì người có quyền ghi repository mở lần chạy đã có và chọn **Re-run jobs** để tạo lại. Không cần merge để tải artifact từ lần chạy do push tạo ra. Nếu GitHub yêu cầu bật Actions hoặc phê duyệt workflow của fork, chủ repository cần thực hiện thao tác đó trên GitHub. Download ZIP ở nút Code chỉ tải source và các binary gốc, không phải bản build sửa.

CI dùng cùng lệnh bên dưới để khôi phục tài nguyên, build, kiểm tra bảo toàn binary/source, chạy test updater với dịch vụ giả lập và test UI/cấu hình với dependency giả. Không chạy EXE automation, không dùng secret của GitHub hoặc tài khoản game, không tạo GitHub Release và không tự merge. Gói CI vẫn là bản review chưa qua Windows 11 test, kế thừa các constant nhúng của binary gốc; không upload source đã hydrate, source test fixture hoặc thư mục generated vào artifact.

### Tự build

Prerequisites: .NET SDK **8.0.425** (global.json cho phép patch mới hơn trong cùng feature band 8.0.4xx), Python 3.12, quyền đọc repository và tải package qua HTTPS từ NuGet. SDK được pin để runner có SDK 9/10 không chọn Roslyn yêu cầu runtime cao hơn net8 của test harness. Không cần chạy game hoặc cung cấp credential. Dùng checkout hiện tại; không tạo worktree nếu không được yêu cầu.

Từ root repository, PowerShell trên Windows:

```powershell
python -m venv .build/build-venv
$buildPython = ".\.build\build-venv\Scripts\python.exe"
& $buildPython -m pip install -r analysis/chickenautoex-107/tools/requirements.txt
dotnet tool restore
dotnet restore ChickenAutoEx.sln --locked-mode
& $buildPython tools/prepare_build.py
dotnet build ChickenAutoEx.sln -c Release --no-restore
python tools/check_thuy_lao_baseline.py
python tools/check_ky_cuoc_baseline.py
python tools/check_trung_ac_baseline.py
python tools/check_dungeon_reaudit_baseline.py
python tools/check_ac_tac_baseline.py
& $buildPython tools/verify_build.py
dotnet test tests/ChickenAutoEx.Startup.Tests/ChickenAutoEx.Startup.Tests.csproj -c Release --no-build --no-restore
& $buildPython tools/package_build.py
```

Linux dùng `.build/build-venv/bin/python` thay đường dẫn Python trên. `dotnet` phải nằm trong PATH và SDK/ILSpy phải nhìn thấy runtime .NET 8. Nếu môi trường chặn ghi thư mục home, đặt `DOTNET_CLI_HOME` và `NUGET_PACKAGES` vào thư mục writable riêng trước khi restore. Không dùng shell tracing hoặc in biến môi trường chứa credential.

Lockfiles và tool manifest được commit. `PackageDownload` net35 chỉ cung cấp reference assemblies cho decompiler; **ứng dụng mới target net48**. Script `prepare_build.py`:

1. Kiểm tra ZIP/payload SHA-256 của bản Ex đã review, đọc PE như dữ liệu.
2. Trích tất cả 27 tài nguyên và icon; thay riêng resource Newtonsoft bằng DLL của package 13.0.4.
3. Dùng ILSpy để lấy lại sáu literal đã che, ghép vào bản sao của bốn file C# trong `.build/chickenautoex/hydrated`. Sau đó áp dụng các delta Thủy Lao, Kỳ Cuộc/pet AOE, Trừng Ác và Ác Tặc đã khai báo lên Game, sinh source redacted riêng cho bốn bộ mô phỏng; phục hồi partial v0.3 theo hash cho các đối chứng rà soát. Auth/debug/config và code Game ngoài delta vẫn có nội dung gốc; verifier đảo từng delta trước khi so toàn bộ source.
4. Ghi manifest có hash; không in key/password. Native DLL không được thực thi.

**`.build`, `bin`, `obj` chứa dữ liệu nhạy cảm kế thừa và binary chưa qua Windows test, phải giữ ngoài Git.** Việc khôi phục constant chỉ nhằm giữ hành vi baseline; không thử dùng key API bên thứ ba. Gói build vẫn kế thừa các constant ấy trong binary, nên chỉ phục vụ review/test cô lập, chưa phải bản phát hành công khai. Không commit thư mục generated hoặc file account/game config của người dùng.

Output ứng dụng: `src/ChickenAutoEx/bin/Release/net48`. Gói review hiện tại: `.build/artifacts/EAGLE-Auto-v0.5-net48.zip` cùng SHA256. Gói gồm EXE, `.exe.config`, Newtonsoft.Json.dll, Zen.Barcode.Core.dll và tài liệu. EasyHook native được nhúng và code gốc sẽ giải nén lúc khởi động. Không cần AutoUpdate.exe vì luồng mới chỉ thông báo; không thêm cơ chế tải/cài đặt tự động.

V0.4 bổ sung [Trừng Ác](EAGLE-Auto-TRUNG-AC-SIMULATION.vi.md) và [rà soát cả ba nhiệm vụ](EAGLE-Auto-DUNGEON-REAUDIT.vi.md), gồm guard pause/owner/member và callback AutoLogin. Verifier đảo ngược các delta Trừng Ác trong Game/FrmMain và delta partial rà soát, rồi kiểm tra phần còn lại với baseline, không bỏ qua cả method/file vì khác phiên bản.

V0.5 bổ sung [Ác Tặc](EAGLE-Auto-AC-TAC-SIMULATION.vi.md): map/lịch, state riêng, dialog có giới hạn, điều phối đúng đội và xác nhận boss/map. Verifier đảo lớp delta Ác Tặc trước các lớp v0.4/v0.3/v0.2; shared combat, licensing/VIP và source ngoài các delta vẫn đối chiếu với baseline.

## Kết quả đã xác nhận

Build đầy đủ thành công trên Linux: **0 lỗi, 50 cảnh báo legacy**, có icon, manifest và đủ 27 embedded resource. Kiểm tra tĩnh xác nhận CLR 4/x86, identities phụ thuộc khớp, 26 resource gốc giữ nguyên từng byte; resource JSON được nâng cấp có chủ đích. Cả sáu binary stable/Ex trong repository và snapshot điều tra không đổi.

Bản v0.5 chạy **375 test**, 0 failed/skipped, gồm 66 ca Ác Tặc. 13 đối chứng v0.4 Ác Tặc thất bại đúng hành vi cũ; tổng năm nhóm đối chứng là 43, không có lỗi harness. Bản v0.4 trước đó chạy **309 test**, 0 failed, 0 skipped; 11 đối chứng Trừng Ác và 9 đối chứng rà soát v0.3 thất bại đúng hành vi cũ trong các lượt riêng. Cả 4 đối chứng Thủy Lao và 6 Kỳ Cuộc/pet AOE cũ vẫn qua bộ kiểm tra đối chứng. Bản v0.3 trước đó chạy **207 test**, thêm 72 ca Kỳ Cuộc/pet AOE; xem [mô phỏng và đối chứng Kỳ Cuộc](EAGLE-Auto-KY-CUOC-SIMULATION.vi.md). Bản v0.2 trước đó chạy **135 test**, gồm 41 ca Thủy Lao mới và 94 test cũ; xem [mô phỏng và đối chứng](EAGLE-Auto-THUY-LAO-SIMULATION.vi.md). Bộ test net8 trước đó (không load assembly automation): **94 passed, 0 failed, 0 skipped** trong đợt sửa menu: 37 test updater/schema, 19 test UI/settings và 38 test menu phó bản. Service giả lập Kestrel dùng loopback và socket HTTP/TLS thật; kiểm tra INI hiện hành/mới/cũ, malformed/duplicate/HTML, status lỗi, redirect không tới target, payload quá lớn, UTF-8 sai, timeout headers/body, cancellation và từ chối self-signed TLS. Có thêm kiểm tra schema BaiTrain khi nâng JSON. Test UI/settings/menu dùng Roslyn đi kèm SDK để biên dịch các method thực từ source với dependency giả; xem [phạm vi kiểm thử settings](ChickenAutoEx-UI-SETTINGS.vi.md) và [menu](ChickenAutoEx-DUNGEON-MENU.vi.md). Fixture Python dùng cho test thủ công đã được kiểm tra tám response tương ứng ở đợt startup.

Kiểm tra này **không xác nhận giao diện, CLR 4/WinForms, WMI, AntiDump, native hook hay phụ bản chạy được trên Windows 11**. Thực hiện [hướng dẫn Windows](ChickenAutoEx-WINDOWS11.vi.md) trước khi coi là đã sửa thành công. Reference assemblies chỉ giúp cross-compile; không cung cấp runtime Windows trong cloud.

Các log/hash không chứa key tại [evidence](evidence). Whitespace của source/resources gốc được giữ để đối chiếu, không format lại các module nhạy cảm.
