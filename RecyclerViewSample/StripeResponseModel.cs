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

namespace RecyclerViewSample
{
    public class StripeResponseModel
    {
        public string stripe_id { get; set; }
        public int status { get; set; }
    }
}