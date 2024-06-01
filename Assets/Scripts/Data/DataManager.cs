using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<Floor> floors = new List<Floor>();
    [SerializeField] private GameMechanism gameMechanism;
    [SerializeField] private NetworkInfo networkInfo;

    public List<Floor> Floors { get { return floors; } }

    private Dictionary<int, Floor> floorDict = new Dictionary<int, Floor>();

    public static DataManager Instance { get; private set; }

    private int floorId;
    private int tableId;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        foreach (var floor in Floors)
        {
            floorDict[floor.Id] = floor;
        }
    }
    public void SetCurrentData(int floorId, int tableId)
    {
        this.floorId = floorId;
        this.tableId = tableId;
    }
    public void RequestChooseTable()
    {
        gameMechanism.ClientSendRequestBookingToServer(networkInfo.NetworkId, floorId,tableId);
    }
    public void RequestCancelChoosingTable(int floorId, int tableId)
    {
        gameMechanism.ClientSendRequestCancelChoosingTableToServer(networkInfo.NetworkId, floorId, tableId);
    }
    public void LockTableToAllClient(int floorId, int tableId, bool state)
    {
        gameMechanism.ServerSendLockTable(floorId,tableId,state);
    }
    public void SetBookedTable(int clientId, int floorId, int tableId, 
                                string clientName="", string phoneNumber="")
    {
        
        Table table = floorDict[floorId].TableDict[tableId];

        table.ClientId = clientId;
        table.ClientName = clientName;
        table.ClientPhoneNumber = phoneNumber;        
        if(clientId==networkInfo.NetworkId)
        {
            table.HandleBookTableByYourself(clientId);
        }
        else
        {
            table.HandleBookTable(clientId);
        }
        table.ClearRequest();
        floorDict[floorId].ChangeStateButton();
    }
    public void SetCanceledTable(int clientId, int floorId, int tableId)
    {
        Table table = floorDict[floorId].TableDict[tableId];
        table.ClientId = -1;
        table.HandleCancelBookTable();
        floorDict[floorId].ChangeStateButton();
    }
    public void SetRequestBookingTable(int clientId, int floorId, int tableId)
    {
        Table table = floorDict[floorId].TableDict[tableId];
        table.Enqueue(clientId);
    }
    public void SetRequestCancelBookingTable(int clientId, int floorId, int tableId)
    {
        Table table = floorDict[floorId].TableDict[tableId];
        table.Dequeue(clientId);
    }
    public Table GetTable(int floorId, int tableId)
    {
        return floorDict[floorId].TableDict[tableId];
    }
    public Floor GetFloor(int floorId)
    {
        return floorDict[floorId];
    }

    #region UI
    public void ConfirmBookTable()
    {
        gameMechanism.ClientSendBookingInfoToServer(networkInfo.NetworkId, floorId, tableId, 
                                    networkInfo.ClientName, networkInfo.ClientPhoneNumber);
    }
    public void CancelBookTable()
    {
        gameMechanism.ClientSendCancelInfoToServer(networkInfo.NetworkId, floorId, tableId);
    }
    public void OnCancelRequestChoosingTableClick()
    {
        RequestCancelChoosingTable(floorId, tableId);
    }
    #endregion

}
