namespace CryptoDoge.Model.Entities
{
    public class CaffComment
    {
        public string Id { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
        public Caff Caff { get; set; }
    }
}