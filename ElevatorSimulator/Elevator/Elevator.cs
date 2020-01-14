﻿using System;
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
        private readonly uint TopFloor;

        // The current floor that the elevator is at
        public uint CurrentFloor
        {
            get;
            private set;
        }

        // The list of requests that are currently being serviced by this elevator
        public readonly List<Request> PickedUpRequests;

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

        // True if the elevator is moving
        public bool IsMoving
        {
            get
            {
                return this.LoadingTimeRemaining == 0 && this.Waypoints.Count() > 0;
            }
        }

        // True if the elevator is not loading or unloading anyone and is not going anywhere
        public bool IsIdle
        {
            get
            {
                return this.Waypoints.Count() == 0;
            }
        }

        // True if the elevator is currently relocating
        public bool IsRelocating
        {
            get
            {
                return this.Waypoints.Count() > 0 && this.Waypoints.First().WaypointType == WaypointType.RELOCATION;
            }
        }

        public Elevator(SimulationConfiguration config)
        {
            this.Speed = config.ElevatorSpeed;
            this.EnergyPerTick = config.EnergyPerTick;

            this.LoadingTime = config.LoadingTime;

            this.TopFloor = config.BuildingFloors;

            this.Waypoints = new List<ElevatorWaypoint>();
            this.PickedUpRequests = new List<Request>();
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
                if (this.LoadingTimeRemaining > this.LoadingTime)
                {
                    throw new InvalidElevatorStateException("The elevator cannot have a loading time remaining higher than the max loading time. Max loading time: " + this.LoadingTime + ", Loading time remaining: " + this.LoadingTimeRemaining);
                }
                this.LoadingTimeRemaining--;
                return 0;
            }

            var nextWaypoint = this.Waypoints.First();

            if (nextWaypoint.DestinationFloor > this.TopFloor || nextWaypoint.DestinationFloor < 0)
            {
                throw new InvalidWaypointException("The waypoints have to be between 0 and " + this.TopFloor + ". Invalid waypoint at floor " + nextWaypoint.DestinationFloor);
            }
            if (this.CurrentFloor > this.TopFloor)
            {
                throw new InvalidElevatorStateException("The elevator cannot be higher than the top floor. Top floor: " + this.TopFloor + ", Current elevator floor: " + this.CurrentFloor);
            }

            if (nextWaypoint.DestinationFloor > this.CurrentFloor && nextWaypoint.DestinationFloor > this.CurrentFloor + this.Speed)
            {
                this.CurrentFloor += this.Speed;
            }
            else if (nextWaypoint.DestinationFloor < this.CurrentFloor && nextWaypoint.DestinationFloor < this.CurrentFloor - this.Speed)
            {
                this.CurrentFloor -= this.Speed;
            }
            else
            {
                uint energySpent = (this.CurrentFloor == nextWaypoint.DestinationFloor) ? 0 : this.EnergyPerTick;
                this.CurrentFloor = nextWaypoint.DestinationFloor;
                this.Waypoints.Remove(nextWaypoint);

                if (nextWaypoint.WaypointType != WaypointType.RELOCATION)
                {
                    // Only reset the loading time if the waypoint is not just a relocation waypoint.
                    this.ResetLoadingTime();
                }
                return energySpent;
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

        /// <summary>
        /// Cancels the relocation process of the elevator to enable it to go service a request
        /// </summary>
        public void CancelRelocation()
        {
            if (this.IsRelocating)
            {
                this.Waypoints.RemoveAt(0);
            }
            else
            {
                throw new InvalidElevatorStateException("Cannot cancel the relocation of the elevator if the elevator is not relocating");
            }
        }

    }

}
