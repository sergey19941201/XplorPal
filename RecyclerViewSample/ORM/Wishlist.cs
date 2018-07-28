using SQLite;

namespace RecyclerViewSample.ORM
{
    [Table("Wishlist")]
    public class Wishlist
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int Id { get; set; }
        public string id_public { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string picData { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string duration { get; set; }
        public string min_capacity { get; set; }
        public string max_capacity { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public bool isMyExperience { get; set; }
    }
}