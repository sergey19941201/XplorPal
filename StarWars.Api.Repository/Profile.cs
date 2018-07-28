using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Api.Repository
{
    public class Profile
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string video_url { get; set; }
        public string alien_video_id { get; set; }
        public string video_source { get; set; }
        public string img_path { get; set; }
        public string img_file { get; set; }
        public string bio { get; set; }
        public object avatar { get; set; }
        public object permissions { get; set; }
        public object last_login { get; set; }
        public object first_name { get; set; }
        public object last_name { get; set; }
        public string user_country_id { get; set; }
        public string status { get; set; }
        public object approved_by { get; set; }
        public object phone_num { get; set; }
        public object id_card_image_path { get; set; }
        public object id_card_image_file { get; set; }
        public object gender { get; set; }
        public object birth_date { get; set; }
        public string api_token { get; set; }
    }
}