using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseInput : MonoBehaviour {

	public Unit controlled;
	public LayerMask mouseLayer;
	public Tile hoverOver;


	float[,] wts;

	public int range = 6;

    public static MouseInput instance;

	List<Tile> route;


    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this as MouseInput;
        }            
        else Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {

		route = new List<Tile>();
	
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99999, mouseLayer)) {
			if (hit.transform.parent.name == "Grid") {

				Grid grid = hit.transform.parent.GetComponent<Grid>();
				Tile t = grid.GetTile(hit.point);

				if (hoverOver != t) {
					FindPathing(controlled.tile,t,grid,range);
					route = FindRoute(t,controlled.tile,grid);
					hoverOver = t;
				}
			}
		}
		else hoverOver = null;

		if (wts != null) DrawWeights();
		DrawRoute();
	}
	void DrawWeights () {
		Vector3 offset = new Vector3(-0.5f,1.1f,-0.5f);
		for (int x = 0; x < wts.GetLength(0); x++) {
			for (int y = 0; y < wts.GetLength(1); y++) {
				Debug.DrawLine(new Vector3(x,0,y)+offset,new Vector3(x+1,0,y+1)+offset, Color.Lerp(Color.green,Color.red,(int)wts[x,y]/range));
				Debug.DrawLine(new Vector3(x,0,y+1)+offset,new Vector3(x+1,0,y)+offset, Color.Lerp(Color.green,Color.red,(int)wts[x,y]/range));
			}
		}

	}
	void DrawRoute () {
		Debug.Log(route.Count);
		for (int i = 0; i < route.Count-1; i++) {
			Debug.DrawLine(new Vector3(route[i].x, 1.1f, route[i].y), new Vector3(route[i+1].x, 1.1f, route[i+1].y), Color.blue);
		}
	}



	void FindPathing (Tile start, Tile end, Grid grid, int range) {

		int iterator = 0;

		float[,] weights = new float[grid.xSize,grid.ySize];
		for (int x = 0; x < weights.GetLength(0); x++) {
			for (int y = 0; y < weights.GetLength(1); y++) {
				weights[x,y] = 9999999;
			}
		}
		List<Tile> scanned = new List<Tile>();
		List<Tile> queue = new List<Tile>();

		queue.Add(start);
		weights[start.x,start.y] = 0;

		while (queue.Count > 0) {
			iterator++;
			if(iterator>=9999) return;

			Tile t = queue[0];
			 //grid.grid[queue[0].x,queue[0].y]
			List<Tile> neighbours =	grid.GetNeighbours(t);

			foreach (Tile neighbour in neighbours) {
				float w = weights[t.x,t.y] + grid.GetWeight(t,neighbour);//neighbour.weight + weights[t.x,t.y];
				if (w < weights[neighbour.x,neighbour.y] && w <= range) {
					weights[neighbour.x,neighbour.y] = w;
					if (!scanned.Contains(neighbour)) {
						scanned.Add(neighbour);
						queue.Add(neighbour);
					}
				}

			}

			queue.RemoveAt(0);
			
		}
		wts = weights;
			

	}

	List<Tile> FindRoute (Tile target, Tile start, Grid grid) {

   		List<Tile> newRoute = new List<Tile>();

		if (wts[target.x,target.y] > range) {
			newRoute.Add(start);
			return newRoute;
		}

		newRoute.Add(target);

		bool found = false;

		int iterator = 0;
		while (!found) {
			iterator++;
			if (iterator > 9999) break;

			List<Tile> neighbours = grid.GetNeighbours(newRoute[newRoute.Count-1]);
			float bestW = 99999;
			Tile bestTile = start;
			foreach (Tile n in neighbours) {
				if (n==start) {
					found = true;
					newRoute.Add(n);
				}
				if (wts[n.x,n.y] < bestW) {
					bestW = wts[n.x,n.y];
					bestTile = n;
				}
			}
			newRoute.Add(bestTile);
		}
		return newRoute;

	}

}
