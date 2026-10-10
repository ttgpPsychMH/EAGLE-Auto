using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
    public partial class Game
    {
        // Legacy aliases are private to this module. Prefer specific names and reject ambiguity.
        private static readonly Dictionary<string, int> TrungAcMaps = new Dictionary<string, int>
        {
            { "lacduong", 0 },
            { "locduong", 0 },
            { "tochau", 1 },
            { "daily", 2 },
            { "doily", 2 },
            { "doilu", 2 },
            { "tungson", 3 },
            { "thaiho", 4 },
            { "kinhho", 5 },
            { "voluongson", 6 },
            { "kiemcac", 7 },
            { "donhoang", 8 },
            { "thieulamtu", 9 },
            { "caibangtongda", 10 },
            { "quangminhdien", 11 },
            { "vodangson", 12 },
            { "thienlongtu", 13 },
            { "langbadong", 14 },
            { "ngamison", 15 },
            { "tinhtuchai", 16 },
            { "thienson", 17 },
            { "nhannam", 18 },
            { "nhonnam", 18 },
            { "nhanbac", 19 },
            { "nhonbac", 19 },
            { "thaonguyen", 20 },
            { "lieutay", 21 },
            { "truongbachson", 22 },
            { "truongbochson", 22 },
            { "hoanglongphu", 23 },
            { "nhihai", 24 },
            { "thuongson", 25 },
            { "thachlam", 26 },
            { "thochlam", 26 },
            { "ngockhe", 27 },
            { "namchieu", 28 },
            { "mieucuong", 29 },
            { "tayho", 30 },
            { "longtuyen", 31 },
            { "vodi", 32 },
            { "mailinh", 33 },
            { "namvuc", 34 },
            { "namhai", 34 },
            { "quynhchau", 35 },
            { "huyenvudao", 112 },
            { "baotangdongtang1", 166 },
            { "baotangdongtang2", 169 },
            { "nganngaituyetnguyen", 229 },
            { "baotangdongtang3", 191 },
            { "baotangdongtang4", 192 },
            { "baotangdongtang5", 193 },
            { "thaolieutruong", 199 },
            { "mieunhandong", 200 },
            { "thanhthuson", 201 },
            { "yenvuongcomotang1", 202 },
            { "yenvuongcomotang2", 203 },
            { "yenvuongcomotang3", 204 },
            { "yenvuongcomotang4", 205 },
            { "yenvuongcomotang5", 206 },
            { "yenvuongcomotang6", 207 },
            { "yenvuongcomotang7", 208 },
            { "yenvuongcomotang8", 209 },
            { "yenvuongcomotang9", 210 },
            { "bentausondong", 211 },
            { "kiemgia", 212 },
            { "manhaidong", 213 },
            { "danhancau", 214 },
            { "ontuyendong", 215 },
            { "hoanglongdong", 216 },
            { "thuykinhho", 217 },
            { "tienvuongphan", 218 },
            { "thienkhanhthudong", 219 },
            { "daohoanguyen", 220 },
            { "haitacdong", 221 },
            { "tuyetlangho", 222 },
            { "diemho", 235 },
            { "bachsadiemkhanh", 237 },
            { "bochsadiemkhanh", 237 },
            { "hoadiemson", 244 },
            { "caoxuong", 245 },
            { "laulan", 246 },
            { "thaplymoc", 247 },
            { "thaplumoc", 247 },
            { "hoadiemcoc", 251 },
            { "caoxuongmecung", 252 },
            { "thapkhaclapmacan", 253 },
            { "daiuyen", 249 },
            { "hanhuyetlinh", 255 },
            { "honhuyetlinh", 255 },
            { "tanhoangdiacungtang1", 262 },
            { "tanhoangdiacungtang2", 263 },
            { "tanhoangdiacungtang3", 264 },
            { "tanhoangdiacungtang4", 292 },
            { "datayho", 164 },
            { "dotayho", 164 },
            { "conlonphucdia", 254 },
            { "conlonson", 248 },
            { "thanhnguyen", 282 },
            { "thanhnguyensondong", 283 },
            { "modungsontrang", 284 },
            { "tatmanhihan", 250 },
            { "thanhhoacung", 256 },
            { "lamhaikhecoc", 569 },
            { "macnamthanhnguyen", 573 },
            { "vongxuyenhoahai", 574 },
            { "thienkynamhoai", 575 },
            { "thongthienthapdiacung", 295 },
            { "thongthienthaptang1", 296 },
            { "thongthienthaptang2", 297 },
            { "thongthienthaptang3", 298 },
            { "dinhthongthienthap", 299 },
            { "phungminhtran", 580 },
            { "thuchacotran", 260 },
            { "denhatkhunghingoitailacduong", 238 },
            { "khunghingoitaidaily", 240 },
            { "khunghingoitaitochau", 241 },
            { "hanngoccoc", 243 },
            { "thuynguyetdongthien", 613 },
            { "huyenhai", 611 },
            { "daicondihai", 612 },
            { "denhatkhunghingoitailacdduong", 238 },
            { "denhikhunghingoitailacduong", 239 },
            { "tientrang", 224 },
            { "quangminhdong", 601 },
            { "daycoctieudao", 602 },
            { "linhtinhphong", 603 },
            { "caibangtuudieu", 604 },
            { "daohoatran", 605 },
            { "thaplam", 606 },
            { "nguthandong", 607 },
            { "chietmaiphong", 608 },
            { "chanthap", 609 },
            { "tangthuthuycac", 610 },
            { "hauhoavien", 123 },
            { "tieumocnhanhang", 122 },
            { "duonggiabao", 615 },
            { "dienvotruong", 617 },
            { "quanthienthanh", 581 },
            { "trieukinhthanh", 583 },
            { "laphuthanh", 582 },
        };

        private static bool TryParseTrungAcDestination(string info, out int map, out int x, out int y)
        {
            map = -1; x = y = 0;
            if (string.IsNullOrEmpty(info) || info.Length > 16384) return false;
            var coordinates = Regex.Matches(info, @"\[\s*([+-]?[0-9]+)\s*,\s*([+-]?[0-9]+)\s*\]");
            if (coordinates.Count != 1 || !int.TryParse(coordinates[0].Groups[1].Value, out x)
                || !int.TryParse(coordinates[0].Groups[2].Value, out y) || x < 0 || y < 0 || x > 4095 || y > 4095)
                return false;
            string normalized = TINHKIEM.VietLien(info);
            var matches = new List<string>();
            foreach (string name in TrungAcMaps.Keys) if (normalized.Contains(name)) matches.Add(name);
            foreach (string name in matches)
            {
                bool contained = false;
                foreach (string other in matches)
                    if (other.Length > name.Length && other.Contains(name)) { contained = true; break; }
                if (contained) continue;
                int candidate = TrungAcMaps[name];
                if (map != -1 && candidate != map) { map = -1; return false; }
                map = candidate;
            }
            return map != -1;
        }
    }
}
