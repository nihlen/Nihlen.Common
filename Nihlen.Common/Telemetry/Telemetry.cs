using System.Diagnostics;

namespace Nihlen.Common.Telemetry;

public static class Telemetry
{
    public static ActivitySource ActivitySource = new("Unknown Application", "1.0.0");
}
