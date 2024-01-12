using UnityEngine;

public class CoordinateTranslator : MonoBehaviour
{
    public RectTransform referenceRectTransform; // The RectTransform you are working with, typically the canvas

    // Translates top-left based coordinates to Unity's RectTransform system
    public Vector2 TranslateCoordinates(Vector2 topLeftCoordinates)
    {
        float translatedX = topLeftCoordinates.x - (referenceRectTransform.rect.width / 2);
        float translatedY = (referenceRectTransform.rect.height / 2) - topLeftCoordinates.y;

        return new Vector2(translatedX, translatedY);
    }
}
