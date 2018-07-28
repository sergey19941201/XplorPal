using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using RestSharp;
using Android.Content.PM;
using System.Threading.Tasks;

namespace RecyclerViewSample
{
    [Activity(Label = "FilterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FilterActivity : Activity
    {
        private static int category;
        private ProgressBar activityIndicator;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FilterScreen);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
            TextView doneTV = FindViewById<TextView>(Resource.Id.doneTV);
            TextView textView1 = FindViewById<TextView>(Resource.Id.textView1);
            ImageButton cancel = FindViewById<ImageButton>(Resource.Id.cancel);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            textView1.Text = "Filter your results";

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            cancel.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
            doneTV.Click += async delegate
            {
                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                if (category > 0)
                {
                    var client = new RestClient("http://api.xplorpal.com");
                    var request = new RestRequest("/experience/search", Method.POST);
                    request.AddParameter("categories[] ", category);
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                    Fragments.SearchFragment.content = response.Content;
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                    StartActivity(typeof(Activities.SearchByWordResultActivity));
                }
                else
                {
                    Toast.MakeText(this, "Choose type of experience", ToastLength.Short).Show();
                }
            };

            string path2 = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path2);

            doneTV.Typeface = tf;
            textView1.Typeface = tf;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            //string toast = string.Format("Type is {0}", spinner.GetItemAtPosition(e.Position));
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Choose type of experience")
            {
                category = 0;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Sports")
            {
                category = 1;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Music")
            {
                category = 3;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Romance")
            {
                category = 4;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Adventure")
            {
                category = 5;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Food")
            {
                category = 6;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Art/Architecture")
            {
                category = 7;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Photography")
            {
                category = 8;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Bar hopping")
            {
                category = 9;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Sight-seeing")
            {
                category = 10;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Wine tasting")
            {
                category = 11;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Classes")
            {
                category = 12;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Wedding planning")
            {
                category = 13;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Gardening")
            {
                category = 14;
            }
        }
    }
}