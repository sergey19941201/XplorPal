using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RecyclerViewSample
{
    public class CoverImageExperience
    {
        public int id { get; set; }
        public string img_path { get; set; }
        public string img_file { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string destination_id { get; set; }
        public string is_cover { get; set; }
        public string url { get; set; }
    }

    public class MyExperiencesRepository
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
        public object alien_video_id { get; set; }
        public object video_source { get; set; }
        public string has_cover { get; set; }
        public string status { get; set; }
        public object publish_date { get; set; }
        public string meet_place_address { get; set; }
        public string meet_place_city { get; set; }
        public string meet_place_country { get; set; }
        public object nearby_landmarks { get; set; }
        public object must_have { get; set; }
        public object instructions { get; set; }
        public object approved { get; set; }
        public object approved_by { get; set; }
        public object approve_date { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public List<object> categories { get; set; }
        public CoverImageExperience cover_image { get; set; }
    }

    public class RootObjectMyExperiences
    {
        public List<MyExperiencesRepository> experiences { get; set; }
    }
}