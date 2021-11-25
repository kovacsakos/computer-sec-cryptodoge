using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CryptoDoge.ParserService
{
    public class ParsedCiff
    {
        public string Id { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "height")]
        public int Height { get; set; }

        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "caption")]
        public string Caption { get; set; }

        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "pixels")]
        public List<List<List<int>>> Pixels { get; set; }

        public ParsedCiff()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

}