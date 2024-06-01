using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Table : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private Image tableIcon;
    [SerializeField] private GameObject lockButton;
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public Floor Floor { get; private set; }
    [field: SerializeField] public bool IsLocked { get; private set; }
    public int ClientId { get; set; } = -1;
    public string ClientName { get; set; }
    public string ClientPhoneNumber { get; set; }

    private List<int> clientIds = new List<int>();

    private GameMechanism gameMechanism;
    private NetworkInfo networkInfo;
    private void Start()
    {
        gameMechanism = FindObjectOfType<GameMechanism>();
        networkInfo = FindObjectOfType<NetworkInfo>();
        cancelButton.SetActive(false);
        lockButton.SetActive(false);
    }

    public void OnBookTableClick()
    {
        if (IsLocked) { return; }
        if (ClientId != -1)
        {
            if (gameMechanism.IsServer)
            {
                UIManager.Instance.ToggleClientBookingInfoPanel(true,
                    $"Table {Id} at Floor {Floor.Id}", ClientName, ClientPhoneNumber);
            }
        }
        else if (gameMechanism.IsServer) { return; }
        else
        {
            UIManager.Instance.ToggleConfirmBookNotification(Id, Floor.Id, true);
            DataManager.Instance.SetCurrentData(Floor.Id, Id);
            DataManager.Instance.RequestChooseTable();
        }

    }
    public void OnCancelTableClick()
    {
        if (ClientId == -1) { return; }
        if (gameMechanism.IsServer) { return; }
        UIManager.Instance.ToggleConfirmCancelNotification(Id, Floor.Id, true);
        DataManager.Instance.SetCurrentData(Floor.Id, Id);
    }
    public void OnLockedTableClick()
    {
        if (ClientId != -1) { return; }
        if (!gameMechanism.IsServer) { return; }
        IsLocked = !IsLocked;
        DataManager.Instance.LockTableToAllClient(Floor.Id, Id, IsLocked);
    }
    public void SetLockTable(bool state)
    {
        lockButton.SetActive(state);
        IsLocked = state;
    }


    public void HandleBookTable(int clientId)
    {
        ClientId = clientId;
        tableIcon.color = Color.green;
    }
    public void HandleBookTableByYourself(int clientId)
    {
        ClientId = clientId;
        tableIcon.color = Color.yellow;
    }
    public void HandleCancelBookTable()
    {
        tableIcon.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameMechanism.IsServer&&ClientId==-1)
        {
            lockButton.SetActive(true);
        }
        else
        {
            if (networkInfo.NetworkId != ClientId) { return; }
            cancelButton.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameMechanism.IsServer&&ClientId==-1)
        {
            if(!IsLocked)
            {
                lockButton.SetActive(false);
            }
            
        }
        else
        {
            if (networkInfo.NetworkId != ClientId) { return; }
            cancelButton.SetActive(false);
        }


    }
    public void Dequeue(int number)
    {
        clientIds.Remove(number);
    }
    public void Enqueue(int number)
    {
        if (clientIds.Contains(number)) { return; }
        clientIds.Add(number);
    }
    public int GetCurrentRequestChoosingTable()
    {
        return clientIds[0];
    }
    public void ClearRequest()
    {
        clientIds.Clear();
    }
    public void ResetTable()
    {
        ClientId = -1;
        ClientName = "";
        ClientPhoneNumber = "";
        clientIds.Clear();
        SetLockTable(false);
        HandleCancelBookTable();
        
    }
}
