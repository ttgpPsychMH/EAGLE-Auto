namespace ChickenAutoEx.Startup
{
    internal enum UpdateStatus
    {
        UpToDate, UpdateAvailable, Unavailable, InvalidMetadata, ConfigurationError, Cancelled
    }

    internal sealed class UpdateResult
    {
        internal UpdateStatus Status { get; private set; }
        internal int? RemoteVersion { get; private set; }
        internal string DiagnosticCode { get; private set; }

        internal UpdateResult(UpdateStatus status, string diagnosticCode, int? remoteVersion = null)
        {
            Status = status;
            DiagnosticCode = diagnosticCode;
            RemoteVersion = remoteVersion;
        }
    }
}
