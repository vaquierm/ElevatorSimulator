using ElevatorSimulator.Elevator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.ElevatorAI
{
    class RegularAI : ElevatorAI
    {
        public RegularAI(Building building, ElevatorCollection elevators, SimulationConfiguration config) : base(building, elevators, config)
        {
        }

        public override void HandleRequests()
        {
            // Get all the requests that have not been handled yet
            var notHandledRequests = this.Building.PendingRequests.FindAll(request => !this.Elevators.OnTheWayRequests.Contains(request));

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
                    var sameDirElevators = this.Elevators.Elevators.FindAll(elevator => (elevator.Direction == request.Direction && elevator.Direction == Direction.DOWN && request.Source < elevator.CurrentFloor && !elevator.OnTheWayRequests.Exists(r => r.Direction == Direction.UP)) ||
                            (elevator.Direction == request.Direction && elevator.Direction == Direction.UP && request.Source > elevator.CurrentFloor && !elevator.OnTheWayRequests.Exists(r => r.Direction == Direction.DOWN)));

                    if (sameDirElevators.Count() == 0)
                    {
                        continue;
                    }

                    // Get the closest elevator
                    var closest = ElevatorCollection.GetClosestElevator(sameDirElevators, request.Source);
                    closest.AddWaypoint(new ElevatorWaypoint(request.Source, WaypointType.PICK_UP, request.Destination));
                    closest.OnTheWayRequests.Add(request);

                    continue;
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
