using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TinhKiemAuto.Diagnostics
{
    internal static class DiagnosticRowValidator
    {
        internal static bool Valid(string row, string version)
        {
            if (row == null || row.Length > 1024 || row.IndexOf('\r') >= 0 || row.IndexOf('\n') >= 0) return false;
            string[] cells = row.Split(','); DateTime date;
            if (cells.Length != 17 || !DateTime.TryParseExact(cells[0], "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date)
                || cells[1] != version || !Named<DiagnosticEvent>(cells[2]) || !Named<DiagnosticFeature>(cells[3])
                || !Named<DiagnosticErrorKind>(cells[4]) || !Regex.IsMatch(cells[5], @"\A0x[0-9A-F]{8}\z")
                || (cells[6] != "" && !Regex.IsMatch(cells[6], @"\Awindow_[0-9]{4}\z"))
                || (cells[13] != "" && cells[13] != "On" && cells[13] != "Off") || !Named<DiagnosticReason>(cells[16])) return false;
            for (int i = 7; i <= 15; i++)
            {
                if (i == 13 || cells[i] == "") continue;
                double number;
                if (!double.TryParse(cells[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number)
                    || number < 0 || double.IsInfinity(number) || double.IsNaN(number)) return false;
            }
            return true;
        }
        private static bool Named<T>(string value) where T : struct
        {
            T parsed; return Enum.TryParse<T>(value, out parsed) && Enum.IsDefined(typeof(T), parsed) && parsed.ToString() == value;
        }
    }
}
