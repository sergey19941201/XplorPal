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
    [Activity(Label = "Role", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Role : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login_role);
            Button login_traveler = FindViewById<Button>(Resource.Id.traveler_login_button);
            login_traveler.Click += delegate {
                StartActivity(typeof(Login));
            };
        }
    }
}