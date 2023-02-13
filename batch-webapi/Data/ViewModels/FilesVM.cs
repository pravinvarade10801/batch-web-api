namespace batch_webapi.Data.ViewModels
{
    public class FilesVM
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string MimeType { get; set; }
        public string Hash { get; set; }
        public AttributesVM Attributes { get; set; }
    }
}
