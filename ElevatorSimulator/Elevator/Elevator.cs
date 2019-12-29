using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator.Elevator
{
    class Elevator
    {

        // Speed in floors per tick
        public readonly uint Speed;

        // Energy used per tick
        public readonly uint EnergyPerTick;

        public Elevator(SimulationConfiguration config)
        {
            this.Speed = config.ElevatorSpeed;
            this.EnergyPerTick = config.EnergyPerTick;
        }
    }
}
