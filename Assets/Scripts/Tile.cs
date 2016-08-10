using UnityEngine;
using System.Collections;

public class Tile {

	public GameObject gameObject;
	public int x, y;

	public int height = 0;
	public int weight = 1;


	public Tile(int x, int y) {
		this.x = x;
		this.y = y;		
	}


}
