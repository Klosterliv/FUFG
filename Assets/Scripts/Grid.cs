using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	//public List<List<Tile>> grid;
	public Tile[,] grid;
	public int xSize, ySize;

	public GameObject gridObject;

	// Use this for initialization
	void Awake () {
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
				grid[x, y] = new Tile(this, x, y);
				grid[x, y].gameObject = newTileObject;

				grid[x,y].weight = Random.Range(1,3);
				grid[x,y].height = Random.Range(0,2);

			}			
		}

	}

	public Tile GetTile (Vector3 pos) {
		Debug.DrawRay(pos, Vector3.up*2, Color.red, .1f);
		Debug.DrawRay(grid[(int)(pos.x+0.5f), (int)(pos.z+0.5f)].gameObject.transform.position, Vector3.up*3, Color.green, 0.1f);
		return grid[(int)(pos.x+0.5f), (int)(pos.z+0.5f)];

	}
	public List<Tile> GetNeighbours (Tile t) {

		List<Tile> neighbours = new List<Tile>();
		if (t.x > 0) {
			neighbours.Add(grid[t.x-1,t.y]);
			if (t.y < ySize-1)
				neighbours.Add(grid[t.x-1,t.y+1]);
			if (t.y > 0)
				neighbours.Add(grid[t.x-1, t.y-1]);
		}
		if (t.x < xSize-1) {
			neighbours.Add(grid[t.x+1, t.y]);
			if (t.y < ySize-1)
				neighbours.Add(grid[t.x+1,t.y+1]);
			if (t.y > 0)
				neighbours.Add(grid[t.x+1, t.y-1]);				
		}
		if (t.y > 0) neighbours.Add(grid[t.x,t.y-1]);
		if (t.y < ySize-1) neighbours.Add(grid[t.x,t.y+1]);	

		return neighbours;		
		
	}

	public float GetWeight (Tile from, Tile to) {

		float w = 0;

		int dx = Mathf.Abs(from.x-to.x);
		int dy = Mathf.Abs(from.y-to.y);
		if (dx+dy > 1) {
			w+=0.5f;
		}

		float dh = to.height-from.height;

		if (to.occupied) return 999999;

		return w+to.weight;
	}
	public float GetDistance (Tile from, Tile to) {

		float distance = Vector2.Distance(new Vector2(from.x,from.y), new Vector2(to.x, to.y));

		return distance;
	}
	public float GetHeightDelta (Tile from, Tile to) {
		
		float dh = to.height-from.height;
		return dh;

	}




}
