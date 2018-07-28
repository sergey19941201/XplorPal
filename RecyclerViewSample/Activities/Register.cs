using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RecyclerViewSample.ORM;
using RestSharp;
using SQLite;
using StarWars.Api.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RecyclerViewSampl
{
    [Activity(Label = "Register", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Register : Activity
    {
        RecyclerViewSample.GettingCountry gettingCountry = new RecyclerViewSample.GettingCountry();
        private static int idOfChosenCountry;
        //variable to indicate if the user pressed select image from gallery of take a photo button
        private static string cameraOrGalleryIndicator;
        private static string promptMessage, bal;
        DBRepository dbr = new DBRepository();
        RecyclerViewSample.Login login = new RecyclerViewSample.Login();
        private ProgressBar activityIndicator;

        public override void OnBackPressed()
        {
            StartActivity(typeof(RecyclerViewSample.MainActivity));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(RecyclerViewSample.Resource.Layout.Register);

            idOfChosenCountry = 1;
            Spinner countries = FindViewById<Spinner>(RecyclerViewSample.Resource.Id.countries);
            Button registeringBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.register_button);
            Button cancelBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.cancelBn);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            registeringBn.Typeface = tf;
            cancelBn.Typeface = tf;

            //adapter
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, RecyclerViewSample.GettingCountry.countriesList);
            activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);
            countries.Adapter = adapter;
            countries.ItemSelected += Countries_ItemSelected;

            registeringBn.Click += async delegate
            {
                await Registering();
            };

            cancelBn.Click += delegate
            {
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                OnBackPressed();
                //StartActivity(typeof(RecyclerViewSample.MainActivity));
            };
        }

        private async Task<string> Registering()
        {
            EditText email = FindViewById<EditText>(RecyclerViewSample.Resource.Id.register_email);
            EditText password = FindViewById<EditText>(RecyclerViewSample.Resource.Id.register_password);
            EditText password_confirm = FindViewById<EditText>(RecyclerViewSample.Resource.Id.register_password_confirm);
            EditText name = FindViewById<EditText>(RecyclerViewSample.Resource.Id.name);
            Button registeringBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.register_button);
            Button cancelBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.cancelBn);
            var image =
                FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);

            activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);
            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            email.Typeface = tf;
            password.Typeface = tf;
            password_confirm.Typeface = tf;
            name.Typeface = tf;
            promptMessage = "";
            if (password.Text != password_confirm.Text)
            {
                promptMessage += "Passwords don`t match. \n";
            }
            if (email.Text == "" || email.Text == null || email.Text == " " || email.Text == "  " || email.Text == "   ")
            {
                promptMessage += "E-mail is empty. \n";
            }
            if (password.Text == "" || password.Text == null || password.Text == " " || password.Text == "  " || password.Text == "   " || password.Text.Length < 6)
            {
                promptMessage += "Password is empty or less than 6 symbols. \n";
            }
            if (password_confirm.Text == "" || password_confirm.Text == null || password_confirm.Text == " " || password_confirm.Text == "  " || password_confirm.Text == "   " || password_confirm.Text.Length < 6)
            {
                promptMessage += "Password confirmation is empty or less than 6 symbols. \n";
            }
            if (name.Text == "" || name.Text == null || name.Text == " " || name.Text == "  " || name.Text == "   ")
            {
                promptMessage += "Name is empty. \n";
            }

            if (promptMessage != "")
            {
                try
                {
                    registeringBn.Visibility = Android.Views.ViewStates.Visible;
                    cancelBn.Visibility = Android.Views.ViewStates.Visible;
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                }
                catch { }
                Toast.MakeText(this, promptMessage, ToastLength.Long).Show();
            }
            else
            {
                registeringBn.Visibility = Android.Views.ViewStates.Gone;
                cancelBn.Visibility = Android.Views.ViewStates.Gone;
                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                var client = new RestClient("http://api.xplorpal.com");
                var request = new RestRequest("/register", Method.POST);
                request.AddParameter("email", email.Text);
                request.AddParameter("password", password.Text);
                request.AddParameter("password_confirmation", password_confirm.Text);
                request.AddParameter("last_name", name.Text);
                request.AddParameter("name", name.Text);
                request.AddParameter("user_country_id", idOfChosenCountry);
                //request.AddParameter("profile_img[]", "data:image/jpeg;base64," + bal);
                IRestResponse response = await client.ExecuteTaskAsync(request);
                var content = response.Content;
                //Console.WriteLine(content);
                var myContent = JObject.Parse(content);
                string email_code = myContent["email"].ToString();

                if (email_code.Contains("The email has already been taken"))
                {
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                    Toast.MakeText(this, "The email has already been taken", ToastLength.Short).Show();
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await Logining(email.Text, password.Text);
                    }
                    else
                    {
                        try
                        {
                            activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                        }
                        catch { }
                        Toast.MakeText(this, "Check your fields", ToastLength.Long).Show();
                    }
                }
            }
            registeringBn.Visibility = Android.Views.ViewStates.Visible;
            cancelBn.Visibility = Android.Views.ViewStates.Visible;
            return "";
        }

        public async Task<string> Logining(string email, string password)
        {
            /*string email_val = email;
            string password_val = password;*/
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/login", Method.POST);

            /*
                        request.AddParameter("email", "nemesises@live.com");
                        request.AddParameter("password", "dontoretto23");
              */
            request.AddParameter("email", email);
            request.AddParameter("password", password);

            IRestResponse response = await client.ExecuteTaskAsync(request);
            var content = response.Content;
            // Toast.MakeText(this, content, ToastLength.Long).Show();
            //Console.WriteLine(content.ToString());

            if (content.Length > 100)//response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var usr = JsonConvert.DeserializeObject<List<User>>(content);
                var usr_ = usr[0];
                try
                {
                    RecyclerViewSample.Login.token = usr_.api_token;
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.name = usr_.name;
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.email_ = usr_.email;
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.user_id = usr_.id;
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.user_country_id = Convert.ToInt32(usr_.user_country_id);
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.birth_date = usr_.birth_date.ToString();
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.gender = usr_.gender.ToString();
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.phone_num = usr_.phone_num.ToString();
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.interests = usr_.bio;
                }
                catch { }
                try
                {
                    RecyclerViewSample.Login.password = password;
                }
                catch { }
                try
                {
                    //removing this part of the content
                    RecyclerViewSample.Login.avatar = Convert.FromBase64String(usr_.avatar_base64.Replace("data:image/jpeg;base64,", ""));
                }
                catch { }

                //declaring path for RETRIEVING DATA
                string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                var user_table = db.Table<RecyclerViewSample.ORM.UsersDataTable>();

                RecyclerViewSample.MainActivity.isLogined = false;

                //clearing table
                foreach (var item in user_table)
                {
                    dbr.RemoveUsersData(item.Id);
                }

                dbr.InsertRecord(RecyclerViewSample.Login.name,
                    RecyclerViewSample.Login.email_,
                    RecyclerViewSample.Login.avatar,
                    RecyclerViewSample.Login.token,
                    RecyclerViewSample.Login.birth_date,
                    RecyclerViewSample.Login.gender,
                    RecyclerViewSample.Login.phone_num,
                    RecyclerViewSample.Login.interests,
                                 RecyclerViewSample.Login.user_country_id,
                    RecyclerViewSample.Login.user_id,
                    RecyclerViewSample.Login.password);
                RecyclerViewSample.MainActivity.isLogined = true;
                GettingJSON gj = new GettingJSON();


                //await gj.VictorSologoob(token, lat, lon);
                //await gj.VictorSologoob(token, "38.8951100", "-77.0363700");
                if (RecyclerViewSample.Login.lat_For_places_of_interest_table != null)
                {
                    await gj.VictorSologoob(
                        RecyclerViewSample.Login.token,
                        RecyclerViewSample.Login.lat_For_places_of_interest_table,
                        RecyclerViewSample.Login.lng_For_places_of_interest_table);
                    login.addTo_placesOfInterest();
                }
                else
                {
                    //gj.VictorSologoob(token, "38.8951100", "-77.0363700");
                    await gj.VictorSologoob(
                        RecyclerViewSample.Login.token,
                        RecyclerViewSample.Activities.NEWstartActivity.lat,
                        RecyclerViewSample.Activities.NEWstartActivity.lon);

                    login.addTo_placesOfInterest();
                }

                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = System.IO.Path.Combine(path, "iootext.txt");
                using (var streamWriter = new StreamWriter(filePath, true))
                {
                    streamWriter.WriteLine(RecyclerViewSample.Login.token.ToString());
                }
                using (var streamReader = new StreamReader(filePath))
                {
                    string containing = streamReader.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine(containing);
                }
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                StartActivity(typeof(RecyclerViewSampl.ImageUpload));
                //StartActivity(typeof(MainActivity));
            }
            else
            {
                try
                {
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                }
                catch { }
                string toast = "Check your Email and Password";
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
            return "";
        }


        private void Countries_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //retrieving the id of chosen country
            idOfChosenCountry = gettingCountry.retrievingChoosenCountryId(RecyclerViewSample.GettingCountry.countriesList[e.Position].ToString());
        }
    }
}
