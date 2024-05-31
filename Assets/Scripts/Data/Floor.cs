using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 
public class Floor : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public List<Table> tables;
    [SerializeField] private TMP_Text floorButtonText;
    [SerializeField] private Image floorButton;
    private Dictionary<int, Table> tableDict = new Dictionary<int,Table>();
    public Dictionary<int, Table> TableDict{get{return tableDict;}}
    public bool CanWork{get;private set;} = false;
    private void Start()
    {
        foreach (var table in tables)
        {
            tableDict[table.Id] = table;
        }
        ChangeStateButton();
        CanWork = true;
    }
    public int CountTableLeft()
    {
        int count = 0;
        foreach(Table t in tables)
        {
            if(t.ClientId==-1)
            {
                count++;
            }
        }
        return count;
    }
    public void ChangeStateButton()
    {
        int count = CountTableLeft();
        if(count==0)
        {
            floorButton.color = Color.red;
            floorButtonText.text = $"Floor {Id}\n (Full)";
        }
        else
        {
            floorButton.color = Color.white;
            floorButtonText.text = $"Floor {Id}\n({count} left)";
        }
    }
}
