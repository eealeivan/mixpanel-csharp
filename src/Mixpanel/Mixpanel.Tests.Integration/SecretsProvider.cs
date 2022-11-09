using System;
using Microsoft.Extensions.Configuration;
using Mixpanel.Tests.Integration.DefaultHttpClient;

namespace Mixpanel.Tests.Integration
{
    public static class SecretsProvider
    {
        public static string MixpanelProjectId { get; }
        public static string MixpanelProjectToken { get; }
        public static string MixpanelServiceAccountUsername { get; }
        public static string MixpanelServiceAccountSecret { get; }

        static SecretsProvider()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<DefaultHttpClientTests>();

            IConfiguration configuration = builder.Build();

            MixpanelProjectId = configuration["Mixpanel:Project:Id"];
            if (string.IsNullOrWhiteSpace(MixpanelProjectId))
            {
                throw new Exception("Mixpanel:ProjectId is empty.");
            }

            MixpanelProjectToken = configuration["Mixpanel:Project:Token"];
            if (string.IsNullOrWhiteSpace(MixpanelProjectToken))
            {
                throw new Exception("Mixpanel:ProjectToken is empty.");
            }

            MixpanelServiceAccountUsername = configuration["Mixpanel:ServiceAccount:Username"];
            if (string.IsNullOrWhiteSpace(MixpanelServiceAccountUsername))
            {
                throw new Exception("Mixpanel:ServiceAccount:Username is empty.");
            }

            MixpanelServiceAccountSecret = configuration["Mixpanel:ServiceAccount:Secret"];
            if (string.IsNullOrWhiteSpace(MixpanelServiceAccountSecret))
            {
                throw new Exception("Mixpanel:ServiceAccount:Secret is empty.");
            }
        }
    }
}