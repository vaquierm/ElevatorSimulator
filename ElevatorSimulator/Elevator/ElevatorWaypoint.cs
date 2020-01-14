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

        // The type of waypoint
        public readonly WaypointType WaypointType;

        public ElevatorWaypoint(uint destinationFloor, WaypointType waypointType)
        {
            this.DestinationFloor = destinationFloor;
            this.WaypointType = waypointType;
        }
    }

    enum WaypointType {
        PICK_UP, DROP_OFF, RELOCATION
    }
}
