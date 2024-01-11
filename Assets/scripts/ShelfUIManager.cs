using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShelfUIManager : MonoBehaviour
{
    public GameObject shelfPrefab; 

    // 10% buffer on all sides
    private float screenBuffer = 0.1f; 

    public void OnButtonClick()
    {
        Debug.Log("Destroying UI element");
        gameObject.SetActive(false);
    }

    // CreateShelves now takes two parameters: numberOfShelves and shelfHeightPercent
    public void CreateShelves(int numberOfShelves, float shelfHeightPercent)
    {
        // Calculate the total usable height considering the buffer
        float usableHeight = Screen.height * (1 - 2 * screenBuffer);

        // Calculate individual shelf height based on the percentage
        float shelfHeight = usableHeight * (shelfHeightPercent / 100f);

        // Calculate starting Y position for the first shelf
        float startY = Screen.height * screenBuffer + (usableHeight - shelfHeight * numberOfShelves) / 2;

        for (int i = 0; i < numberOfShelves; i++)
        {
            GameObject newShelf = Instantiate(shelfPrefab, transform);

            // Setting up the shelf position and scale according to screen size
            RectTransform rt = newShelf.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(screenBuffer, startY / Screen.height + i * shelfHeight / Screen.height);
            rt.anchorMax = new Vector2(1 - screenBuffer, startY / Screen.height + (i + 1) * shelfHeight / Screen.height);
            rt.offsetMin = rt.offsetMax = Vector2.zero; // To ensure it stretches to fill the anchors
        }
    }
}

