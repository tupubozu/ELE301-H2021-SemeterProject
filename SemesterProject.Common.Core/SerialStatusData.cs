using System;

namespace SemesterProject.Common.Core
{
	public class SerialStatusData
	{
		public enum DataSource { Physical, Simulation }

		public DataSource Source {get; private set;}
		public ushort UnitNumber { get; private set; }
		public DateTime Timestamp { get; private set; }
		public byte InputStatus { get; private set; }
		public byte OutputStatus { get; private set; }
		public ushort Thermistor { get; private set; }
		public ushort Analog1 { get; private set; }
		public ushort Analog2 { get; private set; }
		public ushort TemperatureIC1 { get; private set; }
		public ushort TemperatureIC2 { get; private set; }

		public SerialStatusData(ushort init_UnitNumber, DateTime init_Timestamp, byte init_InputStatus, byte init_OutputStatus, ushort init_Thermistor, ushort init_Analog1, ushort init_Analog2, ushort init_TemperatureIC1, ushort init_TemperatureIC2, DataSource init_Source)
		{
			UnitNumber = init_UnitNumber;
			Timestamp = init_Timestamp;
			InputStatus = init_InputStatus;
			OutputStatus = init_OutputStatus;
			Thermistor = init_Thermistor;
			Analog1 = init_Analog1;
			Analog2 = init_Analog2;
			TemperatureIC1 = init_TemperatureIC1;
			TemperatureIC2 = init_TemperatureIC2;
			Source = init_Source;
		}
	}
}
