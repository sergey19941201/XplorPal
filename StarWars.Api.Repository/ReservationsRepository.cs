using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Api.Repository
{
    public class Destination
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
        public object lat { get; set; }
        public object lng { get; set; }
    }

    public class Reservation
    {
        public int id { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string people_qty { get; set; }
        public string phone { get; set; }
        public string date { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string message { get; set; }
        public string confirmation_number { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string destination_id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string css_class { get; set; }
        public string res_date { get; set; }
        public string user_id { get; set; }
        public string is_private { get; set; }
        public string is_set { get; set; }
        public string provider_id { get; set; }
        public object allDay { get; set; }
        public object title { get; set; }
        public string parent_repeater { get; set; }
        public string is_repeated { get; set; }
        public object parent_reservation { get; set; }
        public object is_reviewed { get; set; }
        public int spots_available { get; set; }
        public Destination destination { get; set; }
    }

    public class RootObjectReservations
    {
        public List<Reservation> reservations { get; set; }
    }
}
