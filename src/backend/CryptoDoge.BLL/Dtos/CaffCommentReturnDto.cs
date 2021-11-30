using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.BLL.Dtos
{
    public class CaffCommentReturnDto
    {
        public string Id { get; set; }
        public string Comment { get; set; }
        public string CaffId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Added { get; set; }
    }
}
