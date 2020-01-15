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

        // The associated drop off that will be added once the request is picked up
        public uint AssociatedNext
        {
            get
            {
                if (this.WaypointType != WaypointType.PICK_UP)
                {
                    throw new InvalidWaypointException("The associated next attribute is only valid for waypoints of type Pick Up");
                }
                return _associatedNext;
            }
        }

        private readonly uint _associatedNext;

        public ElevatorWaypoint(uint destinationFloor, WaypointType waypointType)
        {
            this.DestinationFloor = destinationFloor;
            this.WaypointType = waypointType;

            if (waypointType == WaypointType.PICK_UP)
            {
                throw new InvalidWaypointException("The waypoint must have have an associated next to be type Pick Up");
            }
        }

        public ElevatorWaypoint(uint destinationFloor, WaypointType waypointType, uint associatedNext)
        {
            this.DestinationFloor = destinationFloor;
            this.WaypointType = waypointType;

            if (waypointType != WaypointType.PICK_UP)
            {
                throw new InvalidWaypointException("The waypoint must be of type Pick Up to have an associated next");
            }
            this._associatedNext = associatedNext;
        }

    }

    enum WaypointType {
        PICK_UP, DROP_OFF, RELOCATION
    }
}
