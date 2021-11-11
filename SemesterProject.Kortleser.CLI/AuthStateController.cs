﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemesterProject.Common.Core;
using SemesterProject.SerialCommunication;

namespace SemesterProject.Kortleser.CLI
{
    internal class AuthStateController
    {
        public event EventHandler<SerialStatusUpdateEventArgs> Closed;
        public event EventHandler<SerialStatusUpdateEventArgs> Breached;
        public event EventHandler<SerialStatusUpdateEventArgs> KeypadPress;
        public event EventHandler<SerialStatusUpdateEventArgs> AuthSuccess;
        public event EventHandler<SerialStatusUpdateEventArgs> AuthFailure;
        public event EventHandler<SerialStatusUpdateEventArgs> AuthTimeout;
        public event EventHandler<SerialStatusUpdateEventArgs> RequestAccessTable;

        public const int PotentiometerThresholdClosed = 250;
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
        private ushort keyCode = 0;

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
            keyCode = (ushort)(keyCode * 10 + pinNumber);

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
                    keyCode = 0;
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
                    else if (currentTime - lastKeypadPress >= CheckTimeoutSpan)CurrentState = State.Closed;
                    else if (keypressCounter == 3) 
                    {
                        if (!(AuthTable is null))
                        {
                            IEnumerable<bool> lookupResult;
                            lock (AuthTable)
                            {
                                lookupResult = AuthTable.Select((UserPermission up, int i) =>
                                {
                                    return up.PassCode == keyCode;
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
            SerialStatusUpdateEventArgs e = new SerialStatusUpdateEventArgs()
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
                    AuthFailure?.Invoke(this, e);
                    break;
                case (_, State.Authorized):
                    AuthSuccess?.Invoke(this, e);
                    break;
                case (_, State.Breached):
                    Breached?.Invoke(this, e);
                    break;
            }
        }
    }
}