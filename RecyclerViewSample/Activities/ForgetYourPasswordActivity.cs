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
using SQLite;
using RestSharp;
using Android.Content.PM;

namespace RecyclerViewSample
{
    [Activity(Label = "ForgetYourPasswordActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ForgetYourPasswordActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ForgetYourPassword);

            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/password/email", Method.POST);

            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<ORM.UsersDataTable>();

            EditText login_email = FindViewById<EditText>(Resource.Id.login_email);
            FindViewById<Button>(Resource.Id.tryBn).Click += async delegate 
            {
                foreach (var item in user_table)
                {
                    if (item.email == login_email.Text)
                    {
                        request.AddParameter("email", item.email);
                        request.AddParameter("_token", item.api_token);
                        var response = await client.ExecuteTaskAsync(request);
                        var content =  response.Content;
                    }
                }
            };
        }
    }
}