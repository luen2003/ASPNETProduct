namespace IdentityServer.Providers.SecretGenerator.Implementations
{
    public class Secret128BitRandomGeneratorProvider : BaseRandomSecretGeneratorProvider
    {
        private const int KeyLength = 128;

        public Secret128BitRandomGeneratorProvider()
            : base(KeyLength)
        {
        }
    }
}
