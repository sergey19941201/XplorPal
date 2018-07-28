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
using Com.Bumptech.Glide;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.Design.Widget;
using com.refractored.fab;
using System.Timers;
using System.Threading;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using RecyclerViewSample.ORM;
using SQLite;
using Android.Content.PM;

namespace RecyclerViewSample
{
    [Activity(Label = "Tours_detail", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Tours_detail : Activity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        //indicator to detect from where we should load image (from search, or from default)
        public static string searchOrMovieAdapterIndicator;
        //Database declaration
        DBRepository dbr = new DBRepository();
        private static int count_data_rows_in_users_table;
        private static bool recordExistsInWhishlist;
        string imageUrl;
        public static string current_experience_id, titleStatic;
        private Android.App.FragmentManager fragmentManager;
        private Fragments.LoginOrRegistrationFragment loginOrRegFragment;

        public static string image_url_static;

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            throw new NotImplementedException();
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Tour_Detail);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/HelveticaNeueLight")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);

            recordExistsInWhishlist = false;

            fragmentManager = this.FragmentManager;
            loginOrRegFragment = new Fragments.LoginOrRegistrationFragment();

            string text = Intent.GetStringExtra("Title");
            string price = Intent.GetStringExtra("Price");
            imageUrl = Intent.GetStringExtra("ImageUrl");
            string description_tour = Intent.GetStringExtra("Description");
            string location = Intent.GetStringExtra("Location");
            string duration = Intent.GetStringExtra("Duration");
            string min_capacity = Intent.GetStringExtra("Min_capacity");
            string max_capacity = Intent.GetStringExtra("Max_capacity");
            string lat = Intent.GetStringExtra("Lat");
            string lng = Intent.GetStringExtra("Lng");
            TextView title = FindViewById<TextView>(Resource.Id.Title);
            ImageView image = FindViewById<ImageView>(Resource.Id.image);
            TextView price_text = FindViewById<TextView>(Resource.Id.price);
            TextView descript = FindViewById<TextView>(Resource.Id.description);
            TextView location_val = FindViewById<TextView>(Resource.Id.location_value);
            TextView duration_val = FindViewById<TextView>(Resource.Id.duration_value);
            TextView capacity = FindViewById<TextView>(Resource.Id.capacity_value);

            title.Typeface = tf;
            price_text.Typeface = tf;
            descript.Typeface = tf;
            location_val.Typeface = tf;
            duration_val.Typeface = tf;
            capacity.Typeface = tf;

            var scrollView = FindViewById<com.refractored.fab.ObservableScrollView>(Resource.Id.scrollViewDetail);
            var likeBn = FindViewById<ImageButton>(Resource.Id.likeBn);
            likeBn.SetBackgroundResource(Resource.Drawable.likeWhite);
            likeBn.Click += delegate
            {
                if (MainActivity.isLogined == true)
                {
                    Toast.MakeText(this, "Your experience added to wishlist", ToastLength.Short).Show();
                    likeBn.SetBackgroundResource(Resource.Drawable.likeRed);

                    //creating wishlist table
                    dbr.CreateWishlistTable();

                    //declaring path for RETRIEVING DATA
                    string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                    var db = new SQLiteConnection(dbPath);
                    var wishlist_table = db.Table<Wishlist>();
                    //declaring path for RETRIEVING DATA ENDED
                    count_data_rows_in_users_table = 0;
                    //checking if the place of interest exists
                    foreach (var item in wishlist_table)
                    {
                        count_data_rows_in_users_table = 1;
                        if (item.name == title.Text && item.price == price)
                        {
                            recordExistsInWhishlist = true;
                            break;
                        }
                        else if (item.name != title.Text && item.price != price)
                        {
                            recordExistsInWhishlist = false;
                        }
                    }
                    //if table is empty we insert a record
                    if (count_data_rows_in_users_table == 0)
                    {
                        /*inserting 
                        IMAGE AS memStream.ToArray() 
                        and other fields to database*/
                        dbr.InsertWhishlistRecord(
                            title.Text, Tours_detail.current_experience_id, price, imageUrl, description_tour, location, duration, min_capacity, max_capacity, lat, lng, false);
                    }

                    if (recordExistsInWhishlist == false && count_data_rows_in_users_table != 0)
                    {
                        /*inserting 
                        IMAGE AS memStream.ToArray() 
                        and other fields to database*/
                        dbr.InsertWhishlistRecord(
                            title.Text, Tours_detail.current_experience_id, price, imageUrl, description_tour, location, duration, min_capacity, max_capacity, lat, lng, false);
                    }
                }
                else
                {
                    loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                }
            };

            if (searchOrMovieAdapterIndicator == "MovieAdapter")
            {
                Glide.With(Application.Context)
                .Load(MovieAdapter.CurrentImageURL)//url)
                .Into(image);
            }
            else if (searchOrMovieAdapterIndicator == "SearchAdapter")
            {
                Glide.With(Application.Context)
                .Load(SearchAdapter.CurrentImageURL)//url)
                .Into(image);
            }


            title.Text = text;
            price_text.Text = "$" + price;
            descript.Text = description_tour;
            location_val.Text = location;
            duration_val.Text = duration + " hours";
            capacity.Text = min_capacity + " - " + max_capacity;

            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            back.Click += delegate
            {
                OnBackPressed();
            };
            Button book = FindViewById<Button>(Resource.Id.bookbutton);
            book.Click += delegate
            {
                Tours_detail.image_url_static = imageUrl;
                StartActivity(typeof(MainActivity));
            };
            
            
            book.Typeface = tf;
            book.Click += delegate
            {
                    var activity = new Intent(this, typeof(SelectAvialability));
                //activity.PutExtra("Title", title.Text);
                titleStatic = title.Text;
                    StartActivity(activity);
            };
        }

        void IScrollDirectorListener.OnScrollDown()
        {
            throw new NotImplementedException();
        }

        void IScrollDirectorListener.OnScrollUp()
        {
            throw new NotImplementedException();
        }
    }
}