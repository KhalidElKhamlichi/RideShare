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
using Android.Graphics;
using System.Net;
using System.Collections.Specialized;
using Java.Lang;
using Android.Util;
using Android.Database;
using Android.Provider;
using Newtonsoft.Json;
using Users;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using System.IO;
using Refractored.Controls;

namespace Ride_Share
{
    [Activity(Label = "Profile", Theme = "@style/Theme.DesignDemo")]
    public class ProfileActivity : Android.Support.V7.App.AppCompatActivity
    {
        TextView Fullname;
        EditText Username, Phonenumber, Date, Email;
        Button Save;
        ImageView ChangeImg;
        CircleImageView ProfileImg;
        DatePickerDialog dateDialog;
        Android.Net.Uri ImageURI;
        string fullPathToImage;
        bool ImgChanged;

        private string errorMsg;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ImgChanged = false;

            SetContentView(Resource.Layout.Profile);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);


            ProfileImg = FindViewById<CircleImageView>(Resource.Id.profileImage);
            ProfileImg.SetImageBitmap(HomeActivity.currentUser.Image);
           

            Fullname = FindViewById<TextView>(Resource.Id.fullname);
            ChangeImg = FindViewById<ImageView>(Resource.Id.editIcon);
            Username = FindViewById<EditText>(Resource.Id.UsernameInput);
            Phonenumber = FindViewById<EditText>(Resource.Id.PhoneInput);
            Email = FindViewById<EditText>(Resource.Id.EmailInput);
            Date = FindViewById<EditText>(Resource.Id.DatePicker);
            Save = FindViewById<Button>(Resource.Id.SaveBtn);

            DateTime today = DateTime.Today;
            today = today.AddDays(2);

            dateDialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
            dateDialog.DatePicker.MinDate = today.Millisecond;
            //dateDialog.DatePicker.SetBackgroundColor(new Android.Graphics.Color(56, 57, 58));

            Date.Focusable = false;
            Date.FocusableInTouchMode = false;
            Date.Clickable = false;
            Date.LongClickable = false;
            Date.SetCursorVisible(false);
            Date.Click += (sender, e) =>
            {
                dateDialog.Show();
            };

            Save.Click += SaveChanges;

            ChangeImg.Click += delegate {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            getUserInfo();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Date.Text = dateDialog.DatePicker.DateTime.ToString("dd/MM/yyy");
        }

        public void getUserInfo()
        {
            RunOnUiThread(() =>
            {
                Fullname.Text = HomeActivity.currentUser.Fullname;
                Username.Text = HomeActivity.currentUser.Username;
                Phonenumber.Text = HomeActivity.currentUser.Phonenumber;
                Email.Text = HomeActivity.currentUser.Email;
                if (HomeActivity.currentUser.BirthDate != null)
                    Date.Text = HomeActivity.currentUser.BirthDate;
                if (HomeActivity.currentUser.Image == null)
                    HomeActivity.currentUser.setImg();
            });
        }

        /*public void getUserInfo()
        {
            Log.Info("myapp", "getUser called");
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/get_user.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("username", HomeActivity.currentUser.Username);

            client.UploadValuesCompleted += uploadUserValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadUserValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {            
            string json = Encoding.UTF8.GetString(args.Result);
            HomeActivity.currentUser = JsonConvert.DeserializeObject<User>(json);
            RunOnUiThread(() =>
            {
                Fullname.Text = HomeActivity.currentUser.Fullname;
                Username.Text = HomeActivity.currentUser.Username;
                Phonenumber.Text = HomeActivity.currentUser.Phonenumber;
                Email.Text = HomeActivity.currentUser.Email;
                if (HomeActivity.currentUser.BirthDate != null)
                    Date.Text = HomeActivity.currentUser.BirthDate;
                if (HomeActivity.currentUser.Image == null)
                    HomeActivity.currentUser.setImg();
            });
           
        }*/

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                //Bitmap img = DecodeBitmapFromStream(data.Data, 150, 150);
               
                //ProfileImg.SetImageBitmap(img);
                ProfileImg.SetImageURI(data.Data);
                ImageURI = data.Data;
                ImgChanged = true;
            }
        }

        void SaveChanges(object sender, EventArgs args)
        {
            if (isInputValid())
            {
                string ImageName;
                WebClient Client = new WebClient();
                string ip = Resources.GetString(Resource.String.ServerIP);
                if (ImgChanged == true)
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
                    ImageName = fullPathToImage.Substring(fullPathToImage.LastIndexOf('/') + 1);
                    Client.Headers.Add("Content-Type", "binary/octet-stream");
                    byte[] result = Client.UploadFile("http://" + ip + "/upload_image.php", "POST", fullPathToImage);
                    string s = Encoding.UTF8.GetString(result, 0, result.Length);
                    //Toast.MakeText(this, s, ToastLength.Long).Show();
                }
                else
                {
                    ImageName = HomeActivity.currentUser.ImageUrl;
                }
                try
                {
                    Log.Info("myapp", "username: " + Username.Text);
                    Uri uri = new Uri("http://" + ip + "/update_user.php");
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("img_name", ImageName);
                    parameters.Add("username", HomeActivity.currentUser.Username);
                    parameters.Add("newusername", Username.Text);
                    parameters.Add("phonenumber", Phonenumber.Text);
                    parameters.Add("email", Email.Text);
                    parameters.Add("birthdate", Date.Text);

                    Client.UploadValuesCompleted += uploadValueComplete;
                    Client.UploadValuesAsync(uri, parameters);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("In Main catch block. Caught: {0}", e.Message);
                    Console.WriteLine("Inner Exception is {0}", e.InnerException);
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
            getUserInfo();//Update current user
            Finish();

        }

        private Bitmap DecodeBitmapFromStream(Android.Net.Uri data, int requestedWidth, int requestedHeight)
        {
            //Decode with InJustDecodeBounds = true to check dimensions
            Stream stream = ContentResolver.OpenInputStream(data);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeStream(stream);

            //Calculate InSamplesize
            options.InSampleSize = CalculateInSampleSize(options, requestedWidth, requestedHeight);

            //Decode bitmap with InSampleSize set
            stream = ContentResolver.OpenInputStream(data); //Must read again
            options.InJustDecodeBounds = false;
            Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options);
            return bitmap;
        }

        private int CalculateInSampleSize(BitmapFactory.Options options, int requestedWidth, int requestedHeight)
        {
            //Raw height and widht of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > requestedHeight || width > requestedWidth)
            {
                //the image is bigger than we want it to be
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                while ((halfHeight / inSampleSize) > requestedHeight && (halfWidth / inSampleSize) > requestedWidth)
                {
                    inSampleSize *= 2;
                }

            }

            return inSampleSize;
        }


        private bool isInputValid()
        {
            bool valid = true;

            if (!ValidationMethods.isValidPhone(Phonenumber.Text))
            {
                valid = false;
                errorMsg += "Phonenumber is not valid.";
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