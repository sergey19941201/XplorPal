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
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RecyclerViewSample.ORM;
using System.IO;
using SQLite;
using StarWars.Api.Repository;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;
using Android.Views.InputMethods;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "ChangeDestination", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ChangeDestination : Activity
    {
        private static string place_of_interestTitle, place_of_interest_price;
        public static bool changedDestinationIndicator;
        public static double lat, lng;
        private static string lat_temp, lng_temp;

        Login login = new Login();
        GettingJSON gj = new GettingJSON();
        MoviesRepository repository = new MoviesRepository();
        private ProgressBar activityIndicator;
        //declaring id fo the last record
        private static int last_places_of_interest_id;
        //is places_of_interest_table empty indicator
        private static bool is_places_of_interest_table_empty;

        //Database declaration
        DBRepository dbr = new DBRepository();
        public static List<RootObjectChangeLocation> listOfCountries = new List<RootObjectChangeLocation>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ChangeLocation);
            EditText change_location = FindViewById<EditText>(Resource.Id.change_location);
            Button get_coordinates = FindViewById<Button>(Resource.Id.change_button);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);

            //creating table of places of interest
            dbr.CreatePlacesOfInterestTable();

            is_places_of_interest_table_empty = true;

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            get_coordinates.Typeface = tf;
            change_location.Typeface = tf;

            //declaring path for RETRIEVING THE IT OF THE LAST RECORD
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var places_of_interest_table = db.Table<PlacesOfInterestTable>();

            foreach (var item in places_of_interest_table)
            {
                is_places_of_interest_table_empty = false;
                last_places_of_interest_id = item.Id;
            }

            get_coordinates.Click += async delegate
            {
                if (!String.IsNullOrWhiteSpace(change_location.Text))
                {
                    //dissmissing keyboard
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(change_location.WindowToken, 0);
                    //dissmissing keyboard ENDED
                    try
                    {
                        SearchAdapter.experiencesStatic.Clear();
                    }
                    catch { }

                    activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                    get_coordinates.Visibility = Android.Views.ViewStates.Gone;
                    changedDestinationIndicator = true;
                    string city_val = change_location.Text;
                    var client = new RestClient("https://maps.googleapis.com/maps/api/geocode/json?address=");
                    var request = new RestRequest(city_val, Method.GET);
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                    var content = response.Content;

                    var responseData1 = JsonConvert.DeserializeObject<RootObjectChangeLocation>(content);

                    if (content == null || content == "" || responseData1.results.Count == 0)
                    {
                        Toast.MakeText(this, "City is empty or incorrect", ToastLength.Short).Show();
                        activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                        get_coordinates.Visibility = Android.Views.ViewStates.Visible;
                    }
                    else
                    {
                        foreach (var data in responseData1.results)
                        {
                            if (!String.IsNullOrWhiteSpace(Login.token))
                            {
                                //replacing dot insead of comma in coordinates
                                foreach (char c in data.geometry.location.lat.ToString())
                                {
                                    if (c == ',')
                                    {
                                        lat_temp += ".";
                                    }
                                    else
                                    {
                                        lat_temp += c;
                                    }
                                }
                                foreach (char c in data.geometry.location.lng.ToString())
                                {
                                    if (c == ',')
                                    {
                                        lng_temp += ".";
                                    }
                                    else
                                    {
                                        lng_temp += c;
                                    }
                                }
                                //replacing dot insead of comma in coordinates ENDED
                                await gj.VictorSologoob(Login.token, lat_temp, lng_temp);
                            }
                            else if (String.IsNullOrWhiteSpace(Login.token))
                            {
                                //replacing dot insead of comma in coordinates
                                foreach (char c in data.geometry.location.lat.ToString())
                                {
                                    if (c == ',')
                                    {
                                        lat_temp += ".";
                                    }
                                    else
                                    {
                                        lat_temp += c;
                                    }
                                }
                                foreach (char c in data.geometry.location.lng.ToString())
                                {
                                    if (c == ',')
                                    {
                                        lng_temp += ".";
                                    }
                                    else
                                    {
                                        lng_temp += c;
                                    }
                                }
                                //replacing dot insead of comma in coordinates ENDED
                                await gj.VictorSologoob(lat_temp, lng_temp);
                            }

                            lat = data.geometry.location.lat + 0.005;
                            lng = data.geometry.location.lng + 0.005;
                            /*
                            //replacing dot insead of comma in coordinates
                            foreach (char c in lat.ToString())
                            {
                                if (c == ',')
                                {
                                    lat_temp += ".";
                                }
                                else
                                {
                                    lat_temp += c;
                                }
                            }
                            foreach (char c in lng.ToString())
                            {
                                if (c == ',')
                                {
                                    lng_temp += ".";
                                }
                                else
                                {
                                    lng_temp += c;
                                }
                            }
                            //replacing dot insead of comma in coordinates ENDED

                            lat = Convert.ToDouble(lat_temp);
                            lng = Convert.ToDouble(lng_temp);*/
                            lat_temp = null;
                            lng_temp = null;

                            foreach (var place_of_interest in await getData())
                            {
                                place_of_interestTitle = place_of_interest.title;
                                place_of_interest_price = place_of_interest.price;
                            }
                            if (is_places_of_interest_table_empty == false)
                            {
                                //updating coordinats for PlacesOfInterestTable
                                dbr.updatePlacesOfInterestTable(last_places_of_interest_id, place_of_interestTitle, place_of_interest_price, data.geometry.location.lat.ToString(), data.geometry.location.lng.ToString());
                            }
                            else if (is_places_of_interest_table_empty == true)
                            {
                                dbr.InsertPlacesOfInterestRecord(place_of_interestTitle, place_of_interest_price, data.geometry.location.lat.ToString(), data.geometry.location.lng.ToString());
                            }

                            foreach (var item in places_of_interest_table)
                            {
                                last_places_of_interest_id = item.Id;
                            }

                            login.addTo_placesOfInterest();

                            Fragments.SearchFragment.searchByWordIndicator = false;
                            Tours_detail.searchOrMovieAdapterIndicator = "MovieAdapter";
                            activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                            get_coordinates.Visibility = Android.Views.ViewStates.Visible;
                            StartActivity(typeof(MainActivity));
                        }
                    }
                    // Toast.MakeText(this, content, ToastLength.Long).Show();
                    Console.WriteLine(content.ToString());
                }
            };
        }

        private async Task<System.Collections.Generic.List<StarWars.Api.Repository.Movie>> getData()
        {
            var places_of_interest = await repository.GetAllFilms(GettingJSON.content);
            return places_of_interest.results;
        }
    }
}