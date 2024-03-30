using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ItemData 
{
    
    public string name;
    public int id, count;
    [Multiline]
    public string description;
    public bool isUniq;
    
    public ItemData(string name, string description, int id, int count, bool isUniq) 
    {
        this.name = name;
        this.description = description;
        this.id = id;
        this.count = count;
        this.isUniq = isUniq;
    }
}
