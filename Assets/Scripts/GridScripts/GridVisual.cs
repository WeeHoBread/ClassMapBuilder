using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private GridBuilder gridBuilder;
    GridCustom<GridBuilder.GridObject> gridM;
    private Mesh mesh;

    //Storage to for actual grid's size
    private int gridW;
    private int gridH;

    [SerializeField]private Transform tilePrefab;
    [SerializeField]private Transform wallPrefab;



    public void GenerateGridVisual()
    {
        GetGridInfo();

        //Reset new position
        Vector3 newPos = gridM.GetWorldPosition(0, 0);
        transform.position = newPos;

        //Generate grid base on new position
        for (int x = 0; x < gridW*10; x+=10)
        {
            for(int z = 0; z < gridH*10; z+=10)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(this.transform.position.x+x, 0, this.transform.position.z+z), Quaternion.identity);
                
                spawnedTile.name = $"Tile {(x)/10} {(z)/10}";
                spawnedTile.transform.parent = this.gameObject.transform.GetChild(0);
            }
        }
        GenerateGridBarrier();
    }

    public void RefreshGrid()
    {
        GetGridInfo();

        foreach(Transform child in this.gameObject.transform.GetChild(0))
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in this.gameObject.transform.GetChild(1))
        {
            GameObject.Destroy(child.gameObject);
        }

        //Reset new position
        Vector3 newPos = gridM.GetWorldPosition(0, 0);
        transform.position = newPos;

        //Generate grid base on new position
        for (int x = 0; x < gridW * 10; x += 10)
        {
            for (int z = 0; z < gridH * 10; z += 10)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(this.transform.position.x + x, 0, this.transform.position.z + z), Quaternion.identity);

                spawnedTile.name = $"Tile {(x) / 10} {(z) / 10}";
                spawnedTile.transform.parent = this.gameObject.transform.GetChild(0);
            }
        }
        GenerateGridBarrier();
    }

    private void UpdateGrid()
    {
        gridW = gridBuilder.gridWidth;
        gridH = gridBuilder.gridHeight;

        MeshUtils.CreateEmptyMeshArrays(gridW * gridH, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < gridW; x++)
        {
            for (int y = 0; y < gridH; y++)
            {
                //An index for each grid, numbering them from 0 to number of grids -1 as it counted from 0
                int index = x * gridH + y;
                //Get the size of each square based on the cellsize
                Vector3 quadSize = new Vector3(1, 1) * gridM.GetCellSize();
                

                //Add a quad (Square) to the mesh array based on coordinate
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridM.GetWorldPosition(x, y) + quadSize / 2, 0f, quadSize, Vector2.zero, Vector2.zero);

            }
        }

        //Update the mesh after the loop
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void PlaceMesh()
    {
        #region This makes one grid
        Vector3[] verticies = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        verticies[0] = new Vector3(0, 1);
        verticies[1] = new Vector3(1, 1);
        verticies[2] = new Vector3(0, 0);
        verticies[3] = new Vector3(1, 0);

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 1);
        uv[2] = new Vector2(0, 0);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        #endregion
    }

    private void GetGridInfo()
    {
        gridM = gridBuilder.grid;
        gridW = gridBuilder.gridWidth;
        gridH = gridBuilder.gridHeight;
    }

    public void GenerateGridBarrier()
    {
        GetGridInfo();

        Vector3 corner01Pos = gridM.GetWorldPosition(0, 0);
        Vector3 corner02Pos = gridM.GetWorldPosition(gridW, 0);
        Vector3 corner03Pos = gridM.GetWorldPosition(gridW, gridH);
        Vector3 corner04Pos = gridM.GetWorldPosition(0, gridH);

        //Instantiate each wall, rotate and scale
        var wall01 = Instantiate(wallPrefab, corner01Pos, Quaternion.identity);
        wall01.localScale = new Vector3(1, 20, gridH);
        wall01.name = "Wall 01";

        var wall02 = Instantiate(wallPrefab, corner02Pos, Quaternion.identity);
        wall02.rotation = Quaternion.Euler(0, 270, 0);
        wall02.localScale = new Vector3(1, 20, gridW);
        wall02.name = "Wall 02";

        var wall03 = Instantiate(wallPrefab, corner03Pos, Quaternion.identity);
        wall03.rotation = Quaternion.Euler(0, 180, 0);
        wall03.localScale = new Vector3(1, 20, gridH);
        wall03.name = "Wall 03";

        var wall04 = Instantiate(wallPrefab, corner04Pos, Quaternion.identity);
        wall04.rotation = Quaternion.Euler(0, 90, 0);
        wall04.localScale = new Vector3(1, 20, gridW);
        wall04.name = "Wall 04";

        wall01.transform.parent = this.gameObject.transform.GetChild(1);
        wall02.transform.parent = this.gameObject.transform.GetChild(1);
        wall03.transform.parent = this.gameObject.transform.GetChild(1);
        wall04.transform.parent = this.gameObject.transform.GetChild(1);
    }


}
