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
using Environment = System.Environment;
using SQLite;
using RecyclerViewSample.ORM;
using StarWars.Api.Repository;
using Android.Graphics;
using Com.Bumptech.Glide;
using Android.Content.PM;

namespace RecyclerViewSample
{
    [Activity(Label = "Profile", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Profile : Activity
    {
        ORM.DBRepository dbr = new ORM.DBRepository();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Profile);
            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            //Here is buttons for actions in Profile activity
            Button logoutButton = FindViewById<Button>(Resource.Id.log_out);
            Button changepasswordButton = FindViewById<Button>(Resource.Id.change_password);
            Button editprofileButton = FindViewById<Button>(Resource.Id.edit_profile);
            TextView log_out_TV = FindViewById<TextView>(Resource.Id.log_out_TV);
            TextView nameTV = FindViewById<TextView>(Resource.Id.nameTV);
            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            logoutButton.Typeface = tf;
            changepasswordButton.Typeface = tf;
            editprofileButton.Typeface = tf;
            log_out_TV.Typeface = tf;
            nameTV.Typeface = tf;
            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<ORM.UsersDataTable>();
            var placesOfInterestTable = db.Table<ORM.PlacesOfInterestTable>();
            var whishlistTable = db.Table<ORM.Wishlist>();
            //var countryTable = db.Table<ORM.CountryTable>();
            //declaring path for RETRIEVING DATA ENDED
            ImageView image = FindViewById<ImageView>(Resource.Id.circleImageView1);

            //assigning name from the database to textfield for name
            foreach (var item in user_table)
            {
                try
                {
                    nameTV.Text = item.name;
                    Login.avatar = item.avatar;
                    Glide.With(Application.Context)
                      .Load(item.avatar)
                      .Into(image);
                }
                catch { }
            }

            changepasswordButton.Click += delegate
            {
                StartActivity(typeof(ForgetYourPasswordActivity));
            };

            back.Click += delegate
            {
                OnBackPressed();
            };
            FindViewById<Button>(Resource.Id.edit_profile).Click += delegate
            {
                StartActivity(typeof(RecyclerViewSampl.Activities.EditProfileActivity));
            };
            FindViewById<Button>(Resource.Id.log_out).Click += delegate
            {
                log_out_TV.Visibility = ViewStates.Visible;
                logoutButton.Visibility = ViewStates.Gone;

                //clearing lists
                Activities.Wishlist.wishlistClassForRecycler.Clear();
                Activities.MyBookings.myExpListClassForRecycler.Clear();

                if (MovieAdapter.moviesStatic != null)
                {
                    MovieAdapter.moviesStatic.Clear();
                }
                //clearing all existing tables. WE NEED FOR EACH TABLE EACH TRY-CATCH BECAUSE WE MUST NOT MISS ANY TABLE
                try
                {
                    dbr.DropUsersTable();
                    /*foreach (var item in user_table)
                    {
                        dbr.RemoveUsersData(item.Id);
                    }*/
                }
                catch { }
                try
                {
                    dbr.DropPlacesOfInterestTable();
                    /*foreach (var item in placesOfInterestTable)
                    {
                        dbr.RemovePlacesOfInterestRecord(item.Id);
                    }*/
                }
                catch { }
                try
                {
                    dbr.DropWishlistTable();
                    /*foreach (var item in whishlistTable)
                    {
                        dbr.RemoveWishlistRecord(item.Id);
                    }*/
                }
                catch { }
                /*try
                {
                    dbr.DropCountryTable();
                    /*foreach (var item in countryTable)
                    {
                        dbr.RemoveCountryRecord(item.Id);
                    }
                }
                catch { }*/
                //clearing all existing tables ENDED

                //cleaning variables
                Tours_detail.titleStatic = "";
                RecyclerViewSample.Login.token = "";
                RecyclerViewSample.Login.name = "";
                RecyclerViewSample.Login.email_ = "";
                RecyclerViewSample.Login.user_id = 0;
                RecyclerViewSample.Login.birth_date = "";
                RecyclerViewSample.Login.gender = "";
                RecyclerViewSample.Login.phone_num = "";
                RecyclerViewSample.Login.interests = "";
                RecyclerViewSample.Login.password = "";
                RecyclerViewSample.Login.user_country_id = 0;
                RecyclerViewSample.Login.avatar = null;
                //cleaning variables ENDED
                StartActivity(typeof(MainActivity));
            };
        }
    }
}