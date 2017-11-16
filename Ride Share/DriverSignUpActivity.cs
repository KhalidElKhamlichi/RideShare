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
using System.IO;
using Android.Graphics;
using System.Collections.Specialized;
using Android.Database;
using Android.Provider;
using Android.Util;
using Java.Lang;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;


namespace Ride_Share
{
    [Activity(Label = "Signup", Theme = "@style/Theme.DesignDemo")]
    public class DriverSignUpActivity : Android.Support.V7.App.AppCompatActivity
    {
        ImageView LicenseImg;
        Button SignUpBtn;
        Android.Net.Uri ImageURI;
        EditText LicensePlate, CarType, CarColor;
        string fullPathToImage;
        string Username;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Username = Intent.GetStringExtra("Username");

            SetContentView(Resource.Layout.DriverSignUp);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            SignUpBtn = FindViewById<Button>(Resource.Id.CompleteSignUpBtn);
            LicensePlate = FindViewById<EditText>(Resource.Id.LicensePlateInput);
            CarType = FindViewById<EditText>(Resource.Id.CarTypeInput);
            CarColor = FindViewById<EditText>(Resource.Id.CarColorInput);
            LicenseImg = FindViewById<ImageView>(Resource.Id.LicenseImage);

            SignUpBtn.Click += CompleteSignUp;

            LicenseImg.Click += delegate {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                LicenseImg = FindViewById<ImageView>(Resource.Id.LicenseImage);
                LicenseImg.SetImageURI(data.Data);
                ImageURI = data.Data;
            }
        }

        void CompleteSignUp(object sender, EventArgs args)
        {            
            ICursor cursor = null;
            try
            {
                var docID = DocumentsContract.GetDocumentId(ImageURI);
                var id = docID.Split(':')[1];
                var whereSelect = MediaStore.Images.ImageColumns.Id + "=?";
                var projections = new string[] { MediaStore.Images.ImageColumns.Data };
                // Try internal storage first
                cursor = ContentResolver.Query(MediaStore.Images.Media.InternalContentUri, projections, whereSelect, new string[] { id }, null);
                if (cursor.Count == 0)
                {
                    // not found on internal storage, try external storage
                    cursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, projections, whereSelect, new string[] { id }, null);
                }
                var colData = cursor.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Data);
                cursor.MoveToFirst();
                fullPathToImage = cursor.GetString(colData);
                
                //Toast.MakeText(this, Str, ToastLength.Long).Show();
            }
            catch (Error err)
            {
                Log.Error("MediaPath", err.Message);
            }
            finally
            {
                cursor?.Close();
                cursor?.Dispose();
            }

            try
            {
                WebClient Client = new WebClient();
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                string ip = Resources.GetString(Resource.String.ServerIP);
                byte[] result = Client.UploadFile("http://" + ip + "/upload_image.php", "POST", fullPathToImage);
                string s = Encoding.UTF8.GetString(result, 0, result.Length);
                //Toast.MakeText(this, s, ToastLength.Long).Show();

                string ImageName = fullPathToImage.Substring(fullPathToImage.LastIndexOf('/') + 1);
                Uri uri = new Uri("http://" + ip + "/update_driver.php");
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("img_name", ImageName);
                parameters.Add("license_plate", LicensePlate.Text);
                parameters.Add("car_type", CarType.Text);
                parameters.Add("car_color", CarColor.Text);
                parameters.Add("username", Username);

                Client.UploadValuesCompleted += uploadValueComplete;
                Client.UploadValuesAsync(uri, parameters);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                Console.WriteLine("Inner Exception is {0}", e.InnerException);
            }

            
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {
            try
            {
                string receivedString = Encoding.UTF8.GetString(args.Result);

                var dialog = new AlertDialog.Builder(this);
                dialog.SetMessage(receivedString);
                dialog.SetNeutralButton("OK", delegate
                {

                });
                dialog.Show();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                Console.WriteLine("Inner Exception is {0}", e.InnerException);
            }
            
           
            var intent = new Intent(this, typeof(HomeActivity));
            intent.PutExtra("Username", Username);
            StartActivity(intent);
            Finish();

        }
    }
}