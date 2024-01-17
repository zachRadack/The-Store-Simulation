using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

// This handles data for each shelf and its structure. 
[System.Serializable]
public class ShelfData
{
    public Vector2 dimensions;
    public Vector2 position;
    public Vector3Int position3d;
    public int inventoryCount;
    public bool isDirty;
    public string shelfType;
    public Dictionary<shelfTile.ShelfGrid, string> ShelvingGrid;
    public int maxShelfY;
    
    public ShelfData(Vector2 _dimensions, Vector2 _position)
    {
    
        dimensions = translateDimensions(_dimensions);
        position = TranslatePosition(_position, _dimensions);
    }

    // Translates top-left based dimensions to Unity's RectTransform system
    private Vector2 translateDimensions(Vector2 originalDimensions)
    {
        RectTransform canvasRect = TriggerUI.CanvasRectTransform;
    

        float x = originalDimensions.x - canvasRect.sizeDelta.x;
        float y = originalDimensions.y - canvasRect.sizeDelta.y;

        return new Vector2(x, y);
    }

    // Translates top-left based coordinates to Unity's RectTransform system
    private Vector2 TranslatePosition(Vector2 originalPosition, Vector2 size)
    {
        RectTransform canvasRect = TriggerUI.CanvasRectTransform;

        float x = originalPosition.x - (canvasRect.sizeDelta.x / 2) + (size.x / 2);
        float y = (canvasRect.sizeDelta.y / 2) - originalPosition.y - (size.y / 2);

        return new Vector2(x, y);
    }


    
}