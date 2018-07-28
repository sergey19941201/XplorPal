using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using RestSharp;
using Stripe;
using System;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "StripeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class StripeActivity : Activity
    {
        EditText cardET, monthET, yearET, cvvET;
        Button confirmBn;
        ProgressBar activityIndicator;
        public static string reservationId, totalAmount;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnteringCardData);

            cardET = FindViewById<EditText>(Resource.Id.cardET);
            monthET = FindViewById<EditText>(Resource.Id.monthET);
            yearET = FindViewById<EditText>(Resource.Id.yearET);
            cvvET = FindViewById<EditText>(Resource.Id.cvvET);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            confirmBn = FindViewById<Button>(Resource.Id.confirmBn);
            //Secure Background Screen Preview in Xamarin
            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

            confirmBn.Click += (s, e) =>
            {
                activityIndicator.Visibility = ViewStates.Visible;
                confirmBn.Visibility = ViewStates.Gone;
                StripeConfiguration.SetApiKey("sk_test_eydXW0qoj2r9kXTVI37uuj94");
                var planService = new StripePlanService();
                CreateToken(cardET.Text, Convert.ToInt32(monthET.Text), Convert.ToInt32(yearET.Text), cvvET.Text);
            };
        }

        private string CreateToken(string cardNumber, int cardExpMonth, int cardExpYear, string cardCVC)
        {
            StripeConfiguration.SetApiKey("sk_test_eydXW0qoj2r9kXTVI37uuj94");
            var tokenOptions = new StripeTokenCreateOptions()
            {
                Card = new StripeCreditCardOptions()
                {
                    Number = cardNumber,
                    ExpirationYear = cardExpYear,
                    ExpirationMonth = cardExpMonth,
                    Cvc = cardCVC
                }
            };

            var tokenService = new StripeTokenService();
            StripeToken stripeToken;
            try
            {
                stripeToken = tokenService.Create(tokenOptions);
                MakePayment(stripeToken.Id.ToString());
                return stripeToken.Id;// This is the token
            }
            catch
            {
                Toast.MakeText(this, "You entered wrong data", ToastLength.Short).Show();
                activityIndicator.Visibility = ViewStates.Gone;
                confirmBn.Visibility = ViewStates.Visible;
            }
            return null; 
        }
        private async void MakePayment(string stripeToken)
        {
            var reservDeta = JsonConvert.DeserializeObject<RootObjectReservationRepo>(PrepareReservationForPayment.content);

            var amount = /*(int)Convert.ToInt64(Convert.ToDouble(*/ReservationsAdapter.totalMoney/*))*/;
            var amount_int = amount.Split(new char[] { '.' });
            reservationId = reservDeta.data.id.ToString();
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/stripe_charge", Method.POST);
            request.AddParameter("stripe_token", stripeToken);
            request.AddParameter("api_token", Login.token);
            request.AddParameter("amount", amount_int[0]);
            request.AddParameter("reservation_id", reservationId);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            var content = response.Content;
            var des = JsonConvert.DeserializeObject<StripeResponseModel>(content);
            if (des.status == 200)
            {
                Toast.MakeText(this, "Payment was successful", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
            else
                Toast.MakeText(this, "Something went wrong", ToastLength.Short).Show();
            activityIndicator.Visibility = ViewStates.Gone;
            confirmBn.Visibility = ViewStates.Visible;
        }
    }

    public class PaymentModel
    {
        public decimal Amount { get; set; }
        public string Token { get; set; }
    }
}
