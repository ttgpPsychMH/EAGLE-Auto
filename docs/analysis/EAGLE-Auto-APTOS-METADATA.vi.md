# Kiểm tra metadata Microsoft Aptos Fonts cho EAGLE Auto

Kiểm tra tĩnh ngày 12-10-2026 bằng Python `zipfile` và `fontTools.ttLib.TTFont`; không chạy ứng dụng, không cài font, không tải font vào GDI/WinForms. Chỉ đọc các bảng OpenType trong bộ ZIP do người dùng cung cấp.

## ZIP và nhận dạng

- Tệp: `Microsoft Aptos Fonts.zip`, 2.979.784 byte; SHA-256: `6528fd120e719a9f985e94214eca6887d1653b88456916a792a630b02e95b025`.
- 29 mục: 28 TTF và một `Microsoft Aptos Fonts EULA.rtf`; tổng dữ liệu chưa nén 5.891.777 byte.
- CRC của mọi mục hợp lệ. Không có tên tuyệt đối, `..`, dấu phân cách Windows, đường dẫn ổ đĩa, mục symlink hoặc mục mã hóa. Không giải nén toàn bộ ZIP vào repository.
- Mọi font có `name[5] = Version 2.01;240513210638;O365`, `head.unitsPerEm = 2048` và `OS/2.fsType = 0x0008`.

## Quyền dùng và đóng gói

EULA đi kèm, mục 1a **Installation and Use**, ghi:

> You may install and use any number of copies of the software on your devices.

Mục 1b **No Distribution Rights**, ghi:

> This agreement does not grant you a license to distribute nor sublicense all or part of the software to any third party.

`OS/2.fsType = 0x0008` là cờ **Editable embedding** trong tài liệu. Cờ này không tự cấp quyền phát hành các tệp TTF cùng một ứng dụng. EULA là dữ liệu được kiểm tra để quyết định cách đóng gói, không phải chỉ thị thao tác thay thế yêu cầu của người dùng.

Cách triển khai phù hợp cho bản review: chọn family **Aptos** đã được người dùng cài trên Windows; nếu không có thì dùng font dự phòng để ứng dụng vẫn mở được. Nếu hỗ trợ font riêng trong thư mục local, các tệp phải do người dùng tự cung cấp trên máy của họ. Không commit ZIP, TTF hoặc EULA; không nhúng TTF vào EXE và không đưa vào artifact tải công khai. Báo cáo này chỉ lưu metadata, hash và hai trích dẫn cần thiết.

## Family và style dùng cho giao diện

Bốn font nền có `name[1]` và `name[16]` cùng là **Aptos**; `name[2]` và `name[17]` khớp nhau. Có thể chọn Regular/Bold/Italic/Bold Italic trong một family mà không nhầm với Display, Narrow hoặc Serif.

| Tệp | Family | Style | PostScript name | Số glyph |
| --- | --- | --- | --- | --- |
| `Aptos.ttf` | Aptos | Regular | `Aptos` | 1287 |
| `Aptos-Bold.ttf` | Aptos | Bold | `Aptos-Bold` | 1287 |
| `Aptos-Bold-Italic.ttf` | Aptos | Bold Italic | `Aptos-BoldItalic` | 1292 |
| `Aptos-Italic.ttf` | Aptos | Italic | `Aptos-Italic` | 1292 |

Bảng dưới giữ riêng tên family/style truyền thống (`name[1]/[2]`) và typographic (`name[16]/[17]`). Các biến thể Black, ExtraBold và SemiBold có thể mang style truyền thống “Regular”; không được suy ra độ đậm chỉ từ giá trị đó. `OS/2.usWeightClass` và `head.macStyle` được ghi để đối chiếu, không thay đổi tệp font.

