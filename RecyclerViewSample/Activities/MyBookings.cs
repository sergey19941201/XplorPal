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
using StarWars.Api.Repository;
using Newtonsoft.Json;
using Android.Content.PM;
using Android.Graphics;
using SQLite;
using System.Threading.Tasks;
using RestSharp;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "MyBookings", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MyBookings : Activity
    {
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager layoutManager;
        //private static int count_data_rows_in_myExperiences_table;
        GetMyExperiences getMyExperiences = new GetMyExperiences();
        ORM.DBRepository dbr = new ORM.DBRepository();
        ORM.DBRepository dbr1 = new ORM.DBRepository();

        //LIST WITH IMAGE
        public static List<MyExperiencesClassForRecycler> myExpListClassForRecycler = new List<MyExperiencesClassForRecycler>();
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyBookings);

            //clearing list
            myExpListClassForRecycler.Clear();

            ProgressBar activityIndicatorMyBookings = FindViewById<ProgressBar>(Resource.Id.activityIndicatorMyBookings);
            TextView messageTV = FindViewById<TextView>(Resource.Id.messageTV);
            TextView MyExp_title_TV = FindViewById<TextView>(Resource.Id.MyExp_title_TV);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            messageTV.Typeface = tf;
            MyExp_title_TV.Typeface = tf;

            await getMyExperiences.GettingMyExperiences(Login.token);
            try
            {
                var responseData = JsonConvert.DeserializeObject<RootObjectMyExperiences>(GetMyExperiences.content);
                //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE 
                for (int i = responseData.experiences.Count - 1; i >= 0; i--)
                {
                    if (responseData.experiences[i].cover_image == null)
                    {
                        myExpListClassForRecycler.Add(
                            new MyExperiencesClassForRecycler
                            {
                                _id = responseData.experiences[i].id.ToString(),
                                _name = responseData.experiences[i].title,
                                _price = responseData.experiences[i].price,
                                _description = responseData.experiences[i].description,
                                _location = responseData.experiences[i].location,
                                _duration = responseData.experiences[i].duration,
                                _min_capacity = responseData.experiences[i].min_capacity,
                                _max_capacity = responseData.experiences[i].max_capacity,
                                _lat = responseData.experiences[i].lat,
                                _lng = responseData.experiences[i].lng,
                                _status = responseData.experiences[i].status
                            });
                    }
                    else if (responseData.experiences[i].cover_image.url != null)
                    {
                        myExpListClassForRecycler.Add(
                            new MyExperiencesClassForRecycler
                            {
                                _id = responseData.experiences[i].id.ToString(),
                                _name = responseData.experiences[i].title,
                                _price = responseData.experiences[i].price,
                                _description = responseData.experiences[i].description,
                                _location = responseData.experiences[i].location,
                                _duration = responseData.experiences[i].duration,
                                _min_capacity = responseData.experiences[i].min_capacity,
                                _max_capacity = responseData.experiences[i].max_capacity,
                                _lat = responseData.experiences[i].lat,
                                _lng = responseData.experiences[i].lng,
                                _image_url = responseData.experiences[i].cover_image.url,
                                _status = responseData.experiences[i].status
                            });
                    }
                }
                //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE ENDED

                recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerView.SetLayoutManager(layoutManager);

                var myExpListClassForRecyclerAdapter = new MyExperiencesAdapter(myExpListClassForRecycler, this);
                recyclerView.SetAdapter(myExpListClassForRecyclerAdapter);

                ImageButton back = FindViewById<ImageButton>(Resource.Id.back);

                back.Click += async delegate
                {
                    if (Fragments.SearchFragment.searchByWordIndicator == true)
                    {
                        FindViewById<ProgressBar>(Resource.Id.activityIndicatorMyBookings).Visibility = ViewStates.Visible;

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
                        //Wishlist wishlist = new Wishlist();
                        //await SearchByWord(Fragments.SearchFragment.searchWord);
                        await searchFunction(Fragments.SearchFragment.searchWord);
                    }
                    else if (Fragments.SearchFragment.searchByWordIndicator == false)
                    {
                        StartActivity(typeof(MainActivity));
                    }
                };
                activityIndicatorMyBookings.Visibility = ViewStates.Gone;

                if (myExpListClassForRecycler.Count != 0)
                {
                    messageTV.Visibility = ViewStates.Gone;
                }
                else
                {
                    messageTV.Text = "You have no experiences now";
                }
            }
            catch { }

        }

        /*public async Task<string> SearchByWord(string word)
        {
            var client = new RestClient("http://api.xplorpal.com/experience");
            var request = new RestRequest("/search", Method.POST);
            request.AddParameter("search", Fragments.SearchFragment.searchWord);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            Fragments.SearchFragment.content = response.Content;
            Activities.ChangeDestination.changedDestinationIndicator = false;
            StartActivity(typeof(SearchByWordResultActivity));
            return "";
        }*/

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

        public override async void OnBackPressed()
        {
            if (Fragments.SearchFragment.searchByWordIndicator == true)
            {
                FindViewById<ProgressBar>(Resource.Id.activityIndicatorMyBookings).Visibility = ViewStates.Visible;

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
                /*Wishlist wishlist = new Wishlist();
                await wishlist.SearchByWord(Fragments.SearchFragment.searchWord);*/
                //await SearchByWord(Fragments.SearchFragment.searchWord);
                await searchFunction(Fragments.SearchFragment.searchWord);
            }
            else if (Fragments.SearchFragment.searchByWordIndicator == false)
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
    public class MyExperiencesClassForRecycler
    {
        public string _id { get; set; }
        public string _name { get; set; }
        public string _price { get; set; }
        public string _description { get; set; }
        public string _location { get; set; }
        public string _duration { get; set; }
        public string _min_capacity { get; set; }
        public string _max_capacity { get; set; }
        public string _lat { get; set; }
        public string _lng { get; set; }
        public string _image_url { get; set; }
        public string _status { get; set; }
    }
}