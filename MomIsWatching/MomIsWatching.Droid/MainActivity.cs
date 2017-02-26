using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

using WebSocket4Net;
using Android.Util;
using System.Security.Cryptography;
using System.Text;
using Android.Content;
using Android.Widget;

namespace MomIsWatching.Droid
{
    [Activity(Label = "MomIsWatching", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static WebSocket Websocket;
        public static string DeviceId;

        private PendingIntent _pIntent;
        private int _interval;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            DeviceId = CalculateMd5Hash(Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId));
            _interval = 10 * 1000; // Seconds

            Websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?DeviceId=" + DeviceId);
            Websocket.Open();
            Websocket.MessageReceived += MessageReceiver;
            StartService();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Websocket.Close();
        }

        private void MessageReceiver(object sender, EventArgs e)
        {
            var arguments = (MessageReceivedEventArgs) e;
            var newInterval = int.Parse(arguments.Message);
            if(_interval != newInterval)
            {
                _interval = newInterval;
                StopService();
                StartService();
            }
            Log.Debug("mytag", "Message:" + arguments.Message);
        }

        public void StartService()
        {
            Log.Debug("mytag", "Starting service");
            var intent = new Intent(Xamarin.Forms.Forms.Context, typeof(LocationUpdateReceiver));
            _pIntent = PendingIntent.GetBroadcast(Xamarin.Forms.Forms.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
            var firstMillis = SystemClock.ElapsedRealtime();
            var alarm = (AlarmManager)Xamarin.Forms.Forms.Context.GetSystemService("alarm");
            alarm.SetRepeating(AlarmType.RtcWakeup, firstMillis, _interval, _pIntent);
        }

        public void StopService()
        {
            Log.Debug("mytag", "Stopping service");
            AlarmManager alarm = (AlarmManager)Xamarin.Forms.Forms.Context.GetSystemService("alarm");
            alarm.Cancel(_pIntent);
        }

        private static string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            foreach (byte t in hash)
                sb.Append(t.ToString("X2"));

            return sb.ToString();
        }
    }
}

