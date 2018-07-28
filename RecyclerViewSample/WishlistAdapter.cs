using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Movie = StarWars.Api.Repository.Movie;
using System.IO;

namespace RecyclerViewSample
{
    public class WishlistAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private Activity _context;
        private readonly List<WishlistClassForRecycler> movies;
        public static string description;
        //declaring global variable to pass url to Tours_detail 
        public static string CurrentImageURL;
        public WishlistAdapter(List<WishlistClassForRecycler> movies, Activity context)
        {
            this.movies = movies;
            _context = context;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                var movieViewHolder = (MovieViewHolder)holder;
                movieViewHolder.MovieNameTextView.Text = movies[position]._name;
                movieViewHolder.DirectedByTextView.Text = "$" + movies[position]._price;
                movieViewHolder.Experience_id_TV.Text = movies[position]._id_public.ToString();
                description = movies[position]._description;
                Glide.With(Application.Context)
                  .Load(movies[position].picData)
                  .Into(movieViewHolder.Image);
            }
            catch { }
            /*Stream stream = new MemoryStream(movies[position]._image);
            Bitmap bmp = BitmapFactory.DecodeByteArray(movies[position]._image, 0, movies[position]._image.Le);
            movieViewHolder.Image = movies[position]._image;
            //change the background if there are no tours
            MainActivity.mainLayout.SetBackgroundResource(Resource.Drawable.Filter);
            //change the value of toursIndicator if there are no tours ENDED

            /*Glide.With(Application.Context)
                  .Load(movies[position]._image)//cover_image.url)
                  .Placeholder(Resource.Drawable.Icon)
                  .Into(movieViewHolder.Image);*/
            //passing url to Tours_detail using global variable
            //CurrentImageURL = movies[position]._image;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MovieRow, parent, false);

            return new MovieViewHolder(layout, OnItemClick);

        }

        public override int ItemCount
        {
            get { return movies.Count; }
        }

        void OnItemClick(int position)
        {
            var activity2 = new Intent(_context, typeof(WishDetail));
            activity2.PutExtra("Title", movies[position]._name);
            activity2.PutExtra("Price", movies[position]._price);
            activity2.PutExtra("Description", movies[position]._description);
            activity2.PutExtra("Location", movies[position]._location);
            activity2.PutExtra("Duration", movies[position]._duration);
            activity2.PutExtra("Min_capacity", movies[position]._min_capacity);
            activity2.PutExtra("Max_capacity", movies[position]._max_capacity);
            activity2.PutExtra("ImageUrl", movies[position].picData);
            activity2.PutExtra("isMyExperience", movies[position]._isMyExperience.ToString());
            Tours_detail.current_experience_id = movies[position]._id_public.ToString();
            _context.StartActivity(activity2);
        }
    }
}