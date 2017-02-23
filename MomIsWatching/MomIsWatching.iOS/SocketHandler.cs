using MomIsWatching.iOS;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(SocketHandler))]
namespace MomIsWatching.iOS
{
    class SocketHandler : ISocketHandler
    {
        public void sendSocketInfo(Position position)
        {
            
        }
    }
}