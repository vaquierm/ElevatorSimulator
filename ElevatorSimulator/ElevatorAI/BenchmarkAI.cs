using ElevatorSimulator.Elevator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.ElevatorAI
{
    class BenchmarkAI : ElevatorAI
    {
        public BenchmarkAI(Building building, ElevatorCollection elevators) : base(building, elevators)
        {
            
        }

        /// <summary>
        /// Make decisions with the pending requests and order the elevators to move accordingly
        /// </summary>
        public override void HandleRequests()
        {
            // Match the closest elevator that is availible to the requests
            foreach (var request in this.Building.PendingRequests)
            {
                if (this.HandledRequests.Contains(request))
                {
                    // If the request has already been handled, continue
                    continue;
                }

                var availibleElevators = this.Elevators.Where(elevator => elevator.Waypoints.Count() == 0).ToList();

                if (availibleElevators.Count() == 0)
                {
                    // If there are no more availible elevators, break
                    break;
                }

                var distanceElevatorToRequest = availibleElevators.Select(elevator => Math.Abs((int) elevator.CurrentFloor - (int) request.Source)).ToList();
                int closestElevatorIndex = distanceElevatorToRequest.IndexOf(distanceElevatorToRequest.Min());

                // Add the waypoints to the elevators
                availibleElevators[closestElevatorIndex].Waypoints.Add(new ElevatorWaypoint(request.Source));
                availibleElevators[closestElevatorIndex].Waypoints.Add(new ElevatorWaypoint(request.Destination));

                // Mark the request as handled
                this.HandledRequests.Add(request);
            }
        }
    }
}
