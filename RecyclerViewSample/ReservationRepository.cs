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
    public class Data
    {
        public string email { get; set; }
        public string name { get; set; }
        public string parent_reservation { get; set; }
        public string people_qty { get; set; }
        public int is_private { get; set; }
        public int user_id { get; set; }
        public string provider_id { get; set; }
        public int status { get; set; }
        public string destination_id { get; set; }
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public int id { get; set; }
    }

    public class RootObjectReservationRepo
    {
        public Data data { get; set; }
    }
}