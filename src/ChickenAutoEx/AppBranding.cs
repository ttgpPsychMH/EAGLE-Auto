namespace TinhKiemAuto
{
    // Presentation only. Global.Version remains the legacy protocol version.
    internal static partial class AppBranding
    {
        internal const string Name = "EAGLE Auto";
        internal const string WindowTitle = Name + " " + DisplayVersion;
        internal const string Author = "tenkafuku";
        internal const string AboutTitle = "Về EAGLE Auto";
        internal const string Attribution = "Được sửa lại dựa trên Chicken Auto 107, vibe coding bằng Codex bởi tenkafuku.";
        internal const string AboutText = WindowTitle + "\n\n" + Attribution;
        internal const string UpdateTitle = "Thông tin cập nhật EAGLE Auto";
        internal const string ChangelogText = WindowTitle + " — đang hoàn thiện\n\n"
            + Attribution + "\n\n"
            + "Bản sửa hiện tại:\n"
            + "• Khôi phục project .NET Framework 4.8 và xử lý lỗi khởi động/cập nhật.\n"
            + "• Sửa lựa chọn hồi MP và đọc cấu hình map tùy chọn.\n"
            + "• Sửa menu nhiệm vụ/phó bản khi thiếu nhân vật hoặc đội trưởng.\n"
            + "• Đổi tên hiển thị, file chạy và thông tin giới thiệu sang " + WindowTitle + ".\n\n"
            + "Khả năng hoạt động trong game đang được kiểm thử.";
    }
}
