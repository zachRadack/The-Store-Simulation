using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject uiElement; 

    public void OnPointerClick(PointerEventData eventData)
    {
        //uiElement.SetActive(false); // Deactivates the UI element
        Destroy(uiElement); // Completely removes the UI element
    }
}
