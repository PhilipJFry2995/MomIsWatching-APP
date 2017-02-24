using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using WebSocket4Net;
using Android.Util;
using System.Security.Cryptography;
using System.Text;

namespace MomIsWatching.Droid
{
    [Activity(Label = "MomIsWatching", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static WebSocket websocket;
        public static int interval;
        public static string deviceId;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            deviceId = CalculateMD5Hash(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
            websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?deviceId=" + deviceId);
            websocket.Open();
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(messageReceiver);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            websocket.Close();
        }

        private void messageReceiver(object sender, EventArgs e)
        {
            Log.Debug("mytag", "Message:" + e.ToString());
        }

        private static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

