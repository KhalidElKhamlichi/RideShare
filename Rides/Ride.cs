using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

using Users;
using System.Collections.Specialized;
using Android.Util;

namespace Rides
{
    public class Ride
    {
        public int Id;
        public string Origin { set; get; }
        public string Destination { set; get; }
        public string Date { set; get; }
        public string Time { set; get; }
        public bool Stops { set; get; }
        public bool Smokes { set; get; }
        public int Seats { set; get; }
        public string Backbag { set; get; }
        public string Travelbag { set; get; }
        public string Driver;
        public string Submitter;
        public User Userdriver { set; get; }
        public User UserSubmitter { set; get; }

        //public string ip = "rideshare.hopto.org:12000";
        public string ip = "172.20.10.6";
        //public string ip = "192.168.1.12";
        public Ride() { }
        public Ride(string origin, string destination, string date, string time, bool stops, bool smokes, int seats, string backbag, string travelbag, string driver)
        {
            Origin = origin;
            Destination = destination;
            Date = date;
            Time = time;
            Stops = stops;
            Smokes = smokes;
            Seats = seats;
            Backbag = backbag;
            Travelbag = travelbag;
            Driver = driver;
            //setUserDriver();
        }

        public override string ToString()
        {
            return "From: "+Origin+" To: "+Destination;
        }

        public void setUserDriver()
        {
            Log.Info("myapp", "setUser called");
            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + ip + "/get_user.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", Driver);

            client.UploadValuesCompleted += uploadUserValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadUserValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {           
            string json = Encoding.UTF8.GetString(args.Result);
            Userdriver = JsonConvert.DeserializeObject<User>(json);
            Userdriver.setImg();
            Userdriver.setCar();
        }

        public void setUserSubmitter()
        {
            Log.Info("myapp", "setUser called");
            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + ip + "/get_user.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", Submitter);

            client.UploadValuesCompleted += uploadUserSubmitterValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadUserSubmitterValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            string json = Encoding.UTF8.GetString(args.Result);
            UserSubmitter = JsonConvert.DeserializeObject<User>(json);
            UserSubmitter.setImg();
        }


    }
}
