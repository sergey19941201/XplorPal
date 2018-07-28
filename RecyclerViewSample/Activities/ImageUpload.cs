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
using Android.Graphics;
using RestSharp;
using System.IO;
using Android.Util;
using Android.Provider;
using Android.Content.PM;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StarWars.Api.Repository;
using SQLite;
using System.Threading.Tasks;

namespace RecyclerViewSampl
{
    [Activity(Label = "ImageUpload", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ImageUpload : Activity
    {
        //variable to indicate if the user pressed select image from gallery of take a photo button
        private static string cameraOrGalleryIndicator;
        private static string bal;
        private ProgressBar activityIndicator;
        Button uploadImageFromGalleryBn, uploadImageFromCameraBn, cancelBn, confirm_button;
        TextView uploadYourAvatarTV;
        RecyclerViewSample.ORM.DBRepository dbr = new RecyclerViewSample.ORM.DBRepository();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(RecyclerViewSample.Resource.Layout.ImageUpload);

             uploadImageFromGalleryBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.uploadImageFromGalleryBn);
             uploadImageFromCameraBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.uploadImageFromCameraBn);
             uploadYourAvatarTV = FindViewById<TextView>(RecyclerViewSample.Resource.Id.uploadYourAvatarTV);
             cancelBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.cancelBn);
             confirm_button = FindViewById<Button>(RecyclerViewSample.Resource.Id.confirm_button);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            confirm_button.Typeface = tf;
            cancelBn.Typeface = tf;
            uploadYourAvatarTV.Typeface = tf;
            uploadImageFromGalleryBn.Typeface = tf;
            uploadImageFromCameraBn.Typeface = tf;

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

            confirm_button.Click += async delegate
            {
                await UploadingImage();
            };

            cancelBn.Click += delegate
            {
                try
                {
                    activityIndicator.Visibility = ViewStates.Gone;
                    cancelBn.Visibility = ViewStates.Visible;
                    confirm_button.Visibility = ViewStates.Visible;
                    uploadImageFromCameraBn.Visibility = ViewStates.Visible;
                    uploadImageFromGalleryBn.Visibility = ViewStates.Visible;
                }
                catch { }
                StartActivity(typeof(RecyclerViewSample.MainActivity));
            };
        }

        private async Task<string> UploadingImage()
        {
            var image =
                FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);

            activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);

            bal = "";
            //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE
            image.BuildDrawingCache(true);
            Bitmap bitmap = image.GetDrawingCache(true);
            if (bitmap != null)
            {
                uploadYourAvatarTV.Text = "Uploading...";
                activityIndicator.Visibility = ViewStates.Visible;
                cancelBn.Visibility = ViewStates.Gone;
                confirm_button.Visibility = ViewStates.Gone;
                uploadImageFromCameraBn.Visibility = ViewStates.Gone;
                uploadImageFromGalleryBn.Visibility = ViewStates.Gone;
                image.SetImageBitmap(bitmap);
                Bitmap b = Bitmap.CreateBitmap(image.GetDrawingCache(true));
                MemoryStream memStream = new MemoryStream();
                b.Compress(Bitmap.CompressFormat.Png, 0, memStream);
                //memStream.ToArray();
                //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE ENDED
                byte[] ba = memStream.ToArray();
                bal = Base64.EncodeToString(ba, Base64.Default);

                var client = new RestClient("http://api.xplorpal.com/profile/" + RecyclerViewSample.Login.user_id);
                var request = new RestRequest("/set_avatar", Method.POST);
                request.AddParameter("profile_img[]", "data:image/jpeg;base64," + bal);
                request.AddParameter("api_token", RecyclerViewSample.Login.token);

                IRestResponse response = await client.ExecuteTaskAsync(request);
                var content = response.Content;
                var myContent = JObject.Parse(content);
                string response_code = myContent["response_code"].ToString();
                if (response_code == "200")
                {
                    //NOW WE NEED TO GET IMAGE URL TO WRITE IT TO DATABASE
                    var image_retrieving_client = new RestClient("http://api.xplorpal.com");
                    var image_retrieving_request = new RestRequest("/login", Method.POST);
                    image_retrieving_request.AddParameter("email", RecyclerViewSample.Login.email_);
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
                    /*else
                    {
                        Toast.MakeText(this, "Check your Password", ToastLength.Long).Show();
                        StartActivity(typeof(ImageUpload));
                    }*/
                    //NOW WE NEED TO GET IMAGE URL TO WRITE IT TO DATABASE ENDED
                    string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                    var db = new SQLiteConnection(dbPath);
                    var user_table = db.Table<RecyclerViewSample.ORM.UsersDataTable>();
                    // clearing table
                    foreach (var item in user_table)
                    {
                        dbr.RemoveUsersData(item.Id);
                    }

                    /*//setting variables 
                    RecyclerViewSample.Login.gender = maleOrFemale.ToString();
                    RecyclerViewSample.Login.name = name.Text;
                    RecyclerViewSample.Login.birth_date = birthdate.Text;
                    RecyclerViewSample.Login.email_ = email.Text;
                    RecyclerViewSample.Login.phone_num = phone_num.Text;
                    RecyclerViewSample.Login.interests = bio.Text;
                    RecyclerViewSample.Login.gender = maleOrFemale.ToString();
                    //setting variables ENDED*/

                    //inserting to users table
                    dbr.InsertRecord(RecyclerViewSample.Login.name, RecyclerViewSample.Login.email_,
                        RecyclerViewSample.Login.avatar, RecyclerViewSample.Login.token,
                        RecyclerViewSample.Login.birth_date, RecyclerViewSample.Login.gender,
                        RecyclerViewSample.Login.phone_num, RecyclerViewSample.Login.interests,
                                     RecyclerViewSample.Login.user_country_id,
                        RecyclerViewSample.Login.user_id, RecyclerViewSample.Login.password);
                }
                //Console.WriteLine(content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                    cancelBn.Visibility = ViewStates.Visible;
                    confirm_button.Visibility = ViewStates.Visible;
                    uploadImageFromCameraBn.Visibility = ViewStates.Gone;
                    uploadImageFromGalleryBn.Visibility = ViewStates.Gone;
                    Toast.MakeText(this, "Image Uploaded", ToastLength.Long).Show();
                    StartActivity(typeof(RecyclerViewSample.MainActivity));
                }
                else
                {

                }
            }
            else if (bitmap == null)
            {
                Toast.MakeText(this, "Choose image please", ToastLength.Short).Show();
            }

            return "";
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
    }
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Android.Graphics.Bitmap bitmap;
    }

    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            /*if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }*/

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}
