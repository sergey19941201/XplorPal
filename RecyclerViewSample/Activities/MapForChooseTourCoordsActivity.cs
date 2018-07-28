using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using static Android.Gms.Maps.GoogleMap;
using Android.Views.InputMethods;
using Android.Graphics;
using System.Globalization;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "MapForChooseTourCoordsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MapForChooseTourCoordsActivity : Activity, IOnMapReadyCallback, IOnMapLongClickListener
    {
        string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private CameraUpdate cameraUpdate;
        private static double lat_search_target_users_position, lng_search_target_users_position;
        //this variable is to detect if the title of experience is too long
        private static bool long_title_indicator;
        private static string lat_temp_NEW_start_activity, lng_temp_NEW_start_activity, lat_tmp, lng_tmp;
        public static string addOrEditTourIndicator;
        //chosen place coordinates
        public static string chosenLatOfExp, chosenLngOfExp;
        PlacesOfInterstInfo placesOfInterstInfo = new PlacesOfInterstInfo();
        //users place location marker
        Marker usersPlaceLocationMarker;
        Button applyChangesBn, searchBn, cenrterPosBn;
        EditText searchET;
        TextView longTapTV;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MapForChooseTourCoords);

            applyChangesBn = FindViewById<Button>(Resource.Id.applyChangesBn);
            searchBn = FindViewById<Button>(Resource.Id.searchBn);
            searchET = FindViewById<EditText>(Resource.Id.searchET);
            longTapTV = FindViewById<TextView>(Resource.Id.longTapTV);
            applyChangesBn.Visibility = ViewStates.Gone;
            //Button for centering the location of the user
            cenrterPosBn = FindViewById<Button>(Resource.Id.centerPositionBn);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            applyChangesBn.Typeface = tf;
            searchBn.Typeface = tf;
            searchET.Typeface = tf;
            longTapTV.Typeface = tf;
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(false)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();

                lat_temp_NEW_start_activity = null;
                lng_temp_NEW_start_activity = null;
            }
            _mapFragment.GetMapAsync(this);

            if (ChangeDestination.changedDestinationIndicator == true)
            {
                if (Tours_detail.searchOrMovieAdapterIndicator != "SearchAdapter")
                {
                    string lat_replaced = ChangeDestination.lat.ToString();
                    string lng_replaced = ChangeDestination.lng.ToString();
                    if (lat_replaced.Contains(","))
                    {
                        lat_replaced = ChangeDestination.lat.ToString().Replace(',', '.');
                    }
                    if (lng_replaced.Contains(","))
                    {
                        lng_replaced = ChangeDestination.lng.ToString().Replace(',', '.');
                    }

                    LatLng target_location = new LatLng(Convert.ToDouble(lat_replaced, (CultureInfo.InvariantCulture)),
                        Convert.ToDouble(lng_replaced, (CultureInfo.InvariantCulture)));
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                }
                else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                {
                    foreach (var o in SearchAdapter.experiencesStatic)
                    {
                        if (!String.IsNullOrWhiteSpace(o.lat) && !String.IsNullOrWhiteSpace(o.lng))
                        {
                            foreach (char c in o.lat)
                            {
                                if (c == ',')
                                {
                                    lat_tmp += ".";
                                }
                                else
                                {
                                    lat_tmp += c;
                                }
                            }
                            foreach (char c in o.lng)
                            {
                                if (c == ',')
                                {
                                    lng_tmp += ".";
                                }
                                else
                                {
                                    lng_tmp += c;
                                }
                            }
                            lat_search_target_users_position = Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)) + 0.005;
                            lng_search_target_users_position = Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture)) + 0.005;

                            lat_tmp = "";
                            lng_tmp = "";
                        }
                    }
                    LatLng target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                }
                _mapFragment.GetMapAsync(this);
            }
            // _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;


            placesOfInterstInfo.places_of_interest();
            if (ChangeDestination.changedDestinationIndicator == true)
            {

            }
            if (ChangeDestination.changedDestinationIndicator != true)
            {
                if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter" || String.IsNullOrWhiteSpace(Tours_detail.searchOrMovieAdapterIndicator))
                {
                    string lat_replaced = NEWstartActivity.lat;
                    string lng_replaced = NEWstartActivity.lon;
                    if (lat_replaced.Contains(","))
                    {
                        lat_replaced = NEWstartActivity.lat.Replace(',', '.');
                    }
                    if (lng_replaced.Contains(","))
                    {
                        lng_replaced = NEWstartActivity.lon.Replace(',', '.');
                    }
                    lat_temp_NEW_start_activity = lat_replaced;
                    lng_temp_NEW_start_activity = lng_replaced;
                    /*//replacing dot instead of comma in coordinates
                    foreach (char c in Activities.NEWstartActivity.lat)
                    {
                        if (c == ',')
                        {
                            lat_temp_NEW_start_activity += ".";
                        }
                        else
                        {
                            lat_temp_NEW_start_activity += c;
                        }
                    }
                    foreach (char c in Activities.NEWstartActivity.lon)
                    {
                        if (c == ',')
                        {
                            lng_temp_NEW_start_activity += ".";
                        }
                        else
                        {
                            lng_temp_NEW_start_activity += c;
                        }
                    }
                    //replacing dot instead of comma in coordinates ENDED*/

                    //LatLng location = new LatLng(Convert.ToDouble(Activities.NEWstartActivity.lat), Convert.ToDouble(Activities.NEWstartActivity.lon));
                    try
                    {
                        LatLng location = new LatLng(Convert.ToDouble(lat_temp_NEW_start_activity, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_temp_NEW_start_activity, (CultureInfo.InvariantCulture)));
                        /*lat_temp_NEW_start_activity = "";
                        lng_temp_NEW_start_activity = "";*/
                        CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                        builder.Target(location);
                        builder.Zoom(15);
                        /*builder.Bearing(155);
                        builder.Tilt(65);*/
                        CameraPosition cameraPosition = builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                        _mapFragment.GetMapAsync(this);
                    }
                    catch { }
                }
                else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                {
                    foreach (var o in SearchAdapter.experiencesStatic)
                    {
                        if (!String.IsNullOrWhiteSpace(o.lat) && !String.IsNullOrWhiteSpace(o.lng))
                        {
                            foreach (char c in o.lat)
                            {
                                if (c == ',')
                                {
                                    lat_tmp += ".";
                                }
                                else
                                {
                                    lat_tmp += c;
                                }
                            }
                            foreach (char c in o.lng)
                            {
                                if (c == ',')
                                {
                                    lng_tmp += ".";
                                }
                                else
                                {
                                    lng_tmp += c;
                                }
                            }

                            lat_search_target_users_position = Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)) + 0.005;
                            lng_search_target_users_position = Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture)) + 0.005;

                            lat_tmp = "";
                            lng_tmp = "";
                        }
                    }
                    //centring camera on target location
                    LatLng target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                    _mapFragment.GetMapAsync(this);
                    //centring camera on target location ENDED
                }
            }
            int p;
            p = 10;
            if (ChangeDestination.changedDestinationIndicator == true)
            {
                /*if (Tours_detail.searchOrMovieAdapterIndicator != "SearchAdapter")
                {
                    LatLng target_location = new LatLng(Convert.ToDouble(ChangeDestination.lat, (CultureInfo.InvariantCulture)),
                        Convert.ToDouble(ChangeDestination.lng, (CultureInfo.InvariantCulture)));
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                }
                else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                {
                    foreach (var o in SearchAdapter.experiencesStatic)
                    {
                        if (!String.IsNullOrWhiteSpace(o.lat) && !String.IsNullOrWhiteSpace(o.lng))
                        {
                            foreach (char c in o.lat)
                            {
                                if (c == ',')
                                {
                                    lat_tmp += ".";
                                }
                                else
                                {
                                    lat_tmp += c;
                                }
                            }
                            foreach (char c in o.lng)
                            {
                                if (c == ',')
                                {
                                    lng_tmp += ".";
                                }
                                else
                                {
                                    lng_tmp += c;
                                }
                            }
                            lat_search_target_users_position = Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)) + 0.005;
                            lng_search_target_users_position = Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture)) + 0.005;

                            lat_tmp = "";
                            lng_tmp = "";
                        }
                    }
                    LatLng target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                }
                _mapFragment.GetMapAsync(this);*/
            }

            searchBn.Click += delegate
            {
                if (!String.IsNullOrWhiteSpace(searchET.Text))
                {
                    //dissmissing keyboard
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(searchET.WindowToken, 0);
                    //dissmissing keyboard ENDED

                    new Thread(new ThreadStart(() =>
                    {
                        var geo = new Geocoder(this);

                        var addresses = geo.GetFromLocationName(searchET.Text, 1);

                        RunOnUiThread(() =>
                        {
                            var addressText = FindViewById<TextView>(Resource.Id.searchET);

                            addresses.ToList().ForEach((addr) =>
                            {

                                LatLng search_location = new LatLng(Convert.ToDouble(addr.Latitude, (CultureInfo.InvariantCulture)),
                                    Convert.ToDouble(addr.Longitude, (CultureInfo.InvariantCulture)));
                                CameraPosition.Builder builder_search = CameraPosition.InvokeBuilder();
                                builder_search.Target(search_location);
                                builder_search.Zoom(15);
                                CameraPosition search_cameraPosition = builder_search.Build();
                                cameraUpdate = CameraUpdateFactory.NewCameraPosition(search_cameraPosition);
                                _mapFragment.GetMapAsync(this);
                            });
                        });
                    })).Start();
                }
            };

            applyChangesBn.Click += delegate
              {
                  if (addOrEditTourIndicator == "edit")
                  {
                      OnBackPressed();
                  }
                  else if (addOrEditTourIndicator == "add")
                  {
                      StartActivity(typeof(RecyclerViewSampl.AddNewTourActivity));
                  }
              };


            cenrterPosBn.Click += (s, e) =>
            {
                if (ChangeDestination.changedDestinationIndicator == false)
                {
                    if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter" || String.IsNullOrWhiteSpace(Tours_detail.searchOrMovieAdapterIndicator))
                    {
                        LatLng location = new LatLng(Convert.ToDouble(lat_temp_NEW_start_activity, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_temp_NEW_start_activity, (CultureInfo.InvariantCulture)));
                        /*lat_temp_NEW_start_activity = "";
                        lng_temp_NEW_start_activity = "";*/
                        CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                        builder.Target(location);
                        builder.Zoom(15);
                        /*builder.Bearing(155);
                        builder.Tilt(65);*/
                        CameraPosition cameraPosition = builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                        _mapFragment.GetMapAsync(this);
                    }
                    else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                    {
                        //centring camera on target location
                        LatLng target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                        CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                        target_builder.Target(target_location);
                        target_builder.Zoom(15);
                        CameraPosition target_cameraPosition = target_builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                        _mapFragment.GetMapAsync(this);
                        //centring camera on target location ENDED 
                    }
                }
                else if (ChangeDestination.changedDestinationIndicator == true)
                {
                    if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter")
                    {
                        LatLng target_location = new LatLng(Convert.ToDouble(ChangeDestination.lat, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(ChangeDestination.lng, (CultureInfo.InvariantCulture)));
                        CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                        target_builder.Target(target_location);
                        target_builder.Zoom(15);
                        CameraPosition target_cameraPosition = target_builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                        _mapFragment.GetMapAsync(this);
                    }
                    else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                    {
                        //centring camera on target location
                        LatLng target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                        CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                        target_builder.Target(target_location);
                        target_builder.Zoom(15);
                        CameraPosition target_cameraPosition = target_builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                        _mapFragment.GetMapAsync(this);
                        //centring camera on target location ENDED
                    }
                }
            };
        }
        public void OnMapReady(GoogleMap map)
        {
            _map = map;
            if (_map != null)
            {
                _map.UiSettings.ZoomControlsEnabled = true;
                _map.UiSettings.CompassEnabled = true;
                _map.MoveCamera(cameraUpdate);
                _map.SetOnMapLongClickListener(this);

                //adding current_users_location_marker
                MarkerOptions current_users_location_marker = new MarkerOptions();
                if (ChangeDestination.changedDestinationIndicator == false)
                {
                    current_users_location_marker.SetPosition(new LatLng(Convert.ToDouble(lat_temp_NEW_start_activity, (CultureInfo.InvariantCulture)),
                        Convert.ToDouble(lng_temp_NEW_start_activity, (CultureInfo.InvariantCulture))));
                    current_users_location_marker.SetTitle("Your current location");
                }
                else if (ChangeDestination.changedDestinationIndicator == true)
                {
                    current_users_location_marker.SetPosition(new LatLng(ChangeDestination.lat, ChangeDestination.lng));
                    current_users_location_marker.SetTitle("Your target location");
                }
                BitmapDescriptor image = BitmapDescriptorFactory.FromResource(Resource.Drawable.currentLocation);
                current_users_location_marker.SetIcon(image);

                if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter")
                {
                    if (MovieAdapter.moviesStatic != null)
                    {
                        foreach (var i in MovieAdapter.moviesStatic)
                        {
                            if (!String.IsNullOrWhiteSpace(i.lat) && !String.IsNullOrWhiteSpace(i.lng))
                            {
                                foreach (char c in i.lat)
                                {
                                    if (c == ',')
                                    {
                                        lat_tmp += ".";
                                    }
                                    else
                                    {
                                        lat_tmp += c;
                                    }
                                }
                                foreach (char c in i.lng)
                                {
                                    if (c == ',')
                                    {
                                        lng_tmp += ".";
                                    }
                                    else
                                    {
                                        lng_tmp += c;
                                    }
                                }

                                MarkerOptions place_of_interest_marker = new MarkerOptions();
                                place_of_interest_marker.SetPosition(new LatLng(Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)),
                                    Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture))));
                                lat_tmp = "";
                                lng_tmp = "";

                                long_title_indicator = false;

                                //making title shorter
                                int count_char_in_title = 0;
                                string short_title = "";
                                foreach (char c in i.title)
                                {
                                    count_char_in_title++;
                                    short_title += c;
                                    if (count_char_in_title == 32)
                                    {
                                        long_title_indicator = true;
                                        break;
                                    }
                                }
                                //making title shorter ENDED

                                if (long_title_indicator == false)
                                {
                                    place_of_interest_marker.SetTitle(short_title + ". Price: " + i.price + "$");
                                }
                                else if (long_title_indicator == true)
                                {
                                    place_of_interest_marker.SetTitle(short_title + "... Price: " + i.price + "$");
                                }

                                _map.AddMarker(place_of_interest_marker);
                            }
                        }
                    }
                }
                else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                {
                    foreach (var i in SearchAdapter.experiencesStatic)
                    {
                        if (!String.IsNullOrWhiteSpace(i.lat) && !String.IsNullOrWhiteSpace(i.lng))
                        {
                            foreach (char c in i.lat)
                            {
                                if (c == ',')
                                {
                                    lat_tmp += ".";
                                }
                                else
                                {
                                    lat_tmp += c;
                                }
                            }
                            foreach (char c in i.lng)
                            {
                                if (c == ',')
                                {
                                    lng_tmp += ".";
                                }
                                else
                                {
                                    lng_tmp += c;
                                }
                            }

                            MarkerOptions place_of_interest_marker = new MarkerOptions();
                            place_of_interest_marker.SetPosition(new LatLng(Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)),
                                Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture))));
                            lat_tmp = "";
                            lng_tmp = "";

                            long_title_indicator = false;

                            //making title shorter
                            int count_char_in_title = 0;
                            string short_title = "";
                            foreach (char c in i.title)
                            {
                                count_char_in_title++;
                                short_title += c;
                                if (count_char_in_title == 32)
                                {
                                    long_title_indicator = true;
                                    break;
                                }
                            }
                            //making title shorter ENDED

                            if (long_title_indicator == false)
                            {
                                place_of_interest_marker.SetTitle(short_title + ". Price: " + i.price + "$");
                            }
                            else if (long_title_indicator == true)
                            {
                                place_of_interest_marker.SetTitle(short_title + "... Price: " + i.price + "$");
                            }

                            _map.AddMarker(place_of_interest_marker);

                        }
                    }

                    current_users_location_marker.SetPosition(new LatLng(lat_search_target_users_position, lng_search_target_users_position));
                    current_users_location_marker.SetTitle("Your target location");
                }
                _map.AddMarker(current_users_location_marker);
                //adding placesOfInterestMarker from items of the recycler list ENDED

                //adding myExperiences markers to map
                foreach (var i in MyBookings.myExpListClassForRecycler)
                {
                    if (!String.IsNullOrWhiteSpace(i._lat) && !String.IsNullOrWhiteSpace(i._lng))
                    {
                        foreach (char c in i._lat)
                        {
                            if (c == ',')
                            {
                                lat_tmp += ".";
                            }
                            else
                            {
                                lat_tmp += c;
                            }
                        }
                        foreach (char c in i._lng)
                        {
                            if (c == ',')
                            {
                                lng_tmp += ".";
                            }
                            else
                            {
                                lng_tmp += c;
                            }
                        }

                        MarkerOptions my_experience_marker = new MarkerOptions();
                        my_experience_marker.SetPosition(new LatLng(Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture))));
                        lat_tmp = "";
                        lng_tmp = "";

                        long_title_indicator = false;

                        //making title shorter
                        int count_char_in_title = 0;
                        string short_title = "";
                        foreach (char c in i._name)
                        {
                            count_char_in_title++;
                            short_title += c;
                            if (count_char_in_title == 32)
                            {
                                long_title_indicator = true;
                                break;
                            }
                        }
                        //making title shorter ENDED

                        if (long_title_indicator == false)
                        {
                            my_experience_marker.SetTitle(short_title + ". Price: " + i._price + "$");
                        }
                        else if (long_title_indicator == true)
                        {
                            my_experience_marker.SetTitle(short_title + "... Price: " + i._price + "$");
                        }


                        BitmapDescriptor my_experience_image = BitmapDescriptorFactory.FromResource(Resource.Drawable.myExpMarker);
                        my_experience_marker.SetIcon(my_experience_image);

                        _map.AddMarker(my_experience_marker);

                        //_map.AddMarker(my_experience_marker);

                    }
                }
                //adding myExperiences markers to map ENDED

                //adding wishlist markers to map
                //declaring path for RETRIEVING DATA
                ORM.DBRepository dbr = new ORM.DBRepository();
                string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
                var db = new SQLiteConnection(dbPath);
                var wishlist_table = db.Table<ORM.Wishlist>();
                //creating wishlist table
                dbr.CreateWishlistTable();
                foreach (var i in wishlist_table)
                {
                    if (!String.IsNullOrWhiteSpace(i.lat) && !String.IsNullOrWhiteSpace(i.lng))
                    {
                        foreach (char c in i.lat)
                        {
                            if (c == ',')
                            {
                                lat_tmp += ".";
                            }
                            else
                            {
                                lat_tmp += c;
                            }
                        }
                        foreach (char c in i.lng)
                        {
                            if (c == ',')
                            {
                                lng_tmp += ".";
                            }
                            else
                            {
                                lng_tmp += c;
                            }
                        }

                        MarkerOptions wishlist_item_marker = new MarkerOptions();
                        wishlist_item_marker.SetPosition(new LatLng(Convert.ToDouble(lat_tmp, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_tmp, (CultureInfo.InvariantCulture))));
                        lat_tmp = "";
                        lng_tmp = "";

                        long_title_indicator = false;

                        //making title shorter
                        int count_char_in_title = 0;
                        string short_title = "";
                        foreach (char c in i.name)
                        {
                            count_char_in_title++;
                            short_title += c;
                            if (count_char_in_title == 32)
                            {
                                long_title_indicator = true;
                                break;
                            }
                        }
                        //making title shorter ENDED

                        if (long_title_indicator == false)
                        {
                            wishlist_item_marker.SetTitle(short_title + ". Price: " + i.price + "$");
                        }
                        else if (long_title_indicator == true)
                        {
                            wishlist_item_marker.SetTitle(short_title + "... Price: " + i.price + "$");
                        }


                        BitmapDescriptor wishlist_image = BitmapDescriptorFactory.FromResource(Resource.Drawable.wishMarker);
                        wishlist_item_marker.SetIcon(wishlist_image);

                        _map.AddMarker(wishlist_item_marker);

                    }
                }
                //adding wishlist markers to map
            }
        }

        void IOnMapLongClickListener.OnMapLongClick(LatLng point)
        {
            if (usersPlaceLocationMarker != null)
            {
                usersPlaceLocationMarker.Remove();
            }
            applyChangesBn.Visibility = ViewStates.Visible;
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(point);
            usersPlaceLocationMarker = _map.AddMarker(markerOptions);
            chosenLatOfExp = point.Latitude.ToString();
            chosenLngOfExp = point.Longitude.ToString();
        }
    }
}