using System;

namespace SemesterProject.SerialCommunication
{
	public partial class SerialCommunicator
	{
		partial class DataParserStateMachine
		{
			public class ParserData
			{
				public string[] Fields = new string[10];

				public void Clear()
				{
					for (int i = 0; i < Fields.Length; i++)
					{
						Fields[i] = string.Empty;
					}
				}
				public static SerialStatusData ParseStatusData(ParserData data)
				{
					int offset = data.Fields[data.Fields.Length - 1] != string.Empty ? 1 : 0;
					int date = Convert.ToInt32(data.Fields[1], 10);
					int time = Convert.ToInt32(data.Fields[2], 10);

					return new SerialStatusData(
						init_UnitNumber: Convert.ToUInt16(data.Fields[0], 10),
						init_Timestamp: new DateTime(
							year: date / 10000,
							month: (date % 10000) / 100,
							day: date % 100,
							hour: time / 10000,
							minute: (time % 10000) / 100,
							second: time % 100),
						init_InputStatus: Convert.ToByte(data.Fields[3], 2),
						init_OutputStatus: offset == 1 ? Convert.ToByte(data.Fields[4], 2) : (byte)0,
						init_Thermistor: Convert.ToUInt16(data.Fields[4 + offset], 10),
						init_Analog1: Convert.ToUInt16(data.Fields[5 + offset], 10),
						init_Analog2: Convert.ToUInt16(data.Fields[6 + offset], 10),
						init_TemperatureIC1: Convert.ToUInt16(data.Fields[7 + offset], 10),
						init_TemperatureIC2: Convert.ToUInt16(data.Fields[8 + offset], 10),
						init_Source: offset == 1 ? SerialStatusData.DataSource.Simulation : SerialStatusData.DataSource.Physical
						);

				}
			}
		}
	}
}
