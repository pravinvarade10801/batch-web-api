using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;

namespace batch_webapi.Data.Services
{
    public class KeyVault : IKeyVault
    {
        private readonly IConfiguration _configuration;

        public KeyVault(IConfiguration configuration) {
        _configuration= configuration;  
        }
        public string GetStorageConnectionStringSecret()
        {
            var keyVaultUrl = _configuration.GetValue<string>("KeyVaultURL");
            var secretName = _configuration.GetValue<string>("StorageConnectionsSecret");

            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            var value = client.GetSecret(secretName).Value.Value;
            return value;
        }
        public string GetDatabaseConnectionStringSecret()
        {
            var keyVaultUrl = _configuration.GetValue<string>("KeyVaultURL");
            var secretName = _configuration.GetValue<string>("DbConnectionStringSecret");

            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            var value = client.GetSecret(secretName).Value.Value;
            return value;
        }
    }
}
