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
        public int Source
        {
            get;
            private set;
        }

        // Destination floor of the request
        public int Destination
        {
            get;
            private set;
        }

        public Request(int source, int destination)
        {
            this.Destination = destination;
            this.Source = source;
        }
    }
}
