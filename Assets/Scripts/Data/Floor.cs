using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public List<Table> tables;
    private Dictionary<int, Table> tableDict = new Dictionary<int,Table>();
    public Dictionary<int, Table> TableDict{get{return tableDict;}}
    private void Start()
    {
        foreach (var table in tables)
        {
            tableDict[table.Id] = table;
        }
    }

    public bool IsFull()
    {
        foreach (Table t in tables)
        {
            if (!t.IsBooked)
            {
                return false;
            }
        }
        return true;
    }
}
