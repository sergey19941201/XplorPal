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
    [Table("CountryTable")]
    public class CountryTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int Id { get; set; }
        public int country_id { get; set; }
        public string country_name { get; set; }
    }
}