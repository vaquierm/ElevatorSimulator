using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class Request
    {
        // Source floor of the request
        public uint Source
        {
            get;
            private set;
        }

        // Destination floor of the request
        public uint Destination
        {
            get;
            private set;
        }

        public Request(uint source, uint destination)
        {
            this.Destination = destination;
            this.Source = source;
        }
    }
}
