using ElevatorSimulator.Elevator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.ElevatorAI
{
    class BenchmarkAI : ElevatorAI
    {
        public BenchmarkAI(Building building, ElevatorCollection elevators) : base(building, elevators)
        {
            
        }

        /// <summary>
        /// Make decisions with the pending requests and order the elevators to move accordingly
        /// </summary>
        public override void HandleRequests()
        {
            
        }
    }
}
