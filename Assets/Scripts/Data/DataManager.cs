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
    public void SetBookedTable(int clientId, int floorId, int tableId)
    {
        Table table = floorDict[floorId].TableDict[tableId];
        table.ClientId = clientId;
        
        if(clientId==networkInfo.NetworkId)
        {
            table.HandleBookTableByYourself(clientId);
        }
        else
        {
            table.HandleBookTable(clientId);
        }
    }
    public void SetCanceledTable(int clientId, int floorId, int tableId)
    {
        Table table = floorDict[floorId].TableDict[tableId];
        table.ClientId = -1;
        table.HandleCancelBookTable();
    }

    #region UI
    public void ConfirmBookTable()
    {
        gameMechanism.ClientSendBookingInfoToServer(networkInfo.NetworkId, floorId, tableId);
    }
    public void CancelBookTable()
    {
        gameMechanism.ClientSendCancelInfoToServer(networkInfo.NetworkId, floorId, tableId);
    }
    #endregion

}
