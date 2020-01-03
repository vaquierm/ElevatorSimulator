using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ElevatorSimulator
{
    class ElevatorSimulation
    {
        static void Main(string[] args)
        {
            var config_files = Directory.GetFiles("./", "*.json");

            foreach (var config_file in config_files)
            {
                var config = JsonConvert.DeserializeObject<SimulationConfiguration>(File.ReadAllText(config_file));

                Console.WriteLine("Configuration:\n" + config);

                var report = new Simulation(config).StartSimulation();
                var reportPerDay = report / new MetricsReport(config.SimulationDays, config.SimulationDays, config.SimulationDays);
                var reportPerPersonPerDay = reportPerDay / new MetricsReport(config.TotalResidents, config.TotalResidents, config.TotalResidents);
                var reportPerPersonPerDayPerElevatorUse = reportPerPersonPerDay / new MetricsReport(config.AverageRequestsPerResidentPerDay, config.AverageRequestsPerResidentPerDay, config.AverageRequestsPerResidentPerDay);


                Console.WriteLine("\nResults: ");
                Console.WriteLine(report + " total");
                Console.WriteLine(reportPerDay + " per day");
                Console.WriteLine(reportPerPersonPerDay + " per person per day");
                Console.WriteLine(reportPerPersonPerDayPerElevatorUse + " per person per day per elevator use");

                Console.WriteLine("\n\n");

            }
            Console.ReadKey();
        }
    }
}
