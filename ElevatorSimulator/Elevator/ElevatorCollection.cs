using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class ElevatorCollection : IEnumerable<Elevator>
    {
        // List of elevators
        public List<Elevator> Elevators
        {
            get;
            private set;
        }

        // Count of all requests that are currently being serviced by the elevators
        public int PickedUpRequestsCount
        {
            get
            {
                return this.Elevators.Sum(elevator => elevator.PickedUpRequests.Count());
            }
        }

        public List<Request> OnTheWayRequests
        {
            get
            {
                List<Request> onTheWayRequests = new List<Request>();

                foreach (Elevator elevator in this.Elevators)
                {
                    onTheWayRequests.AddRange(elevator.OnTheWayRequests);
                }

                return onTheWayRequests;
            }
        }
        
        public ElevatorCollection(SimulationConfiguration config)
        {
            this.Elevators = new List<Elevator>();

            for (int i = 0; i < config.NumberOfElevators; i++)
            {
                this.Elevators.Add(new Elevator(config));
            }
        }

        public uint Tick()
        {
            uint totalEnergyUsed = 0;
            for (int i = 0; i < this.Elevators.Count(); i++)
            {
                totalEnergyUsed += this.Elevators[i].Tick();
            }

            return totalEnergyUsed;
        }

        public List<Elevator> GetIdleElevators()
        {
            return this.Elevators.FindAll(elevator => elevator.IsIdle);
        }

        public IEnumerator<Elevator> GetEnumerator()
        {
            return this.Elevators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Elevator GetClosestElevator(List<Elevator> elevatorList, uint floor)
        {
            var distanceElevatorToRequest = elevatorList.Select(elevator => Math.Abs((int) elevator.CurrentFloor - (int) floor)).ToList();
            int closestElevatorIndex = distanceElevatorToRequest.IndexOf(distanceElevatorToRequest.Min());

            return elevatorList[closestElevatorIndex];
        }
    }
}
