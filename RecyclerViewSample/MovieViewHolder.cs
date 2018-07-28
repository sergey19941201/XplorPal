using System;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace RecyclerViewSample
{
    public class MovieViewHolder : RecyclerView.ViewHolder
    {
        public TextView MovieNameTextView { get; set; }
        public TextView DirectedByTextView { get; set; }
        public TextView Experience_id_TV { get; set; }
    public TextView Filter_text { get; set; }
        public ImageView Image { get; set; }

        public MovieViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MovieNameTextView = itemView.FindViewById<TextView>(Resource.Id.movieNameText);
            DirectedByTextView = itemView.FindViewById<TextView>(Resource.Id.directedByText);
            Experience_id_TV = itemView.FindViewById<TextView>(Resource.Id.experience_id_TV);
            Image = itemView.FindViewById<ImageView>(Resource.Id.image);
            itemView.Click += (s, e) => listener(Position);
        }
    }
}