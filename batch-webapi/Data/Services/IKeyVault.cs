namespace batch_webapi.Data.Services
{
    public interface IKeyVault
    {
        public string GetStorageConnectionStringSecret();
        public string GetDatabaseConnectionStringSecret();

    }
}
