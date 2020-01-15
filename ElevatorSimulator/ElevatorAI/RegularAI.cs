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

                


                

                
            }

        }

        private int[] TimePenalty(Request request, Elevator.Elevator elevator)
        {
            int numberPeopleInElevator = elevator.PickedUpRequests.Count();
            int numberOfWaitingPeople = elevator.Waypoints.Count(w => w.WaypointType == WaypointType.PICK_UP);
            var nextWaypoint = elevator.Waypoints.First();

            int penalty = 0;
            bool onWay = false;

            // Check if the request in fully on the way
            if (request.Direction == Direction.UP && elevator.CurrentFloor <= request.Source && request.Destination <= nextWaypoint.DestinationFloor)
            {
                onWay = true;
            }
            else if (request.Direction == Direction.DOWN && elevator.CurrentFloor >= request.Source && request.Destination >= nextWaypoint.DestinationFloor) {
                onWay = true;
            }

            // If the request is fully on the way, calculate the penalty that that creates
            if (onWay) {
                if (elevator.CurrentFloor == request.Source)
                {
                    penalty += (int) (this.LoadingTime - elevator.LoadingTimeRemaining);
                }
                else
                {
                    penalty += (int) this.LoadingTime;
                }

                if (request.Destination != nextWaypoint.DestinationFloor)
                {
                    penalty += (int)this.LoadingTime;
                }

                // The penalty applies for everyone waiting
                penalty *= (numberOfWaitingPeople + numberPeopleInElevator);

                // Add the waiting time of the person making the request
                penalty += (int) Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - (int)request.Source)) / this.SpeedPerTick);

                return new int[] { penalty, 0 };
            }

            return new int[]{ };

        }
    }
}
