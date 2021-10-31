using System;

namespace SerialCommunicator
{
	partial class SerialStatusStringParserFsm
	{
		enum ReaderStatusFlag { FieldA, FieldB, FieldC, FieldD, FieldE, FieldF, FieldG, FieldH, FieldI, FieldJ, Closed, MsgBegin }
		enum DataCollectionFlag { On, Off, Trigger }

		ReaderStatusFlag readerStatus = ReaderStatusFlag.Closed;
		DataCollectionFlag collection = DataCollectionFlag.On;
		ParserData parserData;

		public SerialStatusStringParserFsm()
		{
			parserData = new ParserData();
		}

		public bool Update(char data, out SerialStatusUpdateEventArgs e)
		{
			updateReaderStatus(data);
			bool ctrl = updateParsedData(data);
			if (ctrl)
			{
				e = new SerialStatusUpdateEventArgs();
				e.statusData = ParserData.ParseStatusData(parserData);
			}
			else
			{
				e = SerialStatusUpdateEventArgs.Empty;
			}
			return ctrl;
		}
		private void updateReaderStatus(char data)
		{
			switch (data)
			{
				case '$':
					if (readerStatus == ReaderStatusFlag.Closed)
					{
						readerStatus = ReaderStatusFlag.MsgBegin;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'A':
					if (readerStatus == ReaderStatusFlag.MsgBegin)
					{
						readerStatus = ReaderStatusFlag.FieldA;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'B':
					if (readerStatus == ReaderStatusFlag.FieldA)
					{
						readerStatus = ReaderStatusFlag.FieldB;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'C':
					if (readerStatus == ReaderStatusFlag.FieldB)
					{
						readerStatus = ReaderStatusFlag.FieldC;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'D':
					if (readerStatus == ReaderStatusFlag.FieldC)
					{
						readerStatus = ReaderStatusFlag.FieldD;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'E':
					if (readerStatus == ReaderStatusFlag.FieldD)
					{
						readerStatus = ReaderStatusFlag.FieldE;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'F':
					if (readerStatus == ReaderStatusFlag.FieldE)
					{
						readerStatus = ReaderStatusFlag.FieldF;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'G':
					if (readerStatus == ReaderStatusFlag.FieldF)
					{
						readerStatus = ReaderStatusFlag.FieldG;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'H':
					if (readerStatus == ReaderStatusFlag.FieldG)
					{
						readerStatus = ReaderStatusFlag.FieldH;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'I':
					if (readerStatus == ReaderStatusFlag.FieldH)
					{
						readerStatus = ReaderStatusFlag.FieldI;
						collection = DataCollectionFlag.Off;
					}
					break;
				case 'J':
					if (readerStatus == ReaderStatusFlag.FieldI)
					{
						readerStatus = ReaderStatusFlag.FieldJ;
						collection = DataCollectionFlag.Off;
					}
					break;
				case '#':
					readerStatus = ReaderStatusFlag.Closed;
					collection = DataCollectionFlag.Trigger;
					break;
				default:
					collection = DataCollectionFlag.On;
					break;
			}
		}
		private bool updateParsedData(char data)
		{
			bool ctrl = false;
			if (collection == DataCollectionFlag.On)
			{
				switch (readerStatus)
				{
					case ReaderStatusFlag.Closed:
						parserData.Clear();
						break;
					case ReaderStatusFlag.FieldA:
						parserData.fields[0] += data;
						break;
					case ReaderStatusFlag.FieldB:
						parserData.fields[1] += data;
						break;
					case ReaderStatusFlag.FieldC:
						parserData.fields[2] += data;
						break;
					case ReaderStatusFlag.FieldD:
						parserData.fields[3] += data;
						break;
					case ReaderStatusFlag.FieldE:
						parserData.fields[4] += data;
						break;
					case ReaderStatusFlag.FieldF:
						parserData.fields[5] += data;
						break;
					case ReaderStatusFlag.FieldG:
						parserData.fields[6] += data;
						break;
					case ReaderStatusFlag.FieldH:
						parserData.fields[7] += data;
						break;
					case ReaderStatusFlag.FieldI:
						parserData.fields[8] += data;
						break;
					case ReaderStatusFlag.FieldJ:
						parserData.fields[9] += data;
						break;
					default:
						break;
				}
			}
			else if (collection == DataCollectionFlag.Trigger && readerStatus == ReaderStatusFlag.Closed) ctrl = true;

			return ctrl;
		}
	}
}
