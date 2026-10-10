using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
    // Maps and coordinates remain the recovered baseline. No client offset/protocol change.
    public static class AcBaEvents
    {
        public static readonly int[] Schools = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 32, 37 };
        private static readonly int[] Outdoors = { 9, 11, 10, 12, 15, 16, 13, 17, 14, 284, 615 };
        private static readonly int[] Dungeons = { 173, 175, 174, 176, 179, 180, 177, 181, 178, 288, 618 };
        private static readonly string[] Names = { "Thiếu Lâm", "Minh Giáo", "Cái Bang", "Võ Đang", "Nga Mi", "Tinh Túc", "Thiên Long", "Thiên Sơn", "Tiêu Dao", "Mộ Dung", "Đường Môn" };
        private static readonly string[] Tokens = { "thieulam", "minhgiao", "caibang", "vodang", "ngami", "tinhtuc", "thienlong", "thienson", "tieudao", "modung", "duongmon" };
        public static string SchoolName(int school) { int i = Array.IndexOf(Schools, school); return i < 0 ? "chưa xác định" : Names[i]; }
        public static int OutdoorMap(int school) { int i = Array.IndexOf(Schools, school); return i < 0 ? -1 : Outdoors[i]; }
        public static int DungeonMap(int school) { int i = Array.IndexOf(Schools, school); return i < 0 ? -1 : Dungeons[i]; }
        public static int[,] OutdoorRoute(int school)
        {
            switch (school) { case 1:return POINT.ThieuLam;case 2:return POINT.MinhGiao;case 3:return POINT.CaiBang;
                case 4:return POINT.VoDang;case 5:return POINT.NgaMy;case 6:return POINT.TinTuc;case 7:return POINT.ThienLong;
                case 8:return POINT.ThienSon;case 9:return POINT.TieuDao;case 32:return POINT.MoDung;case 37:return POINT.DuongMon;default:return null; }
        }
        public static int[,] DefaultDungeonRoute(int school)
        {
            switch (school) { case 1:return POINT.ThieuLamAcBa;case 2:return POINT.MinhGiaoAcBa;case 3:return POINT.CaiBangAcBa;
                case 4:return POINT.VoDangAcBa;case 5:return POINT.NgaMyAcBa;case 6:return POINT.TinhTucAcBa;case 7:return POINT.ThienLongAcBa;
                case 8:return POINT.ThienSonAcBa;case 9:return POINT.TieuDaoAcBa;case 32:return POINT.MoDungAcBa;case 37:return POINT.DuongMonAcBa;default:return null; }
        }
        public static int[,] ReadRoute(string saved, int school)
        {
            int[,] fallback = DefaultDungeonRoute(school);
            if (string.IsNullOrWhiteSpace(saved)) return fallback;
            string[] points = saved.Split('-');
            if (points.Length < 2 || points.Length > 256) return fallback;
            int[,] route = new int[points.Length, 2];
            for (int i = 0; i < points.Length; i++)
            {
                string[] pair = points[i].Split(','); int x, y;
                if (pair.Length != 2 || !int.TryParse(pair[0].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out x)
                    || !int.TryParse(pair[1].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out y)
                    || x < 1 || y < 1 || x > 10000 || y > 10000) return fallback;
                route[i, 0] = x; route[i, 1] = y;
            }
            return route;
        }
        // Returns false for ambiguous/future/negated text. NPC names may repeat the same school.
        public static bool TryParse(string text, out int school, out bool ended)
        {
            school = -1; ended = false;
            if (string.IsNullOrWhiteSpace(text) || text.Length > 4096) return false;
            string normalized = Regex.Replace(TINHKIEM.ClearSign(text), @"\s+", "").ToLowerInvariant().Replace("ngamy", "ngami");
            if (!normalized.Contains("acba") || normalized.Contains("chuaxuathien") || normalized.Contains("sapxuathien")
                || normalized.Contains("sexuathien") || normalized.Contains("khongxuathien")) return false;
            ended = normalized.Contains("ketthuc") || (normalized.Contains("datieudiet") || normalized.Contains("dabitieudiet")) || normalized.Contains("khongcon") || normalized.Contains("darutlui");
            if (!ended && !normalized.Contains("xuathien")) return false;
            for (int i = 0; i < Tokens.Length; i++)
                if (normalized.Contains(Tokens[i])) { if (school != -1) { school = -1; return false; } school = Schools[i]; }
            return school != -1;
        }
        // Only the existing first system-channel frame is trusted. Unknown/concatenated framing is NOT guessed.
        public static bool TryDecodeSystem(byte[] bytes, string encoding, out string text)
        {
            text = null;
            if (bytes == null || bytes.Length <= 15 || bytes.Length > 65536 || bytes[4] != 218 || bytes[5] != 3 || bytes[10] != 4) return false;
            int count = bytes.Length - 15;
            while (count > 0 && bytes[15 + count - 1] == 0) count--;
            if (count == 0 || count > 4096) return false;
            // Reject embedded terminators/second binary frames, rather than trusting a signature in chat payload.
            for (int i = 15; i < 15 + count; i++) if (bytes[i] == 0) return false;
            for (int i = 15; i + 6 < 15 + count; i++)
                if (((bytes[i] == 218 && bytes[i + 1] == 3) || (bytes[i] == 30 && bytes[i + 1] == 2))
                    && (bytes[i + 6] == 3 || bytes[i + 6] == 4)) return false;
            byte[] body = new byte[count]; Array.Copy(bytes, 15, body, 0, count);
            try
            {
                if (string.Equals(encoding, "UTF-8", StringComparison.OrdinalIgnoreCase)) text = new UTF8Encoding(false, true).GetString(body);
                else if (string.Equals(encoding, "VISCII", StringComparison.OrdinalIgnoreCase)) text = ConverterEx.VISCII2UnicodeEx(body);
                else return false;
                return text != null && text.Length <= 4096;
            }
            catch (DecoderFallbackException) { return false; }
        }
    }
}
