using System;
using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using StarWars.Api.Repository;

namespace RecyclerViewSample
{
    public class ReservationsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private Activity _context;
        public List<Reservation> reservations;
        public static List<Reservation> reservationsStatic;
        public static string reservationId, totalMoney;
        private Android.App.FragmentManager fragmentManager;
        private Fragments.GettingReservsForPaymentFragment gettingReservsForPaymentFragment;

        //public override int ItemCount => throw new NotImplementedException();

        public ReservationsAdapter(List<Reservation> reservations, Activity context)
        {
            this.reservations = reservations;
            _context = context;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var reservationsViewHolder = (ReservationsViewHolder)holder;
            string dateNumber;//=reservations[position].start_time;
            dateNumber = reservations[position].date;
            //dateNumber.Substring(dateNumber.Length - 2);
            reservationsViewHolder.DateTV.Text = dateNumber.Substring(dateNumber.Length - 2) + " " 
                + System.DateTime.Parse(dateNumber).ToString("MMMM", CultureInfo.InvariantCulture)+" "
                + System.DateTime.Parse(dateNumber).Year.ToString();
            reservationsViewHolder.DayTV.Text = System.DateTime.Parse(dateNumber).DayOfWeek.ToString();
            /*reservationsViewHolder.DateTextView.Text = "Date: " + reservations[position].date;
            reservationsViewHolder.StatusTextView.Text = "Status: " + reservations[position].destination.status + " spots available";*/
            // `description = experiences[position].description;
            fragmentManager = this._context.FragmentManager;
            gettingReservsForPaymentFragment = new Fragments.GettingReservsForPaymentFragment();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ReservaionsRow, parent, false);
            reservationsStatic = reservations;
            return new ReservationsViewHolder(layout, OnItemClick);
        }
        public override int ItemCount
        {
            get { return reservations.Count; }
        }

        void OnItemClick(int position)
        {
            
            string dateNumber;//=reservations[position].start_time;
            dateNumber = reservations[position].date;

            var activity2 = new Intent(_context, typeof(RecyclerViewSample.Activities.ReviewAndPayActivity));
            activity2.PutExtra("Title", reservationsStatic[position].destination.title);
           
            activity2.PutExtra("reservationDate", dateNumber.Substring(dateNumber.Length - 2) + " "
                + System.DateTime.Parse(dateNumber).ToString("MMMM", CultureInfo.InvariantCulture) + " "
                + System.DateTime.Parse(dateNumber).Year.ToString());
            double totalPrice = Convert.ToDouble(reservationsStatic[position].destination.price)+7.50;
            reservationId = reservationsStatic[position].id.ToString();
            activity2.PutExtra("reservationId", reservationsStatic[position].id.ToString());
            activity2.PutExtra("totalTV", totalPrice.ToString().Replace(',', '.'));
            totalMoney = totalPrice.ToString().Replace(',', '.');
            gettingReservsForPaymentFragment.Show(fragmentManager, "fragmentManager");
            //_context.StartActivity(activity2);
        }
    }
}