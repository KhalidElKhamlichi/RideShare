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
using System.Net;
using System.Collections.Specialized;
using Android.Graphics;
using Rides;
using Android.Util;
using Newtonsoft.Json;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using Users;
using Ride_Share.Fragments;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Threading.Tasks;
using Refractored.Controls;

namespace Ride_Share
{
    [Activity(Label = "Details", Theme = "@style/Theme.DesignDemo")]
    public class RideDetailsActivity : Android.Support.V7.App.AppCompatActivity, IOnMapReadyCallback
    {

        private string caller;
        private int pos;
        private Ride ride;
        private User user;
        private TextView FullnameView, PhonenumberView, EmailView, FromView, ToView, DateView, TimeView, RemainingView, CarView, SmokeView, StopView, TravelbagView, BackbagView;
        private Button ActionBtn;
        private CircleImageView photo;
        private Address originAddress;
        private Address destinationAddress;
        private GoogleMap Map;
        private LinearLayout l, l1;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            caller = Intent.GetStringExtra("caller");
            pos = int.Parse(Intent.GetStringExtra("pos"));


            SetContentView(Resource.Layout.RideDetails);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            //var mapFragment = ((SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView));
            var mapFragment = ((WorkaroundMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView));
            mapFragment.GetMapAsync(this);
            ScrollView sv = FindViewById<ScrollView>(Resource.Id.scrollView1);
            if (mapFragment != null)
            {
                mapFragment.TouchUp += (sender, args) => sv.RequestDisallowInterceptTouchEvent(false);
                mapFragment.TouchDown += (sender, args) => sv.RequestDisallowInterceptTouchEvent(true);

            }
                l = FindViewById<LinearLayout>(Resource.Id.InfoLayout);
            l1 = FindViewById<LinearLayout>(Resource.Id.rideInfoLayout3);
            photo = FindViewById<CircleImageView>(Resource.Id.profileImage);
            FullnameView = FindViewById<TextView>(Resource.Id.fullname);
            CarView = FindViewById<TextView>(Resource.Id.car);
            PhonenumberView = FindViewById<TextView>(Resource.Id.phonenumber);
            EmailView = FindViewById<TextView>(Resource.Id.Email);
            FromView = FindViewById<TextView>(Resource.Id.FromView);
            ToView = FindViewById<TextView>(Resource.Id.ToView);
            DateView = FindViewById<TextView>(Resource.Id.DateView);
            TimeView = FindViewById<TextView>(Resource.Id.TimeView);
            RemainingView = FindViewById<TextView>(Resource.Id.RemainingView);
            SmokeView = FindViewById<TextView>(Resource.Id.SmokeView);
            StopView = FindViewById<TextView>(Resource.Id.StopView);
            TravelbagView = FindViewById<TextView>(Resource.Id.TravelbagView);
            BackbagView = FindViewById<TextView>(Resource.Id.BackbagView);
            ActionBtn = FindViewById<Button>(Resource.Id.JoinBtn);

            
            if (caller.Equals("created"))
            {
                ActionBtn.Text = "Cancel";
                ride = MyRidesCreatedFragment.rides[pos];
                ActionBtn.Click += cancelRide;
            }
            else
            {
                if (caller.Equals("joined"))
                {
                    ActionBtn.Text = "Leave";
                    ride = MyRidesJoinedFragment.rides[pos];
                    ActionBtn.Click += leaveRide;
                }
                else
                {
                    if (caller.Equals("demands"))
                    {
                        ActionBtn.Text = "Accept";
                        ride = DemandsFragment.rides[pos];
                        ActionBtn.Click += AcceptRide;
                    }
                    else
                    {
                        if (caller.Equals("my demands"))
                        {
                            ActionBtn.Text = "Cancel";
                            ride = MyRidesDemandsFragment.rides[pos];
                        }
                        else
                        {
                            ride = OffersFragment.rides[pos];
                            ActionBtn.Click += JoinRide;
                        }

                    }

                }

            }

