using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{

    [SerializeField]
    private Tilemap backgroundMap;
    [SerializeField]
    private Tilemap interactiveMap; //the tilemap containing the outline for which tiles the player can change.
    [SerializeField]
    private Tilemap playerMap; //The tilemap the player can change
    [SerializeField]
    private const int placedTileLimit = 5; //how many tiles the player can place before they're removed.

    [SerializeField]
    private List<TileBase> tileList; //A list of tiles to allow the player to use.

    [SerializeField]
    private List<TileData> tileData;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private int tileNumber;

    private Queue<Vector3Int> tileQueue = new Queue<Vector3Int>();

    private void Awake()
    {

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileData)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

    }


    private void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePoistion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = backgroundMap.WorldToCell(mousePoistion);

            //TileBase clickedTile = backgroundMap.GetTile(gridPosition);
            TileBase ct2 = interactiveMap.GetTile(gridPosition);

            //print("At position " + gridPosition + " there is a " + clickedTile);
            print("At " + gridPosition + " there is a " + ct2);
            print("Current tile value is " + tileNumber);

            if (ct2 != null) //if there's a block on the interactive map
            {

                Debug.Log("Changing Tile!");
                changeTile(gridPosition, tileNumber); //attempt to change the tile

            }

            //bool interactable = dataFromTiles[clickedTile].changeable;

            //print(clickedTile + ": " + interactable);


        }

    }

    /// <summary>
    /// This changes the tile number/selection that the player is currently attempting to place.
    /// </summary>
    public void changeTileNum(int tNum)
    {

        switch (tNum)
        {
            case 0:
                tileNumber = 0;
                break;

            case 1:
                tileNumber = 1;
                break;

            case 2:
                tileNumber = 2;
                break;

            default:
                tileNumber = 0; //This is the tile number for clear.
                break;

        }


    }

    public void changeTile(Vector3Int position, int tNum)
    {

        switch (tNum)
        {

            case 0:
                //change clear
                Debug.Log("changing tile at " + position);
                playerMap.SetTile(position, null);

                foreach (Vector3Int v in tileQueue)
                {
                    Debug.Log(v);
                }
                //now we need to find it's position in the queue and remove it manually.
                tileQueue = new Queue<Vector3Int>(tileQueue.Where(x => x != position)); //This uses linq to filter the Q by position. It just remakes the new Q without the item we want removed.

                foreach (Vector3Int v in tileQueue)
                {
                    Debug.Log(v);
                }

                break;

            case 1:
                //change forest
                Debug.Log("changing tile to " + tileList[1]);
                //interactiveMap.SetTile(position, tileList[1]);
                playerMap.SetTile(position, tileList[1]);
                tileQueue.Enqueue(position); //queue up the position of the tile
                break;

            case 2: //Trying to test out like a - shape here

                playerMap.SetTile(position, tileList[1]); //Change actual tile eventually
                playerMap.SetTile(new Vector3Int(position.x + 1, position.y, position.z), tileList[1]);
                playerMap.SetTile(new Vector3Int(position.x - 1, position.y, position.z), tileList[1]);

                break;

            default:
                //change clear
                break;

        }

        if(tileQueue.Count > placedTileLimit)
        {
            playerMap.SetTile(tileQueue.Dequeue(), null); //set the tile at the beginning of the queue to null effectively removing it.
        }


    }

    /// <summary>
    /// This makes the bridge for the player to cross in 1-1 when they talk to the slime
    /// </summary>
    public void makeSlimeBridge()
    {

        print("tried to build bridge");

        //42-45
        int start = 42;
        int end = 46;
        int yStart = 0;
        int yEnd = 1;
        for(int x = start; x < end; x++)
        {
            for(int y = yStart; y < yEnd; y++)
            {
                backgroundMap.SetTile(new Vector3Int(x, y, 0), tileList[1]);
            }
        }

    }

}
