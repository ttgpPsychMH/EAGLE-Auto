# Đối chiếu ChickenAuto 116 – Ex 107

Đọc [báo cáo tiếng Việt](REPORT.vi.md) và [ma trận kiểm thử](TEST-MATRIX.vi.md). Đây là phân tích tĩnh, không có functional changes hoặc binary build mới.

Stable 116 là native x86/Qt, không có CLR metadata để khôi phục bằng ILSpy. 107 có source C# và project net48 đã merge. Evidence gồm metadata PE, resource inventory, 16 Qt metaobject được chọn, 256 mục dịch UI, các literal được chọn trước và một dispatch stub; không phải source C++ đầy đủ.

## Tái lập phân tích

Prerequisites đã dùng: Python 3.12; các package pinned trong [requirements](../chickenautoex-107/tools/requirements.txt); .NET SDK 8.0.425; ILSpyCmd 9.1.0.7988 theo manifest repository; GNU `objdump`. Công cụ phân tích đọc executable như dữ liệu. Không cần game, tài khoản, secret, API game hoặc license server. Dùng checkout hiện có, không tạo worktree.

Từ root repository, tạo một venv trong thư mục ngoài checkout và cài requirements. Nếu dùng tool manifest của repository:

```bash
python3 -m venv /workspace/chickenauto-compare-tools/venv
/workspace/chickenauto-compare-tools/venv/bin/python -m pip install -r analysis/chickenautoex-107/tools/requirements.txt
dotnet tool restore
```

Script cần đường dẫn ILSpy executable để giữ nguyên exit code của lần thử native decompilation. Có thể dùng tool đã cài riêng đúng phiên bản:

```bash
dotnet tool install ilspycmd --version 9.1.0.7988 --tool-path /workspace/chickenauto-compare-tools/ilspy
/workspace/chickenauto-compare-tools/venv/bin/python analysis/chickenauto-116-vs-107/tools/compare_static.py \
  --repo "$PWD" \
  --work /workspace/chickenauto-compare-output \
  --ilspy /workspace/chickenauto-compare-tools/ilspy/ilspycmd
```

`--work` phải rỗng và ngoài checkout. Chọn đường dẫn mới khi chạy lại để giữ lần phân tích trước. Khi SDK không nằm trong PATH chuẩn, đặt `DOTNET_ROOT` đúng thư mục SDK; chọn `DOTNET_CLI_HOME` writable, không in environment hoặc credential. `objdump` phải nằm trong PATH. Các tool/version trên đã được dùng trong môi trường hiện tại; lệnh cài package/tool dành cho môi trường mới, không phải một lần cài mới trong lượt báo cáo này.

Kết quả ở `--work/evidence`. Payload 116 và DLL native được giải nén riêng ở `--work`, không chạy hoặc commit chúng. Script chỉ export các tên literal đã chọn trước; không dump arbitrary strings của binary. Các offset Qt rcc/metaobject được xác định cho SHA payload đã review và script từ chối binary khác; không phải scanner chung cho mọi bản ChickenAuto.

## Kiểm tra đã thực hiện

- Hash cả sáu release giống baseline điều tra Ex; payload stable ZIP/7z/wrapper giống nhau từng byte.
- Đọc cây Qt rcc v1 qua ba địa chỉ array được xác định từ registration call: data VA `0xc79a10`, names `0xd7c9c8`, tree `0xd7cbe0`; không tìm resource chỉ bằng tên.
- Parse đủ 13 file resource của bộ này, 256 message QM tiếng Việt và header/method table 16 lớp Qt được chọn.
- ILSpy từ chối stable do thiếu managed metadata, exit 70. Đây là kết quả dự kiến, không phải test build thất bại của 107.
- `QuestSettings` có 0 method Qt; đọc 3 byte `static_metacall` bằng `objdump`. Không suy diễn rằng mọi code nhiệm vụ native đều không tồn tại.
- Không chỉnh source, project, tests, CI, lockfiles, automation/licensing hoặc release binaries. Không chạy game, executable hay DLL.

Không lưu raw source native, toàn bộ disassembly, account/config thật hoặc PDB path cá nhân. Các dữ kiện chưa xác minh được ghi rõ trong báo cáo, thay vì tạo một project 116 giả hoặc khẳng định feature parity.
