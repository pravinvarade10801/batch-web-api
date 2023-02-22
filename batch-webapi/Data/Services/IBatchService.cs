using batch_webapi.Data.ViewModels;
using System;
using System.Threading.Tasks;

namespace batch_webapi.Data.Services
{
    public interface IBatchService
    {
        public Guid CreateBatch(BatchVM batch, Guid batchId);
        public BatchVMWithBatchDetails GetBatch(Guid batchId);
        public Task AddFileToBatch(Guid batchId, string filename, long X_Content_Size,
            string X_MIME_Type = null);

        public bool CheckIfContainerExist(string containername);
        public bool CheckIfFileExist(string filename);
        public bool CheckIfValidBatchId(string batchId);
    }
}
