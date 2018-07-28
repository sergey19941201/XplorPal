
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using RestSharp;
using static RecyclerViewSample.GettingCountry;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "GettingCountriesActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GettingCountriesActivity : Activity
    {
        ProgressBar activityIndicator;
        TextView informTextView;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(RecyclerViewSample.Resource.Layout.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment);

            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            informTextView = FindViewById<TextView>(Resource.Id.Getting_your_exp_TV);

            informTextView.Text = "Getting list of countries";
            activityIndicator.Visibility = ViewStates.Visible;

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            informTextView.Typeface = tf;

            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/countries", Method.POST);
            var response = await client.ExecuteTaskAsync(request);
            var content = response.Content;
            var responseCountries = JsonConvert.DeserializeObject<IEnumerable<RootObjectCountries>>(content);

            GettingCountry.countriesList.Clear();
            listOfCountriesRoot.Clear();

            foreach (var country in responseCountries)//the foreach-loop ZALUPA Victor SOLOGOOB
            {
                GettingCountry.countriesList.Add(country.nicename);
                listOfCountriesRoot.Add(new RootObjectCountries(country.id, country.nicename));
            }
            StartActivity(typeof(RecyclerViewSampl.Register));
        }
    }
}
