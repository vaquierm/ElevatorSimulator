using ElevatorSimulator.Elevator;
using ElevatorSimulator.ElevatorAI;
using ElevatorSimulator.RequestGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class Building
    {
        // Number of floors on the simulation building
        public readonly uint BuildingFloors;

        // Percentage of residents per floor
        public readonly uint[] ResidentPerFloor;

        // Percentage of interest per floor
        public readonly double[] InterestPerFloor;

        // AI making decisions
        public readonly ElevatorAI.ElevatorAI ElevatorAI;

        // Collection of elevators in the building
        public readonly ElevatorCollection Elevators;

        // Request generaotr to generate elevator requests
        public readonly RequestGenerator.RequestGenerator RequestGenerator;

        public List<Request> PendingRequests
        {
            get;
            private set;
        }

        public Building(SimulationConfiguration config)
        {
            this.BuildingFloors = config.BuildingFloors;
            this.ResidentPerFloor = config.ResidentsPerFloor;
            this.InterestPerFloor = config.InterestPerFloor;

            this.Elevators = new ElevatorCollection(config);

            this.ElevatorAI = ElevatorAIFactory.CreateElevatorAI(this, this.Elevators, config);

            this.RequestGenerator = RequestGeneratorFactory.CreateRequestGenerator(this, config);

            this.PendingRequests = new List<Request>();
        }

        /// <summary>
        /// Tick the building by one time unit which moves the elevators according to their waypoints, and services requests
        /// </summary>
        /// <returns> The waiting time that was endured this tick and the energy used by the elevators in the form of a MetricReport </returns>
        public MetricsReport Tick()
        {
            // Generate the new requests made this tick and create add them to the list of requests pending
            var requests = this.RequestGenerator.GenerateRequests();
            this.PendingRequests.AddRange(requests);

            // Notify the smart relocator of the requests
            foreach (var r in requests)
            {
                this.ElevatorAI.Relocator?.NotifyRequest(r);
            }

            // The AI them makes decisions
            this.ElevatorAI.HandleRequests();

            // Make the elevators move
            uint energyUsed = this.Elevators.Tick();

            // Check all the pending requests to see if anyone can be picked up.
            foreach (var elevator in this.Elevators)
            {
                if (elevator.IsMoving)
                {
                    // If the elevator is currently traveling, it cannot pick up anyone 
                    continue;
                }

                // Get all requests that can be picked up by this elevator
                var requestsAtElevator = this.PendingRequests.FindAll(request => request.Source == elevator.CurrentFloor && (elevator.Direction == Direction.NONE || elevator.Direction == request.Direction) && elevator.OnTheWayRequests.Contains(request));

                if (requestsAtElevator.Count() > 0)
                {
                    // If there are requests at the floor of the stopped elevator, pick them up
                    foreach (var pickedUpRequest in requestsAtElevator)
                    {
                        // Add the requests to the list of requests that have been picked up by this elevator
                        this.PendingRequests.Remove(pickedUpRequest);
                        elevator.PickedUpRequests.Add(pickedUpRequest);
                        // Remove the request from the elevator
                        elevator.OnTheWayRequests.Remove(pickedUpRequest);

                        if (elevator.IsRelocating)
                        {
                            elevator.CancelRelocation();
                        }

                        // Add the destination waypoint to the elevator once it picks up the person
                        elevator.AddWaypoint(new ElevatorWaypoint(pickedUpRequest.Destination, WaypointType.DROP_OFF));

                        // Notify the AI that the request has been picked up
                        this.ElevatorAI.NotifyPickUp(pickedUpRequest);
                    }
                    // Reset the timer of the elevator since a new person got in the elevator
                    elevator.ResetLoadingTime();
                }

                // Now check if there are any requests that can be dropped off
                var requestsToDropOff = elevator.PickedUpRequests.FindAll(request => request.Destination == elevator.CurrentFloor);

                if (requestsToDropOff.Count() > 0)
                {
                    // If there are requests that want to get off at thos floor. drop them off
                    foreach (var droppedOffRequest in requestsToDropOff)
                    {
                        // Remove the request from the requests the elevator is currently handlind
                        elevator.PickedUpRequests.Remove(droppedOffRequest);
                        // Notify the AI that the request has been dropped off
                        this.ElevatorAI.NotifyDropOff(elevator, droppedOffRequest);
                    }
                    // Reset the timer of the elevator since a new person left the elevator
                    elevator.ResetLoadingTime();
                }

            }

            return new MetricsReport((uint) this.PendingRequests.Count(), (uint) this.Elevators.PickedUpRequestsCount, energyUsed);
        }
    }
}
