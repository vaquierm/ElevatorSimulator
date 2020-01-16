using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSimulator.Elevator;

namespace ElevatorSimulator.ElevatorAI
{
    class RegularAI : ElevatorAI
    {
        private uint SpeedPerTick;

        private uint LoadingTime;

        public RegularAI(Building building, ElevatorCollection elevators, SimulationConfiguration config) : base(building, elevators, config)
        {
            this.SpeedPerTick = config.ElevatorSpeed;
            this.LoadingTime = config.ElevatorSpeed;
        }


        /// <summary>
        /// Make decisions with the pending requests and order the elevators to move accordingly
        /// </summary>
        public override void HandleRequests()
        {
            // Get all the requests that have not been handled yet
            var notHandledRequests = this.Building.PendingRequests.FindAll(request => !this.Elevators.OnTheWayRequests.Contains(request));

            foreach (var request in notHandledRequests)
            {
                TryAssignRequest(request);
            }
        }

        private void TryAssignRequest(Request request)
        {
            int[][] timePenalty = new int[this.Elevators.Count()][];

            for (int i = 0; i < this.Elevators.Count(); i++)
            {
                var elevator = this.Elevators.Elevators[i];

                if (elevator.IsIdle || elevator.IsRelocating)
                {
                    // If the elevator is not in use or is relocating, it can go directly pick up someone
                    timePenalty[i] = new int[] { (int)Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - (int)request.Source)) / this.SpeedPerTick), 0 };
                    continue;
                }

                timePenalty[i] = this.TimePenalty(request, elevator);
            }

            // Get the elevator that has the minimum penalty
            var minPenaltyElevators = timePenalty.ToList().FindAll(p => p[1] >= 0 && p[0] == timePenalty.Min(pe => pe[0]));

            if (minPenaltyElevators.Count() > 0)
            {

                var elevatorToAssign = this.Elevators.Elevators[timePenalty.ToList().IndexOf(minPenaltyElevators.First())];

                elevatorToAssign.AddWaypoint(new ElevatorWaypoint(request.Source, WaypointType.PICK_UP, request.Destination));
                elevatorToAssign.OnTheWayRequests.Add(request);
            }

        }

        private int[] TimePenalty(Request request, Elevator.Elevator elevator)
        {
            var bound = elevator.Bound();
            if (bound >= 0)
            {
                if (request.Direction == elevator.Direction)
                {
                    if ((request.Direction == Direction.DOWN && elevator.CurrentFloor >= request.Source && bound < request.Destination) || (request.Direction == Direction.UP && elevator.CurrentFloor <= request.Source && bound > request.Destination))
                    {
                        return new int[] { (int)elevator.LoadingTimeRemaining + (int)Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - request.Source)) / this.SpeedPerTick), 0 };
                    }
                    else
                    {
                        return new int[] { int.MaxValue, -1 };
                    }
                }
                else
                {
                    return new int[] { int.MaxValue, -1 };
                }
            }

            var waypoints = elevator.PredictiveWaypoints;

            var waiting = new List<Request>(elevator.OnTheWayRequests);
            var pickedUp = new List<Request>(elevator.PickedUpRequests);

            int penalty = 0;
            int waitTime = 0;

            int index = -1;
            int temp = (int) elevator.CurrentFloor;
            for (int i = 0; i < waypoints.Count(); i++)
            {
                if (request.Direction != elevator.Direction || (i < 0 && request.Source < temp && request.Source < waypoints[i].DestinationFloor) || (i < 0 && request.Source > temp && request.Source > waypoints[i].DestinationFloor))
                {
                    waitTime += (int)Math.Ceiling(Math.Abs((double)(temp - waypoints[i].DestinationFloor)) / this.SpeedPerTick);

                    if (temp != waypoints[i].DestinationFloor)
                    {
                        waitTime += (int) this.LoadingTime;
                    }
                }

                // Simulate the unloading of the elevator
                pickedUp.AddRange(waiting.FindAll(r => r.Source == waypoints[i].DestinationFloor));

                if (request.Direction == elevator.Direction && ((request.Source <= temp && request.Source > waypoints[i].DestinationFloor) || (request.Source >= temp && request.Source < waypoints[i].DestinationFloor)))
                {
                    // If the request should get a waypoint at index i
                    index = i;
                    penalty += (waiting.Count() + pickedUp.Count()) * (int) this.LoadingTime;
                }

                if (request.Direction == elevator.Direction && ((request.Destination <= temp && request.Destination > waypoints[i].DestinationFloor) || (request.Source >= temp && request.Source < waypoints[i].DestinationFloor)))
                {
                    // If we would drop off the request here
                    penalty += (waiting.Count() + pickedUp.Count()) * (int)this.LoadingTime;
                }
            }

            return new int[] { penalty + waitTime, index };
        }
    }
}
