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
        
        // Elevators that can be controlled by the AI
        public ElevatorCollection Elevators;

        // The building that the elevators are in
        public Building Building;

        // The requests that have elevators on their way for
        public List<Request> HandledRequests;

        public ElevatorAI(Building building, ElevatorCollection elevators)
        {
            this.Building = building;
            this.Elevators = elevators;

            this.HandledRequests = new List<Request>();
        }

        public abstract void HandleRequests();

        public void NotifyPickUp(Request request)
        {
            this.HandledRequests.Remove(request);
        }

        public abstract void NotifyDropOff(Request request);
    }

    class ElevatorAIFactory
    {
        public static ElevatorAI CreateElevatorAI(string AIType, Building building, ElevatorCollection elevators)
        {
            switch (AIType)
            {
                case "BENCHMARK":
                    return new BenchmarkAI(building, elevators);
                default:
                    throw new UnknownAIException("The Elevator AI: " + AIType + " is unknown.");

            }
        }
    }

    interface ElevatorController
    {
        void HandleRequests();

        void NotifyPickUp(Request request);

        void NotifyDropOff(Request request);
    }
}
