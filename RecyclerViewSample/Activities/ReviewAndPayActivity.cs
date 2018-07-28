
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
using Com.Bumptech.Glide;
using Newtonsoft.Json;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "ReviewAndPayActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReviewAndPayActivity : Activity
    {
        TextView titleTV, totalTV, reservationDate, travellersTV;
        Button payForExperience, cancelBn;
        ImageButton back;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ReviewAndPay);

            titleTV = FindViewById<TextView>(Resource.Id.titleTV);
            reservationDate = FindViewById<TextView>(Resource.Id.reservationDate);
            travellersTV = FindViewById<TextView>(Resource.Id.travellersTV);
            totalTV = FindViewById<TextView>(Resource.Id.totalTV);
            payForExperience = FindViewById<Button>(Resource.Id.payForExperience);
            cancelBn= FindViewById<Button>(Resource.Id.cancelBn);
            back = FindViewById<ImageButton>(Resource.Id.back);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            titleTV.Typeface = tf;
            reservationDate.Typeface = tf;
            travellersTV.Typeface = tf;
            totalTV.Typeface = tf;

            titleTV.Text=Intent.GetStringExtra("Title");
            StripeActivity.reservationId = Intent.GetStringExtra("reservationId");
            reservationDate.Text = "Reservation date: "+Intent.GetStringExtra("reservationDate");
            travellersTV.Text = "Travelers: " + SelectAvialability.countAdult;
            totalTV.Text = "USD $"+ ReservationsAdapter.totalMoney;
            
            payForExperience.Click+=delegate 
            {
                StripeActivity.totalAmount= Intent.GetStringExtra("totalTV");
                StartActivity(typeof(StripeActivity));
            };

            cancelBn.Click += delegate 
            {
                OnBackPressed();
            };
            back.Click += delegate
            {
                OnBackPressed();
            };
        }
    }
}
