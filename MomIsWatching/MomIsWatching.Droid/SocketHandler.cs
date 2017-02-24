using System;
using MomIsWatching.Droid;
using Plugin.Geolocator.Abstractions;
using Android.Util;
using Plugin.Battery;

[assembly: Xamarin.Forms.Dependency(typeof(SocketHandler))]
namespace MomIsWatching.Droid
{
    class SocketHandler : ISocketHandler
    {
        Position position;

        public void sendSocketInfo(Position position)
        {
            this.position = position;
            //{ 'deviceId': 'blabla', 'location': '32.32;12.12', 'charge': 42, 'isSos': 0 }
            string package = "{ "
                + "\"deviceId\":" + "\"" + MainActivity.deviceId + "\","
                + "\"location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
                + "\"charge\":" + CrossBattery.Current.RemainingChargePercent + ","
                + "\"isSos\":" + "1 }";
            Log.Debug("tag", "Package:" + package);
            MainActivity.websocket.Send(package);
        }
    }
}