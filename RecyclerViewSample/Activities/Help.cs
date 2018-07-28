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
using Android.Content.PM;
using Android.Graphics;

namespace RecyclerViewSample
{
    [Activity(Label = "AboutActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Help : Activity
    {
        TextView helpTV, help_link, titleTV;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Help);
            // Create your application here
            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);

            helpTV = FindViewById<TextView>(Resource.Id.helpTV);
            help_link = FindViewById<TextView>(Resource.Id.help_link);
            titleTV = FindViewById<TextView>(Resource.Id.titleTV);
            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            helpTV.Typeface = tf;
            help_link.Typeface = tf;
            titleTV.Typeface = tf;

            help_link.Click += delegate
            {
                StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://admin@xplorpal.com")));
            };

            back.Click += delegate
            {
                OnBackPressed();
            };
        }
    }
}