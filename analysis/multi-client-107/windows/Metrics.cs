using System;
using System.Globalization;

namespace EagleMultiClientMonitor
{
    public static class Metrics
    {
        public static double CpuPercent(double cpuMilliseconds, double elapsedSeconds, int processors)
        {
            if (cpuMilliseconds < 0 || elapsedSeconds <= 0 || processors <= 0
                || double.IsNaN(cpuMilliseconds) || double.IsInfinity(cpuMilliseconds)
                || double.IsNaN(elapsedSeconds) || double.IsInfinity(elapsedSeconds))
                throw new ArgumentOutOfRangeException();
            return cpuMilliseconds / (elapsedSeconds * 1000.0 * processors) * 100.0;
        }
        public static string Number(double value) { return value.ToString("F2", CultureInfo.InvariantCulture); }
        public static string Csv(string value) { return "\"" + (value ?? "").Replace("\"", "\"\"") + "\""; }
    }
}
