using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class ElevatorWaypoint
    {
        // The destination of the elevator
        public readonly uint DestinationFloor;

        public ElevatorWaypoint(uint destinationFloor)
        {
            this.DestinationFloor = destinationFloor;
        }
    }
}
