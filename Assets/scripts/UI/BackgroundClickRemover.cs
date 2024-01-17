using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackgroundClickRemover : MonoBehaviour
{
 
    public void OnButtonClick()
    {
        Debug.Log("Destroying UI element");
        gameObject.SetActive(false);
    }

    
}
