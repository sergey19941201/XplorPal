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
using RestSharp;
using Newtonsoft.Json.Linq;
using RecyclerViewSample.ORM;
using Android.Util;
using Android.Graphics;
using System.IO;
using SQLite;
using Com.Bumptech.Glide;
using Newtonsoft.Json;
using StarWars.Api.Repository;
using Android.Content.PM;
using Android.Provider;

namespace RecyclerViewSampl.Activities
{
    [Activity(Label = "Activity1", ScreenOrientation = ScreenOrientation.Portrait)]
    public class EditProfileActivity : Activity
    {
        private static List<string> genderList = new List<string>();
        private static int maleOrFemale;
        private static string promptMessage;
        private static byte[] picData;
        private ProgressBar activityIndicator;
        //variable to indicate if the user pressed select image from gallery of take a photo button
        private static string cameraOrGalleryIndicator, bal;
        DBRepository dbr = new DBRepository();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(RecyclerViewSample.Resource.Layout.EditProfile);

            var image = FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
            var name = FindViewById<EditText>(RecyclerViewSample.Resource.Id.name);
            var birthdate = FindViewById<EditText>(RecyclerViewSample.Resource.Id.birth_date);
            var email = FindViewById<EditText>(RecyclerViewSample.Resource.Id.email);
            var phone_num = FindViewById<EditText>(RecyclerViewSample.Resource.Id.phone_num);
            var bio = FindViewById<EditText>(RecyclerViewSample.Resource.Id.bio);
            Button uploadImageFromGalleryBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.uploadImageFromGalleryBn);
            Button uploadImageFromCameraBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.uploadImageFromCameraBn);
            Button applyChangesBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.applyChangesBn);
            Button cancelBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.cancelBn);
            Spinner genderSpinner = FindViewById<Spinner>(RecyclerViewSample.Resource.Id.gender_spinner);
            activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            name.Typeface = tf;
            birthdate.Typeface = tf;
            email.Typeface = tf;
            phone_num.Typeface = tf;
            bio.Typeface = tf;
            uploadImageFromGalleryBn.Typeface = tf;
            uploadImageFromCameraBn.Typeface = tf;
            applyChangesBn.Typeface = tf;
            cancelBn.Typeface = tf;

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                var imageView =
                    FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                uploadImageFromCameraBn.Click += TakeAPicture;
            }

            uploadImageFromGalleryBn.Click += delegate
            {
                cameraOrGalleryIndicator = "gallery";
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };
            cancelBn.Click += delegate
            {
                StartActivity(typeof(RecyclerViewSample.Profile));
            };
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<RecyclerViewSample.ORM.UsersDataTable>();

            foreach (var item in user_table)
            {
                RecyclerViewSample.Login.name = item.name;
                RecyclerViewSample.Login.birth_date = item.birth_date;
                RecyclerViewSample.Login.email_ = item.email;
                RecyclerViewSample.Login.phone_num = item.phone_num;
                RecyclerViewSample.Login.interests = item.biography;
                RecyclerViewSample.Login.gender = item.gender;
                RecyclerViewSample.Login.token = item.api_token;
                RecyclerViewSample.Login.user_country_id = item.country_id;
                RecyclerViewSample.Login.user_id = item.user_id;
                Glide.With(Application.Context)
                      .Load(item.avatar)
                      .Into(image);
                RecyclerViewSample.Login.password = item.password;
            }

            name.Text = RecyclerViewSample.Login.name;
            birthdate.Text = RecyclerViewSample.Login.birth_date;
            email.Text = RecyclerViewSample.Login.email_;
            phone_num.Text = RecyclerViewSample.Login.phone_num;
            bio.Text = RecyclerViewSample.Login.interests;
            try
            {
                Glide.With(Application.Context)
                      .Load(RecyclerViewSample.Login.avatar)
                      .Into(image);
            }catch { }

            genderList.Clear();
            genderList.Add("Male");
            genderList.Add("Female");
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, genderList);
            genderSpinner.Adapter = adapter;
            genderSpinner.ItemSelected += GenderSpinner_ItemSelected;

            //setting position to spinner
            if (RecyclerViewSample.Login.gender == "1")
            {
                genderSpinner.SetSelection(0);
            }
            else if (RecyclerViewSample.Login.gender == "2")
            {
                genderSpinner.SetSelection(1);
            }
            //setting position to spinner ENDED

            applyChangesBn.Click += async delegate
            {
                activityIndicator.Visibility = ViewStates.Visible;
                applyChangesBn.Visibility = ViewStates.Gone;
                promptMessage = "";
                if (name.Text == "" || name.Text == null || name.Text == " " || name.Text == "  " || name.Text == "   ")
                {
                    promptMessage += "Name is empty. \n";
                }
                if (birthdate.Text == "" || birthdate.Text == null || birthdate.Text == " " || birthdate.Text == "  " || birthdate.Text == "   ")
                {
                    promptMessage += "Birthdate is empty. \n";
                }
                if (email.Text == "" || email.Text == null || email.Text == " " || email.Text == "  " || email.Text == "   ")
                {
                    promptMessage += "E-mail is empty. \n";
                }
                if (phone_num.Text == "" || phone_num.Text == null || phone_num.Text == " " || phone_num.Text == "  " || phone_num.Text == "   ")
                {
                    promptMessage += "Phone number is empty. \n";
                }
                if (bio.Text.Length < 30)
                {
                    promptMessage += "Your interests must have at least 30 characters. \n";
                }

                if (promptMessage != "")
                {
                    Toast.MakeText(this, promptMessage, ToastLength.Long).Show();
                    applyChangesBn.Visibility = ViewStates.Visible;
                    activityIndicator.Visibility = ViewStates.Gone;
                }
                else
                {
                    bal = "";
                    //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE
                    image.BuildDrawingCache(true);
                    Bitmap bitmap = image.GetDrawingCache(true);
                    if (bitmap != null)
                    {
                        image.SetImageBitmap(bitmap);
                        Bitmap b = Bitmap.CreateBitmap(image.GetDrawingCache(true));
                        MemoryStream memStream = new MemoryStream();
                        b.Compress(Bitmap.CompressFormat.Png, 0, memStream);
                        //memStream.ToArray();
                        //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE ENDED

                        byte[] ba = memStream.ToArray();
                        bal = Base64.EncodeToString(ba, Base64.Default);
                    }


                    var client = new RestClient("http://api.xplorpal.com/profile/" + RecyclerViewSample.Login.user_id);
                    var request = new RestRequest("/guide_update", Method.POST);
                    request.AddParameter("name", name.Text);
                    request.AddParameter("gender", maleOrFemale);
                    request.AddParameter("birth_date", birthdate.Text);
                    request.AddParameter("email", email.Text);
                    request.AddParameter("phone_num", phone_num.Text);
                    request.AddParameter("bio", bio.Text);
                    request.AddParameter("profile_img[]", "data:image/jpeg;base64," + bal);
                    //request.AddParameter("profile_image", "data:image/png;base64," + bal);
                    request.AddParameter("api_token", RecyclerViewSample.Login.token);
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                    var content = response.Content;
                    try
                    {
                        var myContent = JObject.Parse(content);
                        string response_code = myContent["response_code"].ToString();
                        if (response_code == "200")
                        {
                            //NOW WE NEED TO GET IMAGE URL TO WRITE IT TO DATABASE
                            var image_retrieving_client = new RestClient("http://api.xplorpal.com");
                            var image_retrieving_request = new RestRequest("/login", Method.POST);
                            image_retrieving_request.AddParameter("email", email.Text);
                            image_retrieving_request.AddParameter("password", RecyclerViewSample.Login.password);

                            IRestResponse image_retrieving_response = await image_retrieving_client.ExecuteTaskAsync(image_retrieving_request);
                            var image_retrieving_content = image_retrieving_response.Content;

                            if (image_retrieving_content.Length > 100)//response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                try
                                {
                                    var usr = JsonConvert.DeserializeObject<List<User>>(image_retrieving_content);
                                    var usr_ = usr[0];
                                    //removing this part of the content
                                    RecyclerViewSample.Login.avatar = Convert.FromBase64String(usr_.avatar_base64.Replace("data:image/jpeg;base64,", ""));
                                }
                                catch { }
                            }
                            else
                            {
                                Toast.MakeText(this, "Check your Password", ToastLength.Long).Show();
                                StartActivity(typeof(EditProfileActivity));
                            }
                            //NOW WE NEED TO GET IMAGE URL TO WRITE IT TO DATABASE ENDED

                            // clearing table
                            foreach (var item in user_table)
                            {
                                dbr.RemoveUsersData(item.Id);
                            }

                            //setting variables 
                            RecyclerViewSample.Login.gender = maleOrFemale.ToString();
                            RecyclerViewSample.Login.name = name.Text;
                            RecyclerViewSample.Login.birth_date = birthdate.Text;
                            RecyclerViewSample.Login.email_ = email.Text;
                            RecyclerViewSample.Login.phone_num = phone_num.Text;
                            RecyclerViewSample.Login.interests = bio.Text;
                            RecyclerViewSample.Login.gender = maleOrFemale.ToString();
                            //setting variables ENDED

                            //inserting to users table
                            dbr.InsertRecord(name.Text, email.Text, RecyclerViewSample.Login.avatar, RecyclerViewSample.Login.token,
                                birthdate.Text, RecyclerViewSample.Login.gender,
                                             phone_num.Text, bio.Text, RecyclerViewSample.Login.user_country_id, RecyclerViewSample.Login.user_id, RecyclerViewSample.Login.password);

                            Toast.MakeText(this, "Changes adopted", ToastLength.Long).Show();

                            StartActivity(typeof(RecyclerViewSample.MainActivity));
                        }
                        else
                        {
                            applyChangesBn.Visibility = ViewStates.Visible;
                            activityIndicator.Visibility = ViewStates.Gone;
                            Toast.MakeText(this, "Changes not accepted", ToastLength.Long).Show();
                        }
                    }
                    catch { }
                }
            };

        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (cameraOrGalleryIndicator == "gallery")
            {
                if (resultCode == Result.Ok)
                {
                    var imageView =
                        FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                    imageView.Visibility = ViewStates.Visible;
                    imageView.SetImageURI(data.Data);
                }
            }
            else if (cameraOrGalleryIndicator == "camera")
            {
                // Make it available in the gallery

                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);
                // Display in ImageView. We will resize the bitmap to fit the display.
                // Loading the full sized image will consume to much memory
                // and cause the application to crash.
                var imageView =
                        FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = imageView.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    imageView.Visibility = ViewStates.Visible;
                    imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                    //Toast.MakeText(this, "Ôîòêà ñîõðàíåíà", ToastLength.Short).Show();
                }

                // Dispose of the Java side bitmap.
                GC.Collect();
            }
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "XplorPal");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            cameraOrGalleryIndicator = "camera";
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void GenderSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            maleOrFemale = e.Position + 1;
        }
    }
}