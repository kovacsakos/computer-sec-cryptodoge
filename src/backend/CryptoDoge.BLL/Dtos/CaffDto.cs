﻿using CryptoDoge.Model.Entities;
using System;
using System.Collections.Generic;

namespace CryptoDoge.BLL.Dtos
{
    public class CaffDto
    {
        public string Id { get; set; }

        public int NumberOfAnimations { get; set; }

        public int CreationYear { get; set; }

        public int CreationMonth { get; set; }

        public int CreationDay { get; set; }

        public int CreationHour { get; set; }

        public int CreationMinute { get; set; }

        public string Creator { get; set; }

        public string UploadedBy { get; set; }
        public string UploadedByName { get; set; }

        public ICollection<string> Captions { get; set; }

        public ICollection<string> Tags { get; set; }

        public ICollection<CiffDto> Ciffs { get; set; }

        public ICollection<CaffCommentReturnDto> Comments { get; set; }

        public DateTime CreationDate => new(CreationYear, CreationMonth, CreationDay, CreationHour, CreationMinute, 0);
    }
}
