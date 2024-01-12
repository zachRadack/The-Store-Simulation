using UnityEngine;

[System.Serializable]
public class ShelfData
{
    public Vector2 dimensions;
    public Vector2 position;
    
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