            if (caller.Equals("demands") || caller.Equals("my demands"))
            {
                Log.Info("myapp", "caller: " + caller);
                user = ride.UserSubmitter;
            }
            else
                user = ride.Userdriver;

            RunOnUiThread(() => photo.SetImageBitmap(user.Image));
            FullnameView.Text = user.Fullname;
            PhonenumberView.Text = user.Phonenumber;
            EmailView.Text = user.Email;
            FromView.Text += ride.Origin;
            ToView.Text += ride.Destination;
            DateView.Text += ride.Date;
            TimeView.Text += ride.Time;
            if (ride.Smokes)
                SmokeView.Text += "Yes";
            else
                SmokeView.Text += "No";
            if (ride.Stops)
                StopView.Text += "Yes";
            else
                StopView.Text += "No";
            TravelbagView.Text += ride.Travelbag;
            BackbagView.Text += ride.Backbag;

            if (caller.Equals("demands") || caller.Equals("my demands"))
            {
                RemainingView.Text = "";
                CarView.Text = "";
                l.RemoveView(CarView);
                l1.RemoveView(RemainingView);

                LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(l1.LayoutParameters);
                ll.RightMargin = 200;
                ll.LeftMargin = 200;
                ActionBtn.LayoutParameters = ll;
            }
            else
            {
                RemainingView.Text += ride.Seats;
                CarView.Text = user.car.Type;
            }

            //GeoLocation
            var geo = new Geocoder(this);

            var addressesO = geo.GetFromLocationName(ride.Origin + ", Maroc", 1);
            originAddress = addressesO[0];
            var addressesD = geo.GetFromLocationName(ride.Destination + ", Maroc", 1);
            destinationAddress = addressesD[0];

            string strGoogleDirectionUrl = "https://maps.googleapis.com/maps/api/directions/json?origin=" + ride.Origin + ",Maroc" + "&destination=" + ride.Destination + ",Maroc" + "&key=AIzaSyCS4xW5LbEkzjcd5ATuNnaX4Jss6uyU--c";
            string strJSONDirectionResponse = await FnHttpRequest(strGoogleDirectionUrl);
            var objRoutes = JsonConvert.DeserializeObject<googledirectionclass>(strJSONDirectionResponse);

            string encodedPoints = objRoutes.routes[0].overview_polyline.points;
            var lstDecodedPoints = FnDecodePolylinePoints(encodedPoints);
            //convert list of location point to array of latlng type
            var latLngPoints = new LatLng[lstDecodedPoints.Count];
            int index = 0;
            foreach (Location loc in lstDecodedPoints)
            {
                latLngPoints[index++] = new LatLng(loc.Latitude, loc.Longitude);
            }

            var polylineoption = new PolylineOptions();
            polylineoption.InvokeColor(Color.Red);
            polylineoption.Geodesic(true);
            polylineoption.Add(latLngPoints);
            Map.AddPolyline(polylineoption);
        }

