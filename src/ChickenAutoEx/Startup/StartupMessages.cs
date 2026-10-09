namespace ChickenAutoEx.Startup
{
    internal static class StartupMessages
    {
        internal static string ForUpdate(UpdateResult result)
        {
            switch (result.Status)
            {
                case UpdateStatus.UpToDate:
                    return "Đã kiểm tra phiên bản máy chủ: " + result.RemoteVersion + ".\n";
                case UpdateStatus.UpdateAvailable:
                    return "Có phiên bản mới: " + result.RemoteVersion + ". Bạn có thể cập nhật thủ công; ứng dụng tiếp tục khởi động.\n";
                case UpdateStatus.Cancelled:
                    return "";
                default:
                    return "Không kiểm tra được cập nhật (" + result.DiagnosticCode + "). Ứng dụng tiếp tục khởi động.\n";
            }
        }
    }
}
