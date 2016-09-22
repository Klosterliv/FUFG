using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseInput : MonoBehaviour {

	public static MouseInput instance;

	public Unit controlled;
	public LayerMask mouseLayer;
	public Tile hoverOver;

	Ctrl ctrl;
	Action.Type action;
	List<Action> turnActions;

	int moves = 0;

	MouseOverType mouseOver = MouseOverType.None;
	bool mouseHit = false;
	Vector3 mousePoint = Vector3.zero;
	Unit hoverUnit;
	Unit targetUnit;
	ActionButton activated;


	// pathing & route
	float[,] wts;
	public int range = 6;
	List<Tile> route;


	// graphics etc
	int moveOrder = 0; // unit move destination order in route chain
	bool unitMoving = false;
	//List<Tile> moveRoute;
	float moveT = 0;
	public float animSpeed = 10;
	public GameObject moveOutlineObject;
	public GameObject previewPrefab;
	public GameObject mouseOverPrefab;
	/*List<GameObject> moveOutline;
	List<GameObject> guardZones;
	List<Tile> previewTargets;
	List<GameObject> previewObjects;
	LineRenderer routeRenderer;
	GameObject mouseOverObject;*/
	string tooltipText = "";
	bool waitingForAnim = false;
	Vector3 tempOrientation;


	public enum Ctrl {
		Move,
		Orient,
		Ability,
	};
	public enum MouseOverType {
		None,
		Unit,
		Tile,
	};
	/*
	public enum ActionType {
		Rush,
		Move,
		Orient,
		Strike,
		Ability,
	};*/


    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this as MouseInput;
        }            
        else Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {

		turnActions = new List<Action>();

		route = new List<Tile>();
		//moveRoute = new List<Tile>();
		//moveOutline = new List<GameObject>();
		//previewTargets = new List<Tile>();
		//previewObjects = new List<GameObject>();
		//routeRenderer = GetComponent<LineRenderer>();

		//mouseOverObject = (GameObject) Instantiate(mouseOverPrefab, transform.position, Quaternion.identity);
		//mouseOverObject.GetComponentInChildren<Renderer>().material.SetColor("_TintColor",Color.green);
		//mouseOverObject.SetActive(false);

		tempOrientation = Vector3.zero;

	}


	void MouseRay() {

		if (EventSystem.current.IsPointerOverGameObject()) {
			mouseHit = false;
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99999, mouseLayer)) {

			string hitTag = hit.collider.tag;

			switch (hitTag) {

			case "Unit":
				mouseOver = MouseOverType.Unit;
				break;
			case "Tile":
				mouseOver = MouseOverType.Tile;
				break;
			default: 
				mouseOver = MouseOverType.None;
				break;
			}

			switch (mouseOver) {

			case MouseOverType.Unit: 
				{
					if (ctrl == Ctrl.Move) {
						mouseHit = true;
						hoverUnit = hit.transform.GetComponentInParent<Unit>();
						hoverOver = hoverUnit.tile;
						if (hoverUnit != controlled) {
							Tile closest = FindClosestAdjacent(hoverUnit.tile);
							if (closest != null) {
								if (wts[closest.x,closest.y] > 0 && moves > 1) {
									route = FindRoute(closest, controlled.tile, closest.grid);
									tooltipText = "Rush: "+wts[closest.x,closest.y];
								}
								else if (closest == controlled.tile) {
									tooltipText = "Strike";
								}
							}
						}
						break;
					}
					else if (ctrl == Ctrl.Ability) {
						mouseHit = true;
						hoverUnit = hit.transform.GetComponentInParent<Unit>();
						hoverOver = hoverUnit.tile;
						if (hoverUnit != controlled) {
							// in range
							//if (hoverUnit.tile.grid.GetWeight(controlled.tile,hoverUnit.tile) <= activated.ability.range) {
							float dist = hoverUnit.tile.grid.GetDistance(controlled.tile,hoverUnit.tile);
							if (dist <= activated.ability.range) {
								tooltipText = (activated.ability.name+", distance: "+Mathf.Round(dist)+"/"+activated.ability.range);
							}
							else {
								tooltipText = "too far ("+Mathf.Round(dist)+"/"+activated.ability.range+")";
							}
						}
						break;
					}
					else if (ctrl == Ctrl.Orient) {
						mouseHit = true;
						hoverUnit = hit.transform.GetComponentInParent<Unit>();
						hoverOver = hoverUnit.tile;
						UI.instance.outlines.DrawMouseOver(hoverOver);
						if (hoverUnit != controlled) {

						}
						break;
					}
					else {
						Debug.LogError("not viable");
						break;
					}

				}


			case MouseOverType.Tile:
				{
					mouseHit = true;

					Grid grid = hit.transform.parent.GetComponent<Grid>();
					Tile t = grid.GetTile(hit.point);

					if (hoverOver != t) {
						hoverOver = t;
						if (controlled != null && ctrl == Ctrl.Move) {
							//TODO:: THIS IS SHIT WTF
							//FindPathing(controlled.tile,grid,range);
							route = FindRoute(t,controlled.tile,grid);
							//DrawOutline();
							UI.instance.outlines.DrawMouseOver(hoverOver);
							//DrawMouseOver();
						}
					}
					break;
				}
					

			case MouseOverType.None:
				{
					hoverOver = null;
					//mouseOverObject.SetActive(false);
					mouseHit = false;
					break;
				}

			}
				
			mousePoint = hit.point;

		}
		else { 
			mouseOver = MouseOverType.None;
			hoverOver = null;
			//mouseOverObject.SetActive(false);
			mouseHit = false;
		}

		
	}

	void Click() {

		switch (ctrl) {
		case Ctrl.Move:
			if (route.Count > 1) {
				controlled.ClearGuardTargets();
				if(mouseOver == MouseOverType.Unit) {
					//TODO:: bad!! ugly!! YUCK
					//List<Tile> nbrs = hoverUnit.tile.grid.GetNeighbours(hoverUnit.tile);
					//if (nbrs.Contains(controlled.tile)) {
						// STRIKE!!!
					//	controlled.Orient(new Vector3(targetUnit.tile.x-controlled.tile.x, 0, targetUnit.tile.y-controlled.tile.y));
					//	targetUnit.Strike(controlled.tile, controlled.strength*1.2f, controlled.strength*0.5f);
					//	moves-=1;
					//}

					controlled.SetGuard(0.2f);

					Mover.instance.AddMoving(new Moving(controlled, route, wts[route[0].x,route[0].y], controlled.rushAbility, hoverUnit));
					moves-=2;

					action = Action.Type.Rush;

					if (wts[route[0].x,route[0].y] > range) {
						// Run
						moves-=2;
					}
					else {
						// Move
						moves-=1;
					}

					waitingForAnim = true;
					//targetUnit = hoverUnit;
				}
				else if(mouseOver == MouseOverType.Tile) {
					Mover.instance.AddMoving(new Moving(controlled, route, wts[route[0].x,route[0].y]));

					action = Action.Type.Move;

					if (wts[route[0].x,route[0].y] > range) {
						// Run
						moves-=2;
						controlled.SetGuard(0.2f);
					}
					else {
						// Move
						moves-=1;
						controlled.SetGuard(0.5f);
					}

					waitingForAnim = true;

				}

				//Debug.Log(wts[route[0].x,route[0].y]);


				//moveRoute = route;
				//unitMoving = true;
				//moveOrder = moveRoute.Count-1;
			}
			break;
		case Ctrl.Orient:

			if (mouseOver == MouseOverType.Unit) {

				//TODO : bad.
				List<Tile> nbrs = hoverUnit.tile.grid.GetNeighbours(hoverUnit.tile);
				if (!nbrs.Contains(controlled.tile)) return;
				// ................... //

				// ................... //
				action = Action.Type.Ability;
				targetUnit = hoverUnit;

				controlled.Orient(new Vector3(targetUnit.tile.x-controlled.tile.x, 0, targetUnit.tile.y-controlled.tile.y));
				targetUnit.Strike(controlled.tile, controlled.strength*1.2f, controlled.strength*0.5f);
				moves-=1;

			}
			else action = Action.Type.Orient;

			OrientUnit();
			break;
		case Ctrl.Ability:
			if (mouseOver == MouseOverType.Unit) { 
				if (activated.ability.IsValidTarget(controlled, hoverUnit)) {
					// in range
					if (hoverUnit.tile.grid.GetDistance(controlled.tile,hoverUnit.tile) <= activated.ability.range) {
						action = Action.Type.Ability;
						activated.Use(hoverUnit);
						moves-=1;
						//Skip();
						controlled.ClearGuardTargets();
						ctrl = Ctrl.Move;
					}
				}
			}
			break;
		}



	}
	
	// Update is called once per frame
	void Update () {

		if (Mover.instance.busy) {
			//MoveUnit();
			//ClearRoute();
			//ClearOutline();
			return;
		}
		else if (waitingForAnim) {
			waitingForAnim = false;
			if (moves > 0) {
				FindPathing(controlled.tile,controlled.tile.grid,range);
			}
			else {
				Skip();
			}
		}
		if (moves < 1) Skip();
		if (Input.GetButtonDown("Skip")) Skip();

		if ((Input.GetMouseButton(1))) {
			ctrl = Ctrl.Orient;
			//UI.instance.outlines.DrawPreviews(previewTargets);
			//DrawPreviews();
			//ClearOutline();
			//ClearRoute();
		}
		else {
			//ClearPreviews();
			if (ctrl == Ctrl.Orient) {
				//DrawOutline();
				controlled.OrientVisual(controlled.facing);
				//Debug.Log(controlled.facing);
				ctrl = Ctrl.Move;			
			}

		}			

		switch (ctrl) {
		case Ctrl.Move:
			MouseRay();
			if (mouseHit) {
				if (Input.GetMouseButtonDown(0)) {
					Click();
				}
				//if (wts != null) DrawOutline();
				UI.instance.outlines.DrawRoute(route);
				UI.instance.outlines.DrawOutline(wts, range, controlled.tile.grid, moves);
				//DrawRoute();
			}
			break;
		case Ctrl.Orient:
			tooltipText = "Orient";
			MouseRay();
			if (mouseHit) {
				//tooltipText = "Orient";
				Vector3 orientation = GetOrientation();
				controlled.OrientVisual(orientation);
				//previewTargets.Clear();
				UI.instance.outlines.DrawPreviews(controlled.GetGuardTargets(orientation));
				//previewTargets.AddRange(controlled.GetGuardTargets(orientation));
				//GetOrientation();
				if (Input.GetMouseButtonDown(0)) {
					Click();
				}
			}
			else tooltipText = "";
			break;
		case Ctrl.Ability: 
			MouseRay();
			if (mouseHit) {
				if (Input.GetMouseButtonDown(0)) {
					Click();
				}
			}
			//if (wts != null) DrawOutline();
			UI.instance.outlines.DrawRoute(route);
			//ClearOutline();
			//ClearRoute();
			break;
		default:
			Debug.Log("unknown");
			break;
		}

		UI.instance.UpdateActionPreviewTooltip(mousePoint, tooltipText);


		// (sketch....)

		// controlled 
		// raycast - find hoverover
		// click to move
		// when moved - click to turn
		// ...
		// next turn


	}

	Tile FindClosestAdjacent(Tile tile) {

		List<Tile> neighbours = tile.grid.GetNeighbours(tile);
		float w = 99999;
		Tile closest = null;

		foreach (Tile n in neighbours) {
			if (wts[n.x,n.y] < w) {
				closest = n;
				w=wts[n.x,n.y];
			}				
		}
		return closest;

	}

	void FindPathing (Tile start, Grid grid, int range) {
		//Debug.Log("find pathing");
		int iterator = 0;
		if(moves>1) range*=2;

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

		UI.instance.outlines.DrawOutline(wts, range, controlled.tile.grid, moves);
		//DrawOutline();
			

	}

	List<Tile> FindRoute (Tile target, Tile start, Grid grid) {

   		List<Tile> newRoute = new List<Tile>();

		int mRange = range;
		if (moves>1) {
			mRange *= 2;

		}
		if (wts[target.x,target.y] > mRange) {
			newRoute.Add(start);
			tooltipText = "...";
			return newRoute;
		}

		if (wts[target.x,target.y] > range) tooltipText = "Sprint: "+wts[target.x,target.y];
		else tooltipText = "Move: "+wts[target.x,target.y];

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
					//Debug.Log("i:"+iterator);
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

	public void SetControlled(Unit unit) {
		controlled = unit;
		range = unit.moveSpeed;
		ctrl = Ctrl.Move;
		UI.instance.UpdateButtons(controlled.abilities, unit);
		moves = 2;
		controlled.ClearGuardTargets();
		FindPathing(controlled.tile,controlled.tile.grid,range);

		//DrawWeights();
	}

	void OrientUnit() {
		controlled.Orient(GetOrientation());
		controlled.ApplyGuard();
		moves-=1;
		//Skip();
	}
	Vector3 GetOrientation() {

		Tile ct = controlled.tile;

		Vector3 unitToMouseDir = new Vector3(hoverOver.x-ct.x,0,hoverOver.y-ct.y);

		// vector down to 8dir
		float angle = Mathf.Atan2( unitToMouseDir.x, unitToMouseDir.z );
		int octant = Mathf.RoundToInt( 8 * angle / (2*Mathf.PI) + 8 ) % 8;

		Vector3 fDir = Vector3.zero;

		// 8dir into vector
		switch (octant) {
		case 0:
			fDir = new Vector3(0,0,1);
			break;	
		case 1:
			fDir = new Vector3(1,0,1);
			break;
		case 2:
			fDir = new Vector3(1,0,0);
			break;
		case 3:
			fDir = new Vector3(1,0,-1);
			break;
		case 4:
			fDir = new Vector3(0,0,-1);
			break;
		case 5:
			fDir = new Vector3(-1,0,-1);
			break;
		case 6:
			fDir = new Vector3(-1,0,0);
			break;
		case 7:
			fDir = new Vector3(-1,0,1);
			break;

		}

		Debug.DrawRay(controlled.transform.position, fDir*5, Color.green);
		return fDir;
		
	}
	public void Skip () {
		hoverOver = null;
		// TODO : FIX ALL THIS MB
		controlled.delay += controlled.actionDelay;
		TurnHandler.instance.TimeStep();
	}
	void CommitAction() {

		//DrawOutline();		
	}



	public void ActivateAbility(ActionButton button) {
		activated = button;
		ctrl = Ctrl.Ability;
	}

}
