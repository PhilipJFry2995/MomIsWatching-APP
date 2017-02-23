using System;
using MomIsWatching.Droid;
using Plugin.Geolocator.Abstractions;
using WebSocket4Net;
using Android.Util;
using Plugin.Battery;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

[assembly: Xamarin.Forms.Dependency(typeof(SocketHandler))]
namespace MomIsWatching.Droid
{
    class SocketHandler : ISocketHandler
    {
        WebSocket websocket;
        Position position;

        public void sendSocketInfo(Position position)
        {
            this.position = position;
            websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?deviceId=" +
                Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId)); // ?deviceId=...
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.Open();
        }
        private void websocket_Opened(object sender, EventArgs e)
        {
            //{ 'deviceId': 'blabla', 'location': '32.32;12.12', 'charge': 42, 'isSos': 0 }
            string package = "{"
                + "\"deviceId\":" + "\"" + Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) + "\","
                + "\"location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
                + "\"charge\":" + CrossBattery.Current.RemainingChargePercent
                + "\"isSos\":" + "1}";
            Log.Debug("tag", "Package:" + package);
            websocket.Send(package);
            websocket.Close();
        }
    }
}