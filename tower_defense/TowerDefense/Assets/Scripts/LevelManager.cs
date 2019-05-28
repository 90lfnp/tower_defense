using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject[] tilePrefebs;

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;

    private Point blueSpawn, redSpawn;

    [SerializeField]
    private GameObject bluePortalPrefeb;

    [SerializeField]
    private GameObject redPortalPrefeb;

    public Portal BluePortal { get; set; }

    private Point mapSize;

    private Stack<Node> path;

    public Stack<Node> Path
    {
        get
        {
            if (path == null)
            {
                GeneratePath();
            }

            return new Stack<Node>(new Stack<Node>(path));
        }
    }



    public Dictionary<Point, TileScript> Tiles { get; set; }

    public float tileSize
    {
        get {return tilePrefebs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    public Point BlueSpawn { get => blueSpawn;}


    // Start is called before the first frame update
    void Start()
    {
       

        CreateLevel();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
 
    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>(); 

        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);
       
        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();

            for (int x = 0; x < mapX; x++)
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        cameraMovement.SetLimits(new Vector3(maxTile.x + tileSize, maxTile.y - tileSize));

        SpawnPortals();
    }
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        bool walkAble;
        bool isEmpty;
        if(string.Equals(tileType, "0"))
        {
            walkAble = true;
            isEmpty = false;
        }
        else
        {
            walkAble = false;
            isEmpty = true;
        }

        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefebs[tileIndex]).GetComponent<TileScript>();
        
        newTile.GetComponent<TileScript>().Setup(new Point(x, y), new Vector3(worldStart.x + (tileSize * x), worldStart.y - (tileSize * y), 0), map, walkAble, isEmpty);

       

   
    }
    
    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;

        string data = bindData.text.Replace(Environment.NewLine, string.Empty);

        return data.Split('-');
    }

    private void SpawnPortals()
    {
        blueSpawn = new Point(0, 4);
        GameObject tmp = (GameObject)Instantiate(bluePortalPrefeb, Tiles[BlueSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        BluePortal = tmp.GetComponent<Portal>();
        BluePortal.name = "BluePortal";

        redSpawn = new Point(19, 4);

        Instantiate(redPortalPrefeb, Tiles[redSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    public void GeneratePath()
    {
        path = Astar.GetPath(BlueSpawn, redSpawn);
    }
    


}
