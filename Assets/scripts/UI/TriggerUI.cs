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
    public DatabaseConnectionManager dbConnectionManager;
    private ShelvingUiService _shelvingUiService;

    void Awake()
    {
        CanvasRectTransform = this.gameObject.GetComponent<RectTransform>();
        dbConnectionManager = new DatabaseConnectionManager();
        _shelvingUiService = new ShelvingUiService(dbConnectionManager);
    }
    public void OnPointerClickShelfUI(int shelfId)
    {
        List<List<float>> shelfDimensionsList = _shelvingUiService.getShelvesBackgroundData(shelfId);
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
        Debug.Log("Creating shelf " + shelfDimensionsList.Count + " shelves");

        ShelfUIManagerScript.createEntireShelf(BackgroundshelfDimensions, shelfDimensionsList);

    }

}
