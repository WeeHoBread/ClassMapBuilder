using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeArchive.Archive;

public class TestDummy : MonoBehaviour
{
    [SerializeField] private HeatMapVisual heatmapVisual;
    [SerializeField] private HeatMapBoolVisual heatmapBoolVisual;
    [SerializeField] private HeatMapGenericVisual heatMapGenericVisual;
    private GridCustom<HeatMapGridObj> grid;
    private GridCustom<StringGridObject> gridString; 

    private void Start()
    {
        grid = new GridCustom<HeatMapGridObj>(14, 10, 10, new Vector3(-70, -50), (GridCustom<HeatMapGridObj> g, int x, int y  ) => new HeatMapGridObj(g, x, y));
        gridString = new GridCustom<StringGridObject>(14, 10, 10, new Vector3(-70, -50), (GridCustom<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));

        //heatmapVisual.SetGrid(grid);
        //heatmapBoolVisual.SetGrid(grid);
        heatMapGenericVisual.SetGrid(grid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // On Left click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            HeatMapGridObj heatMapGridObj = grid.GetGridObject(mousePos);

            if (heatMapGridObj != null)
            {
                heatMapGridObj.AddValue(5);
            }

            //grid.SetValue(mousePos, true);
        }

        if (Input.GetMouseButtonDown(1)) // On Right click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gridString.GetGridObject(mousePos).AddLetter("A");
        }

    }

}

public class HeatMapGridObj
{
    private const int Min = 0;
    private const int Max = 100;

    private GridCustom<HeatMapGridObj> grid;
    private int x;
    private int y;
    private int value;

    public HeatMapGridObj(GridCustom<HeatMapGridObj> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int addValue)
    {
        value += addValue;
        value = Mathf.Clamp(value, Min, Max);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized()
    {
        return (float)value / Max;
    }

    public override string ToString()
    {
        return value.ToString();
    }

}

public class StringGridObject
{
    private GridCustom<StringGridObject> grid;
    private int x;
    private int y;

    private string letters;
    private string numbers;

    public StringGridObject(GridCustom<StringGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        letters = "";
        numbers = "";
    }

    public void AddLetter(string letter)
    {
        letters += letter;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void AddNumber(string number)
    {
        numbers += number;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return letters + "\n" + numbers;
    }
}

