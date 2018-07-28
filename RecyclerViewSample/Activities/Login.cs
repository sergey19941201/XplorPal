using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Plugin.Geolocator;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Environment = System.Environment;
using StarWars.Api.Repository;
using Android.Locations;
using Android.Util;
using RecyclerViewSample.ORM;
using SQLite;
using System.Threading.Tasks;
using Com.Bumptech.Glide;
using Android.Content.PM;

namespace RecyclerViewSample
{
    [Activity(Label = "Login", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Login : AppCompatActivity
    {
        public static string token, name, email_, gender, phone_num, interests, birth_date, password;
        public static byte[] avatar;

        //local variables lat,lng to pass coordinats from database to recycler view
        public static string lat_For_places_of_interest_table, lng_For_places_of_interest_table;

        //user_id
        public static int user_id, user_country_id;
        private ProgressBar activityIndicator;

        Profile profile = new Profile();
        DBRepository dbr = new DBRepository();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            Button login = FindViewById<Button>(Resource.Id.login_button);
            Button cancelBn = FindViewById<Button>(Resource.Id.cancelBn);
            TextView forgetYourPassword = FindViewById<TextView>(Resource.Id.forgetYourPassword);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            login.Typeface = tf;
            cancelBn.Typeface = tf;
            login.Visibility = ViewStates.Visible;
            cancelBn.Visibility = ViewStates.Visible;
            forgetYourPassword.Visibility = ViewStates.Visible;
            //activityIndicator.Visibility = Android.Views.ViewStates.Gone;
            login.Click += async delegate
            {
                await Logining();
                //Geolocation();
            };

            cancelBn.Click += delegate
            {
                login.Visibility = Android.Views.ViewStates.Visible;
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                forgetYourPassword.Visibility = ViewStates.Visible;
                StartActivity(typeof(MainActivity));
            };

            //here we create DB
            dbr.CreateDB();
            //here we create table
            dbr.CreateUsersTable();
            //button if the users forgets password
            forgetYourPassword.Click += Login_Click;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ForgetYourPasswordActivity));
        }


