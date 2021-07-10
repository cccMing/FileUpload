﻿using System.Collections.Generic;

namespace FileUpload.Models
{
    public class UploadSettings
    {
        public string StoragePath { get; set; }
        public long? MaxStorageLength { get; set; }
        public long MaxLength { get; set; }
        public List<string> SupportedExtensions { get; } = new List<string>();
        public bool IsOverrideEnabled { get; set; }
        public bool IsDownloadEnabled { get; set; }
        public bool IsBrowserEnabled { get; set; }
        public bool IsDeleteEnabled { get; set; }
        public bool IsListed { get; set; }
        public string BackupTemplate { get; set; }
        public List<string> Roles { get; set; }

        public string DateTimeFormat { get; set; }

        public bool IsSupportedExtension(string extension)
        {
            if (SupportedExtensions == null || SupportedExtensions.Count == 0)
                return true;

            return SupportedExtensions.Contains(extension);
        }
    }
}
