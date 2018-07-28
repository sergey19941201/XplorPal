using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace RecyclerViewSample.Fragments
{
    class LoginOrRegistrationFragment : DialogFragment
    {
        Button loginBn;
        Button registrationBn;
        Button cancelBn;
        TextView youMustBeLoggedInTextView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.LoginOrRegistrationFragment, container, false);

            loginBn = rootView.FindViewById<Button>(Resource.Id.LoginBn);
            registrationBn = rootView.FindViewById<Button>(Resource.Id.registrationBn);
            cancelBn = rootView.FindViewById<Button>(Resource.Id.CancelBn);
            youMustBeLoggedInTextView = rootView.FindViewById<TextView>(Resource.Id.youMustBeLoggedInTextView);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(rootView.Context.Assets, path);
            loginBn.Typeface = tf;
            registrationBn.Typeface = tf;
            cancelBn.Typeface = tf;
            youMustBeLoggedInTextView.Typeface = tf;

            loginBn.Click += LoginBn_Click;
            registrationBn.Click += RegistrationBn_Click;
            cancelBn.Click += CancelBn_Click;

            return rootView;
        }

        private void CancelBn_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void RegistrationBn_Click(object sender, EventArgs e)
        {
            Dismiss();
            this.Activity.StartActivity(new Intent(this.Activity, typeof(RecyclerViewSample.Activities.GettingCountriesActivity)));
        }

        private void LoginBn_Click(object sender, EventArgs e)
        {
            Dismiss();
            this.Activity.StartActivity(new Intent(this.Activity, typeof(Login)));
        }
    }
}