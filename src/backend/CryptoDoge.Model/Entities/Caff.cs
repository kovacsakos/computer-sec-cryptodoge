using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Model.Entities
{
    public class Caff
    {
        public string Id { get; set; }

        public int NumberOfAnimations { get; set; }

        public int CreationYear { get; set; }

        public int CreationMonth { get; set; }

        public int CreationDay { get; set; }

        public int CreationHour { get; set; }

        public int CreationMinute { get; set; }

        public string Creator { get; set; }

        public User UploadedBy { get; set; }

        public ICollection<Ciff> Ciffs { get; set; }
        public ICollection<CaffComment> Comments { get; set; }

        public DateTime CreationDate => new(CreationYear, CreationMonth, CreationDay, CreationHour, CreationMinute, 0);
    }
}
