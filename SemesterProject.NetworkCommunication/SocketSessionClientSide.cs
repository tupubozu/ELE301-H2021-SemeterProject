using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SemesterProject.Common.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security;

namespace SemesterProject.NetworkCommunication
{
    public class SocketSessionClientSide : IDisposable
    {
        const ushort validator = 1999; 
        Queue<SerialStatusData> statusDataQueue = new Queue<SerialStatusData>();
        public Dictionary<int, int> AllowedKeyTable = new Dictionary<int, int>();

        Aes crypto;
        Socket server;
        NetworkStream serverStream;

        UdpClient broadcastInterceptor;
        CancellationTokenSource broadcastCanceller;
        Task broadcastListener;

        CancellationTokenSource canceller;
        Task worker;

        public bool IsCompleted { get => worker.IsCompleted; }


        public SocketSessionClientSide(Aes aes)
        {
            init();

            crypto= aes;

            server = null;

            broadcastInterceptor = new UdpClient(9001);

            Log.Information("Starting broadcast listener on {0}", broadcastInterceptor.Client.LocalEndPoint);
            broadcastListener = Task.Run(() =>
            {
                try
                {
                    for (; ; ) 
                    {
                        broadcastCanceller.Token.ThrowIfCancellationRequested();
                        broadcastUpdate();
                        if (broadcastInterceptor is null) break; 
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Log.Debug(ex, "Broadcast listener stopped");
                }
                catch (Exception ex) 
                {
                    Log.Error(ex, "Unknown error");
                }

            }, broadcastCanceller.Token);
            Log.Information("Started broadcast listener on {0}", broadcastInterceptor.Client.LocalEndPoint);


        }
        public SocketSessionClientSide(IPEndPoint serverEnd,Aes aes)
        {
            init();
            broadcastInterceptor = null;
            crypto = aes;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(serverEnd);
            serverStream = new NetworkStream(server);
            worker = work();
        }
        ~SocketSessionClientSide()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            Log.Debug("Dispose {0}", this.GetType().Name);
            try
            {
                Log.Debug("Stopping broadcast listener: {0}", this.GetType().Name);
                broadcastCanceller?.Cancel();
                if (!broadcastListener.IsCompleted)
                    broadcastListener?.Wait();
                Log.Debug("Stopped broadcast listener: {0}", this.GetType().Name);
                broadcastListener?.Dispose();
                broadcastCanceller?.Dispose();

                Log.Debug("Stopping worker: {0}", this.GetType().Name);
                canceller?.Cancel();
                if (!worker.IsCompleted)
                    worker?.Wait();
                Log.Debug("Stopped worker: {0}", this.GetType().Name);
                worker?.Dispose();
                canceller?.Dispose();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unkown error");
            }
        }

        void init()
        {
            broadcastCanceller = new CancellationTokenSource();
            canceller = new CancellationTokenSource();
        }

        void broadcastUpdate()
        {
            try
            {
                if (server is null)
                {
                    if (broadcastInterceptor.Available != 0)
                    {
                        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        
                        byte[] temp = broadcastInterceptor.Receive(ref serverEndPoint);
                        ushort ctrl = BitConverter.ToUInt16(temp, 0);
                        if (ctrl == validator)
                        {
                            Log.Information("Found server on {0}", serverEndPoint.Address);
                            serverEndPoint.Port = 1337;

                            Log.Information("Connecting to server on {0}", serverEndPoint);
                            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            server.Connect(serverEndPoint);
                            Log.Information("Connected to server on {0}", serverEndPoint);
                            serverStream = new NetworkStream(server);
                            worker = work();
                        }
                    }
                }
                else
                {
                    broadcastInterceptor.Dispose();
                    broadcastInterceptor = null;
                }
            }
            catch (SocketException ex)
            {
                Log.Debug(ex, "Network error");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unknown error");
            }
        }
        Task work()
        {
            Task job = Task.Run(() =>
            {
                try
                {
                    for (; ; )
                    {
                        canceller.Token.ThrowIfCancellationRequested();
                        this.update();
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Log.Information(ex, "Worker thread aborted");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unknown error");
                }
            }, canceller.Token);
            return job;
        }

        public void EnqueueStatusData(SerialStatusData data)
        {
            statusDataQueue.Enqueue(data);
        }

        void update()
        {
            try
            {
                if (server.Available != 0)
                {
                    Dictionary<int, int> data;
                    //SerialStatusData data;
                    
                    using (var decryptor = crypto.CreateDecryptor())
                    using (var cryptoStr = new CryptoStream(serverStream, decryptor, CryptoStreamMode.Read))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        data = binaryFormatter.Deserialize(cryptoStr) as Dictionary<int, int>;
                    }

                }

                if (statusDataQueue.Count != 0)
                {
                    using (var encryptor = crypto.CreateEncryptor())
                    using (var cryptoStr = new CryptoStream(serverStream, encryptor, CryptoStreamMode.Read))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(cryptoStr, statusDataQueue.Dequeue());
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Network communiction failed: {0}", server);
            }
            catch (SerializationException ex)
            {
                Log.Error(ex, "Network communiction failed: {0}", server);
            }
            catch (SecurityException ex)
            {
                Log.Error(ex, "Network communiction failed: {0}", server);
            }

        }
    }
}