        //function to decode,encoded points
        List<Location> FnDecodePolylinePoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                return null;
            var poly = new List<Location>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylinechars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length && next5bits >= 32)
                    break;

                LocationManager locMgr;
                locMgr = GetSystemService(LocationService) as LocationManager;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                Criteria locationCriteria = new Criteria();

                locationCriteria.Accuracy = Accuracy.Coarse;
                locationCriteria.PowerRequirement = Power.Medium;
                Location p = new Location(locMgr.GetBestProvider(locationCriteria, true));
                p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                poly.Add(p);
            }

            return poly;
        }

        async Task<string> FnHttpRequest(string strUri)
        {
            WebClient webclient = new WebClient();
            string strResultData = "";
            try
            {
                strResultData = await webclient.DownloadStringTaskAsync(new Uri(strUri));
                Console.WriteLine(strResultData);
            }
            catch
            {
                strResultData = "Exception.";
            }
            finally
            {
                if (webclient != null)
                {
                    webclient.Dispose();
                    webclient = null;
                }
            }

            return strResultData;
        }

        public void OnMapReady(GoogleMap map)
        {
            Map = map;
            Map.AddMarker(new MarkerOptions().SetPosition(new LatLng(originAddress.Latitude, originAddress.Longitude)).SetTitle("Origin"));
            Map.AddMarker(new MarkerOptions().SetPosition(new LatLng(destinationAddress.Latitude, destinationAddress.Longitude)).SetTitle("Destination"));
            FnupdateCameraPosition(new LatLng(originAddress.Latitude, originAddress.Longitude));
        }

        void FnupdateCameraPosition(LatLng pos)
        {
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(pos);
            builder.Zoom(7);
            //builder.Bearing(45);
            //builder.Tilt(10);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            Map.AnimateCamera(cameraUpdate);
        }

        private void cancelRide(object sender, EventArgs args)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetMessage("Cancel Ride?");
            builder.SetPositiveButton("Yes", (s, e) => { deleteRide(); });
            builder.SetNegativeButton("No", (s, e) => {});
            builder.Create().Show();
            
        }

        private void leaveRide(object sender, EventArgs args)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetMessage("Leave Ride?");
            builder.SetPositiveButton("Yes", (s, e) => { deleteJoinRide(); });
            builder.SetNegativeButton("No", (s, e) => { });
            builder.Create().Show();

        }

        private void deleteJoinRide()
        {
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/user_leave_ride.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("ride_id", ride.Id.ToString());
            parameters.Add("username", HomeActivity.currentUser.Username);

            client.UploadValuesCompleted += uploadDeleteJoinValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        private void uploadDeleteJoinValueComplete(object sender, UploadValuesCompletedEventArgs e)
        {

            MyRidesJoinedFragment.rides.Remove(ride);
            
            Finish();
        }

        private void deleteRide()
        {
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/delete_ride.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("ride_id", ride.Id.ToString());

            client.UploadValuesCompleted += uploadDeleteValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        private void uploadDeleteValueComplete(object sender, UploadValuesCompletedEventArgs e)
        {
            if(caller.Equals("created"))
            {
                MyRidesCreatedFragment.rides.Remove(ride);
            }
            else
            {
                MyRidesDemandsFragment.rides.Remove(ride);
            }
            Finish();
        }

        private void JoinRide(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/user_join_ride.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", HomeActivity.currentUser.Username);
            parameters.Add("ride_id", ride.Id.ToString());           

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {

            string receivedString = Encoding.UTF8.GetString(args.Result);

            var dialog = new AlertDialog.Builder(this);
            dialog.SetMessage(receivedString);
            dialog.SetNeutralButton("OK", delegate
            {

            });
            dialog.Show();
            if(receivedString.Contains("Successfully joined."))
            {
                ride.Seats--;
                RemainingView.Text = "Remaining: " + ride.Seats;
            }
            if (receivedString.Contains("Successfully accepted."))
            {
                Finish();
            }

            //Finish();
        }

        private void AcceptRide(object sender, EventArgs args)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.AcceptRideDialog, null);
            Android.Support.V7.App.AlertDialog.Builder alertDialogBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDialogBuilder.SetView(view);

            var seats = view.FindViewById<NumberPicker>(Resource.Id.seats);
            seats.MinValue = 1;
            seats.MaxValue = 6;
            seats.Value = 1;

            alertDialogBuilder.SetCancelable(false)
                .SetPositiveButton("Accept", delegate
                {
                    Demand2Ride(seats.Value.ToString());
                })
                .SetNegativeButton("Cancel", delegate
                {
                    alertDialogBuilder.Dispose();
                });

            Android.Support.V7.App.AlertDialog ad = alertDialogBuilder.Create();
            ad.Show();
        }

        private void Demand2Ride(string s)
        {
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/accept_ride.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("ride_id", ride.Id.ToString());
            parameters.Add("username", HomeActivity.currentUser.Username);
            parameters.Add("seats", s);

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

    }
}