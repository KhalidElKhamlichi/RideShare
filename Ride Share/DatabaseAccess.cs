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

namespace Ride_Share
{
    class DatabaseAccess
    {
        public static WebClient client = new WebClient();

        private static string serverIP = "192.168.1.12";

        public static void RegisterUser(string Fullname, string Email, string Phonenumber, string Username, string Password)
        {
            Uri uri = new Uri("http://"+serverIP+"/insert_user.php");

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("fullname", Fullname);
            parameters.Add("email", Email);
            parameters.Add("phonenumber", Phonenumber);
            parameters.Add("username", Username);
            parameters.Add("password", Password);

            client.UploadValuesAsync(uri, parameters);


        }
    }
}