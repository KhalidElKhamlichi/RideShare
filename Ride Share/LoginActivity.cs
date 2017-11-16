using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Users;
using Newtonsoft.Json;

namespace Ride_Share
{
    [Activity(Label = "RideShare", MainLauncher = true, Theme = "@style/Theme.DesignDemo")]
    public class LoginActivity : Android.Support.V7.App.AppCompatActivity
    {
        Button login_btn;
        TextView signup;
        EditText Username, Password;
        private ProgressBar pb;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        
            SetContentView(Resource.Layout.Login);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            pb = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            login_btn = FindViewById<Button>(Resource.Id.LoginBtn);
            signup = FindViewById<TextView>(Resource.Id.signup);
            Username = FindViewById<EditText>(Resource.Id.UsernameInput);
            Password = FindViewById<EditText>(Resource.Id.PasswordInput);

            login_btn.Click += Login;
            signup.Click += goToSignup;
        }

        void Login(object sender, EventArgs args)
        {
            try
            {
                pb.Visibility = Android.Views.ViewStates.Visible;
                WebClient client = new WebClient();
                string ip = Resources.GetString(Resource.String.ServerIP);
                Uri uri = new Uri("http://" + ip + "/authenticate_user.php");
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("username", Username.Text);
                parameters.Add("password", Password.Text);

                client.UploadValuesCompleted += uploadLoginValueComplete;
                client.UploadValuesAsync(uri, parameters);
            }
            catch(Exception e)
            {
                var dialog = new AlertDialog.Builder(this);
                dialog.SetMessage("Server not available");
                dialog.SetNeutralButton("OK", delegate
                {

                });
                dialog.Show();
            }
            

        }

        void uploadLoginValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            try
            {
                string receivedString = Encoding.UTF8.GetString(args.Result);
                if (receivedString.Contains("Welcome"))
                {
                    
                    var intent = new Intent(this, typeof(HomeActivity));
                    intent.PutExtra("Username", Username.Text);
                    StartActivity(intent);
                    getUserInfo();
                    Finish();
                }
                var dialog = new AlertDialog.Builder(this);
                dialog.SetMessage(receivedString);
                dialog.SetNeutralButton("OK", delegate
                {

                });
                dialog.Show();
                pb.Visibility = Android.Views.ViewStates.Invisible;
            }
            catch (Exception e)
            {
                Console.WriteLine("THIS IS INNER EXCEPTION: "+e.InnerException);
            }
                       
        }

        void goToSignup(object sender, EventArgs args)
        {
            //StartActivity(new Intent(this, typeof(DriverSignUpActivity)));
            StartActivity(new Intent(this, typeof(SignupActivity)));
            //Finish();
        }

        public void getUserInfo()
        {
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/get_user.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", Username.Text);

            client.UploadValuesCompleted += uploadUserValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadUserValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            string json = Encoding.UTF8.GetString(args.Result);
            HomeActivity.currentUser = JsonConvert.DeserializeObject<User>(json);

        }
    }
}