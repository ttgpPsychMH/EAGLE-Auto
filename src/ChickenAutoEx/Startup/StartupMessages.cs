namespace ChickenAutoEx.Startup
{
    internal static class StartupMessages
    {
        internal static string ForUpdate(UpdateResult result)
        {
            switch (result.Status)
            {
                case UpdateStatus.UpToDate:
                    return "Đã kiểm tra dữ liệu cập nhật của bản gốc (máy chủ: " + result.RemoteVersion + ").\n";
                case UpdateStatus.UpdateAvailable:
                    return "Bản gốc có phiên bản mới: " + result.RemoteVersion + ". Ứng dụng tiếp tục khởi động.\n";
                case UpdateStatus.Cancelled:
                    return "";
                default:
                    return "Không kiểm tra được cập nhật (" + result.DiagnosticCode + "). Ứng dụng tiếp tục khởi động.\n";
            }
        }
    }
}
