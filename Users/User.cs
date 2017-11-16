using Android.Graphics;
using Android.Util;
using Cars;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace Users
{
    public class User
    {
        //public int Pid { set; get; }
        public string Fullname { set; get; }
        public string Email { set; get; }
        public string Phonenumber { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string ImageUrl { set; get; }
        public string BirthDate { set; get; }
        public string Type { set; get; }
        public Bitmap Image { set; get; }
        public Car car { set; get; }

        //public string ip = "rideshare.hopto.org:12000";
        public string ip = "172.20.10.6";
        //public string ip = "192.168.1.12";

        public User() { }
        public User(string name, string email, string phonenum, string username, string password, string url, string type)
        {
            Fullname = name;
            Email = email;
            Phonenumber = phonenum;
            Username = username; 
            Password = password;
            ImageUrl = url;
            Type = type;
        }

        public override string ToString()
        {
            return Fullname;
        }

        public void setImg()
        {
            Log.Info("myapp", "setImg called");
            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + ip + "/get_user_img.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", Username);

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            
            try
            {
                string receivedString = Encoding.UTF8.GetString(args.Result);

                WebClient client = new WebClient();
                Uri uri = new Uri("http://" + ip + "/" + receivedString.Trim());

                client.DownloadDataCompleted += LoadImg;
                client.DownloadDataAsync(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                Console.WriteLine("Inner Exception is {0}", e.InnerException);
            }

        }

        void LoadImg(object sender, DownloadDataCompletedEventArgs args)
        {
            try
            {
                var bytes = args.Result; // get the downloaded data
                OnGetImageCompleted(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                Console.WriteLine("Inner Exception is {0}", e.InnerException);
            }

        }

        private void OnGetImageCompleted(byte[] avatarBytes)
        {
            Log.Info("myapp", "GetImageCompleted");

            var imageBitmap = BitmapFactory.DecodeByteArray(avatarBytes, 0, avatarBytes.Length);
            Image = imageBitmap;
           
        }

        public void setCar()
        {
            WebClient client = new WebClient();
            Uri uri = new Uri("http://" + ip + "/get_car.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", Username);

            client.UploadValuesCompleted += uploadCarValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        private void uploadCarValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            try
            {
                string json = Encoding.UTF8.GetString(args.Result);
                car = JsonConvert.DeserializeObject<Car>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                Console.WriteLine("Inner Exception is {0}", e.InnerException);
            }
        }
    }
}
