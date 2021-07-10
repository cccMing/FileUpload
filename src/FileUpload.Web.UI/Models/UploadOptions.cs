using System.Collections.Generic;

namespace FileUpload.Models
{
    public class UploadOptions
    {
        public UploadSettings Default { get; set; }
        public Dictionary<string, UploadSettings> Profiles { get; } = new Dictionary<string, UploadSettings>();
    }
}
