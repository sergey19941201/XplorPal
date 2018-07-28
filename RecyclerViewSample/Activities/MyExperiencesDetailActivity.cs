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
using Com.Bumptech.Glide;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.Design.Widget;
using com.refractored.fab;
using System.Timers;
using System.Threading;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using RecyclerViewSample.ORM;
using SQLite;
using RestSharp;
using Newtonsoft.Json.Linq;
using Android.Content.PM;
using Newtonsoft.Json;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "MyExperiencesDetailActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MyExperiencesDetailActivity : Activity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        //Database declaration
        DBRepository dbr = new DBRepository();
        private static int count_data_rows_in_users_table;
        private static bool recordExistsInWhishlist;
        public static string id_of_my_current_experience;

        Button publishButton, editButton, deleteButton;
        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            throw new NotImplementedException();
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
            throw new NotImplementedException();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MyExperiencesDetail);

            RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
            RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/HelveticaNeueLight")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            recordExistsInWhishlist = false;

            id_of_my_current_experience = Intent.GetStringExtra("Id");
            string text = Intent.GetStringExtra("Title");
            string price = Intent.GetStringExtra("Price");
            string image_url = Intent.GetStringExtra("ImageUrl");
            string description_tour = Intent.GetStringExtra("Description");
            string location = Intent.GetStringExtra("Location");
            string duration = Intent.GetStringExtra("Duration");
            string min_capacity = Intent.GetStringExtra("Min_capacity");
            string max_capacity = Intent.GetStringExtra("Max_capacity");
            string lat = Intent.GetStringExtra("Lat");
            string lng = Intent.GetStringExtra("Lng");
            string status = Intent.GetStringExtra("IsPublished");
            TextView title = FindViewById<TextView>(Resource.Id.Title);
            ImageView image = FindViewById<ImageView>(Resource.Id.image);
            TextView price_text = FindViewById<TextView>(Resource.Id.price);
            TextView descript = FindViewById<TextView>(Resource.Id.description);
            TextView location_val = FindViewById<TextView>(Resource.Id.location_value);
            TextView duration_val = FindViewById<TextView>(Resource.Id.duration_value);
            TextView capacity = FindViewById<TextView>(Resource.Id.capacity_value);
            publishButton = FindViewById<Button>(Resource.Id.publishButton);
            editButton = FindViewById<Button>(Resource.Id.editButton);
            deleteButton = FindViewById<Button>(Resource.Id.deleteButton);

            if(status=="2")
            {
                publishButton.Visibility = ViewStates.Gone;
            }

            ProgressBar activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);

            Glide.With(Application.Context)
                .Load(image_url)
                .Into(image);
            title.Text = text;
            price_text.Text = "$" + price;
            descript.Text = description_tour;
            location_val.Text = location;
            duration_val.Text = duration + " hours";
            capacity.Text = min_capacity + " - " + max_capacity;

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);

            title.Typeface = tf;
            price_text.Typeface = tf;
            descript.Typeface = tf;
            location_val.Typeface = tf;
            duration_val.Typeface = tf;
            capacity.Typeface = tf;
            publishButton.Typeface = tf;
            editButton.Typeface = tf;
            deleteButton.Typeface = tf;


            var scrollView = FindViewById<com.refractored.fab.ObservableScrollView>(Resource.Id.scrollViewDetail);
            var likeBn = FindViewById<ImageButton>(Resource.Id.likeBn);
            likeBn.SetBackgroundResource(Resource.Drawable.likeWhite);
            likeBn.Click += delegate
            {
                Toast.MakeText(this, "Your experience added to wishlist", ToastLength.Short).Show();
                likeBn.SetBackgroundResource(Resource.Drawable.likeRed);

                //creating wishlist table
                dbr.CreateWishlistTable();

                //declaring path for RETRIEVING DATA
                string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                var wishlist_table = db.Table<ORM.Wishlist>();
                //declaring path for RETRIEVING DATA ENDED
                count_data_rows_in_users_table = 0;
                //checking if the place of interest exists
                foreach (var item in wishlist_table)
                {
                    count_data_rows_in_users_table = 1;
                    if (item.name == title.Text && item.price == price)
                    {
                        recordExistsInWhishlist = true;
                        break;
                    }
                    else if (item.name != title.Text && item.price != price)
                    {
                        recordExistsInWhishlist = false;
                    }
                }
                //if table is empty we insert a record
                if (count_data_rows_in_users_table == 0)
                {
                    dbr.InsertWhishlistRecord(
                        title.Text, Tours_detail.current_experience_id, price, image_url, description_tour, location, duration, min_capacity, max_capacity, lat, lng, true);
                }

                if (recordExistsInWhishlist == false && count_data_rows_in_users_table != 0)
                {
                    dbr.InsertWhishlistRecord(
                        title.Text, Tours_detail.current_experience_id, price, image_url, description_tour, location, duration, min_capacity, max_capacity, lat, lng, true);
                }
            };

            ImageButton back = FindViewById<ImageButton>(Resource.Id.back);
            back.Click += delegate
            {
                OnBackPressed();
            };

            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);

            publishButton.Click += delegate
            {
                builder.SetTitle("Publishing");
                builder.SetMessage("Do you want to publish \"" + text + "\"?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("No", (object sender1, DialogClickEventArgs e1) =>
                {
                });
                builder.SetNegativeButton("Yes", async (object sender1, DialogClickEventArgs e1) =>
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    //getting id of the image
                    var responseData = JsonConvert.DeserializeObject<RecyclerViewSample.RootObjectMyExperiences>(RecyclerViewSample.GetMyExperiences.content);
                    var client = new RestClient("http://api.xplorpal.com/experience/" + id_of_my_current_experience);
                    bool image_exists = false;
                    foreach (var item in responseData.experiences)
                    {
                        try
                        {
                            if (item.id.ToString() == id_of_my_current_experience)
                            {
                                int image_id = item.cover_image.id;
                                var request_set_cover = new RestRequest("/setCover", Method.POST);
                                request_set_cover.AddQueryParameter("api_token", Login.token);
                                request_set_cover.AddQueryParameter("image_id", image_id.ToString());
                                var response_set_cover = await client.ExecuteTaskAsync(request_set_cover);
                                string set_cover_content, set_cover_response;
                                set_cover_content = response_set_cover.Content;
                                var set_c_content = JObject.Parse(set_cover_content);
                                set_cover_response = set_c_content["response_code"].ToString();

                                //var client = new RestClient("http://api.xplorpal.com/experience/" + id_of_my_current_experience);
                                var request = new RestRequest("/update_status", Method.POST);
                                request.AddQueryParameter("status", 2.ToString());
                                request.AddQueryParameter("api_token", Login.token);
                                var response = await client.ExecuteTaskAsync(request);
                                activityIndicator.Visibility = ViewStates.Gone;

                                string content, parsed_response;
                                content = response.Content;
                                if (content.Contains("Experience can't be published without cover image."))
                                {
                                    Toast.MakeText(this, "Experience can't be published without image.", ToastLength.Short).Show();
                                }
                                var parsed_content = JObject.Parse(content);
                                parsed_response = parsed_content["response_code"].ToString();
                                if (parsed_response == "200")
                                {
                                    Toast.MakeText(this, "Experience published successfully", ToastLength.Short).Show();
                                    StartActivity(typeof(MainActivity));
                                }

                                image_exists = true;
                                break;
                            }
                        }
                        catch { }
                    }
                    if (image_exists == false)
                    {
                        activityIndicator.Visibility = ViewStates.Gone;
                        Toast.MakeText(this, "Image doesn`t exist", ToastLength.Short).Show();
                    }

                    image_exists = false;
                });
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            };
            editButton.Click += delegate
            {
                builder.SetTitle("Editing");
                builder.SetMessage("Do you want to edit \"" + text + "\"?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("No", (object sender1, DialogClickEventArgs e1) =>
                {
                });
                builder.SetNegativeButton("Yes", (object sender1, DialogClickEventArgs e1) =>
                {
                    StartActivity(typeof(RecyclerViewSampl.EditTourActivity));
                });
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            };
            deleteButton.Click += delegate
            {
                builder.SetTitle("Deleting");
                builder.SetMessage("Do you want to delete \"" + text + "\"?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("No", (object sender1, DialogClickEventArgs e1) =>
                {
                });
                builder.SetNegativeButton("Yes", async (object sender1, DialogClickEventArgs e1) =>
                {
                    activityIndicator.Visibility = ViewStates.Visible;
                    var client = new RestClient("http://api.xplorpal.com/experience/" + id_of_my_current_experience);
                    var request = new RestRequest("/destroy", Method.POST);
                    request.AddQueryParameter("api_token", Login.token);
                    var response = await client.ExecuteTaskAsync(request);
                    activityIndicator.Visibility = ViewStates.Gone;

                    string content, response_code;
                    content = response.Content;

                    var myContent = JObject.Parse(content);
                    response_code = myContent["response_code"].ToString();

                    if (response_code == "200")
                    {
                        //removing record from wishlist if it exists there
                        //creating wishlist table
                        dbr.CreateWishlistTable();

                        //declaring path for RETRIEVING DATA
                        string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                        var db = new SQLiteConnection(dbPath);
                        var wishlist_table = db.Table<ORM.Wishlist>();
                        //declaring path for RETRIEVING DATA ENDED

                        //checking if the place of interest exists
                        foreach (var item in wishlist_table)
                        {
                            count_data_rows_in_users_table = 1;
                            if (item.name == title.Text)
                            {
                                dbr.RemoveWishlistRecord(item.Id);
                                break;
                            }
                        }
                        //removing record from wishlist if it exists there ENDED
                        Toast.MakeText(this, "You have removed \"" + text + "\"", ToastLength.Short).Show();
                        StartActivity(typeof(MyBookings));
                    }
                    else if (response_code == "400")
                    {
                        Toast.MakeText(this, "Unable to remove \"" + text + "\"", ToastLength.Short).Show();
                    }
                });
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            };
        }

        public void OnScrollDown()
        {
            throw new NotImplementedException();
        }

        public void OnScrollUp()
        {
            throw new NotImplementedException();
        }
    }
}