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
    private void ToggleFloorButtons(bool state)
    {
        foreach (var button in floorButtons)
        {
            button.interactable = state;
        }
    }
}
