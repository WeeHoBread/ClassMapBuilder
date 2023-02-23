using System.Collections.Generic;
using UnityEngine;


public class PlacableObjects : MonoBehaviour
{

    public static PlacableObjects Create(Vector3 worldPos, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSo)
    {
        Transform placedObjectTransform 
            = Instantiate(placedObjectTypeSo.prefab, worldPos, //Object and Position
            Quaternion.Euler(0, placedObjectTypeSo.GetRotationAngle(dir), 0)); //Rotation

        PlacableObjects placedObject = placedObjectTransform.GetComponent<PlacableObjects>();

        placedObject.placedObjectTypeSO = placedObjectTypeSo;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;

    }    
    
    [HideInInspector]
    public PlacedObjectTypeSO placedObjectTypeSO;

    public bool breakable;

    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;
    
    public List<Vector2Int> GetGridPositionList() //Return the object's transform based on its origin and direction
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void InactiveSelf()
    {
        gameObject.SetActive(false);
    }

    public void TurnActive()
    {
        gameObject.SetActive(true);
    }

}
