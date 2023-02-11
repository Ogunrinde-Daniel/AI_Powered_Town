using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum TILETYPE { EMPTY_LAND, ROAD, HOUSE, BANK, FARM, LIBRARY, SCHOOL, HANGOUT_SPOT, MARKET };
public enum POSSESIONVALUE { EMPTY_LAND = 0, ROAD = 0 , HOUSE = 500, BANK_ACCOUNT = 0, FARM  = 1000, LIBRARY = 1000, SCHOOL = 2000, HANGOUT_SPOT = 0, MARKET = 0 };

public class ClickHandler : MonoBehaviour
{
    public TextMeshProUGUI aboutText;
    public string text;
    public bool active = true;
    private void OnMouseEnter()
    {
        if (!active)
        {
            aboutText.gameObject.SetActive(false);
            return;
        }

        aboutText.gameObject.SetActive(true);
        aboutText.text = text;
    }
    private void OnMouseExit()
    {
        aboutText.gameObject.SetActive(false);
    }

}

public class SpawnWorldLand : MonoBehaviour
{

    public GameObject road;
    public GameObject emptyLand;
    public GameObject house;
    public GameObject farm;
    public GameObject bank;
    public GameObject library;
    public GameObject school;
    public GameObject market;
    public GameObject hangoutSpot;



    public TextMeshProUGUI aboutText;

    public int worldWidth;
    public int worldHeight;

    public CustomTileMap tileMap;
    List<GameObject> mapObjects;

    void Awake()
    {
        worldWidth = GameData.tileWidth;
        worldHeight = GameData.tileHeight;

        tileMap = new CustomTileMap(worldWidth, worldHeight, (int)emptyLand.GetComponent<SpriteRenderer>().bounds.size.x);
        mapObjects = new List<GameObject>();

        reloadMap();
    }

    public void reloadMap()
    {
        //clear map
        for (int i = mapObjects.Count - 1; i >= 0; i--)
        {
            Destroy(mapObjects[i], 0.1f);
        }
        mapObjects.Clear();

        mapObjects = new List<GameObject>();
        GameObject tempTile = new GameObject();
        for (int i = 0; i < tileMap.map.Count; i++)
        {
            switch (tileMap.map[i].tileType)
            {
                case (int)TILETYPE.EMPTY_LAND:
                    tempTile = Instantiate(emptyLand, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.ROAD:
                    tempTile = Instantiate(road, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.HOUSE:
                    tempTile = Instantiate(house, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.FARM:
                    tempTile = Instantiate(farm, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.BANK:
                    tempTile = Instantiate(bank, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.LIBRARY:
                    tempTile = Instantiate(library, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.SCHOOL:
                    tempTile = Instantiate(school, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.HANGOUT_SPOT:
                    tempTile = Instantiate(hangoutSpot, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
                case (int)TILETYPE.MARKET:
                    tempTile = Instantiate(market, new Vector2(tileMap.map[i].xScreenPos, tileMap.map[i].yScreenPos), Quaternion.identity);
                    break;
            }
            tempTile.AddComponent<BoxCollider2D>();
            tempTile.GetComponent<BoxCollider2D>().isTrigger = true;
            tempTile.AddComponent<ClickHandler>();
            tempTile.GetComponent<ClickHandler>().aboutText = aboutText;
            tempTile.GetComponent<ClickHandler>().text = tileMap.map[i].about();
            tempTile.transform.SetParent(this.transform);
            mapObjects.Add(tempTile);

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var tile in mapObjects)
            {
                tile.GetComponent<ClickHandler>().active = !tile.GetComponent<ClickHandler>().active;
            }

        }

    }
}

public class CustomTileMap{
    public int mapWidth;
    public int mapHeight;
    public int tileWidth;  //assumes the tile is a square

    private int roadToLand = 6; //after how many land should there be road

    public List<CustomTile> map = new List<CustomTile>();

    public CustomTileMap(int mapWidth, int mapHeight, int tileWidth)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.tileWidth = tileWidth;

        generateDefaultMap();
    }

    public void generateDefaultMap()
    {
        int row, col;
        CustomTile tempTile;
        for (row = 0; row < mapWidth; row++)
        {
            for (col = 0; col < mapHeight; col++)
            {
                tempTile = new CustomTile();

                tempTile.x = row;
                tempTile.y = col;
                //should we add a road
                if ( (row % roadToLand == 0) || (col % roadToLand == 0) ) 
                    tempTile.tileType = (int)TILETYPE.ROAD;
                else
                    tempTile.tileType = (int)TILETYPE.EMPTY_LAND;

                tempTile.xScreenPos = row * tileWidth + (tileWidth / 2f);
                tempTile.yScreenPos = -(col * tileWidth + (tileWidth / 2f));
                tempTile.totalValue = (int)TILETYPE.EMPTY_LAND;
                map.Add(tempTile);

            }
        }

    }

    public Vector2 getPosition(int row, int col)
    {
        int posInMap = (row * mapWidth) + col;
        if (posInMap >= map.Count) return Vector2.zero;
        if (posInMap < 0) return Vector2.zero;
        return new Vector2( map[posInMap].xScreenPos, map[posInMap].yScreenPos);
    }

    public int coordToLinear(int row, int col)
    {
        return (row * mapWidth) + col;
    }


    public bool isTileRoad(int row, int col)
    {
        int posInMap = (row * mapWidth) + col;
        if (posInMap >= map.Count)
            return false;
        if (posInMap < 0)
            return false;
        if (map[posInMap].tileType == (int)TILETYPE.ROAD)
            return true;

        return false;
    }

    public bool isTileEmpty(int row, int col) {
        int posInMap = (row * mapWidth) + col;
        if (posInMap >= map.Count) return false;
        if (posInMap < 0) return false;
        if (map[posInMap].tileType == (int)TILETYPE.EMPTY_LAND) return true;
       
        return false;
    }

    public bool isTileInBounds(int row, int col)
    {
        int posInMap = (row * mapWidth) + col;
        if (posInMap >= map.Count)
            return false;
        if (posInMap < 0)
            return false;

        return true;
        ;
    }

    public Stack<int[]> getPath(int[] start, int[] destination)
    {
        char[][] matrix = mapToChar(start, destination);
        return PathFind.unitTest_AStar(matrix, start, destination);
    }


    public char[][] mapToChar(int[] start, int[] destination)
    {
        char[][] charMap = new char[mapWidth][];
        for (int i = 0; i < mapWidth; i++){
            charMap[i] = new char[mapHeight];
        }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (map[coordToLinear(i, j)].tileType == (int)TILETYPE.ROAD)
                    charMap[i][j] = '-';
                else
                    charMap[i][j] = 'X';
                if( (i == start[0] && j == start[1])
                    || (i == destination[0] && j == destination[1]))
                    charMap[i][j] = '-';

            }
        }



        return charMap;
    }

}

public class CustomTile {
    public int x;
    public int y;
    public int tileType;
    public float xScreenPos;
    public float yScreenPos;

    public int totalValue;
    public string owner = "Public";
    public string about()
    {
        string about = "";
        about += "Owner: " + owner + "\n";
        about += "ID: " + x + "," + y + "\n";
        about += "Value: " + totalValue + "\n";
        about += "Tile Type: " + ((TILETYPE) tileType).ToString() + "\n";

        return about;
    }


}

