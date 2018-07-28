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

namespace RecyclerViewSample
{
    [Activity(Label = "AboutActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.About);
            // Create your application here
            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            back.Click += delegate
            {
                OnBackPressed();
            };
        }
    }
}