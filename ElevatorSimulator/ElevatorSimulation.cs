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
            uint[] interestPerFloor = { 5, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var config = new SimulationConfiguration(3, 86400, 2, 1, "BENCHMARK", "UNIFORM", 10, 1, 5, residentsPerFloor, 5, interestPerFloor);
            var report = new Simulation(config).StartSimulation();
            var reportPerDay = report / new MetricsReport(config.SimulationDays, config.SimulationDays);
            var reportPerPersonPerDay = reportPerDay / new MetricsReport(config.TotalResidents, config.TotalResidents);
            var reportPerPersonPerDayPerElevatorUse = reportPerPersonPerDay / new MetricsReport(config.AverageRequestsPerResidentPerDay, config.AverageRequestsPerResidentPerDay);

            Console.WriteLine(report + " total");
            Console.WriteLine(reportPerDay + " per day");
            Console.WriteLine(reportPerPersonPerDay + " per person per day");
            Console.WriteLine(reportPerPersonPerDayPerElevatorUse + " per person per day per elevator use");
            Console.ReadKey();
        }
    }
}
