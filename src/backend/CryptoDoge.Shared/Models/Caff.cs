using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Shared.Models
{
    public class Caff
    {
        [DataMember(Name = "num_anim")]
        public int Num_anim { get; set; }

        [DataMember(Name = "creationYear")]
        public int CreationYear { get; set; }

        [DataMember(Name = "creationMonth")]
        public int CreationMonth { get; set; }

        [DataMember(Name = "creationDay")]
        public int CreationDay { get; set; }

        [DataMember(Name = "creationHour")]
        public int CreationHour { get; set; }

        [DataMember(Name = "creationMinute")]
        public int CreationMinute { get; set; }

        [DataMember(Name = "creator")]
        public string Creator { get; set; }

        [DataMember(Name = "ciffs")]
        public List<Ciff> Ciffs { get; set; }

        public DateTime CreationDate
        {
            get
            {
                return new DateTime(CreationYear, CreationMonth, CreationDay, CreationHour, CreationMinute, 0);
            }
        }
    }
}
