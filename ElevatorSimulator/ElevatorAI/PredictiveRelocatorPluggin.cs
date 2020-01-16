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

            // Keep track of a rolling average of the past half an hour
            this.N = (config.TotalResidents * config.AverageRequestsPerResidentPerDay) / 48;

            this.RequestQueue = new Queue<Request>();
        }

        public void NotifyRequest(Request newRequest)
        {
            if (this.IsReady)
            {
                var outdatedRequest = this.RequestQueue.Dequeue();
                // If we are ready, we must remove the request that was made N requests ago from our running average
                this.RequestsDistribution[outdatedRequest.Source]--;
            }

            // Add the request to the queue
            this.RequestQueue.Enqueue(newRequest);

            // Add to the count of requests that were made on the floor of the request
            this.RequestsDistribution[newRequest.Source]++;
        }

        public void RelocateIdleElevator(Elevator.Elevator elevator)
        {
            if (!IsReady)
            {
                // If not ready, return
                return;
            }

            var otherElevators = this.Elevaotrs.Elevators.FindAll(e => e != elevator);

            List<int> otherElevatorPositions = new List<int>();

            foreach (Elevator.Elevator e in otherElevators)
            {
                if (e.IsIdle)
                {
                    otherElevatorPositions.Add((int) e.CurrentFloor);
                }
                else
                {
                    otherElevatorPositions.Add((int) e.Waypoints.Last().DestinationFloor);
                }
            }

            var bestPos = BestPosition(otherElevatorPositions);

            if (bestPos != elevator.CurrentFloor)
            {
                elevator.AddWaypoint(new ElevatorWaypoint(bestPos, WaypointType.RELOCATION));
            }
            
        }

        private uint BestPosition(List<int> otherElevatorPositions)
        {
            int currentBestPos = int.MaxValue;
            int bestScore = int.MaxValue;



            for (int potentialPos = 0; potentialPos < this.RequestsDistribution.Count(); potentialPos++)
            {
                int potentialScore = 0;
                otherElevatorPositions.Add(potentialPos);

                for (int i = 0; i < this.RequestsDistribution.Count(); i++)
                {
                    var distancesToElevaotrs = otherElevatorPositions.Select(p => Math.Abs(p - i));

                    potentialScore += (int) RequestsDistribution[i] * distancesToElevaotrs.Min();
                }

                otherElevatorPositions.RemoveAt(otherElevatorPositions.Count() - 1);

                if (potentialScore < bestScore)
                {
                    bestScore = potentialScore;
                    currentBestPos = potentialPos;
                }
            }

            if (currentBestPos == int.MaxValue)
            {
                throw new InvalidRelocationException("A problem occured when finding the best relocation floor");
            }

            return (uint) currentBestPos;
        }



    }
}
