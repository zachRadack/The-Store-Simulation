using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShelfUIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject shelfPrefabDebug; 
    public GameObject shelfPrefabBackground;
    public GameObject shelfPrefabShelf;

    // this represents the sizeDelta y of the shelf, which is the height of the shelf.
    public float thicknessOfShelves = -452f;



    // TODO: All of these variables are used in createshelves function, will probably be deleted later
    public int defaultShelfCount = 5;
     // Maximum number of shelves
    private int maxShelfCount;
    // Height of each shelf as a percentage of screen height
    private float shelfHeightPercentage; 
    
    public void OnButtonClick()
    {
        Debug.Log("Destroying UI element");
        gameObject.SetActive(false);
    }

    // CreateShelves now takes two parameters: numberOfShelves and shelfHeightPercent
    public void CreateShelves(int numberOfShelves, float heightPercentage)
    {
        maxShelfCount = Mathf.Clamp(numberOfShelves, 1, defaultShelfCount);
        shelfHeightPercentage = Mathf.Clamp(heightPercentage, 0, 100);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float buffer = 0.1f; // 10% buffer

        float usableWidth = screenWidth * (1 - 2 * buffer);
        float usableHeight = screenHeight * (1 - 2 * buffer);
        float totalShelvesHeight = usableHeight * (shelfHeightPercentage / 100f);
        float singleShelfHeight = totalShelvesHeight / maxShelfCount;
        //Debug.Log("singleShelfHeight: " + singleShelfHeight);
        float startYPosition = -(usableHeight / 2) + (singleShelfHeight / 2);

        for (int i = 0; i < maxShelfCount; i++)
        {
            GameObject shelf = Instantiate(shelfPrefabDebug, canvas.transform);
            RectTransform rt = shelf.GetComponent<RectTransform>();

            rt.sizeDelta = new Vector2(usableWidth - screenWidth, -(screenHeight-singleShelfHeight));
            rt.anchoredPosition = new Vector2(0, startYPosition + (i * singleShelfHeight));
        }
    }

    public void CreateCustomShelves(List<ShelfData> shelfDataList)
    {
        foreach (ShelfData shelfData in shelfDataList)
        {
            GameObject shelf = Instantiate(shelfPrefabDebug, canvas.transform);
            RectTransform rt = shelf.GetComponent<RectTransform>();

            // Set the size and position of the shelf based on ShelfData
            rt.sizeDelta = new Vector2(shelfData.dimensions.x, shelfData.dimensions.y);
            rt.anchoredPosition = new Vector2(shelfData.position.x, shelfData.position.y);
        }
    }

    public void CreateShelfBackground(List<float> BackgroundshelfDimensions){
        GameObject shelf = Instantiate(shelfPrefabBackground, canvas.transform);
        RectTransform rt = shelf.GetComponent<RectTransform>();

        // Set the size and position of the shelf based on ShelfData
        rt.sizeDelta = new Vector2(BackgroundshelfDimensions[2], BackgroundshelfDimensions[3]);
        rt.anchoredPosition = new Vector2(BackgroundshelfDimensions[0], BackgroundshelfDimensions[1]);
    }
    public void CreateCustomShelves(List<float> shelfDimensions)
    {
        // shelfDimensions is a list of floats that contains the dimensions of the shelves. the first is x anchor, second is y anchor, third is x size delta, fourth is y size delta.
        // TODO: ShelfDimensions may have other variables later to create an air pocket above the shelf, to allow easier placing of product representations.
        GameObject shelf = Instantiate(shelfPrefabShelf, canvas.transform);
        RectTransform rt = shelf.GetComponent<RectTransform>();

        // Set the size and position of the shelf based on ShelfData
        rt.sizeDelta = new Vector2(shelfDimensions[2], thicknessOfShelves);
        rt.anchoredPosition = new Vector2(shelfDimensions[0], shelfDimensions[1]);
    }

}


