using batch_webapi.Data.ViewModels;
using System;

namespace batch_webapi.Data.Models
{
    public class Files
    {
        public int Id { get; set; }
        public Guid BatchId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string MimeType { get; set; }
        public string Hash { get; set; }        
        public string Links { get; set; }
    }
}
