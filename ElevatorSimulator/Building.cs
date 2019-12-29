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

        public Building(SimulationConfiguration config)
        {
            this.BuildingFloors = config.BuildingFloors;
            this.ResidentPerFloor = config.ResidentsPerFloor;
            this.InterestPerFloor = config.InterestPerFloor;
        }

        public void Tick()
        {

        }
    }
}
