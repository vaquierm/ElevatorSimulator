using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class ElevatorSimulation
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Simulation starting...");
            uint[] residentsPerFloor = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            uint[] interestPerFloor = { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var config = new SimulationConfiguration(1, 86400, 4, 1, "BENCHMARK", "UNIFORM", 10, 1, 5, residentsPerFloor, 10, interestPerFloor);
            var report = new Simulation(config).StartSimulation();

            Console.WriteLine(report);
            Console.ReadKey();
        }
    }
}
