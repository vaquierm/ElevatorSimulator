using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class Simulation
    {
        // Number of days the simulation will run for
        public readonly uint SimulationDays;
        // Number of ticks per day
        public readonly long TicksPerDay;

        // Current time of the simulation in ticks
        public long CurrentTick
        {
            get;
            private set;
        }

        // Building that the simulation runs on
        public Building Building
        {
            get;
            private set;
        }


        public Simulation(SimulationConfiguration config)
        {
            // Check the validity of the configuration
            config.CheckValidity();

            this.SimulationDays = config.SimulationDays;
            this.TicksPerDay = config.TicksPerDay;

            this.Building = new Building(config);
        }

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <returns> A Metrics report that shows all metrics of the simulation </returns>
        public MetricsReport StartSimulation()
        {
            Console.WriteLine("Simulation starting...");

            MetricsReport simulationMetrics = new MetricsReport(0, 0, 0);

            while (this.CurrentTick < this.TicksPerDay * this.SimulationDays)
            {
                if (this.CurrentTick % this.TicksPerDay == 0)
                    Console.WriteLine("Simulation start of day " + this.CurrentTick / this.TicksPerDay);

                // Increment the simulation time
                this.CurrentTick++;

                // Make the building tick and update the simulation metrics
                simulationMetrics += this.Building.Tick();
            }

            Console.WriteLine("Simulation ending...");

            return simulationMetrics;
        }
    }

    class SimulationConfiguration
    {
        // Number of days the simulation will run for
        public readonly uint SimulationDays;
        // Number of ticks per day
        public readonly long TicksPerDay;

        // Number of elevators that are used in the simulation
        public readonly uint NumberOfElevators;
        // Energy spent per elevator per tick
        public readonly uint EnergyPerTick;

        // The type of AI that will be used during simulation
        public readonly string AIType;
        // True to activate the smart relocation of idle elevators after dropping off passengers.
        public readonly bool SmartRelocation;

        // Request Generator type
        public readonly string RequestGeneratorType;

        // Number of floors in the simulation building
        public readonly uint BuildingFloors;
        // Speed in floors per tick that elevators travel
        public readonly uint ElevatorSpeed;
        // Loading time in ticks
        public readonly uint LoadingTime;
        // Rsidents per floor in an array
        public readonly uint[] ResidentsPerFloor;
        // Average number of requests per day per residents
        private uint _averageRequestsPerResidentPerDay;
        public uint AverageRequestsPerResidentPerDay
        {
            get {
                return _averageRequestsPerResidentPerDay;
            }
            set
            {
                if (value <= 0)
                {
                    throw new InvalidSimulationConfigException("The average number of requests a resident makes per day must be positive and non zero");
                }
                _averageRequestsPerResidentPerDay = value;
            }
        }
        // Interest per floor (The main floor and prehaps floors with shared spaces have higher interest)
        public readonly double[] InterestPerFloor;
        // Interest per floor non normalized
        private readonly uint[] InterestPerFloorNonNormalized;

        // Total number of resident sin the building
        public uint TotalResidents
        {
            get
            {
                return (uint) this.ResidentsPerFloor.Sum(x => (uint) x);
            }
        }

        public SimulationConfiguration(uint simulationDays, uint ticksPerDay, uint numberOfElevators, uint energyPerTick, string AIType, bool smartRelocation, string requestGeneratorType, uint buildingFloors, uint elevatorSpeed, uint loadingTime, uint[] residentsPerFloor, uint[] interestPerFloor)
        {
            this.SimulationDays = simulationDays;
            this.TicksPerDay = ticksPerDay;

            this.NumberOfElevators = numberOfElevators;
            this.EnergyPerTick = energyPerTick;

            this.AIType = AIType;
            this.SmartRelocation = smartRelocation;
            this.RequestGeneratorType = requestGeneratorType;

            this.BuildingFloors = buildingFloors;
            this.ElevatorSpeed = elevatorSpeed;
            this.LoadingTime = loadingTime;

            this.ResidentsPerFloor = residentsPerFloor;

            this.AverageRequestsPerResidentPerDay = 2;

            long totalInterest = interestPerFloor.Sum(x => x);
            this.InterestPerFloor = interestPerFloor.Select(interest => ((double)interest / totalInterest)).ToArray();

            this.InterestPerFloorNonNormalized = interestPerFloor;

            // Check if the configuration is valid
            this.CheckValidity();
        }

        public void CheckValidity()
        {
            if (this.BuildingFloors != this.ResidentsPerFloor.Count())
            {
                throw new InvalidSimulationConfigException("The list of residents per floor must match the number of floors in the building");
            }
            if (this.BuildingFloors != this.InterestPerFloor.Count())
            {
                throw new InvalidSimulationConfigException("The list of residents per floor must match the number of floors in the building");
            }

            if (this.TicksPerDay <= 0)
            {
                throw new InvalidSimulationConfigException("The number of ticks per day must be positive and non zero");
            }
            if (this.BuildingFloors < 1)
            {
                throw new InvalidSimulationConfigException("There must be at least more than one floor in the building");
            }
            if (this.ElevatorSpeed <= 0)
            {
                throw new InvalidSimulationConfigException("The speed of the elevator in floors per tick must be a positive number");
            }
            if (this.LoadingTime <= 0)
            {
                throw new InvalidSimulationConfigException("The loading time must be bigger than 0 ticks");
            }
            if (this.SimulationDays <= 0)
            {
                throw new InvalidSimulationConfigException("The number of simulation days must be positive and non zero");
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Simulation days: " + this.SimulationDays + "\n");
            builder.Append("Ticks per day: " + this.TicksPerDay + "\n");

            builder.Append("Number of elevators: " + this.NumberOfElevators + "\n");
            builder.Append("Energy per tick: " + this.EnergyPerTick + "\n");

            builder.Append("AI type: " + this.AIType + "\n");
            builder.Append("Smart Relocator: " + this.SmartRelocation + "\n");
            builder.Append("Request generator type: " + this.RequestGeneratorType + "\n");

            builder.Append("Building floors: " + this.BuildingFloors + "\n");
            builder.Append("Elevator speed: " + this.ElevatorSpeed + "\n");

            builder.Append("Loading time: " + this.LoadingTime + "\n");

            builder.Append("Residents per floor: [" + string.Join(", ", this.ResidentsPerFloor) + "]\n");
            builder.Append("Interest per floor: [" + string.Join(", ", this.InterestPerFloorNonNormalized) + "]\n");

            return builder.ToString();
        }
    }
}
