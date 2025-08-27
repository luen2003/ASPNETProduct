namespace IdentityServer.Models.Settings
{
    public class DefaultOTPSettings : ISettings
    {
        public int TimeStep { get; } = 120;
        public int Tolerance { get; } = 3;
        public int DigitLength { get; } = 6;
    }
}
