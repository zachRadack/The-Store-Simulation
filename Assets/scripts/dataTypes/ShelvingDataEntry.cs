using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is used to store the data for each shelf in the game and used as a middle man by the MainShelvingManager.
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
