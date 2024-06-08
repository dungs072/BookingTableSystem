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
    [SerializeField] private TMP_Text phoneNumberText;
    [SerializeField] private TMP_Text nameUserText;

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
    [Header("Client Info booking info")]
    [SerializeField] private GameObject clientBookingInfoPanel;
    [SerializeField] private TMP_Text tableFloorInfo;
    [SerializeField] private TMP_InputField phoneNumberInfo;
    [SerializeField] private TMP_InputField nameClientInfo;


    [Header("Other")]
    [SerializeField] private TMP_InputField ipAddressInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField phoneInput;
    [SerializeField] private GameObject topNotification;
    [SerializeField] private GameObject blurPage;
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
    public void ToggleTopNotification(bool state)
    {
        topNotification.SetActive(state);
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
    public void SetUserNameText(string userName)
    {
        nameUserText.text = "Name: "+userName;
    }
    public void SetPhoneText(string phoneText)
    {
        phoneNumberText.text = "Phone: "+phoneText;
    }

    public string GetIPAddressInput()
    {
        return ipAddressInput.text;
    }
    public string GetNameInput()
    {
        return nameInput.text;
    }
    public string GetPhoneInput()
    {
        return phoneInput.text;
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
        blurPage.SetActive(state);
        confirmBookNotificationPanel.SetActive(state);
    }
    private IEnumerator CountDownToRequest(int tableId, int floorId)
    {
        float currentTime = maxRequestBookingTime;
        while(currentTime>=0f)
        {
            currentTime-=Time.deltaTime;
            confirmBookText.text = $"Are you sure about booking table {tableId} on the floor {floorId}?\n Time remaining: {currentTime.ToString("0")}s";
            yield return null;
        }
        ToggleConfirmBookNotification(tableId, floorId,false);
        DataManager.Instance.RequestCancelChoosingTable(floorId,tableId);
    }
    public void ToggleConfirmCancelNotification(int tableId, int floorId, bool state)
    {
        if(state)
        {
            confirmCancelText.text = $"Are you sure about canceling booking table {tableId} on the floor {floorId}?";
        }
        blurPage.SetActive(state);
        confirmCancelNotificationPanel.SetActive(state);
    }
    public void ToggleGeneralNotification(bool state, string message)
    {
        blurPage.SetActive(state);
        generalNotification.SetActive(state);
        generalText.text = message;
    }
    public void ToggleClientBookingInfoPanel(bool state, string tableFloor, 
                                    string clientName, string phoneNumber)
    {
        clientBookingInfoPanel.SetActive(state);
        tableFloorInfo.text = tableFloor;
        nameClientInfo.text = clientName;
        phoneNumberInfo.text = phoneNumber;
        blurPage.SetActive(state);
    }
    
}
