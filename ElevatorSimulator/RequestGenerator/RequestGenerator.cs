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

        // The base probability of a request having a destination at a certain floor
        public readonly double[] DestinationProbabilityPerFloor;

        // The building the request generator is associated to
        public readonly Building Building;

        // Random number generator used for generating requests
        private Random Rand = new Random();

        public RequestGenerator(Building building, SimulationConfiguration config)
        {
            this.Building = building;

            this.RequestProbabilityPerFloor = new double[this.Building.BuildingFloors];

            for (int i = 0; i < this.Building.BuildingFloors; i++)
            {
                this.RequestProbabilityPerFloor[i] = ((double) config.AverageRequestsPerResidentPerDay * building.ResidentPerFloor[i]) / config.TicksPerDay;
            }

            double meanInterest = config.InterestPerFloor.Sum() / this.Building.BuildingFloors;

            for (int i = 0; i < this.Building.BuildingFloors; i++)
            {
                // Boosts entries with interest above average, and decreases entries with interest below average
                double weightAdjustment = config.InterestPerFloor[i] / meanInterest;
                this.RequestProbabilityPerFloor[i] = this.RequestProbabilityPerFloor[i] * weightAdjustment;
            }

            this.DestinationProbabilityPerFloor = (double[]) this.RequestProbabilityPerFloor.Clone();

        }

        /// <summary>
        /// Generate elevator requests for each floor
        /// </summary>
        /// <returns> List of requests based on the probabilities of requests happening at each floor</returns>
        public virtual List<Request> GenerateRequests()
        {
            var requests = new List<Request>();

            for (uint i = 0; i < this.Building.BuildingFloors; i++)
            {
                // If the generated number is smaller than the request probability, generate a request
                if (this.Rand.NextDouble() < this.RequestProbabilityPerFloor[i])
                {
                    requests.Add(new Request(i, this.TargetFloor(i)));
                }
            }

            return requests;
        }

        /// <summary>
        /// Generate the target floor of a request.
        /// The Target floor cannot be the same as the source floor
        /// </summary>
        /// <param name="sourceFloor"> The source floor of the request to prevent the target floor to be the same </param>
        /// <returns> The destination floor of the requests </returns>
        private uint TargetFloor(uint sourceFloor)
        {
            double[] score = new double[this.Building.BuildingFloors];

            var rand = new Random();

            for (int i = 0; i < score.Length; i++)
            {
                if (i != sourceFloor)
                {
                    score[i] = this.DestinationProbabilityPerFloor[i] * rand.NextDouble();
                }
                else
                {
                    score[i] = double.NegativeInfinity;
                }
            }

            return (uint) score.ToList().IndexOf(score.Max());
        }

    }


    class RequestGeneratorFactory
    {
        public static RequestGenerator CreateRequestGenerator(Building building, SimulationConfiguration config)
        {
            switch (config.RequestGeneratorType)
            {
                case "UNIFORM":
                    return new RequestGenerator(building, config);
                case "DAY_CYCLES":
                    return new DayCyclesRequestGenerator(building, config);
                default:
                    throw new UnknownRequestGeneratorException("The request generator type: " + config.RequestGeneratorType + " is unknown.");
            }
        }
    }
}
