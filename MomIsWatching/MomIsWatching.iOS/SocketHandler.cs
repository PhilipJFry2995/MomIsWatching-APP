using MomIsWatching.iOS;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using Security;
using Foundation;
using Plugin.Battery;
using System.Security.Cryptography;
using Plugin.Geolocator;

[assembly: Xamarin.Forms.Dependency(typeof(SocketHandler))]
namespace MomIsWatching.iOS
{
    class SocketHandler : ISocketHandler
	{
		public async void sendSocketInfo(Position position)
		{
			string package = "{"
				+ "\"DeviceId\":" + "\"" + AppDelegate.deviceId + "\","
				+ "\"Location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
												   + "\"Charge\":" + Math.Abs(CrossBattery.Current.RemainingChargePercent) + ","
				+ "\"IsSos\":" + "1}";
			Console.WriteLine("Package:" + package);
			AppDelegate.websocket.Send(package);
		}
	}
}