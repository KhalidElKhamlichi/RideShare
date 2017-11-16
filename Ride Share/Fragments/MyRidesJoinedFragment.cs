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
using SupportFragment = Android.Support.V4.App.Fragment;
using System.Net;
using Rides;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using static Android.Widget.AdapterView;
using System.Collections.Specialized;
using Android.Graphics;

namespace Ride_Share.Fragments
{
    public class MyRidesJoinedFragment : SupportFragment
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

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            Log.Info("myapp", "MyRidesJoined oncreateview called");

            View view = inflater.Inflate(Resource.Layout.Offers, container, false);

            pb = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            refreshLayout.SetColorSchemeColors(Color.Red, Color.Green, Color.Blue, Color.Yellow);
            refreshLayout.Refresh += RefreshLayout_Refresh;

            RidesListView = view.FindViewById<ListView>(Resource.Id.rides_listview);

            getRides();

            RidesListView.ItemClick += OnItemClick;

            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            rides = null;
        }

        private void getRides()
        {
            client = new WebClient();

            string ip = Resources.GetString(Resource.String.ServerIP);
            uri = new Uri("http://" + ip + "/get_rides.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", HomeActivity.currentUser.Username);
            parameters.Add("joined", "true");

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            Task task = new Task(() => {
                try
                {
                    Log.Info("myapp", "Task called");
                    string json = Encoding.UTF8.GetString(args.Result);
                    rides = JsonConvert.DeserializeObject<List<Ride>>(json);
                    foreach (Ride r in rides)
                    {
                        r.setUserDriver();

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
                bool done = false;
                if(rides.Count > 0)
                { 
                    while (!done)
                    {
                        if (rides[rides.Count - 1].Userdriver != null)
                        {
                            if (rides[rides.Count - 1].Userdriver.Image != null)
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
            intent.PutExtra("caller", "joined");
            StartActivity(intent);
        }
    }
}