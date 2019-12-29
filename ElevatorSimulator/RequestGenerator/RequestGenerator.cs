using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.RequestGenerator
{
    class RequestGenerator
    {
        // The base probability of a request being made from or to a floor at any point in time
        public readonly double[] RequestProbabilityPerFloor;

        // The building the request generator is associated to
        public readonly Building Building;

        public RequestGenerator(Building building, SimulationConfiguration config)
        {
            this.Building = building;

            this.RequestProbabilityPerFloor = new double[this.Building.BuildingFloors];

            for (int i = 0; i < this.Building.BuildingFloors; i++)
            {
                this.RequestProbabilityPerFloor[i] = (config.AverageRequestsPerResidentPerDay * building.ResidentPerFloor[i]) / config.TicksPerDay;
            }

            double meanInterest = config.InterestPerFloor.Sum() / this.Building.BuildingFloors;

            for (int i = 0; i < this.Building.BuildingFloors; i++)
            {
                // Boosts entries with interest above average, and decreases entries with interest below average
                double weightAdjustment = config.InterestPerFloor[i] / meanInterest;
                this.RequestProbabilityPerFloor[i] = this.RequestProbabilityPerFloor[i] * weightAdjustment;
            }

        }

    }
}
