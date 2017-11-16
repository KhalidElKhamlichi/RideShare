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

namespace Ride_Share.Fragments
{
    public class MyRidesDemandsFragment : SupportFragment
    {
        public static List<Ride> rides { get; set; }
        private string username;
        private ProgressBar pb;
        private WebClient client;
        private Uri uri;
        private ListView RidesListView;
        private SwipeRefreshLayout refreshLayout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            username = HomeActivity.currentUser.Username;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            Log.Info("myapp", "MyRidesDemands oncreateview called");

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
            parameters.Add("username", username);
            parameters.Add("my_demands", "true");

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
                    Console.WriteLine("demands rides: " + rides.Count);

                    for (int j = rides.Count - 1; j >= 0; j--)
                    {
                        if (rides[j].Submitter.Equals(HomeActivity.currentUser.Username, StringComparison.InvariantCultureIgnoreCase) && rides[j].Driver.Equals("empty"))
                        {
                            rides[j].setUserSubmitter();
                        }
                        else
                        {
                            rides.RemoveAt(j);
                        }

                    }
                    //i = rides.Count - 1;
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
                        if (rides[i].UserSubmitter != null)
                        {
                            if (rides[i].UserSubmitter.Image != null)
                            {
                                Log.Info("myapp", "Adapter called");
                                Console.WriteLine("demands rides: "+rides.Count);
                                var adapter = new MyRideAdapter(this.Activity, rides);
                                RidesListView.Adapter = adapter;
                                pb.Visibility = ViewStates.Gone;
                                done = true;
                            }
                        }

                    }
                }
                else
                {
                    Console.WriteLine("demands no rides");
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
            intent.PutExtra("caller", "my demands");
            StartActivity(intent);
        }


        public void setUsername(string Username)
        {
            username = Username;
        }
    }
}