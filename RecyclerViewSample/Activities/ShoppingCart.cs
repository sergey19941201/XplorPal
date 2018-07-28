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

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "ShoppingCart", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ShoppingCart : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ShoppingCart);
            // Create your application here
            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            back.Click += delegate
            {
                OnBackPressed();
            };
        }
    }
}