using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FakeSocketDevice
{
    public class FakeTcpServer
    {
        private TcpListener _listener;
        private Thread _acceptThread;
        private Thread _broadcastThread;
        private volatile bool _isRunning;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();
        private readonly int _port;
        private readonly int _broadcastIntervalMs;

        public FakeTcpServer(int port = 2001, int broadcastIntervalMs = 10000)
        {
            _port = port;
            _broadcastIntervalMs = broadcastIntervalMs;
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();

            Log($"TCP Server started on port {_port}");

            // Start accepting connections in background
            _acceptThread = new Thread(AcceptClients)
            {
                IsBackground = true,
                Name = "FakeTcpServer_Accept"
            };
            _acceptThread.Start();

            // Start broadcasting GUIDs in background
            _broadcastThread = new Thread(BroadcastGuids)
            {
                IsBackground = true,
                Name = "FakeTcpServer_Broadcast"
            };
            _broadcastThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            // Close all client connections
            foreach (var kvp in _clients)
            {
                try
                {
                    kvp.Value.Close();
                }
                catch { }
            }
            _clients.Clear();

            // Stop the listener
            try
            {
                _listener?.Stop();
            }
            catch { }

            Log("TCP Server stopped");
        }

        private void AcceptClients()
        {
            while (_isRunning)
            {
                try
                {
                    if (_listener.Pending())
                    {
                        var client = _listener.AcceptTcpClient();
                        var clientId = Guid.NewGuid().ToString().Substring(0, 8);
                        var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";

                        _clients.TryAdd(clientId, client);
                        Log($"Client connected: {endpoint} (ID: {clientId})");

                        // Send welcome message
                        var welcomeMsg = $"Connected to FakeSocketDevice. You will receive a GUID every {_broadcastIntervalMs / 1000} seconds.\r\n";
                        SendToClient(client, welcomeMsg);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (SocketException) when (!_isRunning)
                {
                    // Expected when stopping
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Log($"Error accepting client: {ex.Message}");
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void BroadcastGuids()
        {
            while (_isRunning)
            {
                try
                {
                    Thread.Sleep(_broadcastIntervalMs);

                    if (!_isRunning)
                        break;

                    var guid = Guid.NewGuid().ToString();
                    var message = $"{guid}\r\n";
                    var timestamp = DateTime.Now.ToString("HH:mm:ss");

                    if (_clients.Count > 0)
                    {
                        Log($"[{timestamp}] Broadcasting: {guid} to {_clients.Count} client(s)");
                    }

                    // Send to all connected clients
                    foreach (var kvp in _clients)
                    {
                        if (!SendToClient(kvp.Value, message))
                        {
                            // Client disconnected, remove it
                            if (_clients.TryRemove(kvp.Key, out _))
                            {
                                Log($"Client disconnected: {kvp.Key}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Log($"Error broadcasting: {ex.Message}");
                    }
                }
            }
        }

        private bool SendToClient(TcpClient client, string message)
        {
            try
            {
                if (!client.Connected)
                    return false;

                var stream = client.GetStream();
                var bytes = Encoding.ASCII.GetBytes(message);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}
