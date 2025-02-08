using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using static recoveryNodindow.myPackages.serverTcpClient.ServerManager.ClientO;
using static recoveryNodindow.myPackages.serverTcpClient.ServerManager;

namespace recoveryNodindow.myPackages
{
    public class serverTcpClient
    {
        public class ServerManager
        {
            public List<ClientO> allClients = new List<ClientO>();
            public readonly object lockObj = new object();
            private Thread serverReceiveClient { get; set; }
            private Thread serverRemoveClient { get; set; }
            private bool isRunning = true;
            private TcpListener server { get; set; }
            public ServerManager(IPAddress iPAddress, int port, onMessage eventHandler)
            {
                this.server = new TcpListener(iPAddress, port);
                server.Start();

                serverReceiveClient = new Thread(() =>
                {
                    while (isRunning)
                    {
                        try
                        {
                            TcpClient newClient = server.AcceptTcpClient();
                            lock (lockObj)
                            {
                                allClients.Add(new ClientO(newClient, eventHandler));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao aceitar cliente: {ex.Message}");
                        }
                    }
                });
                serverReceiveClient.Start();
                serverRemoveClient = new Thread(() =>
                {
                    while (isRunning)
                    {
                        lock (lockObj)
                        {
                            this.allClients = allClients.Where(client => client.client.Connected == true).ToList();
                        }
                    }
                });
                serverRemoveClient.Start();
            }
            public void closeServer()
            {
                isRunning = false;
                allClients.ForEach(client => client.closeConnection());
                this.server.Stop();
            }
            public int getFreePort(int startPort)
            {
                for (int port = startPort; port <= 65535; port++)
                {
                    TcpListener listener = null;
                    try
                    {
                        listener = new TcpListener(IPAddress.Loopback, port);
                        listener.Start(); // Se iniciar sem erro, a porta está livre
                        listener.Stop();
                        return port;
                    }
                    catch (SocketException)
                    {
                        // Porta já está em uso, tenta a próxima
                        continue;
                    }
                    finally
                    {
                        listener?.Stop(); // Garante que o listener pare caso tenha sido iniciado
                    }
                }
                throw new Exception("Nenhuma porta disponível encontrada.");
            }


            public class ClientO
            {
                public TcpClient client { get; set; }
                public delegate void onMessage(object sender, EventArgsOnMessage e);
                public event onMessage onMessageEvent;

                private StreamReader streamReader;
                private StreamWriter streamWriter;
                public Thread clientReceiveMsgThread;
                private bool isRunning = true;

                public ClientO(TcpClient client, onMessage eventHandler)
                {
                    this.client = client;
                    onMessageEvent += eventHandler;

                    streamReader = new StreamReader(client.GetStream());
                    streamWriter = new StreamWriter(client.GetStream()) { AutoFlush = true };

                    clientReceiveMsgThread = new Thread(() =>
                    {
                        try
                        {
                            while (isRunning)
                            {
                                string message = streamReader.ReadLine();
                                if (message == null) // Cliente desconectado
                                    break;

                                OnMessageEventInvoke(message);
                            }
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("Cliente desconectado inesperadamente.");
                        }
                    });
                    clientReceiveMsgThread.Start();
                }

                public bool sendMessage(string message)
                {
                    try
                    {
                        streamWriter.WriteLine(message);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Erro ao enviar mensagem: {e.Message}");
                        return false;
                    }
                }
                public void closeConnection()
                {
                    isRunning = false;
                    client.Close();
                }

                public void OnMessageEventInvoke(string message)
                {
                    onMessageEvent?.Invoke(this, new EventArgsOnMessage { Message = message, ClientO = this });
                }

                public class EventArgsOnMessage : EventArgs
                {
                    public string Message { get; set; }
                    public ClientO ClientO { get; set; }

                    public void Send(string message)
                    {
                        ClientO.sendMessage(message);
                    }
                }
            }
        }
        public class ClientConnect
        {
            private StreamWriter streamWriter;
            private StreamReader streamReader;
            public delegate void onMessage(object sender, EventArgsOnMessage e);
            public event onMessage onMessageEvent;
            private TcpClient server;
            private Thread serverReceiveMsgThread;
            private bool isRunning = true;
            public ClientConnect(IPAddress iPAddress, int port, onMessage eventHandler)
            {
                this.server = new TcpClient();
                onMessageEvent += eventHandler;
                this.server.Connect(iPAddress, port);
                this.streamWriter = new StreamWriter(server.GetStream()) { AutoFlush = true };
                this.streamReader = new StreamReader(server.GetStream());

                serverReceiveMsgThread = new Thread(() =>
                {
                    try
                    {
                        while (isRunning)
                        {
                            string message = streamReader.ReadLine();
                            if (message == null) // Servidor desconectado
                                break;

                            OnMessageEventInvoke(message);
                        }
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Conexão com o servidor perdida.");
                    }
                });
                serverReceiveMsgThread.Start();
            }
            public void closeConnection()
            {
                isRunning = false;
                serverReceiveMsgThread?.Abort();
                server.Close();
            }
            public void OnMessageEventInvoke(string mensage)
            {
                // Verifique se há assinantes para o evento
                onMessageEvent?.Invoke(this, new EventArgsOnMessage { mensage = mensage, clientConnect = this });
            }
            public void sendMessage(string message)
            {
                streamWriter.WriteLine(message);
            }
            public class EventArgsOnMessage : EventArgs
            {
                public string mensage { get; set; }
                public ClientConnect clientConnect { get; set; }
                public void send(string message)
                {
                    clientConnect.sendMessage(message);
                }
            }
        }
    }
}
