using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShelfUIManager : MonoBehaviour
{
    public GameObject shelfPrefabDebug; 
    public GameObject shelfPrefabBackground;
    public GameObject shelfPrefabShelf;
    public Canvas canvas;
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
}


