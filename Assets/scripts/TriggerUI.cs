using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerUI : MonoBehaviour
{
    public GameObject prefabUI;
    // TODO: Make this be able to load elements relevant to the shelf
    public void TriggerShelfUICall(){
        Instantiate(prefabUI, transform.position, Quaternion.identity, transform.parent);
    }
}
