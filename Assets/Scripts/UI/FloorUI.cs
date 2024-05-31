using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloorUI : MonoBehaviour
{
    [SerializeField] private List<Button> floorButtons = new List<Button>();

    [SerializeField] private List<GameObject> roomObjects = new List<GameObject>();
    private void Start()
    {
        //ToggleFloors(true);
        StartCoroutine(InitializeFloor());
    }
    private IEnumerator InitializeFloor()
    {
        while(true)
        {
            bool flag = false; 
            for(int i =0;i<roomObjects.Count;i++)
            {
                if(!roomObjects[i].GetComponent<Floor>().CanWork)
                {
                    flag = true;
                }
            }
            if(!flag)
            {
                break;
            }
            yield return null;
        }
        OnFloorButtonClick(0);
    }
    public void OnFloorButtonClick(int floorIndex)
    {

        for (int i = 0; i < floorButtons.Count; i++)
        {
            if (i == floorIndex)
            {
                floorButtons[i].interactable = false;
                roomObjects[i].SetActive(true);
            }
            else
            {
                floorButtons[i].interactable = true;
                roomObjects[i].SetActive(false);
            }
        }
    }
    private void ToggleFloors(bool state)
    {
        foreach(var room in roomObjects)
        {
            room.SetActive(state);
        }
    }
    private void ToggleFloorButtons(bool state)
    {
        foreach (var button in floorButtons)
        {
            button.interactable = state;
        }
    }
}
