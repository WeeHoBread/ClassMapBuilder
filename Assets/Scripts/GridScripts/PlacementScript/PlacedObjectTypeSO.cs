using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName = "ScriptableObjects/Buildings")]
public class PlacedObjectTypeSO : ScriptableObject
{
    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    };

    public enum ObjectType
    {
        Object,
        SpawnPoint,
        EndPoint,
    };

    public ObjectType objectType;
    public bool oneInstanceObject;

    public string nameString;
    public Transform prefab;
    public Transform ghost;
    public int width;
    public int height;

    private int maxChildIndex;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    public ObjectType GetObjectType()
    {
        return objectType;
    }

    public bool isOneInstanceObject()
    {
        return oneInstanceObject;
    }

    #region Functions for decideding which variants to place
    public int GetMaxVariantIndex()
    {
        maxChildIndex = prefab.childCount;
        return maxChildIndex;
    }

    public void SetNextActiveChild(int variantNo)
    {
        //Cycle through children set all to in active
        for (int i = 0; i < prefab.childCount; i++)
        {
            prefab.GetChild(i).gameObject.SetActive(false);
        }

        //Set specific child to be active
        prefab.GetChild(variantNo).gameObject.SetActive(true);  
    }
    #endregion
}
