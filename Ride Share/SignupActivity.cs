using System;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using System.Collections.Specialized;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;


namespace Ride_Share
{
    [Activity(Label = "Signup", Theme = "@style/Theme.DesignDemo")]
    public class SignupActivity : Android.Support.V7.App.AppCompatActivity
    {
        Button signup_btn;
        TextView login;
        EditText Fullname, Email, Phonenumber, Username, Password, confirmPassword;
        RadioGroup radioGroup;
        RadioButton radioButton;

        private string errorMsg = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Signup);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            signup_btn = FindViewById<Button>(Resource.Id.ContinueBtn);
            //login = FindViewById<TextView>(Resource.Id.login);
            Fullname = FindViewById<EditText>(Resource.Id.FullnameInput);
            Email = FindViewById<EditText>(Resource.Id.EmailInput);
            Phonenumber = FindViewById<EditText>(Resource.Id.PhoneInput);
            Username = FindViewById<EditText>(Resource.Id.UsernameInput);
            Password = FindViewById<EditText>(Resource.Id.PasswordInput);
            radioGroup = FindViewById<RadioGroup>(Resource.Id.RadioLayout);
            confirmPassword = FindViewById<EditText>(Resource.Id.ConfirmPasswordInput);

            signup_btn.Click += signupAndLogin;
            //login.Click += goToLogin;
        }

        void signupAndLogin(object sender, EventArgs args)
        {

            if(isInputValid())
            {
                try
                {
                    radioButton = FindViewById<RadioButton>(radioGroup.CheckedRadioButtonId);

                    WebClient client = new WebClient();
                    string ip = Resources.GetString(Resource.String.ServerIP);
                    Uri uri = new Uri("http://" + ip + "/insert_user.php");
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("fullname", Fullname.Text);
                    parameters.Add("email", Email.Text);
                    parameters.Add("phonenumber", Phonenumber.Text);
                    parameters.Add("username", Username.Text);
                    parameters.Add("password", Password.Text);
                    parameters.Add("type", radioButton.Text);

                    client.UploadValuesCompleted += uploadValueComplete;
                    client.UploadValuesAsync(uri, parameters);
                }
                catch (Exception e)
                {
                    var dialog = new AlertDialog.Builder(this);
                    dialog.SetMessage("Server not available");
                    dialog.SetNeutralButton("OK", delegate
                    {

                    });
                    dialog.Show();
                }
                
            }
            else
            {
                var dialog = new AlertDialog.Builder(this);
                dialog.SetMessage(errorMsg);
                dialog.SetNeutralButton("OK", delegate
                {

                });
                dialog.Show();
            }

        }

        void goToLogin(object sender, EventArgs args)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));
            Finish();
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

            HomeActivity.currentUser = new Users.User(Fullname.Text, Email.Text, Phonenumber.Text, Username.Text, Password.Text, "profile1.png", radioButton.Text);

            if (radioButton.Text == "Passenger")
            {
                var intent = new Intent(this, typeof(HomeActivity));
                intent.PutExtra("Username", Username.Text);
                StartActivity(intent);
                Finish();
            }
            else
            {

                var intent = new Intent(this, typeof(DriverSignUpActivity));
                intent.PutExtra("Username", Username.Text);
                StartActivity(intent);
                Finish();
            }
            
        }

        private bool isInputValid()
        {
            bool valid = true;

            if (!Password.Text.Equals(confirmPassword.Text))
            {
                valid = false;
                errorMsg += "Passwords do not match.";
            }

            if (!ValidationMethods.isValidEmail(Email.Text))
            {
                valid = false;
                errorMsg += "\nEmail is not valid.";
            }

            return valid;
        }

        
    }
}