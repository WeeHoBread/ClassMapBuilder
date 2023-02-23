using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    private GridHeatMap gridM;
    private Mesh mesh;
    private bool updatingMesh;


    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(GridHeatMap grid)
    {
        gridM = grid;
        UpdateHeatMapVisual();
        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, GridHeatMap.OnGridValueChangedEventArgs e)
    {
        //UpdateHeatMapVisual();
        updatingMesh = true;

    }

    private void LateUpdate()
    {
        //Placed so function will only run once per frame
        if(updatingMesh)
        {
            updatingMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void UpdateHeatMapVisual()
    {
        //Create a mesh area base on the grid, basically an overlay
        MeshUtils.CreateEmptyMeshArrays(gridM.GetWidth() * gridM.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < gridM.GetWidth(); x++)
        {
            for(int y = 0; y < gridM.GetHeight(); y++)
            {
                //An index for each grid, numbering them from 0 to number of grids -1 as it counted from 0
                int index = x * gridM.GetHeight() + y; 
                //Get the size of each square based on the cellsize
                Vector3 quadSize = new Vector3(1, 1) * gridM.GetCellSize();

                //Aquire quad's UV value for material
                int gridValue = gridM.GetValue(x, y);
                float gridValueNormalized = (float)gridValue / GridHeatMap.HEAT_MAP_MAX_VALUE; //Converts gridvalue into float to prevent error
                Vector2 gridValueUV = new Vector2(gridValueNormalized , 0f);

                //Add a quad (Square) to the mesh array based on coordinate
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridM.GetWorldPosition(x, y) + quadSize/2, 0f, quadSize,  gridValueUV, gridValueUV);

            }
        }

        //Update the mesh after the loop
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;


    }
        

}