        public async Task<string> Logining()
        {
            Button login = FindViewById<Button>(Resource.Id.login_button);
            Button cancelBn = FindViewById<Button>(Resource.Id.cancelBn);
            TextView forgetYourPassword = FindViewById<TextView>(Resource.Id.forgetYourPassword);
            EditText email = FindViewById<EditText>(Resource.Id.login_email);
            EditText passwordET = FindViewById<EditText>(Resource.Id.login_password);
            if (String.IsNullOrWhiteSpace(email.Text) || String.IsNullOrWhiteSpace(passwordET.Text))
            {
                try
                {
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                }
                catch { }
                Toast.MakeText(this, "Credentials must not be empty", ToastLength.Short).Show();
            }
            else
            {
                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                login.Visibility = Android.Views.ViewStates.Gone;
                cancelBn.Visibility = Android.Views.ViewStates.Gone;
                forgetYourPassword.Visibility = ViewStates.Gone;

                string path2 = "fonts/HelveticaNeueLight.ttf";
                Typeface tf = Typeface.CreateFromAsset(Assets, path2);
                email.Typeface = tf;
                passwordET.Typeface = tf;

                string email_val = email.Text;
                string password_val = passwordET.Text;
                var client = new RestClient("http://api.xplorpal.com");
                var request = new RestRequest("/login", Method.POST);

                /*
                            request.AddParameter("email", "nemesises@live.com");
                            request.AddParameter("password", "dontoretto23");
                */
                request.AddParameter("email", email.Text);
                request.AddParameter("password", passwordET.Text);

                IRestResponse response = await client.ExecuteTaskAsync(request);

                var content = response.Content;
                // Toast.MakeText(this, content, ToastLength.Long).Show();
                Console.WriteLine(content.ToString());

                if (content.Length > 100)//response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        var usr = JsonConvert.DeserializeObject<List<User>>(content);
                        var usr_ = usr[0];
                        try
                        {
                            token = usr_.api_token;
                        }
                        catch { }
                        try
                        {
                            name = usr_.name;
                        }
                        catch { }
                        try
                        {
                            email_ = usr_.email;
                        }
                        catch { }
                        try
                        {
                            user_id = usr_.id;
                        }
                        catch { }
                        try

                        {
                            birth_date = usr_.birth_date.ToString();
                        }
                        catch { }
                        try
                        {
                            gender = usr_.gender.ToString();
                        }
                        catch { }
                        try
                        {
                            phone_num = usr_.phone_num.ToString();
                        }
                        catch { }
                        try
                        {
                            interests = usr_.bio;
                        }
                        catch { }
                        try
                        {
                            password = passwordET.Text;
                        }
                        catch { }
                        try
                        {
                            avatar = Convert.FromBase64String(usr_.avatar_base64.Replace("data:image/jpeg;base64,", ""));
                        }
                        catch { }
                        try{
                            user_country_id = Convert.ToInt32(usr_.user_country_id);
                        }catch{}
                    }
                    catch { }
                    /*ImageView Image = FindViewById<ImageView>(Resource.Id.imageView1);
                    /*  Glide.With(Application.Context)
                      .Load(avatar)
                      .Into(Image);
                     //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE
                     Image.BuildDrawingCache(true);
                     Bitmap bitmap = Image.GetDrawingCache(true);
                     Image.SetImageBitmap(bitmap);
                     Bitmap b = Bitmap.CreateBitmap(Image.GetDrawingCache(true));
                     MemoryStream memStream = new MemoryStream();
                     b.Compress(Bitmap.CompressFormat.Webp, 100, memStream);
                     //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE ENDED
                     */

                    //declaring path for RETRIEVING DATA
                    string dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
                    var db = new SQLiteConnection(dbPath);
                    var user_table = db.Table<ORM.UsersDataTable>();

                    MainActivity.isLogined = false;

                    //clearing table
                    foreach (var item in user_table)
                    {
                        dbr.RemoveUsersData(item.Id);
                    }

                    dbr.InsertRecord(name, email_, avatar, token, birth_date, gender, phone_num, interests,
                                     RecyclerViewSample.Login.user_country_id, user_id, password);
                    MainActivity.isLogined = true;
                    GettingJSON gj = new GettingJSON();


                    //await gj.VictorSologoob(token, lat, lon);
                    //await gj.VictorSologoob(token, "38.8951100", "-77.0363700");
                    if (lat_For_places_of_interest_table != null)
                    {
                        await gj.VictorSologoob(token, lat_For_places_of_interest_table, lng_For_places_of_interest_table);
                        addTo_placesOfInterest();
                    }
                    else
                    {
                        //await gj.VictorSologoob(token, "38.8951100", "-77.0363700");
                        await gj.VictorSologoob(token, Activities.NEWstartActivity.lat, Activities.NEWstartActivity.lon);

                        addTo_placesOfInterest();
                    }

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var filePath = System.IO.Path.Combine(path, "iootext.txt");
                    using (var streamWriter = new StreamWriter(filePath, true))
                    {
                        streamWriter.WriteLine(token.ToString());
                    }
                    using (var streamReader = new StreamReader(filePath))
                    {
                        string containing = streamReader.ReadToEnd();
                        System.Diagnostics.Debug.WriteLine(containing);
                    }
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                    login.Visibility = Android.Views.ViewStates.Visible;
                    cancelBn.Visibility = Android.Views.ViewStates.Visible; forgetYourPassword.Visibility = ViewStates.Visible;
                    //StartActivity(typeof(MainActivity));

                    if (String.IsNullOrEmpty(Tours_detail.titleStatic))
                        StartActivity(typeof(MainActivity));
                    else
                        StartActivity(typeof(SelectAvialability));
                }
                else
                {
                    try
                    {
                        login.Visibility = Android.Views.ViewStates.Visible;
                        cancelBn.Visibility = Android.Views.ViewStates.Visible; forgetYourPassword.Visibility = ViewStates.Visible;
                        activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                    }
                    catch { }
                    string toast = "Check your Email and Password";
                    Toast.MakeText(this, toast, ToastLength.Long).Show();
                }
            }
            return "";
        }

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

        //adding coordinats to places of interest table
        public string addTo_placesOfInterest()
        {
            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<ORM.UsersDataTable>();

            //adding coordinats to places of interest table
            new PlacesOfInterstInfo().places_of_interest();
            var places_of_interest_table = db.Table<ORM.PlacesOfInterestTable>();

            foreach (var item in places_of_interest_table)
            {
                lat_For_places_of_interest_table = item.lat;
                lng_For_places_of_interest_table = item.lng;
            }
            return "";
        }
        //adding coordinats to places of interest table ENDED
    }
}