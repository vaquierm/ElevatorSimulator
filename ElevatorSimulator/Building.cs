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
        public readonly double[] ResidentPerFloor;

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

            this.ElevatorAI = new BenchmarkAI(this, this.Elevators);

            this.RequestGenerator = new RequestGenerator.RequestGenerator(this, config);
        }

        public void Tick()
        {
            // Generate the new requests made this tick and create add them to the AI
            var requests = this.RequestGenerator.GenerateRequests();
            this.ElevatorAI.NewRequests(requests);

            // The AI them makes decisions
            this.ElevatorAI.HandleRequests();

            // Make the elevators move
            uint energyUsed = this.Elevators.Tick();

            // Check all the pending requests to see if anyone can be picked up.
            foreach (Request request in this.ElevatorAI.PendingRequests)
            {
                //TODO Logic to load people into elevators
            }
        }
    }
}
