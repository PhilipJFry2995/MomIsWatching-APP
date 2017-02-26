using System.Globalization;
using MomIsWatching.Droid;
using Plugin.Geolocator.Abstractions;
using Android.Util;
using Plugin.Battery;

[assembly: Xamarin.Forms.Dependency(typeof(SocketHandler))]
namespace MomIsWatching.Droid
{
    class SocketHandler : ISocketHandler
    {
        public void sendSocketInfo(Position position)
        {
            //{ 'DeviceId': 'blabla', 'location': '32.32;12.12', 'charge': 42, 'isSos': 0 }
            string package = "{ "
                + "\"DeviceId\":" + "\"" + MainActivity.DeviceId + "\","
                + "\"Location\":" + "\"" + position.Latitude.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + ";" + position.Longitude.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + "\","
                + "\"Charge\":" + CrossBattery.Current.RemainingChargePercent + ","
                + "\"IsSos\":" + "1 }";
            Log.Debug("tag", "Package:" + package);
            MainActivity.Websocket.Send(package);
        }
    }
}