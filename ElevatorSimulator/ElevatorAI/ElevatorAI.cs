using ElevatorSimulator.Elevator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.ElevatorAI
{
    abstract class ElevatorAI : ElevatorController
    {
        public List<Request> PendingRequests
        {
            get;
            private set;
        }

        public List<Request> PickedUpRequests
        {
            get;
            private set;
        }

        public ElevatorCollection Elevators;

        public Building Building;

        public ElevatorAI(Building building, ElevatorCollection elevators)
        {
            this.Building = building;
            this.Elevators = elevators;
        }

        public abstract void HandleRequests();

        public void NewRequests(List<Request> newRequests)
        {
            this.PendingRequests.AddRange(newRequests);
        }
    }

    interface ElevatorController
    {
        void HandleRequests();
        void NewRequests(List<Request> newRequests);
    }
}
