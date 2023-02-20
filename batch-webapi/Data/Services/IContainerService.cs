using System.Threading.Tasks;

namespace batch_webapi.Data.Services
{
    public interface IContainerService
    {
        Task CreateContainer(string containerName);
        bool CheckIfContainerExist(string containerName);

        Task AddFile(string fileName, string filePath, string containerName, string contentType);

        bool GetFile(string fileName, string containerName);
    }

}
