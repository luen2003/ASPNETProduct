using Serilog;

namespace IdentityServer.Models;

public class SerilogSetting
{
    public int LimitFiles { get; set; } = 700;

    public long Size { get; set; } = 1073741824;

    public RollingInterval RollingInterval { get; set; } = RollingInterval.Hour;
}