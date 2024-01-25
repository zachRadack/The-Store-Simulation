using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CategoryReach : MonoBehaviour
{
    [SerializeField]
    private Tilemap reachMap;

    [SerializeField]
    private float MaxReach;
    [SerializeField]
    private Color maxReachColor, minReachColor, noReachColor;
    private Dictionary<Vector3Int, float> _categoryReachTiles = new Dictionary<Vector3Int, float>();



    public void AddCategoryReach(Vector3Int gridPosition, float reachAmount)
    {
        Debug.Log("AddCategoryReach: gridPosition: " + gridPosition + " reachAmount: " + reachAmount);
        changeReach(gridPosition, reachAmount);
        VisualizeReach();
    }

    private void changeReach(Vector3Int gridPosition, float changeBy)
    {
       if(!_categoryReachTiles.ContainsKey(gridPosition))
       {
           _categoryReachTiles.Add(gridPosition, 0f);
       }
       float newValue = _categoryReachTiles[gridPosition] + changeBy;

       if(newValue <= 0f)
       {
            _categoryReachTiles.Remove(gridPosition);

            reachMap.SetTileFlags(gridPosition, TileFlags.None);
            reachMap.SetColor(gridPosition, noReachColor);
            reachMap.SetTileFlags(gridPosition, TileFlags.LockColor);
       }
       else
       {
           _categoryReachTiles[gridPosition] = Mathf.Clamp(newValue, 0f, MaxReach);
       }
    }

    private void VisualizeReach()
    {
        foreach(KeyValuePair<Vector3Int, float> entry in _categoryReachTiles)
        {
            float reachPercent = entry.Value / MaxReach;
            Color newColor = maxReachColor * reachPercent + minReachColor * (1 - reachPercent);

            reachMap.SetTileFlags(entry.Key, TileFlags.None);
            reachMap.SetColor(entry.Key, newColor);
            reachMap.SetTileFlags(entry.Key, TileFlags.LockColor);

        }
    }
}
