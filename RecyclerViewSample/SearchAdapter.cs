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
using Com.Bumptech.Glide;
using RecyclerViewSample.Activities;

namespace RecyclerViewSample
{
    public class SearchAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static Activity _context;
        private List<Experience> experiences;
        public static List<Experience> experiencesStatic;
        public string description;
        //declaring global variable to pass url to Tours_detail 
        public static string CurrentImageURL;
        private RootObjectSearchByWord responseSearch;
        private SearchByWordResultActivity searchByWordResultActivity;

        public SearchAdapter(List<Experience> movies, Activity context)
        {
            this.experiences = movies;
            _context = context;
        }

        public SearchAdapter(RootObjectSearchByWord responseSearch, SearchByWordResultActivity searchByWordResultActivity)
        {
            this.responseSearch = responseSearch;
            this.searchByWordResultActivity = searchByWordResultActivity;
        }

        public SearchAdapter(List<Experience> experiences, SearchByWordResultActivity searchByWordResultActivity)
        {
            this.experiences = experiences;
            this.searchByWordResultActivity = searchByWordResultActivity;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var movieViewHolder = (MovieViewHolder)holder;
            movieViewHolder.MovieNameTextView.Text = experiences[position].title;
            movieViewHolder.DirectedByTextView.Text = "$" + experiences[position].price;
            movieViewHolder.Experience_id_TV.Text = experiences[position].id.ToString();
            description = experiences[position].description;

            //change the background if there are no tours
            SearchByWordResultActivity.mainLayout.SetBackgroundResource(Resource.Drawable.Filter);
            //change the value of toursIndicator if there are no tours ENDED

            Glide.With(Application.Context)
                 .Load(experiences[position].cover_image.url)
                 .Placeholder(Resource.Drawable.empty_image)
                 .Into(movieViewHolder.Image);
            //passing url to Tours_detail using global variable
            //CurrentImageURL = movies[position].cover_image.url;
            int k = 0;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //indicator to detect from where we should load image (from search, or from default)
            Tours_detail.searchOrMovieAdapterIndicator = "SearchAdapter";
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MovieRow, parent, false);
            experiencesStatic = experiences;
            return new MovieViewHolder(layout, OnItemClick);
        }

        public override int ItemCount
        {
            get { return experiences.Count; }
        }

        void OnItemClick(int position)
        {
            var activity2 = new Intent(this.searchByWordResultActivity, typeof(Tours_detail));
            activity2.PutExtra("Title", experiences[position].title);
            activity2.PutExtra("Price", experiences[position].price);
            activity2.PutExtra("Description", experiences[position].description);
            activity2.PutExtra("Location", experiences[position].location);
            activity2.PutExtra("Duration", experiences[position].duration);
            activity2.PutExtra("Min_capacity", experiences[position].min_capacity);
            activity2.PutExtra("Max_capacity", experiences[position].max_capacity);
            activity2.PutExtra("Lat", experiences[position].lat);
            activity2.PutExtra("Lng", experiences[position].lng);
            activity2.PutExtra("ImageUrl", experiences[position].cover_image.url);
            CurrentImageURL = experiences[position].cover_image.url;
            Tours_detail.current_experience_id = experiences[position].id.ToString();
            this.searchByWordResultActivity.StartActivity(activity2);
        }
    }
}