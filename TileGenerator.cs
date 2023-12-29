using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TileGenerator : MonoBehaviour
{
    public Tile[] tiles;
    public int tileMapSizeX;
    public int tileMapSizeY;
    public int startXpos;
    public int startYpos;
    public Transform playerTransform;
    public bool placeCenter = false;
    public Cell[,] Grid;
    public Cell cell;
    public GameObject parent;
    private int lowestEntropyVal;
    private int iterations;
    
    void Start()
    {
        iterations = 0;
        GenerateTiles();
        
        //parent = this.gameObject;

        if(placeCenter == true){
            PlacePlayerInCenter();
        }
    }

    void GenerateTiles()
    {
        Grid = new Cell[tileMapSizeX, tileMapSizeY];

        for (int x = startXpos; x < tileMapSizeX; x++)
        {
            for (int y = startYpos; y < tileMapSizeY; y++)
            {
                Vector3Int vector3 = new Vector3Int(x,y,0);
                cell = Instantiate(cell);
                cell.name = "Cell" + x + "," + y;
                cell.transform.parent = parent.transform;
                cell.transform.position = vector3;
                cell.UpdateCell(false,tiles);
                Grid[x,y] = cell;
            }
        }

        Vector3Int startCollapsePos = new Vector3Int(Random.Range(0,tileMapSizeX),Random.Range(0,tileMapSizeY),0);
        Cell startCell = Grid[startCollapsePos.x,startCollapsePos.y];
        Collapse(tiles,startCell);
        startCell.isCollapsed = true;

        List<Cell> lowestEntropyList = CreateLowEntropyList();
        while(iterations < tileMapSizeX*tileMapSizeY){ //tileMapSizeX*tileMapSizeY
            if(lowestEntropyList.Count > 0){
                Cell currentCell = lowestEntropyList[Random.Range(0,lowestEntropyList.Count)];
                Collapse(currentCell.tiles,currentCell);
                lowestEntropyList.Remove(currentCell);
            }
            iterations++;   //for when a cell is collapsed
        }
    }

    public void UpdateNeighbours(Cell cell){
        Cell upNeighbour = Grid[(int)cell.transform.position.x,(int)cell.transform.position.y+1];
        Cell downNeighbour = Grid[(int)cell.transform.position.x,(int)cell.transform.position.y-1];
        Cell leftNeighbour = Grid[(int)cell.transform.position.x-1,(int)cell.transform.position.y];
        Cell rightNeighbour = Grid[(int)cell.transform.position.x+1,(int)cell.transform.position.y+1];

        List<Cell> neighbours = new()
        {
            upNeighbour,
            downNeighbour,
            leftNeighbour,
            rightNeighbour
        };
        foreach (Cell currentCell in neighbours)
        {
            Vector3 vector = currentCell.transform.position-cell.transform.position;
            Vector3Int neighbourVector = new Vector3Int(Mathf.FloorToInt(vector.x),Mathf.FloorToInt(vector.y),Mathf.FloorToInt(vector.z));
            if(currentCell.isCollapsed == false){
                foreach (Tile thisCellTile in cell.tiles)
                {
                    foreach (Tile currentCellTile in currentCell.tiles)
                    {
                        if(thisCellTile.IsCompatible(currentCellTile,neighbourVector)==false){

                        }
                    }
                }
            }
        }
    }

    public List<Cell> CreateLowEntropyList(){
        List<Cell> lowestEntropy = new List<Cell>();
        lowestEntropyVal = tiles.Length;
        foreach (Cell cell in Grid)
        {
            if(cell.tiles.Length < lowestEntropyVal){
                lowestEntropyVal = cell.tiles.Length;
            }
        }

        foreach (Cell cell in Grid)
        {
            if((cell.tiles.Length == lowestEntropyVal)&&(cell.isCollapsed == false)){
                lowestEntropy.Add(cell);
            }
        }

        return lowestEntropy;
    }

    public void Collapse(Tile[] options, Cell cell){
        if(options.Length > 0){
            Tile choice = options[Random.Range(0,options.Length)];
            cell.isCollapsed = true;
            SpriteRenderer cellSpriteRenderer = cell.GetComponent<SpriteRenderer>();
            SpriteRenderer tileSpriteRenderer = choice.GetComponent<SpriteRenderer>();
            cellSpriteRenderer.sprite = tileSpriteRenderer.sprite;
        }
    }

    void PlacePlayerInCenter(){
        Vector3 center = new Vector3(tileMapSizeX/2,tileMapSizeY/2,0);
        playerTransform.position = center;
    }
}
