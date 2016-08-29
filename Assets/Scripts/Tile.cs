using UnityEngine;
using System.Collections;

public class Tile {

	public Grid grid;

	public GameObject gameObject;
	public int x, y;

	public int height = 0;
	public int weight = 1;

	public bool occupied = false;
	public Unit occupier = null;


	public Tile(Grid grid, int x, int y) {
		this.grid = grid;
		this.x = x;
		this.y = y;		
	}


}
