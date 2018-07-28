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
using RecyclerViewSample.Activities;

namespace RecyclerViewSample
{
    [Activity(Label = "StartScreen", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Black.NoTitleBar")]
    public class StartScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.StartScreen);
            Button login = FindViewById<Button>(Resource.Id.Login);
            Button register = FindViewById<Button>(Resource.Id.Register);
            //Turning on custom fonts
            string path = "fonts/Dosis-Regular.otf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            login.Typeface = tf;
            register.Typeface = tf;
            //Going to Login Screen
            login.Click += delegate {
                StartActivity(typeof(Login));
            };
            register.Click += delegate
            {
                StartActivity(typeof(RecyclerViewSampl.Register));
            };
        }

    }
}