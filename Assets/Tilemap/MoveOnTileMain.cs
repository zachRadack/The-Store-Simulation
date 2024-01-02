using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;



public class MoveOnTileMain : MonoBehaviour
{
    Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

    public Tilemap tilemap;
    public TileAndMovementCost[] tiles;
    Pathfinder<Vector3Int> pathfinder;
    public bool entityFollowsRightClick = false;
    
    public MainShelvingManager MainShelvingManager;

    [System.Serializable]
    public struct TileAndMovementCost
    {
        public Tile tile;
        public bool movable;
        public float movementCost;
    }

    public List<Vector3Int> path;
    [Range(0.001f,1f)]
    public float stepTime;

    //distance function for the pathfinder
    public float DistanceFunc(Vector3Int a, Vector3Int b)
    {
        return (a-b).sqrMagnitude;
    }


    //returns a dictionary of all the tiles that are connected to a and their movement cost
    public Dictionary<Vector3Int,float> connectionsAndCosts(Vector3Int a)
    {
        Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
        foreach (Vector3Int dir in directions)
        {
            foreach (TileAndMovementCost tmc in tiles)
            {

                if (tilemap.GetTile(a+dir)==tmc.tile)
                {
                    if (tmc.movable) {
                        // this is where you would add the movement cost of the tile
                        // for example if you have a tile that costs 2 to move on you would do
                        //Debug.Log("tile cost is " + tmc.movementCost);
                        result.Add(a + dir, tmc.movementCost);
                    }

                }
            }
                
        }
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
    }

    // Update is called once per frame
    void Update()
    {
        if(entityFollowsRightClick){
            if (Input.GetMouseButtonDown(1) ){
                Vector3Int target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Debug.Log(target);
                //Debug.Log(currentCellPos);
                target.z = 0;
                MoveTo(target);

                if(MainShelvingManager.isThereAShelf(target)){
                    Debug.Log("You Targeted:" +target +" there is a shelf at "  );
                    MainShelvingManager.testAddItemToShelf(target);
                    //
                }
                else{
                    Debug.Log("You Targeted:" +target +" there is no shelf at " );
                }
            }else if (Input.GetMouseButtonDown(0) ){
                Vector3Int target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Debug.Log(target);
                //Debug.Log(currentCellPos);
                target.z = 0;
                Debug.Log(MainShelvingManager.GetShelfTile(target));
            }

        }
    }
    
    /**
        * This starts a coroutine that moves the entity to the target tile.
    **/
    public void MoveTo(Vector3Int target)
    {
        pathfinder.GenerateAstarPath(tilemap.WorldToCell(transform.position), target, out path);
        StopAllCoroutines();
        StartCoroutine(Move());
    }

    /**
        * Moves the entity along the path and removes the top of path list.
    **/
    IEnumerator Move()
    {
        while (path.Count > 0)
        {
            // shifts position of entity to center of tile
            Vector3 CellShift=new Vector3(0.5f, 0.5f, 0.0f);;
            //Debug.Log("moving to " + path[0]+CellShift);

            //  
            transform.position = tilemap.CellToWorld(path[0])+CellShift;
            path.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
            
        }
        

    }



    
}

