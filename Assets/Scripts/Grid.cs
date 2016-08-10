using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	//public List<List<Tile>> grid;
	public Tile[,] grid;
	public int xSize, ySize;

	public GameObject gridObject;

	// Use this for initialization
	void Start () {
		Init();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Init () {

		grid = new Tile[xSize,ySize];
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTileObject = (GameObject) Instantiate(gridObject, transform.position + new Vector3(x,0,y), transform.rotation);
				newTileObject.name = x+";"+y;
				newTileObject.transform.parent = transform;
				grid[x, y] = new Tile(x, y);
				grid[x, y].gameObject = newTileObject;

			}			
		}

	}

	public Tile GetTile (Vector3 pos) {
		Debug.DrawRay(pos, Vector3.up*2, Color.red, .1f);
		Debug.DrawRay(grid[(int)(pos.x+0.5f), (int)(pos.z+0.5f)].gameObject.transform.position, Vector3.up*3, Color.green, 0.1f);
		return grid[(int)(pos.x+0.5f), (int)(pos.z+0.5f)];
	}
}
