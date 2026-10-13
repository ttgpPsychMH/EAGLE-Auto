using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TinhKiemAuto.Diagnostics
{
    public sealed class RotatingDiagnosticStore : IDiagnosticStore
    {
        public const int DefaultFileBytes = 2 * 1024 * 1024, DefaultFiles = 4;
        private readonly string folder, session = Guid.NewGuid().ToString("N");
        private readonly int maximumBytes, maximumFiles;
        private StreamWriter writer;
        private long bytes;
        private int part;
        private bool disposed;
        public RotatingDiagnosticStore(string folder, int maximumBytes = DefaultFileBytes, int maximumFiles = DefaultFiles)
        {
            if (maximumBytes < 512 || maximumFiles < 1 || maximumFiles > 16) throw new ArgumentOutOfRangeException();
            this.folder = folder; this.maximumBytes = maximumBytes; this.maximumFiles = maximumFiles;
        }
        public void Write(string row)
        {
            if (disposed) throw new ObjectDisposedException("DiagnosticStore");
            if (row == null || row.Length > 1024 || row.IndexOf('\n') >= 0 || row.IndexOf('\r') >= 0) throw new ArgumentException();
            int length = Encoding.UTF8.GetByteCount(row + "\n");
            int headerLength = 3 + Encoding.UTF8.GetByteCount(DiagnosticSchema.Header + "\n");
            if (length + headerLength > maximumBytes) throw new ArgumentException();
            if (writer == null || bytes + length > maximumBytes) Rotate();
            writer.WriteLine(row); writer.Flush(); bytes += length;
        }
        private void Rotate()
        {
            if (writer != null) writer.Dispose();
            Directory.CreateDirectory(folder);
            string name = "eagle-diagnostics-" + session + "-" + (++part).ToString("D4", CultureInfo.InvariantCulture) + ".csv";
            writer = new StreamWriter(new FileStream(Path.Combine(folder, name), FileMode.CreateNew, FileAccess.Write, FileShare.Read), new UTF8Encoding(true));
            writer.NewLine = "\n"; writer.WriteLine(DiagnosticSchema.Header); writer.Flush();
            bytes = 3 + Encoding.UTF8.GetByteCount(DiagnosticSchema.Header + "\n");
            var managed = new DirectoryInfo(folder).GetFiles("eagle-diagnostics-*.csv")
                .Where(f => Regex.IsMatch(f.Name, @"\Aeagle-diagnostics-[0-9a-f]{32}-[0-9]{4,8}\.csv\z"))
                .OrderByDescending(f => f.LastWriteTimeUtc).ThenByDescending(f => f.Name).ToArray();
            int kept = 1;
            foreach (var file in managed)
            {
                if (file.Name == name) continue;
                if (kept++ >= maximumFiles) file.Delete();
            }
        }
        public void Dispose() { if (disposed) return; disposed = true; if (writer != null) writer.Dispose(); }
    }
}
