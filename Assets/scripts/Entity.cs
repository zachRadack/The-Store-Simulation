using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;

public class Entity : MonoBehaviour
{
    public MoveOnTileMain moveOnTileMainScript;
    //public Shelf shelfScript;
    // Start is called before the first frame update
    void Start()
    {
        Vector3Int target = new Vector3Int(0, 0, 0);
        moveOnTileMainScript.MoveTo(target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void testPathings(){
        Vector3Int target = new Vector3Int(0, 0, 0);
        moveOnTileMainScript.MoveTo(target);
        
    }
}
