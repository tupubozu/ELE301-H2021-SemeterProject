namespace SemesterProject.SerialCommunication
{
	public partial class SerialCommunicator
	{
		partial class DataParserStateMachine
		{
			public enum StateFlag { FieldA, FieldB, FieldC, FieldD, FieldE, FieldF, FieldG, FieldH, FieldI, FieldJ, Closed, MsgBegin }
			enum DataCollectionFlag { On, Off, Trigger }

			public StateFlag CurrentState = StateFlag.Closed;
			DataCollectionFlag collection = DataCollectionFlag.On;
			ParserData parserData;

			public DataParserStateMachine()
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
					e.StatusData = ParserData.ParseStatusData(parserData);
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
						if (CurrentState == StateFlag.Closed)
						{
							CurrentState = StateFlag.MsgBegin;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'A':
						if (CurrentState == StateFlag.MsgBegin)
						{
							CurrentState = StateFlag.FieldA;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'B':
						if (CurrentState == StateFlag.FieldA)
						{
							CurrentState = StateFlag.FieldB;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'C':
						if (CurrentState == StateFlag.FieldB)
						{
							CurrentState = StateFlag.FieldC;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'D':
						if (CurrentState == StateFlag.FieldC)
						{
							CurrentState = StateFlag.FieldD;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'E':
						if (CurrentState == StateFlag.FieldD)
						{
							CurrentState = StateFlag.FieldE;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'F':
						if (CurrentState == StateFlag.FieldE)
						{
							CurrentState = StateFlag.FieldF;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'G':
						if (CurrentState == StateFlag.FieldF)
						{
							CurrentState = StateFlag.FieldG;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'H':
						if (CurrentState == StateFlag.FieldG)
						{
							CurrentState = StateFlag.FieldH;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'I':
						if (CurrentState == StateFlag.FieldH)
						{
							CurrentState = StateFlag.FieldI;
							collection = DataCollectionFlag.Off;
						}
						break;
					case 'J':
						if (CurrentState == StateFlag.FieldI)
						{
							CurrentState = StateFlag.FieldJ;
							collection = DataCollectionFlag.Off;
						}
						break;
					case '#':
						CurrentState = StateFlag.Closed;
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
					switch (CurrentState)
					{
						case StateFlag.Closed:
							parserData.Clear();
							break;
						case StateFlag.FieldA:
							parserData.Fields[0] += data;
							break;
						case StateFlag.FieldB:
							parserData.Fields[1] += data;
							break;
						case StateFlag.FieldC:
							parserData.Fields[2] += data;
							break;
						case StateFlag.FieldD:
							parserData.Fields[3] += data;
							break;
						case StateFlag.FieldE:
							parserData.Fields[4] += data;
							break;
						case StateFlag.FieldF:
							parserData.Fields[5] += data;
							break;
						case StateFlag.FieldG:
							parserData.Fields[6] += data;
							break;
						case StateFlag.FieldH:
							parserData.Fields[7] += data;
							break;
						case StateFlag.FieldI:
							parserData.Fields[8] += data;
							break;
						case StateFlag.FieldJ:
							parserData.Fields[9] += data;
							break;
						default:
							break;
					}
				}
				else if (collection == DataCollectionFlag.Trigger && CurrentState == StateFlag.Closed) ctrl = true;

				return ctrl;
			}
		}
	}
}
