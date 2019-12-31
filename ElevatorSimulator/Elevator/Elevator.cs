using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class Elevator
    {

        // Speed in floors per tick
        public readonly uint Speed;

        // Energy used per tick
        public readonly uint EnergyPerTick;

        // Total number of floors in the building
        private uint TopFloor;

        // The current floor that the elevator is at
        public uint CurrentFloor
        {
            get;
            private set;
        }

        // Waypoints that the elevator has to go to
        public readonly List<ElevatorWaypoint> Waypoints;

        // Loading time the elevator stops for when picking up or dropping off someone
        public readonly uint LoadingTime;

        // The loading time rmaining in ticks before the elevator can move again
        public uint LoadingTimeRemaining
        {
            get;
            private set;
        }

        // True if the elevator is currently loading
        public bool IsMoving
        {
            get
            {
                return this.LoadingTimeRemaining == 0 && this.Waypoints.Count() > 0;
            }
        }

        public Elevator(SimulationConfiguration config)
        {
            this.Speed = config.ElevatorSpeed;
            this.EnergyPerTick = config.EnergyPerTick;

            this.TopFloor = config.BuildingFloors;

            this.Waypoints = new List<ElevatorWaypoint>();
        }

        /// <summary>
        /// Increment the elevator by one time unit
        /// </summary>
        /// <returns> The amount of energy the elevator used this tick </returns>
        public uint Tick()
        {
            if (this.Waypoints.Count() == 0)
            {
                return 0;
            }

            if (this.LoadingTimeRemaining > 0)
            {
                this.LoadingTimeRemaining--;
                return 0;
            }

            var nextWaypoint = this.Waypoints.First();

            if (nextWaypoint.DestinationFloor > this.TopFloor || nextWaypoint.DestinationFloor < 0)
            {
                throw new InvalidWaypointException("The waypoints have to be between 0 and " + this.TopFloor + ". Invalid waypoint at floor " + nextWaypoint.DestinationFloor);
            }

            if (nextWaypoint.DestinationFloor > this.CurrentFloor + this.Speed)
            {
                this.CurrentFloor += this.Speed;
            }
            else if (nextWaypoint.DestinationFloor < this.CurrentFloor - this.Speed)
            {
                this.CurrentFloor -= this.Speed;
            }
            else
            {
                this.CurrentFloor = nextWaypoint.DestinationFloor;
                this.Waypoints.Remove(nextWaypoint);
                this.ResetLoadingTime();
            }

            return this.EnergyPerTick;
        }

        /// <summary>
        /// Resets the loading time of the elevator
        /// </summary>
        public void ResetLoadingTime()
        {
            this.LoadingTimeRemaining = this.LoadingTime;
        }
    }

}
