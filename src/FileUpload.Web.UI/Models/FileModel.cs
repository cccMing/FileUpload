﻿using Neptuo;
using System;
using System.IO;

namespace FileUpload.Models
{
    public class FileModel
    {
        public string Name { get; }
        public long Size { get; }
        public DateTime ModifiedAt { get; set; }

        public FileModel(string name, long size, DateTime modifiedAt)
        {
            Ensure.NotNull(name, "name");
            Ensure.PositiveOrZero(size, "size");
            Name = name;
            Size = size;
            ModifiedAt = modifiedAt;
        }

        public FileModel(FileInfo fileInfo)
            : this(fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime)
        { }
    }
}
