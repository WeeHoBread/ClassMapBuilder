using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapVisual : MonoBehaviour
{
    //Allow data to be store and used later, also to allow it to be shown in the editor (System.Serializable)
    [System.Serializable]
    public struct TilemapSpriteUV
    {
        public TilemapCustom.TilemapObject.TilemapSprite tilemapSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] tilemapSpriteUVArray;
    private Grid2D<TilemapCustom.TilemapObject> gridM;
    private Mesh mesh;
    private bool updatingMesh;
    private Dictionary<TilemapCustom.TilemapObject.TilemapSprite, UVCoords> uvCoordsDictionary;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<TilemapCustom.TilemapObject.TilemapSprite, UVCoords>();

        foreach (TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray)
        {
            uvCoordsDictionary[tilemapSpriteUV.tilemapSprite] = new UVCoords
            {
                uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
            };
        }

    }

    public void SetGrid(TilemapCustom tilemap, Grid2D<TilemapCustom.TilemapObject> grid)
    {
        gridM = grid;
        UpdateHeatMapVisual();

        grid.OnGridObjectChanged += Grid_OnGridValueChanged;

        //tilemap += Tilemap_OnLoaded;
    }

    private void TileMap_OnLoaded(object sender, System.EventArgs e)
    {
        updatingMesh = true;
    }

    private void Grid_OnGridValueChanged(object sender, Grid2D<TilemapCustom.TilemapObject>.OnGridObjectChangedEventArgs e)
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
                TilemapCustom.TilemapObject gridValue = gridM.GetGridObject(x, y);
                TilemapCustom.TilemapObject.TilemapSprite tilemapSprite = gridValue.GetTilemapSprite();
                Vector2 gridUV00, gridUV11;

                //If grid is not has ground, it will be green
                if(tilemapSprite == TilemapCustom.TilemapObject.TilemapSprite.None)
                {
                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.one;
                    quadSize = Vector3.zero; //If there is no ground, quadsize 0  will block rendering of a quad/plane
                }
                else
                {
                    UVCoords uvCoords = uvCoordsDictionary[tilemapSprite];
                    gridUV00 = uvCoords.uv00;
                    gridUV11 = uvCoords.uv11;
                }

                //Add a quad (Square) to the mesh array based on coordinate
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, gridM.GetWorldPosition(x, y) + quadSize/2, 0f, quadSize,  gridUV00, gridUV11);

            }
        }

        //Update the mesh after the loop
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;


    }
        

}
