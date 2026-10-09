using System;
using System.Globalization;

namespace ChickenAutoEx.Startup
{
    internal sealed class UpdateOptions
    {
        internal const int DefaultTimeoutMilliseconds = 5000;
        internal const int MaximumMetadataBytes = 32768;

        internal Uri Endpoint { get; private set; }
        internal int TimeoutMilliseconds { get; private set; }

        internal static bool TryCreate(string endpoint, string timeout, out UpdateOptions options)
        {
            options = null;
            Uri uri;
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out uri)
                || !(uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback))
                || uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0)
                return false;

            int milliseconds = DefaultTimeoutMilliseconds;
            if (!string.IsNullOrWhiteSpace(timeout)
                && (!int.TryParse(timeout, NumberStyles.None, CultureInfo.InvariantCulture, out milliseconds)
                    || milliseconds < 200 || milliseconds > 30000))
                return false;

            options = new UpdateOptions { Endpoint = uri, TimeoutMilliseconds = milliseconds };
            return true;
        }
    }
}
