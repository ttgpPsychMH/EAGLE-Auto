# ChickenAutoEx 107 static investigation

Đọc [báo cáo tiếng Việt](REPORT.vi.md) trước. `recovered/` là source do ILSpy khôi phục và che dữ liệu nhạy cảm, không phải source gốc hay release đã sửa. Không thực thi binary gốc, resource DLL hoặc output compiler.

## Artifacts

- `evidence/inventory.json`: hashes, PE/CLR identity, references, resources, native imports/exports.
- `evidence/startup.il`: IL của Main, trích DLL và handler startup; không chứa key/password.
- `evidence/network-observations.json`: phân biệt phản hồi proxy, GitHub và quan sát người dùng.
- `evidence/source-compile.log`: kiểm tra C# tĩnh; không chạy ứng dụng hoặc xác nhận build đủ resources.
- `recovered/`: 331 C# files, generated project, RESX và text resources.
- `recovery-manifest.json`: provenance file, vị trí redaction và binary assets loại khỏi Git.

## Tái khôi phục bằng công cụ open source

Dùng checkout hiện tại trong môi trường cô lập; không tạo worktree nếu không được yêu cầu. Luôn đặt dữ liệu raw và công cụ **ngoài checkout**, vì mã decompiled gốc có embedded credentials. Không đưa raw source/DLL/binary output vào Git. Kiểm tra `git status --short` trước và sau.

Các lệnh dưới đây đọc binary như dữ liệu. Thay đường dẫn SDK/ILSpy/reference tương ứng với máy của bạn. Những dependency đã dùng trong lần điều tra này:

| Công cụ/reference | Phiên bản |
|---|---|
| .NET SDK chạy ILSpy/Roslyn | 8.0.425 |
| ilspycmd | 9.1.0.7988 |
| Microsoft.NETFramework.ReferenceAssemblies.net35 (NuGet) | 1.0.3 |
| Newtonsoft.Json (NuGet, `lib/net35`) | 12.0.3 |
| Zen.Barcode.Rendering.Framework (NuGet, `lib`) | 3.1.10729.1 |

```bash
python3 -m venv /workspace/chickenauto-analysis-tools/venv
/workspace/chickenauto-analysis-tools/venv/bin/pip install \
  -r /workspace/EAGLE-Auto/analysis/chickenautoex-107/tools/requirements.txt
# Với SDK đã cài: dùng dotnet tool install ilspycmd --version 9.1.0.7988
# --tool-path /workspace/chickenauto-analysis-tools/ilspy.
# Tải reference packages từ api.nuget.org; giải nén như ZIP, không chạy tools trong package.
# Gom mscorlib, System, System.Core, System.Drawing, System.Windows.Forms,
# System.Xml, System.Management từ net35 cùng Newtonsoft.Json 12 và Zen.Barcode
# vào /workspace/chickenauto-analysis-tools/reference.
export DOTNET_ROOT=/workspace/chickenauto-investigation/tools/dotnet
/workspace/chickenauto-analysis-tools/venv/bin/python \
  /workspace/EAGLE-Auto/analysis/chickenautoex-107/tools/recover.py \
  --repo /workspace/EAGLE-Auto \
  --output /workspace/chickenauto-raw-analysis \
  --ilspy /workspace/chickenauto-analysis-tools/ilspy/ilspycmd \
  --references /workspace/chickenauto-analysis-tools/reference
python3 /workspace/EAGLE-Auto/analysis/chickenautoex-107/tools/export_review.py \
  --input /workspace/chickenauto-raw-analysis/decompiled \
  --output /workspace/chickenauto-review-copy/recovered
```

Output phải mới/trống. `recover.py` từ chối release có payload hash khác, archive chứa tên file bất ngờ hoặc output nằm trong checkout. Script xác minh wrapper/ZIP/7z giống nhau, trích resource DLL, ghi metadata và chạy ILSpy C# 7.3. `dnfile` phát hai cảnh báo `invalid compressed int` khi đọc heap metadata bản Ex; các bảng cần dùng và ILSpy vẫn đọc được. Script không khẳng định mọi blob/metadata đều có thể khôi phục nguyên trạng.

`export_review.py` kiểm tra đúng layout của năm vị trí sensitive literal C#, thay bằng `[REDACTED]`, che các lần xuất hiện lặp trong tài nguyên văn bản, kiểm tra không còn các literal ấy trong bản review, và bỏ binary assets. Nó không phải một secret scanner tổng quát; mọi binary/release khác phải review lại trước khi xuất bản. Project sinh tự động còn dùng HintPath của thư mục phân tích và asset bị bỏ; không hứa build snapshot trực tiếp.

## Kiểm tra C# mà không chạy ứng dụng

Chạy Roslyn `csc.dll` từ SDK .NET, truyền `/noconfig /nostdlib+ /target:winexe /unsafe+ /langversion:7.3`, `/out:` ở ngoài checkout, các DLL reference ở trên và toàn bộ `*.cs` qua response file. Chỉ compiler của SDK chạy; không chạy output. Lần kiểm tra thực tế dùng compiler C# 12 với syntax decompiled C# 7.3. Log trong `evidence/` là của snapshot đã redaction. Chưa đóng gói resources hoặc chạy một test suite Windows.

License gốc tại [LICENSE](../../LICENSE) được giữ nguyên. Assembly có code thư viện gộp; không suy diễn rằng license của từng dependency đã được khôi phục đầy đủ. Xác minh license/provenance từng thư viện trước khi phát hành build mới.
