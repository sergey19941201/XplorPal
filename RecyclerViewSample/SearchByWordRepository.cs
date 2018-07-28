using StarWars.Api.Repository;
using System.Collections.Generic;

namespace RecyclerViewSample
{
    public class Experience
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string owner_id { get; set; }
        public string price { get; set; }
        public string min_capacity { get; set; }
        public string max_capacity { get; set; }
        public string location { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string price_rate { get; set; }
        public string duration { get; set; }
        public string duration_type { get; set; }
        public object video_url { get; set; }
        public string alien_video_id { get; set; }
        public string video_source { get; set; }
        public string has_cover { get; set; }
        public string status { get; set; }
        public string publish_date { get; set; }
        public string meet_place_address { get; set; }
        public string meet_place_city { get; set; }
        public string meet_place_country { get; set; }
        public string nearby_landmarks { get; set; }
        public string must_have { get; set; }
        public string instructions { get; set; }
        public string approved { get; set; }
        public string approved_by { get; set; }
        public object approve_date { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public CoverImage cover_image { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class SortBy
    {
        public string __invalid_name__3 { get; set; }
        public string __invalid_name__4 { get; set; }
        public string __invalid_name__5 { get; set; }
        public string __invalid_name__6 { get; set; }
    }

    public class RootObjectSearchByWord
    {
        public List<Experience> experiences { get; set; }
        public List<Category> categories { get; set; }
        public SortBy sort_by { get; set; }
        public string search { get; set; }
    }
}