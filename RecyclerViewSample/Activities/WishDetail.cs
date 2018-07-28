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
    public class WishDetail : Activity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        //Database declaration
        DBRepository dbr = new DBRepository();
        private static int count_data_rows_in_users_table;
        private static bool recordExistsInWhishlist;
        string url;
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

            Tours_detail.image_url_static = null;

            recordExistsInWhishlist = false;

            string text = Intent.GetStringExtra("Title");
            string price = Intent.GetStringExtra("Price");
            url = Intent.GetStringExtra("ImageUrl");
            string description_tour = Intent.GetStringExtra("Description");
            string location = Intent.GetStringExtra("Location");
            string duration = Intent.GetStringExtra("Duration");
            string min_capacity = Intent.GetStringExtra("Min_capacity");
            string max_capacity = Intent.GetStringExtra("Max_capacity");
            bool isMyExperience = Convert.ToBoolean(Intent.GetStringExtra("isMyExperience"));
            //hiding book button for my tours
            if (isMyExperience == true)
            {
                SetContentView(RecyclerViewSample.Resource.Layout.Wish_Detail_for_my_exp);
            }
            else
            {
                SetContentView(RecyclerViewSample.Resource.Layout.Wish_Detail);
            }
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/HelveticaNeueLight")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            var unlikeBn = FindViewById<ImageButton>(Resource.Id.unlikeBn);
            Button book = FindViewById<Button>(Resource.Id.bookbutton);

            TextView title = FindViewById<TextView>(Resource.Id.Title);
            ImageView image = FindViewById<ImageView>(Resource.Id.image);
            TextView price_text = FindViewById<TextView>(Resource.Id.price);
            TextView descript = FindViewById<TextView>(Resource.Id.description);
            TextView location_val = FindViewById<TextView>(Resource.Id.location_value);
            TextView duration_val = FindViewById<TextView>(Resource.Id.duration_value);
            TextView capacity = FindViewById<TextView>(Resource.Id.capacity_value);

            Glide.With(Application.Context)
                      .Load(url)
                      .Into(image);

            var scrollView = FindViewById<com.refractored.fab.ObservableScrollView>(Resource.Id.scrollViewDetail);

            unlikeBn.SetBackgroundResource(Resource.Drawable.DeleteFromWishlist);



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

            book.Click += delegate
            {
                Tours_detail.image_url_static= url;
                var activity = new Intent(this, typeof(SelectAvialability));
                //activity.PutExtra("Title", title.Text);
                Tours_detail.titleStatic = title.Text;
                StartActivity(activity);
            };

            unlikeBn.Click += delegate
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Deleting tour");
                builder.SetMessage("Remove from wishlist?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("No", (object sender1, DialogClickEventArgs e1) =>
                { });
                builder.SetNegativeButton("Yes", (object sender1, DialogClickEventArgs e1) =>
                {
                    //here we create DB
                    dbr.CreateDB();
                    //here we create table
                    dbr.CreateWishlistTable();
                    //declaring path for RETRIEVING DATA
                    string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                    var db = new SQLiteConnection(dbPath);
                    var wishlist_table = db.Table<ORM.Wishlist>();
                    //finding the id of the tour
                    foreach (var item in wishlist_table)
                    {
                        if (item.name == title.Text)
                        {
                            dbr.RemoveWishlistRecord(item.Id);
                            Toast.MakeText(this, "Tour removed from the wishlist", ToastLength.Short).Show();
                            StartActivity(typeof(Activities.Wishlist));
                        }
                    }
                });
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
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