using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject joinPanel;

    [SerializeField] private TMP_Text ipAddressText;
    
    [SerializeField] private TMP_Text titleInfo2Text;

    [Header("Book notification")]
    [SerializeField] private GameObject confirmBookNotificationPanel;
    [SerializeField] private TMP_Text confirmBookText;
    [SerializeField] private float maxRequestBookingTime = 20f;
    [Header("Cancel notification")]
    [SerializeField] private GameObject confirmCancelNotificationPanel;
    [SerializeField] private TMP_Text confirmCancelText;
    [Header("General notification")]
    [SerializeField] private GameObject generalNotification;
    [SerializeField] private TMP_Text generalText;

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
        titleInfo2Text.text = "User connections: " + numberUserConnections.ToString();
    }
    public void SetNetworkIdText(int networkId)
    {
        titleInfo2Text.text = "Network Id: "+networkId.ToString();
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
            confirmBookText.text = $"Are you sure about booking table {tableId} at floor {floorId}?";
            StopAllCoroutines();
            StartCoroutine(CountDownToRequest(tableId,floorId));
        }
        confirmBookNotificationPanel.SetActive(state);
    }
    private IEnumerator CountDownToRequest(int tableId, int floorId)
    {
        float currentTime = maxRequestBookingTime;
        while(currentTime>=0f)
        {
            currentTime-=Time.deltaTime;
            confirmBookText.text = $"Are you sure about booking table {tableId} at floor {floorId}?\n Time remaining: {currentTime.ToString("0")}s";
            yield return null;
        }
        ToggleConfirmBookNotification(tableId, floorId,false);
        DataManager.Instance.RequestCancelChoosingTable(floorId,tableId);
    }
    public void ToggleConfirmCancelNotification(int tableId, int floorId, bool state)
    {
        if(state)
        {
            confirmCancelText.text = $"Are you sure about canceling booking table {tableId} at floor {floorId}?";
        }
        confirmCancelNotificationPanel.SetActive(state);
    }
    public void ToggleGeneralNotification(bool state, string message)
    {
        generalNotification.SetActive(state);
        generalText.text = message;
    }
    
}
