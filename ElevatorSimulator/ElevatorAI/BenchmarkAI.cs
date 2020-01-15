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
        public BenchmarkAI(Building building, ElevatorCollection elevators, SimulationConfiguration config) : base(building, elevators, config)
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
                if (this.Elevators.OnTheWayRequests.Contains(request))
                {
                    // If the request has already been handled, continue
                    continue;
                }

                var availibleElevators = this.Elevators.Elevators.FindAll(elevator => (elevator.IsIdle || elevator.IsRelocating)).ToList();

                if (availibleElevators.Count() == 0)
                {
                    // If there are no more availible elevators, break
                    break;
                }

                // Add the waypoints to the elevator
                var closestElevator = ElevatorCollection.GetClosestElevator(availibleElevators, request.Source);

                // If the elevator was relocating, interrupt the relocation
                if (closestElevator.IsRelocating)
                {
                    closestElevator.CancelRelocation();
                }

                closestElevator.AddWaypoint(new ElevatorWaypoint(request.Source, WaypointType.PICK_UP, request.Destination));
                closestElevator.OnTheWayRequests.Add(request);
            }
        }
    }
}
