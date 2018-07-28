using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using StarWars.Api.Repository;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using System;
using SQLite;
using System.IO;
using Android.Graphics;
using Android.Content.PM;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "Wishlist", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Wishlist : Activity
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager layoutManager;
        private static int count_data_rows_in_wishlist_table;
        Fragments.SearchFragment searchFragment = new Fragments.SearchFragment();
        ORM.DBRepository dbr = new ORM.DBRepository();
        ORM.DBRepository dbr1 = new ORM.DBRepository();

        //LISTVIEW WITH IMAGE
        public static List<WishlistClassForRecycler> wishlistClassForRecycler = new List<WishlistClassForRecycler>();

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Wishlist);

                var repository = new MoviesRepository();
                var films = await repository.GetAllFilms(GettingJSON.content);

                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

                recyclerView.SetLayoutManager(layoutManager);
                var wishlistAdapter = new WishlistAdapter(wishlistClassForRecycler, this);
                recyclerView.SetAdapter(wishlistAdapter);
                ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
                back.Click += async delegate
                {
                    if (Fragments.SearchFragment.searchByWordIndicator == true)
                    {
                        FindViewById<ProgressBar>(Resource.Id.activityIndicatorWishlist).Visibility = ViewStates.Visible;

                        string dbPath1 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                        var db1 = new SQLiteConnection(dbPath1);
                        var placesOfInterestTable = db1.Table<ORM.PlacesOfInterestTable>();

                        try
                        {
                            foreach (var item in placesOfInterestTable)
                            {
                                dbr1.RemovePlacesOfInterestRecord(item.Id);
                            }
                        }
                        catch { }
                        try
                        {
                            MovieAdapter.moviesStatic.Clear();
                        }
                        catch { }
                        //SearchByWordResultActivity searchByWordResultActivity = new SearchByWordResultActivity();
                        //await SearchByWord(Fragments.SearchFragment.searchWord);
                        await searchFunction(Fragments.SearchFragment.searchWord);
                    }
                    else if (Fragments.SearchFragment.searchByWordIndicator == false)
                    {
                        StartActivity(typeof(MainActivity));
                    }
                };
                //declaring path for RETRIEVING DATA
                ORM.DBRepository dbr = new ORM.DBRepository();
                string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                var wishlist_table = db.Table<ORM.Wishlist>();
                //creating wishlist table
                dbr.CreateWishlistTable();
                wishlistClassForRecycler.Clear();
                count_data_rows_in_wishlist_table = 0;
                //checking if the place of interest exists // .AsEnumerable().Reverse() IS TO DISPLAY ITEMS OF RECYCLER FROM REVERSE 
                foreach (var item in wishlist_table.AsEnumerable().Reverse())
                {
                    count_data_rows_in_wishlist_table = 1;
                    wishlistClassForRecycler.Add(new WishlistClassForRecycler
                    {
                        _name = item.name,
                        _id_public = item.id_public,
                        _price = item.price,
                        picData = item.picData,
                        _description = item.description,
                        _location = item.location,
                        _duration = item.duration,
                        _min_capacity = item.min_capacity,
                        _max_capacity = item.max_capacity,
                        _lat = item.lat,
                        _isMyExperience = item.isMyExperience
                        
                    });
                }

                if (count_data_rows_in_wishlist_table > 0)
                {
                    FindViewById<TextView>(Resource.Id.messageTV).Visibility = ViewStates.Gone;
                }
                //declaring path for RETRIEVING DATA ENDED
            }
            catch { }
        }
        public override async void OnBackPressed()
        {
            if (Fragments.SearchFragment.searchByWordIndicator == true)
            {
                FindViewById<ProgressBar>(Resource.Id.activityIndicatorWishlist).Visibility = ViewStates.Visible;

                string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                var placesOfInterestTable = db.Table<ORM.PlacesOfInterestTable>();

                try
                {
                    foreach (var item in placesOfInterestTable)
                    {
                        dbr.RemovePlacesOfInterestRecord(item.Id);
                    }
                }
                catch { }
                try
                {
                    MovieAdapter.moviesStatic.Clear();
                }
                catch { }
                //SearchByWordResultActivity searchByWordResultActivity = new SearchByWordResultActivity();
                //await SearchByWord(Fragments.SearchFragment.searchWord);
                await searchFunction(Fragments.SearchFragment.searchWord);
            }
            else if (Fragments.SearchFragment.searchByWordIndicator == false)
            {
                StartActivity(typeof(MainActivity));
            }
        }

        private async Task<string> searchFunction(string search_Word)
        {
            var client = new RestClient("http://api.xplorpal.com/experience");
            var request = new RestRequest("/search", Method.POST);
            // ORM.DBRepository dbr = new ORM.DBRepository();
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var placesOfInterestTable = db.Table<ORM.PlacesOfInterestTable>();

            try
            {
                foreach (var item in placesOfInterestTable)
                {
                    dbr.RemovePlacesOfInterestRecord(item.Id);
                }
            }
            catch { }
            try
            {
                MovieAdapter.moviesStatic.Clear();
            }
            catch { }
            request.AddParameter("search", search_Word);
            Fragments.SearchFragment.searchByWordIndicator = true;
            Fragments.SearchFragment.searchWord = search_Word;
            IRestResponse response = await client.ExecuteTaskAsync(request);
            Fragments.SearchFragment.content = response.Content;

            if (!String.IsNullOrWhiteSpace(Fragments.SearchFragment.content))
            {
                StartActivity(typeof(SearchByWordResultActivity));
            }
            else
            {
                await searchFunction(Fragments.SearchFragment.searchWord);
            }

            return "";
        }
    }
}