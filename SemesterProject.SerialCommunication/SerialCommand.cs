using System;
using System.Collections.Generic;

namespace SemesterProject.SerialCommunication
{
	public struct SerialCommand
	{
		public enum CommandType { UnitNumber, Date, Time, UpdateRate, Output, InputAutoUpdate }
		public enum OutputPin { P0, P1, P2, P3, P4, P5, P6, P7, All }
		public CommandType Type { get; private set; }
		public int Argument { get; private set; }

		public SerialCommand(CommandType type, int argument)
		{
			Type = type;
			Argument = argument;
		}
		public static SerialCommand SetUnitNumber(ushort unitNum)
		{
			if (unitNum > 1000) throw new ArgumentException();
			return new SerialCommand(CommandType.UnitNumber, unitNum);
		}

		public static SerialCommand SetUpdateRate(ushort rate)
		{
			if (rate > 1000) throw new ArgumentException();
			return new SerialCommand(CommandType.UpdateRate, rate);
		}

		public static SerialCommand SetInputAutoUpdate(bool flag)
		{
			return new SerialCommand(CommandType.InputAutoUpdate, flag ? 1 : 0);
		}
		public static SerialCommand SetDate(DateTime dateTime)
		{
			var temp = dateTime.Date;
			return new SerialCommand(CommandType.InputAutoUpdate, temp.Year * 10000 + temp.Month * 100 + temp.Day);
		}
		public static SerialCommand SetTime(DateTime dateTime)
		{
			var temp = dateTime.TimeOfDay;
			return new SerialCommand(CommandType.InputAutoUpdate, temp.Hours * 10000 + temp.Minutes * 100 + temp.Seconds);
		}
		public static SerialCommand SetOutputPin(OutputPin pin, bool val)
		{
			byte pinVal = val ? (byte)1 : (byte)0;
			byte pinAddr = 0;
			switch (pin)
			{
				case OutputPin.P0:
					pinAddr = 0;
					break;
				case OutputPin.P1:
					pinAddr = 1;
					break;
				case OutputPin.P2:
					pinAddr = 2;
					break;
				case OutputPin.P3:
					pinAddr = 3;
					break;
				case OutputPin.P4:
					pinAddr = 4;
					break;
				case OutputPin.P5:
					pinAddr = 5;
					break;
				case OutputPin.P6:
					pinAddr = 6;
					break;
				case OutputPin.P7:
					pinAddr = 7;
					break;
				case OutputPin.All:
					pinAddr = 9;
					break;
				default:
					break;
			}

			return new SerialCommand(CommandType.Output, pinAddr * 10 + pinVal);
		}

		public static SerialCommand[] SetOutput(byte outputRegister)
		{
			List<SerialCommand> cmdList = new List<SerialCommand>();
			if (outputRegister == 0 || outputRegister == 0xff)
			{
				cmdList.Add(SetOutputPin(OutputPin.All, outputRegister != 0 ? true : false));
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					byte mask = (byte)(1 << i);
					bool pinVal = (outputRegister & mask) == mask;
					OutputPin pin;

					switch (i)
					{
						case 0:
							pin = OutputPin.P0;
							break;
						case 1:
							pin = OutputPin.P1;
							break;
						case 2:
							pin = OutputPin.P2;
							break;
						case 3:
							pin = OutputPin.P3;
							break;
						case 4:
							pin = OutputPin.P4;
							break;
						case 5:
							pin = OutputPin.P5;
							break;
						case 6:
							pin = OutputPin.P6;
							break;
						case 7:
							pin = OutputPin.P7;
							break;
						default:
							pin = OutputPin.All;
							break;
					}
					cmdList.Add(SetOutputPin(pin, pinVal));
				}
			}

			return cmdList.ToArray();
		}

		public static SerialCommand[] SetDateTime(DateTime dateTime)
		{
			SerialCommand[] commands = { SetDate(dateTime), SetTime(dateTime) };
			return commands;
		}

		public override string ToString()
		{
			string[] commandPrefix = { "$N", "$D", "$T", "$S", "$O", "$E" };
			string cmd;
			string arg;

			switch (Type)
			{
				case CommandType.UnitNumber:
					cmd = commandPrefix[0];
					arg = string.Format("{0:000}", Argument);
					break;
				case CommandType.Date:
					cmd = commandPrefix[1];
					arg = string.Format("{0:00000000}", Argument);
					break;
				case CommandType.Time:
					cmd = commandPrefix[2];
					arg = string.Format("{0:000000}", Argument);
					break;
				case CommandType.UpdateRate:
					cmd = commandPrefix[3];
					arg = string.Format("{0:000}", Argument);
					break;
				case CommandType.Output:
					cmd = commandPrefix[4];

					arg = string.Format("{0:00}", Argument);
					break;
				case CommandType.InputAutoUpdate:
					cmd = commandPrefix[5];

					arg = string.Format("{0:0}", Argument);
					break;
				default:
					cmd = string.Empty;
					arg = string.Empty;
					break;
			}

			return $"{cmd}{arg}";
		}
	}
}
