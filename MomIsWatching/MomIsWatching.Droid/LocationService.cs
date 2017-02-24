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
using MomIsWatching.Droid;
using Plugin.Geolocator;
using System.Threading.Tasks;
using WebSocket4Net;
using Plugin.Geolocator.Abstractions;
using Android.Util;
using System.Threading;
using Plugin.Battery;
using Xamarin.Forms;

namespace MomIsWatching.Droid
{
    [Service]
    class LocationService : Service
    {
        public const int ID = 10000;

        private Position position;
        private WebSocket websocket;
        private int TimerWait = 3 * 1000; // seconds * 1000
        private string deviceId;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            deviceId = Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            getCoordinates();

            return StartCommandResult.Sticky;
        }

        private async void getCoordinates()
        {
            Toast.MakeText(Forms.Context, "Gettings gps position...", ToastLength.Short).Show();
            position = await CrossGeolocator.Current.GetPositionAsync(10000);

            Toast.MakeText(Forms.Context, "Sending coordinates to server...", ToastLength.Short).Show();

            websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?deviceId="
                + deviceId); // ?deviceId=...
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.Open();

            Thread.Sleep(TimerWait);

            getCoordinates();
        }
        
        private void websocket_Opened(object sender, EventArgs e)
        {
            //{ 'deviceId': 'blabla', 'location': '32.32;12.12', 'charge': 42, 'isSos': 0 }
            string package = "{"
                + "\"deviceId\":" + "\"" + deviceId + "\","
                + "\"location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
                + "\"charge\":" + CrossBattery.Current.RemainingChargePercent + ","
                + "\"isSos\":" + "0}";
            Log.Debug("tag", "Package:" + package);
            websocket.Send(package);
            websocket.Close();
        }
    }
}