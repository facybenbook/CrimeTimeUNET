﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGen : MonoBehaviour {
	

	[SerializeField]
	private GameObject tilePrefab;

    [System.Serializable]
    public class ColorToTileEntry
    {
        public Color color;
        public TileType tile;
    }

    [SerializeField]
    private ColorToTileEntry[] colorToTileMap;

    private Dictionary<Color, TileType> colorToTile = new Dictionary<Color, TileType>();

	private TileBehavior[,] tileArray;
    public TileBehavior[,] TileArray
    {
        get { return tileArray; }
    }

    [SerializeField]
    private Map selectedMap;

    [SerializeField]
    private CameraFocus cameraFocus;

    private int fieldSize;

    private int fieldSizeX;
    private int fieldSizeZ;

    private TileBehavior copSpawn;
    private TileBehavior gangSpawn;

    // Use this for initialization
    void Start () {

		//generateTiles ();
        foreach (ColorToTileEntry c in colorToTileMap)
            colorToTile.Add(c.color, c.tile);
        generateFromMap(selectedMap);

	}
    /*
	void generateTiles(){
		tileArray = new TileBehavior[fieldSize,fieldSize];
        float tileSpacing = tilePrefab.GetComponent<MeshRenderer>().bounds.size.x;
        int position;
		for (int i = 0; i< fieldSize; i++)
		for(int j = 0; j< fieldSize; j++){
			position = (i*fieldSize + j);
			tileArray[i, j] = ((GameObject)Instantiate(tilePrefab, new Vector3(i*tileSpacing, 0, j*tileSpacing), Quaternion.identity)).GetComponent<TileBehavior>();
                tileArray[i, j].transform.parent = this.transform;
			tileArray[i,j].transform.Rotate(new Vector3(90,0,0));
			if (position%2 == 0)
				tileArray[i,j].GetComponent<MeshRenderer>().material.color = Color.grey;
			tileArray[i,j].GetComponent<TileBehavior>().X = i;
			tileArray[i,j].GetComponent<TileBehavior>().Z = j;
		}
	}
    */
    public void generateFromMap(Map map)
    {

        map.InitializeMap();

        fieldSizeX = map.Layout.width;
        fieldSizeZ = map.Layout.height;
        tileArray = new TileBehavior[fieldSizeX, fieldSizeZ];
        float tileSpacing = map.Tileset.Size;
        Debug.Log(fieldSizeX + "," + fieldSizeZ);
        int position;
        cameraFocus.SetCameraForMap(fieldSizeX, fieldSizeZ, tileSpacing);
        for (int i = 0; i < fieldSizeX; i++)
            for (int j = 0; j < fieldSizeZ; j++)
            {
                position = (i * 5 + j);
                TileType t = colorToTile[map.Layout.GetPixel(i, j)];

                tilePrefab = map.Tileset.TypeToTile[t];

                tileArray[i, j] = ((GameObject)Instantiate(tilePrefab, new Vector3(i * tileSpacing, 0, j * tileSpacing), Quaternion.identity)).GetComponent<TileBehavior>();
                tileArray[i, j].transform.parent = this.transform;
                tileArray[i, j].transform.Rotate(new Vector3(90, 0, 0));
                tileArray[i, j].Coords = new Coordinate(i, j);
                tileArray[i, j].TileType = t;

                if (t == TileType.SpawnpointCops)
                    copSpawn = TileArray[i, j];
                else if(t == TileType.SpawnpointThieves)
                    gangSpawn = TileArray[i, j];

                if (position % 2 == 0 && t == TileType.FloorIndoor)
                    tileArray[i, j].GetComponent<MeshRenderer>().material.color = Color.grey;
            }

        foreach (CommandPanel cp in PlayerRegisterRule.PlayerRegister.Values)
        {
            if (cp.Team == Alliance.Cops)
                cp.SpawnPlayerCharacter(copSpawn);
            else if (cp.Team == Alliance.Robbers)
                cp.SpawnPlayerCharacter(gangSpawn);
        }
    }

}
