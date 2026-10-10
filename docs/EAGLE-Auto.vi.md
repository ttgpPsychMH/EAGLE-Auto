# EAGLE Auto — tên file và version

Ngày cập nhật: 10-10-2026 (Asia/Bangkok). .NET Framework 4.8, x86. Các ví dụ dưới đây dùng **v0.3**; version thực tế của gói luôn ghi trong `BUILD-INFO.json`.

Giải nén gói vào thư mục thử nghiệm riêng, rồi chạy **EAGLE-Auto-v<version>.exe** với version ghi trong `BUILD-INFO.json`. Giữ file `.exe.config` cùng tên bên cạnh EXE và các DLL trong gói. Ví dụ gói v0.3 dùng `EAGLE-Auto-v0.3.exe` và `EAGLE-Auto-v0.3.exe.config`. Không dùng EXE/config cũ còn sót từ bản tải trước.

Với gói v0.3, giao diện hiển thị `EAGLE Auto v0.3`; menu `Về EAGLE Auto` ghi:

> Được sửa lại dựa trên Chicken Auto 107, vibe coding bằng Codex bởi tenkafuku.

## Nâng version sau này

Chỉ sửa giá trị `EagleAutoVersion` trong [EagleAuto.Version.props](../EagleAuto.Version.props), ví dụ `0.1` → `0.2` → `0.3`, rồi build/verify/package theo [hướng dẫn build](ChickenAutoEx-BUILD.vi.md). Version có dạng `major.minor`, mỗi phần từ 0 đến 65534, không có số 0 đứng đầu.

MSBuild sinh hằng version trong thư mục `obj` và tự đồng bộ:

| Thành phần | Khi version là `0.3` |
|---|---|
| File chạy / config | `EAGLE-Auto-v0.3.exe` / `EAGLE-Auto-v0.3.exe.config` |
| Tiêu đề / giới thiệu | `EAGLE Auto v0.3` |
| FileVersion / ProductVersion | `0.3.0.0` / `v0.3` |
| ZIP / artifact GitHub Actions | `EAGLE-Auto-v0.3-net48.zip` / `EAGLE-Auto-v0.3-net48-review` |

Không tăng version mỗi lần build: build lại cùng source vẫn là cùng version sản phẩm. File build cũ có thể còn trong `bin`; script package chỉ lấy file đúng version đã xác minh, không đóng gói EXE/config cũ.

`Global.Version = "107"` là version giao thức cũ, tiếp tục giữ để bảo toàn updater/licensing; CLR AssemblyVersion vẫn `1.0.0.0`. Chỉ assembly simple name đổi theo tên file. Icon, namespace/resource gốc, mutex, stable 116 và licensing/VIP giữ nguyên. Từ v0.2, có [các sửa hẹp Thủy Lao đã kiểm thử bằng mô phỏng](EAGLE-Auto-THUY-LAO-SIMULATION.vi.md); menu Thủy Lao vẫn chưa khả dụng. V0.3 bổ sung [mô phỏng và sửa Kỳ Cuộc/pet AOE](EAGLE-Auto-KY-CUOC-SIMULATION.vi.md), chưa qua kiểm thử Windows/game. Copyright gốc được giữ, không tuyên bố đổi giấy phép của code/dependency.

Kiểm tra Windows 11: tên file/config đi cùng nhau; mở app; tiêu đề/About đúng version; Properties → Details có FileVersion/ProductVersion đúng; đóng/mở lại và thử mở bản thứ hai để kiểm tra mutex. Sau đó theo [checklist Windows](ChickenAutoEx-WINDOWS11.vi.md). Build Linux và test giả lập chưa xác nhận bản vừa đổi tên chạy đúng trên Windows hay trong game.
