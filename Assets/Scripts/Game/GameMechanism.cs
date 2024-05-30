using Unity.VisualScripting;
using UnityEngine;

public class GameMechanism : MonoBehaviour
{
    [SerializeField] private TcpServer server;
    [SerializeField] private Client client;
    private bool isServer = false;
    private bool isClient = false;

    public bool IsServer{get{return isServer;}}
    public bool IsClient{get{return isClient;}}

    public void StartServer()
    {
        server.InitializeServer();
        isServer = true;
        isClient = false;
    }
    public void JoinServer()
    {
        string IPAddress = UIManager.Instance.GetIPAddressInput();
        if(IPAddress.Trim().Length==0){return;}
        client.ConnectToTcpServer(IPAddress);
        isClient = true;
        isServer = false;
    }


    public void ClientSendBookingInfoToServer(int clientId, int floorId, int tableId)
    {
        if(!isClient){return;}
        string message = $"Book:{clientId}:{floorId}:{tableId}";
        client.SendMessages(message);
    }
    public void ClientSendCancelInfoToServer(int clientId, int floorId, int tableId)
    {
        if(!isClient){return;}
        string message = $"Cancel:{clientId}:{floorId}:{tableId}";
        client.SendMessages(message);
    }



    public void ExitGame()
    {
        if(isServer)
        {
            server.OnApplicationQuit();
        }
        if(isClient)
        {
            client.OnApplicationQuit();
        }

        isServer = false;
        isClient = false;
    }
}
