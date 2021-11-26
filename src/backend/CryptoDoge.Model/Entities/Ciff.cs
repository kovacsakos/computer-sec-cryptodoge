using System.Collections.Generic;

namespace CryptoDoge.Model.Entities
{
    public class Ciff
    {
        public string Id { get; set; }

        public int Duration { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public string Caption { get; set; }

        public Caff Caff { get; set; }

        public ICollection<CiffTag> Tags { get; set; }
    }
}