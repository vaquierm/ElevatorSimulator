using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSimulator.Elevator;

namespace ElevatorSimulator.ElevatorAI
{
    class PredictiveRelocatorPluggin
    {
        // The elevators contained in the building
        public readonly ElevatorCollection Elevaotrs;

        // Distribution of where in the building the last N requests were made
        private uint[] RequestsDistribution;

        // A running average of the last N requests and which floors they where made at
        private double[] AverageRequestsPercentage
        {
            get
            {
                return this.RequestsDistribution.Select(count => ((double)count) / this.N).ToArray();
            }
        }

        // The number of requests that are kept track of in the running average
        private readonly uint N;

        // Indicates if the relocator has accurate distributions yet
        private bool IsReady
        {
            get
            {
                return this.RequestQueue.Count() > N;
            }
        }

        private Queue<Request> RequestQueue;

        public PredictiveRelocatorPluggin(ElevatorCollection elevators, SimulationConfiguration config)
        {
            this.Elevaotrs = elevators;

            this.RequestsDistribution = new uint[config.BuildingFloors];

            this.N = (config.TotalResidents * config.AverageRequestsPerResidentPerDay) / 12;

            this.RequestQueue = new Queue<Request>();
        }

        public void NotifyRequest(Request newRequest)
        {
            // Add the request to the queue
            this.RequestQueue.Enqueue(newRequest);

            // Add to the count of requests that were made on the floor of the request
            this.RequestsDistribution[newRequest.Source]++;

            if (this.IsReady)
            {
                var outdatedRequest = this.RequestQueue.Dequeue();
                // If we are ready, we must remove the request that was made N requests ago from our running average
                this.RequestsDistribution[outdatedRequest.Source]--;
            }
        }
    }
}
