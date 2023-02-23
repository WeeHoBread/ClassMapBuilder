using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapTester : MonoBehaviour
{
    [SerializeField] private HeatMapVisual heatMapVisual;
    private GridHeatMap grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridHeatMap(100, 100, 5f, new Vector3(-250, -250));

        heatMapVisual.SetGrid(grid);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            //At mouse position, grid will increase value by of grids up to 
            grid.AddValue(mousePos, 100, 5, 10);

            //int value = grid.GetValue(mousePos);
            //grid.SetValue(mousePos, value +5);
        }
    }
}
