namespace SemesterProject.SerialCommunication
{
	public partial class SerialCommunicator
	{
		partial class DataParserStateMachine
		{
            #region Enums
            public enum StateFlag { FieldA, FieldB, FieldC, FieldD, FieldE, FieldF, FieldG, FieldH, FieldI, FieldJ, Closed, MsgBegin }
			private enum DataCollectionFlag { On, Off, Trigger }
            #endregion

            public StateFlag CurrentState = StateFlag.Closed;
			
			private ParserData parserData;
			private bool triggerParsing = false;

			public DataParserStateMachine()
			{
				parserData = new ParserData();
			}

			/// <summary>
			/// Handles the update process and event data
			/// </summary>
			/// <param name="data">Data to respond to</param>
			/// <param name="e">Event data</param>
			/// <returns>Control flag to trigger an event</returns>
			public bool Update(char data, out SerialStatusUpdateEventArgs e)
			{
				UpdateReaderStatus(data);
				
				if (triggerParsing)
				{
					e = new SerialStatusUpdateEventArgs();
					e.StatusData = ParserData.ParseStatusData(parserData);
				}
				else
				{
					e = SerialStatusUpdateEventArgs.Empty;
				}
				UpdateParserData(data);

				return triggerParsing;
			}

			/// <summary>
			/// Handles internal state update logic
			/// </summary>
			/// <param name="data">Data to respond to</param>
			private void UpdateReaderStatus(char data)
			{
				triggerParsing = false;
                switch (CurrentState)
                {
                    case StateFlag.FieldA:
						if(data == 'B')
							CurrentState = StateFlag.FieldB;
						break;
                    case StateFlag.FieldB:
						if (data == 'C')
							CurrentState = StateFlag.FieldC;
						break;
                    case StateFlag.FieldC:
						if (data == 'D')
							CurrentState = StateFlag.FieldD;
						break;
                    case StateFlag.FieldD:
						if (data == 'E')
							CurrentState = StateFlag.FieldE;
						break;
                    case StateFlag.FieldE:
						if (data == 'F')
							CurrentState = StateFlag.FieldF;
						break;
                    case StateFlag.FieldF:
						if (data == 'G')
							CurrentState = StateFlag.FieldG;
						break;
                    case StateFlag.FieldG:
						if (data == 'H')
							CurrentState = StateFlag.FieldH;
						break;
                    case StateFlag.FieldH:
						if (data == 'I')
							CurrentState = StateFlag.FieldI;
						break;
                    case StateFlag.FieldI:
						if (data == 'J')
							CurrentState = StateFlag.FieldJ;
						break;
                    case StateFlag.FieldJ:
						if (data == '#')
						{
							CurrentState = StateFlag.Closed;
							triggerParsing = true;
						}
						break;
                    case StateFlag.Closed:
						if (data == '$')
							CurrentState = StateFlag.MsgBegin;
						break;
                    case StateFlag.MsgBegin:
						if (data == 'A')
							CurrentState = StateFlag.FieldA;
						break;
                    default:
						break;
                }
			}

			/// <summary>
			/// Handles data parsing into a <see cref="ParserData"/> object
			/// </summary>
			/// <param name="data">Data to respond to</param>
			private void UpdateParserData(char data)
			{
				if (char.IsDigit(data) && CurrentState != StateFlag.Closed)
				{
					switch (CurrentState)
					{
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
				else parserData.Clear();
			}
		}
	}
}
