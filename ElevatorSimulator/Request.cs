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
        public readonly uint Source;

        // Destination floor of the request
        public readonly uint Destination;

        // The direction of the request
        public readonly Direction Direction;

        public Request(uint source, uint destination)
        {
            if (source == destination)
            {
                throw new InvalidRequestStateException("A request cannot have the same source and destination floor");
            }

            this.Destination = destination;
            this.Source = source;

            if (destination > source)
            {
                this.Direction = Direction.UP;
            }
            else
            {
                this.Direction = Direction.DOWN;
            }
        }
    }

    public enum Direction
    {
        UP, DOWN, NONE
    }
}
