using System.Text;

namespace MultiClientSimulation;

// Offline classifier based on the user's screenshot, not a Windows dialog reader or patcher.
internal static class DialogObservation
{
    public const string Title = "Thien Long Bat Bo";
    public const string Message = "因为您运行的客户端过多,导致资源不足,无法启动!";
    public static bool IsKnownWarning(string title, string message, bool ownerIsSelectedGame)
    {
        return ownerIsSelectedGame && title == Title && Normalize(message) == Normalize(Message);
    }
    private static string Normalize(string text)
    {
        var result = new StringBuilder();
        foreach (char c in text ?? "")
        {
            if (char.IsWhiteSpace(c)) continue;
            result.Append(c == '，' ? ',' : c == '！' ? '!' : c);
        }
        return result.ToString();
    }
}
