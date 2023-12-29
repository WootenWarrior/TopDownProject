using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : MonoBehaviour
{
    public bool isCollapsed;
    public Tilemap map;
    public Tile[] tiles;
    public void UpdateCell(bool state, Tile[] tileOptions){
        isCollapsed = state;
        tiles = tileOptions;

    }

    public void recreateTiles(Tile[] tileOptions){
        tiles = tileOptions;
    }
}
