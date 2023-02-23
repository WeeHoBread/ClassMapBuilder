using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeArchive.Archive;

public class GridHeatMap
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;

    private int[,] gridArray;

    private float cellSize;

    private TextMesh[,] debugTextArray;

    private Vector3 originPosition;

    public GridHeatMap(int width, int height, float cellSize, Vector3 originPosition/* Func<TGridObject> createGridObject*/)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(0); y++)
            {
                //gridArray[x, y] = createGridObject();
            }
        }

        bool showDebug = true;
        if (showDebug)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = ArchiveClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) / 2, 20, Color.white, TextAnchor.MiddleCenter);

                    //Create the Grid lines
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            //Create grid lines at end of grid area
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    #region Getting grid position in the world
    //Convert world position into grid position
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    //Get Grid position based on world position
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    #endregion

    #region Get Grid Variables
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    #endregion

    #region Set Grid Value
    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) //Check if value is invalid
        {
            gridArray[x, y] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);            

            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });

            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }

    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public void AddValue(int x, int y, int value)
    {
        SetValue(x, y, GetValue(x, y) + value);
    }

    #endregion

    #region Read Grid value
    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) //Check if value is invalid
        {
            return gridArray[x, y];
        }
        else
        {
            return default(int);
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;

        GetXY(worldPosition, out x, out y);

        return GetValue(x, y);
    }
    #endregion

    //Value is value that will be added, max range is the range, total range is the amount that will drop after the max range is reached
    public void AddValue(Vector3 worldPosition, int value, int maxRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - maxRange));
        GetXY(worldPosition, out int originX, out int originY);

        //Cycle through the range around the targeted grid, range as in grid within range of origin/target
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange-x; y++)
            //Setup in a way, higher the x, lower the y, forming a square shape with the below if statements
            {
                int radius = x + y; //radius of current cycle 
                int currentValue = value;
                if (radius > maxRange)
                {
                    currentValue -= lowerValueAmount * (radius - maxRange);
                }

                AddValue(originX + x, originY + y, currentValue);

                //Multiple if statements so that values are not added to the same grid more than once
                if (x != 0)
                {
                    AddValue(originX - x, originY + y, currentValue);
                }

                if (y != 0)
                {
                    AddValue(originX + x, originY - y, currentValue);

                    if (x != 0)
                    {
                        AddValue(originX - x, originY - y, currentValue);
                    }
                }
            }
        }

    }

}
