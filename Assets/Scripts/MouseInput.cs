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


	// unit move destination order in route chain
	int moveOrder = 0;
	bool unitMoving = false;
	List<Tile> moveRoute;
	float moveT = 0;
	public float animSpeed = 10;


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
		moveRoute = new List<Tile>();
	
	}

	void Click() {

		if (route.Count > 1) {
			moveRoute = route;
			unitMoving = true;
			moveOrder = moveRoute.Count-1;
		}

	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		if (!unitMoving) {
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99999, mouseLayer)) {
				if (hit.transform.parent.name == "Grid") {

					Grid grid = hit.transform.parent.GetComponent<Grid>();
					Tile t = grid.GetTile(hit.point);

					if (hoverOver != t) {
						hoverOver = t;
						if (controlled != null) {
							FindPathing(controlled.tile,t,grid,range);
							route = FindRoute(t,controlled.tile,grid);
						}
					}
					if (Input.GetMouseButtonDown(0)) {
						Click();
					}
				}
			}
			else hoverOver = null;

			if (wts != null) DrawWeights();
			DrawRoute();
		}
		else {
			MoveUnit();
		}

	}
	void DrawWeights () {
		Vector3 offset = new Vector3(-0.5f,.5f,-0.5f);
		for (int x = 0; x < wts.GetLength(0); x++) {
			for (int y = 0; y < wts.GetLength(1); y++) {
				float clerp = (int)(wts[x,y]/(range+1));

				Debug.DrawLine(new Vector3(x+.1f,0,y+.1f)+offset,new Vector3(x+.1f,0,y+.9f)+offset, Color.Lerp(Color.green,Color.red,clerp));
				Debug.DrawLine(new Vector3(x+.9f,0,y+.1f)+offset,new Vector3(x+.9f,0,y+.9f)+offset, Color.Lerp(Color.green,Color.red,clerp));
				Debug.DrawLine(new Vector3(x+.1f,0,y+.1f)+offset,new Vector3(x+.9f,0,y+.1f)+offset, Color.Lerp(Color.green,Color.red,clerp));
				Debug.DrawLine(new Vector3(x+.1f,0,y+.9f)+offset,new Vector3(x+.9f,0,y+.9f)+offset, Color.Lerp(Color.green,Color.red,clerp));

			}
		}

	}
	void DrawRoute () {
		//Debug.Log(route.Count);
		if (route.Count > 0) {
			//Debug.Log(wts[route[0].x,route[0].y]);
		}			
		for (int i = 0; i < route.Count-1; i++) {
			Debug.DrawLine(new Vector3(route[i].x, .5f, route[i].y), new Vector3(route[i+1].x, .5f, route[i+1].y), Color.blue);
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

	void MoveUnit() {
		Vector3 posFrom = controlled.gameObject.transform.position;
		Vector3 posTo = route[moveOrder].gameObject.transform.position;

		controlled.gameObject.transform.position = Vector3.Lerp(posFrom, posTo, moveT);
		if (moveT > .8f) {			
			if (moveOrder > 0) {
				moveOrder--;
				moveT = 0;
			}
			else if (moveT >= 1) {
				MoveComplete ();
			}
		}
		moveT += (Time.deltaTime*animSpeed);

	}
	void MoveComplete () {

		Debug.Log("moved");
		controlled.Moved(wts[route[0].x,route[0].y], route[0]);
		unitMoving = false;
		hoverOver = null;
		TurnHandler.instance.TimeStep();
		
	}
	public void SetControlled(Unit unit) {
		controlled = unit;
		range = unit.moveSpeed;
	}

}
