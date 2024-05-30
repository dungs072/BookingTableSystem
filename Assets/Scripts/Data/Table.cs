using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Table : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private Image tableIcon;
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public Floor Floor { get; private set; }
    public bool IsBooked { get; set; }
    public int ClientId { get; set; } = -1;

    private List<int> clientIds = new List<int>();

    private GameMechanism gameMechanism;
    private NetworkInfo networkInfo;
    private void Start()
    {
        gameMechanism = FindObjectOfType<GameMechanism>();
        networkInfo = FindObjectOfType<NetworkInfo>();
        cancelButton.SetActive(false);
    }

    public void OnBookTableClick()
    {
        if (ClientId != -1) { return; }
        if (gameMechanism.IsServer) { return; }
        UIManager.Instance.ToggleConfirmBookNotification(Id, Floor.Id, true);
        DataManager.Instance.SetCurrentData(Floor.Id, Id);
        DataManager.Instance.RequestChooseTable();
    }
    public void OnCancelTableClick()
    {
        if (ClientId == -1) { return; }
        if (gameMechanism.IsServer) { return; }
        UIManager.Instance.ToggleConfirmCancelNotification(Id, Floor.Id, true);
        DataManager.Instance.SetCurrentData(Floor.Id, Id);
    }
    // asign this to button
    public void OnCancelRequestChoosingTableClick()
    {
        DataManager.Instance.RequestCancelChoosingTable(Floor.Id, Id);
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
        if (networkInfo.NetworkId != ClientId) { return; }
        cancelButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (networkInfo.NetworkId != ClientId) { return; }
        cancelButton.SetActive(false);

    }
    public void Dequeue(int number)
    {
        clientIds.Remove(number);
    }
    public void Enqueue(int number)
    {
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
}
