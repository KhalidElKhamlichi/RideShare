using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Rides;
using System.Net;
using Android.Support.V4.Widget;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Android.Widget.AdapterView;
using Android.Graphics;
using SupportFragment = Android.Support.V4.App.Fragment;
using System.Collections.Specialized;
using System.Globalization;

namespace Ride_Share.Fragments
{
    public class OffersFragment : SupportFragment
    {
        public static List<Ride> rides { get; set; }
        private ProgressBar pb;
        private WebClient client;
        private Uri uri;
        private ListView RidesListView;
        private SwipeRefreshLayout refreshLayout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Info("myapp", "offers OnCreate called");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = null;
            Log.Info("myapp", "offers oncreateview called");
            try
            {
                view = inflater.Inflate(Resource.Layout.Offers, container, false);

                pb = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
                refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                refreshLayout.SetColorSchemeColors(Color.Red, Color.Green, Color.Blue, Color.Yellow);
                refreshLayout.Refresh += RefreshLayout_Refresh;

                RidesListView = view.FindViewById<ListView>(Resource.Id.rides_listview);
                if (rides == null)
                    getRides();
                else
                    showRides();

                RidesListView.ItemClick += OnItemClick;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            rides = null;
        }

        private void showRides()
        {
            Activity.RunOnUiThread(() =>
            {
                Log.Info("myapp", "offers showRides called");
                int i = rides.Count - 1;
                if (i >= 0)
                {
                    bool done = false;
                    while (!done)
                    {
                        if (rides[i].Userdriver != null)
                        {
                            if (rides[i].Userdriver.Image != null)
                            {
                                Log.Info("myapp", "Adapter called");
                                var adapter = new RideAdapter(this.Activity, rides);
                                RidesListView.Adapter = adapter;
                                pb.Visibility = ViewStates.Gone;
                                done = true;
                            }
                        }

                    }

                }
                else
                {
                    var adapter = new RideAdapter(this.Activity, rides);
                    RidesListView.Adapter = adapter;
                    pb.Visibility = ViewStates.Gone;
                }
            });
        }

        private void getRides()
        {
            Log.Info("myapp", "offers getRides called");

            client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            uri = new Uri("http://" + ip + "/get_rides.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", HomeActivity.currentUser.Username);

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            int i = -1;
            Task task = new Task(() => {
                try
                {
                    Log.Info("myapp", "Task called");
                    string json = Encoding.UTF8.GetString(args.Result);
                    rides = JsonConvert.DeserializeObject<List<Ride>>(json);
                   
                    for (int j = rides.Count - 1; j >= 0; j--)
                    {
                        if (!rides[j].Driver.Equals("empty"))
                        {
                            rides[j].setUserDriver();
                        }
                        else
                        {
                            rides.RemoveAt(j);
                        }

                    }
                   
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                    Console.WriteLine("Inner Exception is {0}", e.InnerException);
                }
                
            });


            
                Activity.RunOnUiThread(() =>
                {
                    task.Start();
                    task.Wait();
                    i = rides.Count - 1;
                    if (i >= 0)
                    {
                        bool done = false;
                        while (!done)
                        {
                            if (rides[i].Userdriver != null)
                            {
                                if (rides[i].Userdriver.Image != null)
                                {
                                    Log.Info("myapp", "Adapter called");
                                    var adapter = new RideAdapter(this.Activity, rides);
                                    RidesListView.Adapter = adapter;
                                    pb.Visibility = ViewStates.Gone;
                                    done = true;
                                }
                            }

                        }

                    }
                    else
                    {
                        var adapter = new RideAdapter(this.Activity, rides);
                        RidesListView.Adapter = adapter;
                        pb.Visibility = ViewStates.Gone;
                    }
                });
            
            
        }

        public void OnSearch(object sender, DialogEventArgs e)
        {
            //Log.Info("myapp", "time: " + e.Time+" date: "+e.Date+" origin: "+e.Origin + " dest: " + e.Destination);

            DateTime dateTime = DateTime.ParseExact(e.Time, "HH:mm",
                                        CultureInfo.InvariantCulture);
            DateTime dtplus = dateTime.AddHours(e.TimeFlex);
            DateTime dtminus = dateTime.AddHours(-e.TimeFlex);
            Log.Info("myapp", "time: " + (DateTime.Compare(DateTime.ParseExact(e.Time, "HH:mm", CultureInfo.InvariantCulture), dtplus) <= 0));
            //Log.Info("myapp", "old: " + dateTime.ToString("HH:mm") + " new: " + dt.ToString("HH:mm"));
            List<Ride> searchedRides = (from ride in rides
                                        where ride.Origin.Contains(e.Origin, StringComparison.OrdinalIgnoreCase) && ride.Destination.Contains(e.Destination, StringComparison.OrdinalIgnoreCase) &&
                                        DateTime.Compare(DateTime.ParseExact(e.Time, "HH:mm", CultureInfo.InvariantCulture), dtplus) <= 0 &&
                                        DateTime.Compare(DateTime.ParseExact(e.Time, "HH:mm", CultureInfo.InvariantCulture), dtminus) >= 0 &&
                                        ride.Date.Contains(e.Date, StringComparison.OrdinalIgnoreCase) select ride).ToList<Ride>();
            Log.Info("myapp", "list size: " + searchedRides.Count);
            var adapter = new RideAdapter(Activity, searchedRides);
            RidesListView.Adapter = adapter;
        }

        private void RefreshLayout_Refresh(object sender, EventArgs e)
        {
            //Data Refresh Place  
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.RunWorkerAsync();
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshLayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            getRides();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            Intent intent = new Intent(this.Context, typeof(RideDetailsActivity));
            intent.PutExtra("pos", e.Position.ToString());
            intent.PutExtra("caller", "other rides");
            StartActivity(intent);
        }


    }
}