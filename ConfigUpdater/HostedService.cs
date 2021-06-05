using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Newtonsoft.Json;

namespace ConfigUpdater
{
    public class HostedService : BackgroundService
    {
        private readonly IConfigurationRoot _config;
        private readonly IHostApplicationLifetime _lifeTime;
        private readonly ILogger<HostedService> _logger;
        private static HttpClient _client;

        public HostedService(IHostApplicationLifetime lifeTime, ILogger<HostedService> logger)
        {
            _config = Program.configuration;
            _logger = logger;
            _lifeTime = lifeTime;
            _client = new HttpClient();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await BackgroundProcessing(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    HttpResponseMessage response = await _client.GetAsync("https://dev-136034.okta.com/oauth2/default/v1/keys");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Jwks keySet = JsonConvert.DeserializeObject<Jwks>(responseBody);

                    foreach (Key key in keySet.Keys)
                    {
                        RSAParameters rsaKeyInfo = new RSAParameters
                        {
                            Modulus = TextEncodings.Base64Url.Decode(key.N),
                            Exponent = TextEncodings.Base64.Decode(key.E)
                        };
                        RSA rsa = RSA.Create(rsaKeyInfo);
                        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
                        Console.WriteLine(TextEncodings.Base64.Encode(publicKey));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(default, ex, ex.Message);
                    _lifeTime.StopApplication();
                }
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }
    }
}
