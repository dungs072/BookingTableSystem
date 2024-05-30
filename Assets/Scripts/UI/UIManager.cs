using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject joinPanel;

    [SerializeField] private TMP_Text ipAddressText;
    [SerializeField] private TMP_Text userConnectionsText;
    [Header("Book notification")]
    [SerializeField] private GameObject confirmBookNotificationPanel;
    [SerializeField] private TMP_Text confirmBookText;
    [Header("Cancel notification")]
    [SerializeField] private GameObject confirmCancelNotificationPanel;
    [SerializeField] private TMP_Text confirmCancelText;

    [SerializeField] private TMP_InputField ipAddressInput;
    public static UIManager Instance { get; private set; }

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

    public void SetIPAddressText(string ipAddress)
    {
        ipAddressText.text = "IP Address: " + ipAddress;
    }
    public void SetUserConnectionsText(int numberUserConnections)
    {
        userConnectionsText.text = "User connections: " + numberUserConnections.ToString();
    }

    public string GetIPAddressInput()
    {
        return ipAddressInput.text;
    }
    public void ToggleGamePanel(bool state)
    {
        gamePanel.SetActive(state);
    }
    public void ToggleMenuPanel(bool state)
    {
        menuPanel.SetActive(state);
    }
    public void ToggleJoinPanel(bool state)
    {
        joinPanel.SetActive(state);
    }
    public void ToggleConfirmBookNotification(int tableId, int floorId, bool state)
    {
        if (state)
        {
            confirmBookText.text = $"Are you sure about booking table {tableId} at floor {floorId} ?";
        }
        confirmBookNotificationPanel.SetActive(state);
    }
    public void ToggleConfirmCancelNotification(int tableId, int floorId, bool state)
    {
        if(state)
        {
            confirmCancelText.text = $"Are you sure about canceling booking table {tableId} at floor {floorId} ?";
        }
        confirmCancelNotificationPanel.SetActive(state);
    }
}
