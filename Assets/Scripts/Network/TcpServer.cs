using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class CustomTcpClient
{
    public TcpClient Client { get; set; }
    public int NetworkId { get; set; }
}
public class TcpServer : MonoBehaviour
{
    [SerializeField] private NetworkInfo networkInfo;
    [SerializeField] private TextMovement textMovement;
    private TcpListener server;
    private Thread serverThread;
    private bool isRunning = false;
    private List<CustomTcpClient> clients = new List<CustomTcpClient>();
    private object clientsLock = new object(); // To synchronize access to the clients list
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
    private int numberUsers = 0;
    private int countToward = 2;
    public void InitializeServer()
    {
        serverThread = new Thread(new ThreadStart(StartServer));
        serverThread.IsBackground = true;
        serverThread.Start();
        networkInfo.NetworkId = 1;
    }

    void StartServer()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, 8080);
            server.Start();
            isRunning = true;
            Debug.Log("Server started on port 8080.");
            string localIPAddress = GetLocalIPAddress();
            QueueMainThreadAction(() => UIManager.Instance.SetIPAddressText(localIPAddress));
            QueueMainThreadAction(() => ToggleGamePanel(true));
            QueueMainThreadAction(() => UIManager.Instance.SetUserConnectionsText(numberUsers));
            while (isRunning)
            {
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    Debug.Log("Client connected.");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
                else
                {
                    Thread.Sleep(100); // Reduce CPU usage
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Server error: " + e);
            QueueMainThreadAction(() => ToggleGamePanel(false));
        }
        finally
        {
            server?.Stop();
            QueueMainThreadAction(() => ToggleGamePanel(false));
        }
    }

    private void HandleClientComm(object clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;

        lock (clientsLock)
        {
            CustomTcpClient customTcpClient = new CustomTcpClient();
            customTcpClient.Client = tcpClient;
            customTcpClient.NetworkId = countToward;
            clients.Add(customTcpClient);
        }

        NetworkStream clientStream = tcpClient.GetStream();
        byte[] message = new byte[4096];
        int bytesRead;
        QueueMainThreadAction(() => Debug.Log("Client connected"));
        // please handle for disconnected client
        numberUsers++;
        QueueMainThreadAction(() => UIManager.Instance.SetUserConnectionsText(numberUsers));

        try
        {
            // Send welcome message to the client upon connection
            int clientId = InitializeNetworkClientId(clientStream);
            InitializeTableData(clientId);
            while ((bytesRead = clientStream.Read(message, 0, message.Length)) > 0)
            {
                string clientMessage = Encoding.ASCII.GetString(message, 0, bytesRead);
                Debug.Log("Received: " + clientMessage);
                if (clientMessage.Contains("Book"))
                {
                    QueueMainThreadAction(() => HandleBookTable(clientMessage));
                }
                else if (clientMessage.Contains("Cancel"))
                {
                    QueueMainThreadAction(() => HandleCancelTable(clientMessage));
                }
                else if (clientMessage.Contains("Request"))
                {
                    QueueMainThreadAction(() => HandleRequestBooking(clientMessage));
                }
                else if (clientMessage.Contains("RejectChoosing"))
                {
                    QueueMainThreadAction(() => HandleRequestCancelBooking(clientMessage));
                }

                byte[] buffer = Encoding.ASCII.GetBytes("Message received");
                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Client communication error: " + e);
        }
        finally
        {
            lock (clientsLock)
            {
                //clients.Remove(tcpClient);
                RemoveClient(tcpClient);
            }
            tcpClient.Close();
            Debug.Log("Client disconnected.");
            numberUsers--;
            QueueMainThreadAction(() => UIManager.Instance.SetUserConnectionsText(numberUsers));
        }
    }

    private int InitializeNetworkClientId(NetworkStream clientStream)
    {
        string clientId = "ClientId:" + (countToward).ToString();
        countToward++;
        byte[] welcomeBuffer = Encoding.ASCII.GetBytes(clientId);
        clientStream.Write(welcomeBuffer, 0, welcomeBuffer.Length);
        clientStream.Flush();
        return countToward - 1;
    }
    private void InitializeTableData(int clientId)
    {
        string message = "InitializeFloor\n";
        foreach (var floor in DataManager.Instance.Floors)
        {
            foreach (var table in floor.tables)
            {
                if (table.ClientId == -1) { continue; }
                message += $"{floor.Id}:{table.Id}:{table.ClientId}\n";
            }
        }
        SendMessageToSpecificClient(clientId, message);
    }

    private void RemoveClient(TcpClient client)
    {
        foreach (var customClient in clients)
        {
            if (customClient.Client == client)
            {
                clients.Remove(customClient);
                break;
            }
        }
    }
    private void HandleBookTable(string message)
    {
        string[] texts = message.Split(":");
        int clientId = int.Parse(texts[1]);
        int floorId = int.Parse(texts[2]);
        int tableId = int.Parse(texts[3]);

        Table table = DataManager.Instance.GetTable(floorId, tableId);
        if (clientId == table.GetCurrentRequestChoosingTable())
        {
            string clientName = texts[4];
            string clientPhoneNumber = texts[5];
            DataManager.Instance.SetBookedTable(clientId, floorId, tableId, 
                                            clientName, clientPhoneNumber);
            BroadcastMessages(message);
            SendMessageToSpecificClient(clientId, $"Server response: book table {tableId} at floor {floorId} successfully");
            textMovement.EnqueueText($"Table {tableId} at Floor {floorId} has booked");
        }
        else
        {

            SendMessageToSpecificClient(clientId, "Server response: this table is choosing by someone. \nPlease wait or choose another one");
            // response to specific clients
        }

    }
    private void HandleCancelTable(string message)
    {
        string[] texts = message.Split(":");
        int clientId = int.Parse(texts[1]);
        int floorId = int.Parse(texts[2]);
        int tableId = int.Parse(texts[3]);
        DataManager.Instance.SetCanceledTable(clientId, floorId, tableId);
        BroadcastMessages(message);
    }
    private void HandleRequestBooking(string message)
    {
        string[] texts = message.Split(":");
        int clientId = int.Parse(texts[1]);
        int floorId = int.Parse(texts[2]);
        int tableId = int.Parse(texts[3]);
        DataManager.Instance.SetRequestBookingTable(clientId, floorId, tableId);
    }
    private void HandleRequestCancelBooking(string message)
    {
        string[] texts = message.Split(":");
        int clientId = int.Parse(texts[1]);
        int floorId = int.Parse(texts[2]);
        int tableId = int.Parse(texts[3]);
        DataManager.Instance.SetRequestCancelBookingTable(clientId, floorId, tableId);
    }

    public void BroadcastMessages(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);

        lock (clientsLock)
        {
            foreach (CustomTcpClient customeClient in clients)
            {
                try
                {
                    NetworkStream stream = customeClient.Client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending message to client: " + e);
                }
            }
        }
    }
    public void SendMessageToSpecificClient(int clientId, string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);

        lock (clientsLock)
        {
            foreach (CustomTcpClient customeClient in clients)
            {
                if (customeClient.NetworkId == clientId)
                {
                    try
                    {
                        NetworkStream stream = customeClient.Client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error sending message to client: " + e);
                    }
                    break;
                }
            }
        }
    }


    void Update()
    {
        if (!isRunning) { return; }
        while (mainThreadActions.TryDequeue(out Action action))
        {
            action();
        }
    }

    public void OnApplicationQuit()
    {
        isRunning = false;
        server?.Stop();
        serverThread?.Join(); // Wait for the server thread to finish
        ToggleGamePanel(false);
        Debug.Log("Server stopped");
        numberUsers = 0;
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1"; // Fallback
    }

    private void QueueMainThreadAction(Action action)
    {
        mainThreadActions.Enqueue(action);
    }
    private void ToggleGamePanel(bool state)
    {
        UIManager.Instance.ToggleGamePanel(state);
        UIManager.Instance.ToggleMenuPanel(!state);
    }
}
