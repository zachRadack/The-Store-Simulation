using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShelvingDataEntry
{
    public ShelfKey key;
    public ShelvingData value;

    public ShelvingDataEntry(ShelfKey key, ShelvingData value)
    {
        this.key = key;
        this.value = value;
    }
}
