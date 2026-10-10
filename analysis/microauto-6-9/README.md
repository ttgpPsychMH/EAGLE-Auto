# Hồ sơ review source MicroAuto

Đọc [REPORT.vi.md](REPORT.vi.md) để xem kết luận và thứ tự cải thiện 107. Nhánh chỉ có phân tích, không có bản ứng dụng sửa mới. Source tải lên và output compiler được giữ ngoài checkout.

## Evidence

- [inventory.json](evidence/inventory.json): hash ZIP và các file đọc được; ZIP con mã hóa chưa đọc.
- [summary.json](evidence/summary.json): project, dependencies, số file và kết quả compiler.
- [findings.json](evidence/findings.json): vị trí source, hash, tên khóa INI và vai trò lớp. Số hit tìm chuỗi không chứng minh vắng mặt implementation.
- [compiler.txt](evidence/compiler.txt): stdout/stderr compiler; tên file đã chuẩn hóa về đường dẫn trong ZIP.
- [verification.json](evidence/verification.json): kiểm tra bảo toàn ZIP, 34 file giải nén, sáu release ChickenAuto và các link tài liệu.

## Tái lập phép kiểm tra C#

Đây là kiểm tra compiler của các file C#, không build/execute project tải lên. Dùng SDK .NET 8.0.425 và reference package `Microsoft.NETFramework.ReferenceAssemblies.net35` 1.0.3; không dùng reference assemblies của .NET 8 cho WinForms cũ.

1. Kiểm tra SHA-256 ZIP khớp inventory. Giải nén phần không mã hóa vào thư mục riêng, từ chối path traversal/symlink; không mở ZIP con và không chạy bất kỳ file nào. Kiểm tra từng hash của 34 file.
2. Tạo response file của compiler, mỗi argument một dòng, quoted khi đường dẫn chứa khoảng trắng:

   ```text
   /nostdlib+
   /target:library
   /platform:x86
   /langversion:7.3
   /out:<thư mục riêng>/MicroAuto-static-probe.dll
   /reference:<reference net35>/mscorlib.dll
   /reference:<reference net35>/System.dll
   /reference:<reference net35>/System.Drawing.dll
   /reference:<reference net35>/System.Windows.Forms.dll
   /reference:<reference net35>/Microsoft.VisualBasic.dll
   <đường dẫn của từng file .cs trong 26 Compile item>
   ```

3. Gọi compiler trực tiếp, thay placeholder bằng đường dẫn thực và quote đúng theo shell:

   ```text
   dotnet <SDK>/Roslyn/bincore/csc.dll /noconfig @<compile.rsp>
   ```

Phép thử đã ghi nhận exit 0 / 0 errors / 25 warnings. Output là thư viện kiểm tra, không embed resource và không phải executable phát hành; không chạy output. Không yêu cầu hash output compiler lặp lại vì phép biên dịch không đặt deterministic. Hash source đầu vào mới là đối chiếu chính.

Project gốc khai báo net20, còn probe dùng reference net35 (CLR 2). Vì vậy kết quả không xác nhận full build với đúng reference net20, tài nguyên, dependency native hoặc khả năng chạy trên Windows/game. Khi cần build ứng dụng MicroAuto hoàn chỉnh phải đánh giá riêng các phần này; hiện chưa có lý do để chuyển nhiệm vụ cải thiện 107 sang phục hồi MicroAuto.
