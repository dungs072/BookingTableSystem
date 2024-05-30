using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class Client : MonoBehaviour
{
    [SerializeField] private NetworkInfo networkInfo;
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
    private bool isRunning = false;
    void Update()
    {
        if (!isRunning) { return; }
        while (mainThreadActions.TryDequeue(out Action action))
        {
            action();
        }
    }
    public void ConnectToTcpServer(string IPAddress)
    {
        try
        {
            isRunning = true;
            socketConnection = new TcpClient(IPAddress, 8080);
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
            QueueMainThreadAction(() => ToggleGamePanel(true));
            QueueMainThreadAction(() => UIManager.Instance.SetIPAddressText(IPAddress));
        }
        catch (Exception e)
        {
            isRunning = false;
            Debug.LogError("On client connect exception " + e);
            QueueMainThreadAction(() => ToggleGamePanel(false));
        }
    }

    private void ListenForData()
    {
        try
        {
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        if (serverMessage.Contains("ClientId"))
                        {
                            int clientId = int.Parse(serverMessage.Split(":")[1]);
                            networkInfo.NetworkId = clientId;
                        }
                        if (serverMessage.Contains("Book"))
                        {
                            string[] texts = serverMessage.Split(":");
                            int clientId = int.Parse(texts[1]);
                            int floorId = int.Parse(texts[2]);
                            int tableId = int.Parse(texts[3]);
                            QueueMainThreadAction(() => DataManager.Instance.SetBookedTable(clientId, floorId, tableId));
                        }
                        if(serverMessage.Contains("Cancel"))
                        {
                            string[] texts = serverMessage.Split(":");
                            int clientId = int.Parse(texts[1]);
                            int floorId = int.Parse(texts[2]);
                            int tableId = int.Parse(texts[3]);
                            QueueMainThreadAction(() => DataManager.Instance.SetCanceledTable(clientId, floorId, tableId));
                        }
                        Debug.Log("Server message received: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendMessages(string message)
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                stream.Flush();
                Debug.Log("Client message sent: " + message);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void OnApplicationQuit()
    {
        if (socketConnection != null)
        {
            socketConnection.Close();
        }

        if (clientReceiveThread != null)
        {
            clientReceiveThread.Abort();
        }
        isRunning = false;
        ToggleGamePanel(false);
        Debug.Log("Client Disconnected");

    }
    private void QueueMainThreadAction(Action action)
    {
        mainThreadActions.Enqueue(action);
    }
    public void ToggleGamePanel(bool state)
    {
        UIManager.Instance.ToggleGamePanel(state);
        UIManager.Instance.ToggleJoinPanel(!state);
    }
}