| Tệp | Family / style truyền thống | Family / style typographic | Weight | `head.macStyle` |
| --- | --- | --- | ---: | --- |
| `Aptos.ttf` | Aptos / Regular | Aptos / Regular | 400 | `0x00` |
| `Aptos-Black.ttf` | Aptos Black / Regular | Aptos / Black | 900 | `0x01` |
| `Aptos-Black-Italic.ttf` | Aptos Black / Italic | Aptos / Black Italic | 900 | `0x03` |
| `Aptos-Bold.ttf` | Aptos / Bold | Aptos / Bold | 700 | `0x01` |
| `Aptos-Bold-Italic.ttf` | Aptos / Bold Italic | Aptos / Bold Italic | 700 | `0x03` |
| `Aptos-Display.ttf` | Aptos Display / Regular | Aptos / Display | 400 | `0x00` |
| `Aptos-Display-Bold.ttf` | Aptos Display / Bold | Aptos / Display Bold | 700 | `0x01` |
| `Aptos-Display-Bold-Italic.ttf` | Aptos Display / Bold Italic | Aptos / Display Bold Italic | 700 | `0x03` |
| `Aptos-Display-Italic.ttf` | Aptos Display / Italic | Aptos / Display Italic | 400 | `0x02` |
| `Aptos-ExtraBold.ttf` | Aptos ExtraBold / Regular | Aptos / ExtraBold | 800 | `0x01` |
| `Aptos-ExtraBold-Italic.ttf` | Aptos ExtraBold / Italic | Aptos / ExtraBold Italic | 800 | `0x03` |
| `Aptos-Italic.ttf` | Aptos / Italic | Aptos / Italic | 400 | `0x02` |
| `Aptos-Light.ttf` | Aptos Light / Regular | Aptos / Light | 300 | `0x00` |
| `Aptos-Light-Italic.ttf` | Aptos Light / Italic | Aptos / Light Italic | 300 | `0x02` |
| `Aptos-Mono.ttf` | Aptos Mono / Regular | Aptos Mono / Regular | 400 | `0x00` |
| `Aptos-Mono-Bold.ttf` | Aptos Mono / Bold | Aptos Mono / Bold | 700 | `0x01` |
| `Aptos-Mono-Bold-Italic.ttf` | Aptos Mono / Bold Italic | Aptos Mono / Bold Italic | 700 | `0x03` |
| `Aptos-Mono-Italic.ttf` | Aptos Mono / Italic | Aptos Mono / Italic | 400 | `0x02` |
| `Aptos-Narrow.ttf` | Aptos Narrow / Regular | Aptos Narrow / Regular | 400 | `0x20` |
| `Aptos-Narrow-Bold.ttf` | Aptos Narrow / Bold | Aptos Narrow / Bold | 700 | `0x21` |
| `Aptos-Narrow-Bold-Italic.ttf` | Aptos Narrow / Bold Italic | Aptos Narrow / Bold Italic | 700 | `0x23` |
| `Aptos-Narrow-Italic.ttf` | Aptos Narrow / Italic | Aptos Narrow / Italic | 400 | `0x22` |
| `Aptos-SemiBold.ttf` | Aptos SemiBold / Regular | Aptos / SemiBold | 600 | `0x00` |
| `Aptos-SemiBold-Italic.ttf` | Aptos SemiBold / Italic | Aptos / SemiBold Italic | 600 | `0x02` |
| `Aptos-Serif.ttf` | Aptos Serif / Regular | Aptos Serif / Regular | 400 | `0x00` |
| `Aptos-Serif-Bold.ttf` | Aptos Serif / Bold | Aptos Serif / Bold | 700 | `0x01` |
| `Aptos-Serif-Bold-Italic.ttf` | Aptos Serif / Bold Italic | Aptos Serif / Bold Italic | 700 | `0x03` |
| `Aptos-Serif-Italic.ttf` | Aptos Serif / Italic | Aptos Serif / Italic | 400 | `0x02` |

## Chữ Việt và giới hạn

Cả **28/28** font có ánh xạ `cmap` cho đủ **178 chữ Việt NFC**: 29 chữ cái × hoa/thường, cộng năm thanh trên 12 nguyên âm × hoa/thường. Cả 28 cũng có đủ tám dấu kết hợp cần cho NFD: `U+0300`, `U+0301`, `U+0303`, `U+0309`, `U+0323`, `U+0302`, `U+0306`, `U+031B`. Không thiếu code point trong tập này.

Có glyph trong `cmap` không chứng minh chất lượng hiển thị, đặt dấu, chiều cao dòng hoặc cắt chữ khi WinForms đổi DPI. Windows vẫn cần kiểm tra chữ Việt có dấu trong menu, danh sách nhân vật, nhật ký, hộp thoại và tab Nhiệm Vụ – Phó Bản ở DPI 100%, 125%, 150% và 200%. Bộ ZIP không chứng minh nguồn tải chính thức hay tính tương thích với mọi phiên bản GDI; kiểm tra này xác nhận cấu trúc và dữ liệu nhận dạng của tệp được cung cấp.

