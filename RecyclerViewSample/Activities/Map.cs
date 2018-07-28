using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using StarWars.Api.Repository;
using RecyclerViewSample.ORM;
using System.IO;
using SQLite;
using RecyclerViewSample.Activities;
using Android.Content.PM;
using System.Globalization;

namespace RecyclerViewSample
{
    [Activity(Label = "Map", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Map : Activity, IOnMapReadyCallback
    {
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
        
        /*private static readonly LatLng Passchendaele = new LatLng(50.897778, 3.013333);
        private static readonly LatLng VimyRidge = new LatLng(50.379444, 2.773611);*/
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private CameraUpdate cameraUpdate;
        private static double lat_search_target_users_position, lng_search_target_users_position;
        //this variable is to detect if the title of experience is too long
        private static bool long_title_indicator;
        private static string lat_temp_NEW_start_activity, lng_temp_NEW_start_activity, lat_tmp, lng_tmp;
        PlacesOfInterstInfo placesOfInterstInfo = new PlacesOfInterstInfo();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Map_);
            lat_tmp = "";
            lng_tmp = "";

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

            placesOfInterstInfo.places_of_interest();

            if (ChangeDestination.changedDestinationIndicator != true)
            {
                if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter" || String.IsNullOrWhiteSpace(Tours_detail.searchOrMovieAdapterIndicator))
                {
                    lat_temp_NEW_start_activity = Activities.NEWstartActivity.lat;
                    lng_temp_NEW_start_activity = Activities.NEWstartActivity.lon;

                    //CultureInfo.InvariantCulture to prevent troubles while executing Convert.ToDouble in different language settings
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
                    foreach (var o in SearchAdapter.experiencesStatic)
                    {
                        if (!String.IsNullOrWhiteSpace(o.lat) && !String.IsNullOrWhiteSpace(o.lng))
                        {
                            /*lat_tmp = o.lat;
                            lng_tmp = o.lng;*/
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
                    LatLng target_location;

                    if (lat_search_target_users_position != 0 && lng_search_target_users_position != 0)
                    {
                        target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                    }
                    else
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
                        target_location = new LatLng(Convert.ToDouble(lat_replaced, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_replaced, (CultureInfo.InvariantCulture)));

                    }

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
                if (Tours_detail.searchOrMovieAdapterIndicator != "SearchAdapter")
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
                            /*lat_tmp = o.lat;
                            lng_tmp = o.lng;*/
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
                    LatLng target_location;

                    if (lat_search_target_users_position != 0 && lng_search_target_users_position != 0)
                    {
                        target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                    }
                    else
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
                        target_location = new LatLng(Convert.ToDouble(lat_replaced, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_replaced, (CultureInfo.InvariantCulture)));

                    }
                    CameraPosition.Builder target_builder = CameraPosition.InvokeBuilder();
                    target_builder.Target(target_location);
                    target_builder.Zoom(15);
                    CameraPosition target_cameraPosition = target_builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewCameraPosition(target_cameraPosition);
                }
                _mapFragment.GetMapAsync(this);
            }
            //Button for centering the location of the user
            FindViewById<Button>(Resource.Id.centerPositionBn).Click += delegate
            {
                if (ChangeDestination.changedDestinationIndicator == false)
                {
                    if (Tours_detail.searchOrMovieAdapterIndicator == "MovieAdapter" || String.IsNullOrWhiteSpace(Tours_detail.searchOrMovieAdapterIndicator))
                    {
                        _mapFragment.GetMapAsync(this);
                    }
                    else if (Tours_detail.searchOrMovieAdapterIndicator == "SearchAdapter")
                    {
                        //centring camera on target location
                        LatLng target_location;
                        if (lat_search_target_users_position != 0 && lng_search_target_users_position != 0)
                        {
                            target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                        }
                        else
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
                            target_location = new LatLng(Convert.ToDouble(lat_replaced
                                , (CultureInfo.InvariantCulture)), Convert.ToDouble(lng_replaced,
                                (CultureInfo.InvariantCulture)));

                        }
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
                        LatLng target_location;
                        if (lat_search_target_users_position != 0 && lng_search_target_users_position != 0)
                        {
                            target_location = new LatLng(lat_search_target_users_position, lng_search_target_users_position);
                        }
                        else
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
                            target_location = new LatLng(Convert.ToDouble(lat_replaced, (CultureInfo.InvariantCulture)),
                                Convert.ToDouble(lng_replaced, (CultureInfo.InvariantCulture)));

                        }
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
                                /*lat_tmp = i.lat;
                                lng_tmp = i.lng;*/
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
                            /*lat_tmp = i.lat;
                            lng_tmp = i.lng;*/
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

                    if (lat_search_target_users_position != 0 && lng_search_target_users_position != 0)
                    {
                        current_users_location_marker.SetPosition(new LatLng(lat_search_target_users_position, lng_search_target_users_position));
                        current_users_location_marker.SetTitle("Your target location");
                    }
                    else
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
                        current_users_location_marker.SetPosition(new LatLng(Convert.ToDouble(lat_replaced, (CultureInfo.InvariantCulture)),
                            Convert.ToDouble(lng_replaced, (CultureInfo.InvariantCulture))));
                        current_users_location_marker.SetTitle("Your current location");
                    }
                }
                _map.AddMarker(current_users_location_marker);
                //adding placesOfInterestMarker from items of the recycler list ENDED

                //adding myExperiences markers to map
                foreach (var i in MyBookings.myExpListClassForRecycler)
                {
                    if (!String.IsNullOrWhiteSpace(i._lat) && !String.IsNullOrWhiteSpace(i._lng))
                    {
                        /*lat_tmp = i._lat;
                        lng_tmp = i._lng;*/
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

                        _map.AddMarker(my_experience_marker);

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
                        /*lat_tmp = i.lat;
                        lng_tmp = i.lng;*/
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
    }
}