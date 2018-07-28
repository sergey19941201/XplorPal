using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RecyclerViewSample.Fragments
{
    class GettingReservsForPaymentFragment : DialogFragment
    {
        ProgressBar activityIndicator;
        TextView Getting_your_exp_TV;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment, container, false);

            Getting_your_exp_TV = rootView.FindViewById<TextView>(Resource.Id.Getting_your_exp_TV);
            activityIndicator = rootView.FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            Getting_your_exp_TV.Text = "Getting data";

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(rootView.Context.Assets, path);
            Getting_your_exp_TV.Typeface = tf;

            getRes();
            activityIndicator.Visibility = ViewStates.Visible;
            //this.Dismiss();
            //StartActivity(new Intent(this.Activity, typeof(Map)));
            return rootView;
        }
        private async Task<string> getRes()
        {
            string res = await new PrepareReservationForPayment().PrepareReservationToBePayed(ReservationsAdapter.reservationId);
            this.Dismiss();
            StartActivity(new Intent(this.Activity, typeof(Activities.ReviewAndPayActivity)));
            return res;
        }
    }
}