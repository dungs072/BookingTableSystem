using Unity.VisualScripting;
using UnityEngine;

public class GameMechanism : MonoBehaviour
{
    [SerializeField] private TcpServer server;
    [SerializeField] private Client client;
    private bool isServer = false;
    private bool isClient = false;

    public bool IsServer { get { return isServer; } }
    public bool IsClient { get { return isClient; } }

    public void StartServer()
    {
        server.InitializeServer();
        isServer = true;
        isClient = false;
        UIManager.Instance.ToggleTopNotification(true);
    }
    public void JoinServer()
    {
        string IPAddress = UIManager.Instance.GetIPAddressInput();
        string clientName = UIManager.Instance.GetNameInput();
        string clientPhoneNumber = UIManager.Instance.GetPhoneInput();
        if (IPAddress.Trim().Length == 0) { return; }
        if(clientName.Trim().Length == 0) { return;}
        if(clientPhoneNumber.Trim().Length==0){return;}
        GetComponent<NetworkInfo>().ClientPhoneNumber = clientPhoneNumber;
        GetComponent<NetworkInfo>().ClientName = clientName;
        client.ConnectToTcpServer(IPAddress);
        isClient = true;
        isServer = false;
        UIManager.Instance.SetPhoneText(clientPhoneNumber);
        UIManager.Instance.SetUserNameText(clientName); 
        UIManager.Instance.ToggleTopNotification(false);
    }


    public void ClientSendBookingInfoToServer(int clientId, int floorId, int tableId, 
                                                string clientName, string phoneNumber)
    {
        if (!isClient) { return; }
        string message = $"Book:{clientId}:{floorId}:{tableId}:{clientName}:{phoneNumber}";
        client.SendMessages(message);
    }
    public void ClientSendCancelInfoToServer(int clientId, int floorId, int tableId)
    {
        if (!isClient) { return; }
        string message = $"Cancel:{clientId}:{floorId}:{tableId}";
        client.SendMessages(message);
    }
    public void ClientSendRequestBookingToServer(int clientId, int floorId, int tableId)
    {
        if (!isClient) { return; }
        string message = $"Request:{clientId}:{floorId}:{tableId}";
        client.SendMessages(message);
    }
    public void ClientSendRequestCancelChoosingTableToServer(int clientId, int floorId, int tableId)
    {
        if (!isClient) { return; }
        string message = $"RejectChoosing:{clientId}:{floorId}:{tableId}";
        client.SendMessages(message);
    }
    public void ServerSendLockTable(int floorId, int tableId, bool state)
    {
        if(!isServer){return;}
        string message = $"Lock:{floorId}:{tableId}:{state}";
        server.BroadcastMessages(message);
    }



    public void ExitGame()
    {
        if (isServer)
        {
            server.OnApplicationQuit();
        }
        if (isClient)
        {
            client.OnApplicationQuit();
        }

        isServer = false;
        isClient = false;
    }
}
