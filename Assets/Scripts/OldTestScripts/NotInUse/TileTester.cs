using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTester : MonoBehaviour
{
    [SerializeField] private TilemapVisual tileMapVisual;
    private TilemapCustom tileMap;
    private TilemapCustom.TilemapObject.TilemapSprite tilemapSprite;

    private void Start()
    {
        tileMap = new TilemapCustom(20, 12, 10f, new Vector3(-100, - 60));

        tileMap.SetTilemapVisual(tileMapVisual);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tileMap.SetTilemapSprite(mousePos, tilemapSprite);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Removing tile");
            tilemapSprite = TilemapCustom.TilemapObject.TilemapSprite.None;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Placing Ground Tile");
            tilemapSprite = TilemapCustom.TilemapObject.TilemapSprite.Ground;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Placing Path Tile");
            tilemapSprite = TilemapCustom.TilemapObject.TilemapSprite.Path;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Placing Dirt Tile");
            tilemapSprite = TilemapCustom.TilemapObject.TilemapSprite.Dirt;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            tileMap.Save();
            Debug.Log("saving");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            tileMap.Load();
            Debug.Log("loading");
        }

    }

}
