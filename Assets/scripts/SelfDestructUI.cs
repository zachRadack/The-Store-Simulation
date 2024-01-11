using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructUI : MonoBehaviour
{
    // This method will destroy the GameObject
    public void DestroySelf()
    {
        Debug.Log("Destroying UI element");
        Destroy(gameObject);
    }
}
