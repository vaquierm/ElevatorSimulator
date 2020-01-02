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

        public Building(SimulationConfiguration config)
        {
            this.BuildingFloors = config.BuildingFloors;
            this.ResidentPerFloor = config.ResidentsPerFloor;
            this.InterestPerFloor = config.InterestPerFloor;

            this.Elevators = new ElevatorCollection(config);

            this.ElevatorAI = ElevatorAIFactory.CreateElevatorAI(config.AIType, this, this.Elevators);

            this.RequestGenerator = RequestGeneratorFactory.CreateRequestGenerator(this, config);
        }

        /// <summary>
        /// Tick the building by one time unit which moves the elevators according to their waypoints, and services requests
        /// </summary>
        /// <returns> The waiting time that was endured this tick and the energy used by the elevators in the form of a MetricReport </returns>
        public MetricsReport Tick()
        {
            // Generate the new requests made this tick and create add them to the AI
            var requests = this.RequestGenerator.GenerateRequests();
            this.ElevatorAI.NewRequests(requests);

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
                var requestsAtElevator = this.ElevatorAI.PendingRequests.FindAll(request => request.Source == elevator.CurrentFloor);

                if (requestsAtElevator.Count() > 0)
                {
                    // If there are requests at the floor of the stopped elevator, pick them up
                    foreach (var pickedUpRequest in requestsAtElevator)
                    {
                        // Add the requests to the list of requests that have been picked up
                        this.ElevatorAI.PendingRequests.Remove(pickedUpRequest);
                        this.ElevatorAI.PickedUpRequests.Add(pickedUpRequest);
                    }
                    // Reset the timer of the elevator since a new person got in the elevaotr
                    elevator.ResetLoadingTime();
                }
            }

            return new MetricsReport((uint)this.ElevatorAI.PendingRequests.Count(), energyUsed);
        }
    }
}
