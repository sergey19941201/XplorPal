using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;

namespace RecyclerViewSample
{
    class PrepareReservationForPayment
    {
        public static string content;
        public async Task<string> PrepareReservationToBePayed(string parentResId)
        {
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/instant-order/store", Method.POST);
            request.AddQueryParameter("api_token", Login.token);
            request.AddQueryParameter("email", Login.email_);
            request.AddQueryParameter("name", Login.name);
            request.AddQueryParameter("parent_reservation", parentResId);
            request.AddQueryParameter("people_qty", SelectAvialability.countAdult.ToString());

            var response = await client.ExecuteTaskAsync(request);
            content = response.Content;
            if (response.Content == "")
                await PrepareReservationToBePayed(parentResId);
            return content;
        }
    }
}