using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;



public class MoveOnTileMain : MonoBehaviour
{
    Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };
    public TriggerUI uiTrigger;
    public bool verbose = true;
    public Tilemap tilemap;
    public TileAndMovementCost[] tiles;
    Pathfinder<Vector3Int> pathfinder;
    public bool entityFollowsRightClick = false;
    
    public MainShelvingManager MainShelvingManager;

    public int debugShelfCount = 5;

    //todo: delete later, for testing
    private DatabaseManager databaseManager;


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

        databaseManager = FindObjectOfType<DatabaseManager>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if(entityFollowsRightClick){
            if (Input.GetMouseButtonDown(1) ){
                Vector3Int target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                target.z = 0;
                MoveTo(target);

                if(MainShelvingManager.isThereAShelf(target)){
                    if(verbose){
                        Debug.Log("You Targeted:" +target +" there is a shelf at "  );
                    }
                    MainShelvingManager.testAddItemToShelf(target);
                    
                }
                else{
                    if(verbose){
                        Debug.Log("You Targeted:" +target +" there is no shelf at " );
                    }
                }
            }else if (Input.GetMouseButtonDown(0) ){
                Vector3Int target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Debug.Log(target);
                //Debug.Log(currentCellPos);
                target.z = 0;
                Debug.Log(MainShelvingManager.GetShelfTile(target));
            }else if(Input.GetKeyDown(KeyCode.I)){

                List<ShelfData> shelfDimensions = new List<ShelfData>();
                shelfDimensions.Add(new ShelfData(new Vector2(20,10),new Vector2(150,0)));
                shelfDimensions.Add(new ShelfData(new Vector2(20,10),new Vector2(100,100)));
                //uiTrigger.OnPointerClick(debugShelfCount,100f);
                uiTrigger.OnPointerClickCustom(shelfDimensions);
            }else if(Input.GetKeyDown(KeyCode.O)){
                if (databaseManager != null) {
                    Debug.Log("DatabaseManager found in the scene!");
                    databaseManager.TestDatabaseFunctions();
                    MainShelvingManager.SaveAllShelvesData();
                    Debug.Log("DatabaseManager finished testing database functions.");
                } else {
                    Debug.LogError("DatabaseManager not found in the scene!");
                }
            }

        }
    }
    
    /**
        * This starts a coroutine that moves the entity to the target tile.
    **/
    public void MoveTo(Vector3Int target){

        bool didItFindPath = pathfinder.GenerateAstarPath(tilemap.WorldToCell(transform.position), target, out path);
        if(didItFindPath){
            StopAllCoroutines();
            StartCoroutine(Move());
        }else{
            if(verbose){
                Debug.Log("no path found");
            }
            Vector3Int location = Vector3Int.RoundToInt(target);
            Dictionary<Vector3Int, float> nodes = GetNeighbourNodes(location);
            float closestNode = 1000000000000000;
            foreach (KeyValuePair<Vector3Int, float> node in nodes)
            {
                //Debug.Log(node.Key);
                Vector3Int nodeLocation = Vector3Int.RoundToInt(node.Key);
                // find the closest node to the target and see if path
                //Debug.Log("value:" +node.Value);
                if(node.Value<closestNode){
                    bool didItFindPath2 = pathfinder.GenerateAstarPath(tilemap.WorldToCell(transform.position), nodeLocation, out path);
                    if(didItFindPath2){
                        if(verbose){
                        Debug.Log("found path to node");
                        }
                        StopAllCoroutines();
                        StartCoroutine(Move());
                        return;
                    }else{
                        if(verbose){
                        Debug.Log("no path found");
                        }
                    }
                }
            }
        }
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


    Dictionary<Vector3Int, float> GetNeighbourNodes(Vector3Int pos){
        Dictionary<Vector3Int, float> neighbours = new Dictionary<Vector3Int, float>();
        for (int i = -1; i < 2; i++){
            for (int j = -1; j < 2; j++){
                
                Vector3Int dir = new Vector3Int(i, j,0);
                if (!Physics.Linecast(pos, pos + dir)){
                    neighbours.Add(pos + dir, dir.magnitude);
                }
            }

        }
        return neighbours;

    }

    
}

