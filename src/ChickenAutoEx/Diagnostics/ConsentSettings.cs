using System;
using System.IO;
using System.Text;

namespace TinhKiemAuto.Diagnostics
{
    public enum DiagnosticConsent { Unknown, Declined, Accepted }
    public sealed class ConsentSettings
    {
        public const string Policy = "EAGLE-DIAGNOSTICS-POLICY-1";
        private readonly string folder;
        public ConsentSettings(string folder) { this.folder = folder; }
        public DiagnosticConsent Read()
        {
            try
            {
                string path = Path.Combine(folder, "consent.txt");
                if (!File.Exists(path) || new FileInfo(path).Length > 128) return DiagnosticConsent.Unknown;
                string text = File.ReadAllText(path, Encoding.UTF8);
                if (text == Policy + "\nAccepted") return DiagnosticConsent.Accepted;
                if (text == Policy + "\nDeclined") return DiagnosticConsent.Declined;
            }
            catch { }
            return DiagnosticConsent.Unknown;
        }
        public bool Write(DiagnosticConsent consent)
        {
            if (consent != DiagnosticConsent.Accepted && consent != DiagnosticConsent.Declined) return false;
            try {
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "consent.txt"), Policy + "\n" + consent, new UTF8Encoding(false));
                return true;
            } catch { return false; }
        }
    }
}
