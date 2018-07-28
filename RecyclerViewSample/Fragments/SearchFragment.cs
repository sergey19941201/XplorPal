using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using RestSharp;
using RecyclerViewSample.Activities;
using SQLite;
using System.Threading.Tasks;

namespace RecyclerViewSample.Fragments
{
    public class SearchFragment : DialogFragment
    {
        TextView searchTV;
        EditText SearchET;
        Button searchBn;
        Button cancelBn;
        ProgressBar activityIndicator;
        public static string content, searchWord;
        public static bool searchByWordIndicator;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.SearchFragment, container, false);

            searchTV = rootView.FindViewById<TextView>(Resource.Id.textView1);
            SearchET = rootView.FindViewById<EditText>(Resource.Id.searchET);
            searchBn = rootView.FindViewById<Button>(Resource.Id.searchBn);
            cancelBn = rootView.FindViewById<Button>(Resource.Id.cancelBn);
            activityIndicator = rootView.FindViewById<ProgressBar>(Resource.Id.activityIndicator);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(rootView.Context.Assets, path);
            searchTV.Typeface = tf;
            SearchET.Typeface = tf;
            searchBn.Typeface = tf;
            cancelBn.Typeface = tf;

            searchBn.Click += async delegate
            {
                if (!String.IsNullOrWhiteSpace(SearchET.Text))
                {
                    searchWord = SearchET.Text;
                    searchBn.Visibility = ViewStates.Gone;
                    cancelBn.Visibility = ViewStates.Gone;
                    await searchFunction(searchWord);
                }
                else
                {
                    Toast.MakeText(this.Activity, "Search field is empty", ToastLength.Short).Show();
                }
            };
            cancelBn.Click += delegate
            {
                Dismiss();
            };

            return rootView;
        }
        private async Task<string> searchFunction(string search_Word)
        {
            var client = new RestClient("http://api.xplorpal.com/experience");
            var request = new RestRequest("/search", Method.POST);
            ORM.DBRepository dbr = new ORM.DBRepository();
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var placesOfInterestTable = db.Table<ORM.PlacesOfInterestTable>();

            try
            {
                foreach (var item in placesOfInterestTable)
                {
                    dbr.RemovePlacesOfInterestRecord(item.Id);
                }
            }
            catch { }
            try
            {
                MovieAdapter.moviesStatic.Clear();
            }
            catch { }
            try
            {
                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
            }
            catch { }
            request.AddParameter("search", search_Word);
            Fragments.SearchFragment.searchByWordIndicator = true;
            searchWord = search_Word;
            IRestResponse response = await client.ExecuteTaskAsync(request);
            content = response.Content;

            if (!String.IsNullOrWhiteSpace(content))
            {
                Dismiss();
                this.Activity.StartActivity(new Intent(this.Activity, typeof(SearchByWordResultActivity)));
            }
            else
            {
                await searchFunction(searchWord);
            }

            return "";
        }
    }
}