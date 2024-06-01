using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public class NetworkInfo : MonoBehaviour
{
    [Header("Please dont edit that")]
    [SerializeField] private int networkId;
    public string ClientName{get;set;}
    public string ClientPhoneNumber{get;set;}
    public int NetworkId
    {
        get
        {
            return networkId;
        }
        set
        {
            networkId = value;
        }
    }
}
