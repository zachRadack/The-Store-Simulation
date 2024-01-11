using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerUI : MonoBehaviour
{

    // TODO: Make this be able to load elements relevant to the shelf
    public GameObject uiElement; 

    public ShelfUIManager ShelfUIManagerScript;
    public MainShelvingManager MainShelvingManager;


    // this method will take the number of shelves and call in a function in uiElement with the number of shelves.
    public void OnPointerClick(int numShelves, float shelfHeightPercent)
    {
        ShelfUIManagerScript.CreateShelves(numShelves,shelfHeightPercent);
    }
}
