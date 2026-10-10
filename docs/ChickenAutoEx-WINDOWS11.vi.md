# Kiểm thử ChickenAutoEx trên Windows 11

Bản sửa menu mới chưa được chạy trên Windows. Người dùng xác nhận bản trước mở được và báo lỗi NullReferenceException khi chọn Trừng Ác trước khi thử game. Chỉ chạy bản mới sau khi review source và nguồn binary/resource đã khôi phục. Không chạy executable gốc hoặc bộ tự giải nén để kiểm thử bản sửa. Đợt này cần thêm [kiểm tra menu phó bản](ChickenAutoEx-DUNGEON-MENU.vi.md) và [HP/MP, cấu hình](ChickenAutoEx-UI-SETTINGS.vi.md), có trong gói tải về.

## Chuẩn bị

1. Dùng VM Windows 11 cô lập, có .NET Framework 4.8 hoặc 4.8.1, hỗ trợ process x86/WoW64. Không cần bật .NET Framework 3.5 cho bản build mới.
2. Giữ stable 116 ở máy/thư mục khác. Không chạy stable cùng lúc; mutex legacy vẫn dùng chung tên. Không kết thúc process không thuộc lần thử này.
3. Giải nén bản build vào thư mục mới, writable, chỉ có EXE/config/DLL của gói. Không chép Data/Config/account từ bản đang dùng; không nhập mật khẩu hoặc gắn game.
4. Ban đầu chặn outbound tới Internet/game/API bên thứ ba, chỉ cho loopback. Không sửa hosts/DNS, không bỏ TLS validation, không nhập API key tìm thấy trong source/binary. Fixture không cần credential.
5. So khớp SHA-256 gói với file `.sha256` của lần build. Lưu bản sao config thử; không thay binary stable hoặc resource nhạy cảm.

## Dịch vụ giả lập

Từ root checkout trong cùng máy Windows:

```powershell
python tools/fake_update_service.py --port 8765
```

Trong **bản sao** `ChickenAutoEx.exe.config`, đổi appSettings:

```xml
<add key="UpdateMetadataUrl" value="http://127.0.0.1:8765/current.ini" />
<add key="UpdateTimeoutMilliseconds" value="1000" />
```

Khởi động bản build đã review, quan sát main form và log, sau đó đóng đúng process đó. Với mỗi URL dưới đây, dùng thư mục thử sạch/cùng bộ build, sửa config rồi khởi động lại.

| URL path | Kết quả updater mong đợi | Kết quả UI cần quan sát |
|---|---|---|
| `/current.ini` | Version 107, đã kiểm tra | Form hoạt động, không mở browser |
| `/new.ini` | Version 108, thông báo có bản mới | Không thoát, không tự tải/cài binary |
| `/malformed.ini` | InvalidIni | Form tiếp tục hoạt động |
| `/duplicate.ini` | InvalidIni | Không chấp nhận một dòng hợp lệ trước một dòng sai |
| `/missing.ini` | InvalidIni | Không thoát vì thiếu Version |
| `/parking.ini` | UnexpectedContentType | Không hiện/nạp HTML parking, không mở browser |
| `/oversize.ini` | MetadataTooLarge | Không tải body không giới hạn |
| `/redirect.ini` | Http302 | Không có request thứ hai tới `/current.ini` |
| `/unavailable.ini` | Http503 | Form tiếp tục hoạt động |
| `/timeout.ini` | Timeout sau khoảng 1 giây | UI vẫn repaint/di chuyển được trong lúc chờ |
| Tắt fixture | TransportError | Không mở website cũ, không thoát do update failure |

Trong trường hợp timeout, thử đóng form ngay khi request đang chạy: phải kết thúc bình thường, không có unhandled exception từ callback. Xác nhận `Bin/EasyHook.dll` được trích, Newtonsoft.Json.dll cạnh EXE có đúng hash của gói, Zen.Barcode.Core.dll có mặt. Chưa gọi SetHook/attach process game.

## TLS và lỗi khởi tạo

Để thử TLS, tạo certificate **self-signed dùng riêng cho VM**, không import vào trust store và không commit private key. Khởi động fixture bằng `--certificate <cert.pem> --private-key <key.pem>`, đổi URL sang `https://127.0.0.1:8765/current.ini`. Chứng chỉ không tin cậy phải bị từ chối, ghi TransportError; form tiếp tục hoạt động. Không dùng callback bỏ qua certificate hoặc `-k` để khiến test đạt.

Nếu VM có OpenSSL, có thể tạo certificate thử bằng các lệnh sau; cả hai tệp nằm trong thư mục Git đã bỏ qua. Private key này chỉ dành cho fixture local, không dùng cho dịch vụ thật:

```powershell
New-Item -ItemType Directory -Force .build/test-tls | Out-Null
openssl req -x509 -newkey rsa:2048 -nodes -days 1 -keyout .build/test-tls/key.pem -out .build/test-tls/cert.pem -subj "/CN=localhost" -addext "subjectAltName=IP:127.0.0.1"
python tools/fake_update_service.py --port 8765 --certificate .build/test-tls/cert.pem --private-key .build/test-tls/key.pem
```

Để thử lỗi giải nén, dùng một bản sao trong thư mục không cho ghi và tài khoản thường: phải thấy thông báo lỗi khởi động có loại exception/HResult, không có credential trong chẩn đoán mới và không mở website. Để thử thiếu thư viện, di chuyển Zen.Barcode.Core.dll khỏi **bản sao thử**, mở màn QR nếu có; ghi rõ lỗi xảy ra ở startup hay chỉ khi mở màn ấy. Đây là kiểm thử dependency, không được coi là updater test đã đạt.

Sau các ca loopback đạt, có thể bật outbound **chỉ tới HTTPS metadata tin cậy** và thử URL mặc định của fork. Xác nhận TLS/certificate trên CLR 4 thực tế, status/log version và không có request tới domain cũ. Nếu URL của fork không còn public, dùng public metadata trên endpoint bạn quản lý, không nhập token vào URL.

## Ghi kết quả

Ghi build SHA, Windows build, .NET Framework version, timestamp, từng ca pass/fail, thông báo và exception type/HResult. Che mọi tên account, HWID, đường dẫn cá nhân và credential trong ảnh/log. Không thay “chưa chạy” bằng “pass” dựa trên test net8 trong cloud.

Chỉ sau khi baseline UI/dependency đã đạt mới mở nhiệm vụ kiểm tra game/server có quyền truy cập. Mã phụ bản, offset, hook, AntiDump và licensing/VIP vẫn chưa được sửa hoặc xác nhận. Runtime đổi CLR 2 → CLR 4 có thể ảnh hưởng các thành phần này; không giải quyết bằng cách gỡ gate hoặc bỏ qua xác thực.

Rollback: đóng process thử, bỏ thư mục build thử và quay về bản stable ở thư mục riêng. Không sửa account/config của stable trong quá trình này.
