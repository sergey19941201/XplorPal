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
using Android.Support.V7.Widget;
using RecyclerViewSample.Activities;
using Com.Bumptech.Glide;

namespace RecyclerViewSample
{
    public class MyExperiencesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private Activity _context;
        private readonly List<MyExperiencesClassForRecycler> movies;
        public string description;
        public MyExperiencesAdapter(List<MyExperiencesClassForRecycler> movies, Activity context)
        {
            this.movies = movies;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var movieViewHolder = (MovieViewHolder)holder;
            movieViewHolder.MovieNameTextView.Text = movies[position]._name;
            movieViewHolder.DirectedByTextView.Text = "$" + movies[position]._price;
            movieViewHolder.Experience_id_TV.Text= movies[position]._id.ToString();
            description = movies[position]._description;

            /*Bitmap bmp = BitmapFactory.DecodeByteArray(movies[position].picData, 0, movies[position].picData.Length);
            movieViewHolder.Image.SetImageBitmap(bmp);

            Stream stream = new MemoryStream(movies[position]._image);
            Bitmap bmp = BitmapFactory.DecodeByteArray(movies[position]._image, 0, movies[position]._image.Le);
            movieViewHolder.Image = movies[position]._image;
            //change the background if there are no tours
            MainActivity.mainLayout.SetBackgroundResource(Resource.Drawable.Filter);
            //change the value of toursIndicator if there are no tours ENDED
            */
            Glide.With(Application.Context)
                  .Load(movies[position]._image_url)//cover_image.url)
                  .Placeholder(Resource.Drawable.empty_image)
                  .Into(movieViewHolder.Image);
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
            var activity2 = new Intent(_context, typeof(MyExperiencesDetailActivity));
            activity2.PutExtra("Id", movies[position]._id);
            activity2.PutExtra("Title", movies[position]._name);
            activity2.PutExtra("Price", movies[position]._price);
            activity2.PutExtra("Description", movies[position]._description);
            activity2.PutExtra("Location", movies[position]._location);
            activity2.PutExtra("Duration", movies[position]._duration);
            activity2.PutExtra("Min_capacity", movies[position]._min_capacity);
            activity2.PutExtra("Max_capacity", movies[position]._max_capacity);
            activity2.PutExtra("Lat", movies[position]._lat);
            activity2.PutExtra("Lng", movies[position]._lng);
            activity2.PutExtra("ImageUrl", movies[position]._image_url);
            activity2.PutExtra("IsPublished", movies[position]._status);
            Tours_detail.current_experience_id = movies[position]._id.ToString();
            _context.StartActivity(activity2);
        }
    }
}