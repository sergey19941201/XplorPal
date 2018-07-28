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
using Android.Graphics;
using Android.Content.PM;

namespace RecyclerViewSample
{
    [Activity(Label = "SelectAvialability", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SelectAvialability : Activity
    {
        public static int countAdult;
        private Android.App.FragmentManager fragmentManager;
        private Fragments.LoginOrRegistrationFragment loginOrRegFragment;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectAvialability);

            countAdult = 0;
            string text = Tours_detail.titleStatic;//Intent.GetStringExtra("Title");
            TextView title = FindViewById<TextView>(Resource.Id.Title);
            var travelersTV = FindViewById<TextView>(Resource.Id.travelersTV);
            var adult_number = FindViewById<TextView>(Resource.Id.adultnumber);
            title.Text = text;
            Button next = FindViewById<Button>(Resource.Id.nextbutton);
            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            ImageButton plusAdult = FindViewById<ImageButton>(Resource.Id.plus);
            ImageButton minusAdult = FindViewById<ImageButton>(Resource.Id.minus);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            title.Typeface = tf;
            travelersTV.Typeface = tf;
            adult_number.Typeface = tf;
            FindViewById<TextView>(Resource.Id.textView2).Typeface = tf;

            fragmentManager = this.FragmentManager;
            loginOrRegFragment = new Fragments.LoginOrRegistrationFragment();

            back.Click += delegate
            {
                OnBackPressed();
            };
            next.Typeface = tf;
            next.Click += delegate
            {
                if (countAdult == 0)
                {
                    Toast.MakeText(this, "Select number of travellers", ToastLength.Short).Show();
                }
                else
                {
                    //set title of experience for reservations screen
                    Reservations_list_Activity.tour_name = title.Text;
                    if (MainActivity.isLogined == true)
                    {
                        var activity = new Intent(this, typeof(Reservations_list_Activity));
                        activity.PutExtra("Title", title.Text);
                        StartActivity(activity);
                        //StartActivity(typeof(SelectDate));
                    }
                    else
                    {
                        loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                    }
                }
            };

            plusAdult.Click += delegate
            {
                countAdult++;
                adult_number.Text = countAdult.ToString();
            };
            minusAdult.Click += delegate
            {
                if (countAdult > 0)
                {
                    countAdult--;
                    adult_number.Text = countAdult.ToString();
                }
            };
        }
    }
}