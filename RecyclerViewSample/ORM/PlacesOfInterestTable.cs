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
using SQLite;

namespace RecyclerViewSample.ORM
{
    [Table("PlacesOfInterestTable")]
    public class PlacesOfInterestTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int Id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
    }
}