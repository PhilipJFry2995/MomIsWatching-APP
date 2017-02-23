using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomIsWatching
{
    public interface ISocketHandler
    {
        void sendSocketInfo(Position position);
    }
}
