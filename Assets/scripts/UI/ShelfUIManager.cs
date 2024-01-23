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

    public GameObject shelfOffButton;

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
        //gameObject.SetActive(false);

        
        foreach (Transform child in this.transform)
        {
            if (child == shelfOffButton.transform)
            {
                // Deactivate the specified child
                child.gameObject.SetActive(false);
            }
            else
            {
                // Destroy all other children
                Destroy(child.gameObject);
            }
        }
    }


    public void createEntireShelf(List<float> BackgroundshelfDimensions, List<List<float>> shelfDimensions)
    {
        shelfOffButton.SetActive(true);
        CreateShelfBackground(BackgroundshelfDimensions);
                for (int i = 0; i < shelfDimensions.Count; i++)
        {
            CreateCustomShelves(shelfDimensions[i]);
        }
    }

    //<summary>
    // Creates a shelf background based on the dimensions of the shelf
    //</summary>
    private void CreateShelfBackground(List<float> BackgroundshelfDimensions){
        GameObject shelf = Instantiate(shelfPrefabBackground, canvas.transform);
        RectTransform rt = shelf.GetComponent<RectTransform>();

        // Set the size and position of the shelf based on ShelfData
        rt.sizeDelta = new Vector2(BackgroundshelfDimensions[2], BackgroundshelfDimensions[3]);
        rt.anchoredPosition = new Vector2(BackgroundshelfDimensions[0], BackgroundshelfDimensions[1]);
    }

    //<summary>
    // Creates a single shelf based on the dimensions of the shelf
    //</summary>
    void CreateCustomShelves(List<float> shelfDimensions)
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


