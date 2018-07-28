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
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using StarWars.Api.Repository;

namespace RecyclerViewSample
{
    [Activity(Label = "Reservations_list_Activity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Reservations_list_Activity : Activity
    {
        ProgressBar activityIndicator;
        ImageButton back;
        TextView messageTV,nameOfExpTV,selectTravelDateTV;
        TextView Reservations_title_TV;
        GettingReservations gettingReservations = new GettingReservations();
        private RecyclerView.LayoutManager layoutManager;
        //public static LinearLayout mainLayout;
        private RecyclerView recyclerView;
        public static string tour_name;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Reservations_list);
            /*string text = Intent.GetStringExtra("Title");
            TextView title = FindViewById<TextView>(Resource.Id.Title);
            title.Text = text;*/

            //declaring mainLayout
            //mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicatorReservations);
            messageTV = FindViewById<TextView>(Resource.Id.messageTV);
            nameOfExpTV = FindViewById<TextView>(Resource.Id.nameOfExpTV);
            selectTravelDateTV = FindViewById<TextView>(Resource.Id.selectTravelDateTV);
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            /* //declaring mainLayout
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                 .SetDefaultFontPath("fonts/HelveticaNeueLight")
                 .SetFontAttrId(Resource.Attribute.fontPath)
                 .Build());
            //declaring mainLayout*/
            //mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);

            var url = Tours_detail.image_url_static;

           recyclerView.SetLayoutManager(layoutManager);

            nameOfExpTV.Visibility = Android.Views.ViewStates.Gone;
            selectTravelDateTV.Visibility = Android.Views.ViewStates.Gone;
            activityIndicator.Visibility = Android.Views.ViewStates.Visible;
            messageTV.Visibility = Android.Views.ViewStates.Visible;
            //activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicatorReservations);
            messageTV = FindViewById<TextView>(Resource.Id.messageTV);
            Reservations_title_TV = FindViewById<TextView>(Resource.Id.Reservations_title_TV);
            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            messageTV.Typeface = tf;
            nameOfExpTV.Typeface = tf;
            selectTravelDateTV.Typeface = tf;
            Reservations_title_TV.Typeface = tf;

            nameOfExpTV.Text = tour_name;
            try
            {
                var reservations = await gettingReservations.GetReservations(Login.token);

                var responseSearch = JsonConvert.DeserializeObject<RootObjectReservations>(reservations);

                nameOfExpTV.Visibility = Android.Views.ViewStates.Visible;
                selectTravelDateTV.Visibility = Android.Views.ViewStates.Visible;
                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                messageTV.Visibility = Android.Views.ViewStates.Gone;
                List<Reservation> reservationListReversed = new List<Reservation>();

                for (int i = responseSearch.reservations.Count - 1; i >= 0; i--)
                {
                    reservationListReversed.Add(responseSearch.reservations[i]);
                }

                var reservationsAdapter = new ReservationsAdapter(reservationListReversed, this);
                //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE ENDED

                recyclerView.SetAdapter(reservationsAdapter);

                back = FindViewById<ImageButton>(Resource.Id.back);
                back.Click += delegate
                {
                    OnBackPressed();
                };
            }
            catch { }
        }
    }
}