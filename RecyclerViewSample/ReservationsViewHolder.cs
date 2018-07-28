using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace RecyclerViewSample
{
    public class ReservationsViewHolder : RecyclerView.ViewHolder
    {
        public TextView DateTV { get; set; }
        public TextView DayTV { get; set; }
        public TextView SelectTV { get; set; }


        public ReservationsViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            DateTV = itemView.FindViewById<TextView>(Resource.Id.DateTV);
            DayTV = itemView.FindViewById<TextView>(Resource.Id.DayTV);
            SelectTV = itemView.FindViewById<TextView>(Resource.Id.selectTV);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(itemView.Context.Assets, path);
            DateTV.Typeface = tf; 
            DayTV.Typeface = tf; 
            SelectTV.Typeface = tf; 
            itemView.Click += (s, e) => listener(Position);
        }
    }
}