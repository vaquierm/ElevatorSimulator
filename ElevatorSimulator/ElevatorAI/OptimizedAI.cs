using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSimulator.Elevator;

namespace ElevatorSimulator.ElevatorAI
{
    class OptimizedAI : ElevatorAI
    {
        private uint SpeedPerTick;

        private uint LoadingTime;

        public OptimizedAI(Building building, ElevatorCollection elevators, SimulationConfiguration config) : base(building, elevators, config)
        {
            this.SpeedPerTick = config.ElevatorSpeed;
            this.LoadingTime = config.LoadingTime;
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

                if (elevator.IsIdle || elevator.IsRelocating || (elevator.PickedUpRequests.Count() == 0 && elevator.OnTheWayRequests.Count() == 0))
                {
                    // If the elevator is not in use or is relocating, it can go directly pick up someone
                    timePenalty[i] = new int[] { (int)Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - (int)request.Source)) / this.SpeedPerTick), 0 };
                    timePenalty[i][0] += (int)elevator.LoadingTimeRemaining;
                    continue;
                }

                timePenalty[i] = this.TimePenalty(request, elevator);
            }

            // Get the elevator that has the minimum penalty
            var minPenaltyElevators = timePenalty.ToList().FindAll(p => p[1] >= 0 && p[0] == timePenalty.Min(pe => pe[0]));

            if (minPenaltyElevators.Count() > 0)
            {

                var elevatorToAssign = this.Elevators.Elevators[timePenalty.ToList().IndexOf(minPenaltyElevators.First())];

                if (elevatorToAssign.IsRelocating)
                {
                    elevatorToAssign.CancelRelocation();
                }

                elevatorToAssign.AddWaypoint(new ElevatorWaypoint(request.Source, WaypointType.PICK_UP, request.Destination));
                elevatorToAssign.OnTheWayRequests.Add(request);
            }

        }

        private int[] TimePenalty(Request request, Elevator.Elevator elevator)
        {
            int[] penalty = { int.MaxValue, -1 };
            if (elevator.Direction == request.Direction && elevator.Direction == Direction.DOWN && request.Source < elevator.CurrentFloor && !elevator.OnTheWayRequests.Exists(r => r.Direction == Direction.UP && r.Source > request.Destination))
            {
                // The elevator is going down in the same direction as our request
                // If it is picking up a request to go up, it is below our destination
                penalty[1] = 0;

                // Time for elevator to get to the request
                penalty[0] = (int)Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - (int)request.Source)) / this.SpeedPerTick);
                // Time wasted for other people already travelling in the elevator
                penalty[0] += (int)elevator.LoadingTimeRemaining +
                    (elevator.OnTheWayRequests.FindAll(r => r.Source != request.Source && r.Destination != request.Source).Count()
                    + elevator.OnTheWayRequests.FindAll(r => r.Source != request.Destination && r.Destination != request.Destination).Count()
                    + elevator.PickedUpRequests.FindAll(r => r.Source != request.Source && r.Destination != request.Source).Count()
                    + elevator.PickedUpRequests.FindAll(r => r.Source != request.Destination && r.Destination != request.Destination).Count()
                    ) * (int)this.LoadingTime;
            }
            else if (elevator.Direction == request.Direction && elevator.Direction == Direction.UP && request.Source > elevator.CurrentFloor && !elevator.OnTheWayRequests.Exists(r => r.Direction == Direction.DOWN && r.Source < request.Destination))
            {
                // The elevator is travelling up in the same direction as the request
                // If it is going to pick up someone that wants to go down, it is above the request destination
                penalty[1] = 0;

                // Time for elevator to get to the request
                penalty[0] = (int)Math.Ceiling(Math.Abs((double)((int)elevator.CurrentFloor - (int)request.Source)) / this.SpeedPerTick);
                // Time wasted for other people already travelling in the elevator
                penalty[0] += (int)elevator.LoadingTimeRemaining +
                                    (elevator.OnTheWayRequests.FindAll(r => r.Source != request.Source && r.Destination != request.Source).Count()
                                    + elevator.OnTheWayRequests.FindAll(r => r.Source != request.Destination && r.Destination != request.Destination).Count()
                                    + elevator.PickedUpRequests.FindAll(r => r.Source != request.Source && r.Destination != request.Source).Count()
                                    + elevator.PickedUpRequests.FindAll(r => r.Source != request.Destination && r.Destination != request.Destination).Count()
                                    ) * (int)this.LoadingTime;
            }

            // Now concider the penalty if we pick up this request later after fufilling all requests
            var distinctFloors = new HashSet<int>();
            distinctFloors.Add((int)elevator.CurrentFloor);
            foreach (var r in elevator.OnTheWayRequests)
            {
                distinctFloors.Add((int)r.Source);
                distinctFloors.Add((int)r.Destination);
            }
            foreach (var r in elevator.PickedUpRequests)
            {
                distinctFloors.Add((int)r.Destination);
            }
            int penaltyNoPickup = (int)elevator.LoadingTimeRemaining + (int)Math.Ceiling((double)(distinctFloors.Max() - distinctFloors.Min()) / this.SpeedPerTick) + distinctFloors.Count() * (int)this.LoadingTime;

            var direction = (elevator.OnTheWayRequests.Count() == 0) ? elevator.Direction : elevator.OnTheWayRequests.First().Direction;
            var endPoint = (direction == Direction.DOWN) ? distinctFloors.Min() : distinctFloors.Max();

            penaltyNoPickup += (int)Math.Ceiling(Math.Abs((double)(endPoint - (int)request.Source) / this.SpeedPerTick));

            if (penaltyNoPickup < penalty[0])
            {
                penalty[0] = penaltyNoPickup; // Update the best penalty
                penalty[1] = -1; // Indicate that it is batter not to use the elevator
            }

            return penalty;
        }
    }
}