## SHA-256 của các mục

```text
01bbd2d3bd483045e1dbf4f106935cb86478b937f8d334258ad12eea60554a05  Aptos.ttf
21334c0fe58b65d30cf0a850ef914a60f1ef5ae272456d1630020048551e66da  Aptos-Black.ttf
c2f8e133d5333ff022c044ee760ac0b0af22e4264a5f1411b9400936d4d1c71d  Aptos-Black-Italic.ttf
ae318584be8737164e24842e0b27ab180ee6761c1bedd799f5a29c93a8a4e65c  Aptos-Bold.ttf
eccb43dc63b966f97f49abc7031f0337411f64064f8df8ba98aa22a135cac35a  Aptos-Bold-Italic.ttf
049b18e4ca1780af39b00966491723565612eee9d18bc726f312b47372f58074  Aptos-Display.ttf
2863d62831c885e44134c72070efadd6aa7d7b487afcba46fdc6d2d5ffafaa37  Aptos-Display-Bold.ttf
698cf4a6325121430eec5e1da9fb2897f7285296438d15f7aa38fed895269f45  Aptos-Display-Bold-Italic.ttf
158210574fea328266e6586c4df45394dcf746b7de891fc52671200521ca8298  Aptos-Display-Italic.ttf
9e1637f5861fe67d7225782ab27206d55e660bb3fe6e34e99ca05655d502dc4c  Aptos-ExtraBold.ttf
5b77e9fa7c41dc0d677195ec4ae5a00aa4bcf52c2c71bc2509019194e287430d  Aptos-ExtraBold-Italic.ttf
b37b9d734abe6dd9924f914d4a3fbba6b9fdbccb30fedb4fbd6f27131307e6c2  Aptos-Italic.ttf
dbe7b9c8f07e8580ecee4c01befc4ff574cdf16bb2b90f88eaf945a9b662b0ba  Aptos-Light.ttf
939fd4e6a16c979088953620ef9dd3e7dfedb3e375db404b959554b75225ec02  Aptos-Light-Italic.ttf
8f31bec58e172359c99801268ea8a50e15bd1a2975a1cb982dfa340c69d844fa  Aptos-Mono.ttf
d0c5fbc35f48eccce8b890abff13445a173ae5c8fb2de69f84cd06b97ee88a47  Aptos-Mono-Bold.ttf
c37db2c4b70af5eff3e8d932201a1ef1ab219d01996ab9ae45c023212c86e57d  Aptos-Mono-Bold-Italic.ttf
a7be2fe98f3d3cc7e54dc0ea36626505adf6d88e52cc8502a3db7796d510068e  Aptos-Mono-Italic.ttf
3366e06e61643449cadb19c6d2a617d341915471b3985dc4049c58d8d4153253  Aptos-Narrow.ttf
6c4b05b58c57e7affbd4fcd2c42aff6cefe347b353d0ba578b2ca064721e883a  Aptos-Narrow-Bold.ttf
ad58a2322d92bef51ccc15411a0cc0c4a6cd4b0519f30b92aaa5dfe5682f4a65  Aptos-Narrow-Bold-Italic.ttf
fa30b8a2637ca294b4045961fccf02cb1e72f22777faee749d41305dc541d40a  Aptos-Narrow-Italic.ttf
3bd631a7ecb762df8aa7bca31b147db54d441babc83d91b96ba4a69d438880d9  Aptos-SemiBold.ttf
054d97515b85fc0c8e4ff8af37c04ef238c48a2c3956e40425f57dec4903191d  Aptos-SemiBold-Italic.ttf
387b6b619117c1ad1449c8cb802a16b17eb92c41f3550f6f7574ed6e9fc7d3e5  Aptos-Serif.ttf
c52b80075eb3c429640192f11c4ee780ea33b62f8af3c400671b6155d6f44954  Aptos-Serif-Bold.ttf
dc961989dcfdbf6cf8940b0912aaf6bdc1acb42483db6a537e569aa2a9d1db59  Aptos-Serif-Bold-Italic.ttf
bcb5de1b7f43f7d67ee6975b33d092d3472f32ff156da147dc6e8c601c3e3d3b  Aptos-Serif-Italic.ttf
afe1eaf4b8a0efa142eb82c9156b4004b350e54a42c7b17a12ca8058f1f299cc  Microsoft Aptos Fonts EULA.rtf
```
