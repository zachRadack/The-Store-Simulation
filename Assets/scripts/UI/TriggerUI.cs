using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class TriggerUI : MonoBehaviour
{

    
    // TODO: Make this be able to load elements relevant to the shelf
    public GameObject uiElement; 

    public ShelfUIManager ShelfUIManagerScript;
    public MainShelvingManager MainShelvingManager;
    public static RectTransform CanvasRectTransform;

    void Awake()
    {
        CanvasRectTransform = this.gameObject.GetComponent<RectTransform>();
        
    }

    // this method will take the number of shelves and call in a function in uiElement with the number of shelves.
    public void OnPointerClick(int numShelves, float shelfHeightPercent)
    {
        ShelfUIManagerScript.CreateShelves(numShelves,shelfHeightPercent);
    }
    public void OnPointerClickCustom(List<ShelfData> shelfDimensions)
    {
        ShelfUIManagerScript.CreateCustomShelves(shelfDimensions);
    }
    
    public void OnPointerClickShelfUI(List<List<float>> shelfDimensionsList)
    {
        //TODO: Implement an import that brings in the shelf data from the database, then use it to create the shelves according to the database.
        List<float> BackgroundshelfDimensions = new List<float>();
        bool debugOn = true;
        if (debugOn)
        {
            //Place holder dimensions until import is implemented
            
            BackgroundshelfDimensions.Add(0f);
            BackgroundshelfDimensions.Add(0f);
            BackgroundshelfDimensions.Add(-165f);
            BackgroundshelfDimensions.Add(-92.8f);
            
        }
        
        //TODO: Make this dynamic by allowing diffrent dimensions as stated earlier
        ShelfUIManagerScript.CreateShelfBackground(BackgroundshelfDimensions);
        Debug.Log("Creating shelf " + shelfDimensionsList.Count + " shelves");
        for (int i = 0; i < shelfDimensionsList.Count; i++)
        {
            ShelfUIManagerScript.CreateCustomShelves(shelfDimensionsList[i]);
        }

    }
    
}
