using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Movie = StarWars.Api.Repository.Movie;

namespace RecyclerViewSample
{
    public class MovieAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private Activity _context;
        public List<Movie> movies;
        public static List<Movie> moviesStatic;
        public string description, experience_id;
        //declaring global variable to pass url to Tours_detail 
        public static string CurrentImageURL;

        public MovieAdapter(List<Movie> movies, Activity context)
        {
            this.movies = movies;
            _context = context;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            var movieViewHolder = (MovieViewHolder)holder;
            movieViewHolder.MovieNameTextView.Text = movies[position].title;
            movieViewHolder.DirectedByTextView.Text = "$" + movies[position].price;
            movieViewHolder.Experience_id_TV.Text = movies[position].id.ToString();
            description = movies[position].description;
            experience_id = movies[position].id.ToString();

            //change the background if there are no tours
            MainActivity.mainLayout.SetBackgroundResource(Resource.Drawable.Filter);
            //change the value of toursIndicator if there are no tours ENDED

            Glide.With(Application.Context)
                 .Load(movies[position].cover_image.url)
                 .Placeholder(Resource.Drawable.empty_image)
                 .Into(movieViewHolder.Image);
            //passing url to Tours_detail using global variable
            //CurrentImageURL = movies[position].cover_image.url;
            int k = 0;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //indicator to detect from where we should load image (from search, or from default)
            Tours_detail.searchOrMovieAdapterIndicator = "MovieAdapter";
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MovieRow, parent, false);
            moviesStatic = movies;
            return new MovieViewHolder(layout, OnItemClick);
        }

        public override int ItemCount
        {
            get { return movies.Count; }
        }

        void OnItemClick(int position)
        {
            var activity2 = new Intent(_context, typeof(Tours_detail));
            activity2.PutExtra("Title", movies[position].title);
            activity2.PutExtra("Price", movies[position].price);
            activity2.PutExtra("Description", movies[position].description);
            activity2.PutExtra("Location", movies[position].location);
            activity2.PutExtra("Duration", movies[position].duration);
            activity2.PutExtra("Min_capacity", movies[position].min_capacity);
            activity2.PutExtra("Max_capacity", movies[position].max_capacity);
            activity2.PutExtra("Lat", movies[position].lat);
            activity2.PutExtra("Lng", movies[position].lng);
            activity2.PutExtra("ImageUrl", movies[position].cover_image.url);
            CurrentImageURL = movies[position].cover_image.url;
            Tours_detail.current_experience_id = movies[position].id.ToString();
            _context.StartActivity(activity2);
        }
    }
}