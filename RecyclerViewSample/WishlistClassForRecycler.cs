using Android.Graphics;

namespace RecyclerViewSample
{
    public class WishlistClassForRecycler
    {
        public string _id { get; set; }
        public string _id_public { get; set; }
        public string _name { get; set; }
        public string _price { get; set; }
        public string picData { get; set; }
        public string _description { get; set; }
        public string _location { get; set; }
        public string _duration { get; set; }
        public string _min_capacity { get; set; }
        public string _max_capacity { get; set; }
        public string _lat { get; set; }
        public string _lng { get; set; }
        public bool _isMyExperience { get; set; }
    }
}