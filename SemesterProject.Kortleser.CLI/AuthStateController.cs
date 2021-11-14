using SemesterProject.Common.Core;
using SemesterProject.SerialCommunication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SemesterProject.Kortleser.CLI
{
	internal class AuthStateController
	{
		public event EventHandler<AuthControllerEventArgs> Closed;
		public event EventHandler<AuthControllerEventArgs> Breached;
		public event EventHandler<AuthControllerEventArgs> KeypadPress;
		public event EventHandler<AuthControllerEventArgs> AuthSuccess;
		public event EventHandler<AuthControllerEventArgs> AuthFailure;
		public event EventHandler<AuthControllerEventArgs> AuthTimeout;
		public event EventHandler<AuthControllerEventArgs> RequestAccessTable;

		public const int PotentiometerThresholdClosed = 500;
		SerialStatusData lastData;
		public SortedSet<UserPermission> AuthTable;

		public enum State { Closed, Breached, Authorized, Unauthorized, CheckAuth }
		public State CurrentState { get; private set; } = State.Closed;

		private State PreviousState;
		private DateTime lastKeypadPress;
		private DateTime currentTime;
		private readonly TimeSpan AuthTimeoutSpan = TimeSpan.FromSeconds(10);
		private readonly TimeSpan CheckTimeoutSpan = TimeSpan.FromSeconds(3);
		private byte keypressCounter = 0;
		private byte currentKeyMask = 0;
		private uint keySequence = 0;

		private bool keyPressed = false;
		public void Update(SerialStatusData data)
		{
			currentTime = DateTime.Now;
			PreviousState = CurrentState;

			currentKeyMask = (byte)((data.InputStatus | lastData.InputStatus) ^ lastData.InputStatus);

			byte pinNumber = 0;
			for (int i = 7; i >= 0; i--)
			{
				byte mask = (byte)(1 << i);
				if ((currentKeyMask & mask) == mask) pinNumber = (byte)(8 - i);
			}
			keySequence = (ushort)(keySequence * 10 + pinNumber);

			if (data.InputStatus != lastData.InputStatus && currentKeyMask != 0x00)
			{
				keypressCounter++;
				keyPressed = true;
				lastKeypadPress = currentTime;
			}

			UpdateState(data);
			RespondToState(data);


			lastData = data;
		}

		private void UpdateState(SerialStatusData data)
		{
			switch (CurrentState)
			{
				case State.Closed:
					if (data.Analog1 > PotentiometerThresholdClosed) CurrentState = State.Breached;
					else if (data.InputStatus != lastData.InputStatus && currentKeyMask != 0x00) CurrentState = State.CheckAuth;
					keySequence = 0;
					keypressCounter = 0;
					break;
				case State.Breached:
					if (data.Analog1 <= PotentiometerThresholdClosed) CurrentState = State.Closed;
					break;
				case State.Authorized:
					if (currentTime - lastKeypadPress >= AuthTimeoutSpan)
					{
						if (data.Analog1 > PotentiometerThresholdClosed) CurrentState = State.Breached;
						else CurrentState = State.Closed;
					}
					break;
				case State.Unauthorized:
					if (data.Analog1 > PotentiometerThresholdClosed) CurrentState = State.Breached; // Priority
					else CurrentState = State.Closed;
					break;
				case State.CheckAuth:
					if (data.Analog1 > PotentiometerThresholdClosed) CurrentState = State.Breached; // Priority
					else if (currentTime - lastKeypadPress >= CheckTimeoutSpan) CurrentState = State.Closed;
					else if (keypressCounter == 7)
					{
						if (!(AuthTable is null))
						{
							IEnumerable<bool> lookupResult;
							lock (AuthTable)
							{
								lookupResult = AuthTable.Select((UserPermission up, int i) =>
								{
									return up.CardId == (keySequence / 10000) && up.CardCode == (keySequence % 10000);
								});
							}
							if (lookupResult.Contains(true)) CurrentState = State.Authorized;
							else CurrentState = State.Unauthorized;
						}
						else CurrentState = State.Authorized;
					}
					break;
				default:
					break;
			}
		}

		private void RespondToState(SerialStatusData data)
		{
			AuthControllerEventArgs e = new AuthControllerEventArgs()
			{
				StatusData = data
			};

			if (AuthTable is null)
			{
				RequestAccessTable?.Invoke(this, e);
			}

			if (keyPressed)
			{
				KeypadPress?.Invoke(this, e);
				keyPressed = false;
			}

			switch (PreviousState, CurrentState)
			{
				case (State.Closed, State.Closed):
				case (State.Authorized, State.Authorized):
				case (State.Breached, State.Breached):
				default:
					break;
				case (State.CheckAuth, State.Closed):
					AuthTimeout?.Invoke(this, e);
					break;
				case (_, State.Closed):
					Closed?.Invoke(this, e);
					break;
				case (_, State.Unauthorized):
					e.Permission = AuthTable.Where((UserPermission u) => u.CardId == (keySequence / keypressCounter > 3 ? (int)Math.Pow(10, keypressCounter - 4) : 1))?.First();
					AuthFailure?.Invoke(this, e);
					break;
				case (_, State.Authorized):
					e.Permission = AuthTable.Where((UserPermission u) => u.CardId == (keySequence / 10000) && u.CardCode == (keySequence % 10000))?.First();
					AuthSuccess?.Invoke(this, e);
					break;
				case (_, State.Breached):
					Breached?.Invoke(this, e);
					break;
			}
		}
	}
}
