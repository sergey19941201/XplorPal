using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Plugin.Geolocator;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Net;
using StarWars.Api.Repository;
using RecyclerViewSample.ORM;
using SQLite;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using Android.Content.PM;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "XplorPal", ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true)]
    public class NEWstartActivity : Activity//, ILocationListener
    {
        private ProgressBar activityIndicator;
        private TextView textView1;
        private Button findYourLocBn;

        public static string lat, lon;
        private static bool isOnline;
        GettingJSON gettingJSON = new GettingJSON();
        DBRepository dbr = new DBRepository();

        //My method to turn the GPS on
        private void turnGPSon()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle("Location services are disabled.");
            builder.SetMessage("Turn them on?");
            builder.SetCancelable(true);
            builder.SetPositiveButton("No", (object sender1, DialogClickEventArgs e1) =>
            { });
            builder.SetNegativeButton("Yes", (object sender1, DialogClickEventArgs e1) =>
            {
                StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            });
            Android.App.AlertDialog dialog = builder.Create();
            dialog.Show();
        }
        //My method to turn the GPS on ENDED

        //method to detect if GPS is enabled
        public bool IsGeolocationEnabled()
        {
            return CrossGeolocator.Current.IsGeolocationEnabled;
        }
        //method to detect if GPS is enabled ENDED

        //checking internet connection
        private void checkInternetConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            isOnline = (activeConnection != null) && activeConnection.IsConnected;
        }
        //checking internet connection ended

        public async Task<string> Geolocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            //var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
            var position = await locator.GetPositionAsync();


            if (position == null)
                return "";
            lat = position.Latitude.ToString();
            lon = position.Longitude.ToString();
            /*
                        //static coordinates of Washington:
                        lat = "38.8951100"; lon = "-77.0363700";
                        //Vinnitsya coords:
                        lat = "49.2316534"; lon = "28.4986605";*/

            //replacing dot instead of comma in coordinates
            if (lat.Contains(","))
            {
                lat=lat.Replace(',', '.');
            }
            if (lon.Contains(","))
            {
                lon=lon.Replace(',', '.');
            }

            if (lat == null)
            {
            }
            else
            {

                //here we create DB
                dbr.CreateDB();
                textView1.Text = "Getting user`s info";
                await gettingJSON.VictorSologoob(lat, lon);

                StartActivity(typeof(RecyclerViewSample.MainActivity));
                //StartActivity(typeof(Login));
            }

            return "";
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.NEWstartScreen);

            findYourLocBn = FindViewById<Button>(Resource.Id.findYourLocBn);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            textView1 = FindViewById<TextView>(Resource.Id.textView1);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            findYourLocBn.Typeface = tf;
            textView1.Typeface = tf;

            checkInternetConnection();
            if (isOnline == true)
            {
                //checking if GPS is enabled
                if (IsGeolocationEnabled() == true)
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    textView1.Visibility = ViewStates.Visible;
                    findYourLocBn.Visibility = ViewStates.Gone;
                    //await GetCurrentLocation();
                    await Geolocation();
                }
                else
                {
                    turnGPSon();
                }
            }
            else
            {
                Toast.MakeText(this, "No Internet Connection.\nTurn the Internet connection on and try again", ToastLength.Long).Show();
            }

            findYourLocBn.Click += async delegate
            {
                checkInternetConnection();
                if (isOnline == true)
                {
                    //checking if GPS is enabled
                    if (IsGeolocationEnabled() == true)
                    {
                        activityIndicator.Visibility = ViewStates.Visible;
                        textView1.Visibility = ViewStates.Visible;
                        findYourLocBn.Visibility = ViewStates.Gone;
                        //await GetCurrentLocation();
                        await Geolocation();
                    }
                    else
                    {
                        turnGPSon();
                    }
                }
                else
                {
                    Toast.MakeText(this, "No Internet Connection.\nTurn the Internet connection on and try again", ToastLength.Long).Show();
                }
            };
        }
    }
}

