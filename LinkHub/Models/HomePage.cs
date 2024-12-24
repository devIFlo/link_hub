namespace LinkHub.Models
{
    public class HomePage
    {
        public int Id { get; set; }

        public int LinkId { get; set; }
        public Link? Link { get; set; }

        public int PageId { get; set; }
        public Page? Page { get; set; }
    }
}
