using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mover : MonoBehaviour {

	public float animationSpeed = 1;

	public static Mover instance;

	public bool busy = false;

	List<Moving> moving;

	void Awake() {
		if (instance == null) {
			instance = this as Mover;
			DontDestroyOnLoad(this);
		}
		else Destroy(gameObject);			
		
	}

	// Use this for initialization
	void Start () {
		moving = new List<Moving>();	
	}
	
	// Update is called once per frame
	void Update () {

		if (moving.Count < 1) {
			busy = false;
		}
		else {
			moving.ForEach(MoveUnit);
		}
	
	}
	public void AddMoving(Moving m) {

		moving.Add(m);
		if (m.pushed) m.unit.animator.SetBool("Moving", false);
		else m.unit.animator.SetBool("Moving", true);
		busy = true;
		
	}
	//public void

	void PushUnit (Moving m) {

		if (m.pushedTo == null) {
			if (m.moveOrder < 1) {
				PushComplete (m);
				Debug.Log("push done");
			}
			else {
				Tile next = m.unit.tile.grid.GetTile(m.unit.tile.gameObject.transform.position+m.pushDir);
				if (!next.occupied)
					m.pushedTo = next;
				else {
					PushComplete(m);
					Debug.Log("pushed into "+next.occupier.name);
				}
			}
		}
		else {
			
			Vector3 posFrom = m.unit.gameObject.transform.position;
			Vector3 posTo = m.pushedTo.gameObject.transform.position;

			m.unit.gameObject.transform.position = Vector3.Lerp(posFrom, posTo, m.moveT);

			if (m.moveT > .8f) {			
				if (m.moveOrder > 1) {
					m.moveOrder--;
					m.unit.Pushed(m.pushedTo);
					Tile next = m.unit.tile.grid.GetTile(m.unit.tile.gameObject.transform.position+m.pushDir);
					if (!next.occupied)
						m.pushedTo = next;
					else {
						PushComplete(m);
						Debug.Log("pushed into "+next.occupier.name);
					}
					m.moveT = 0;
				}
				else if (m.moveT >= 1) {
					PushComplete (m);
				}
			}
			m.moveT += (Time.deltaTime*animationSpeed);
		}

	}

	void MoveUnit (Moving m) {

		// TODO: something else
		if (m.pushed) {
			PushUnit(m);
			return;
		}
		
		Vector3 posFrom = m.unit.gameObject.transform.position;
		Vector3 posTo = m.path[m.moveOrder].gameObject.transform.position;

		m.unit.gameObject.transform.position = Vector3.Lerp(posFrom, posTo, m.moveT);
		//controlled.Orient(new Vector3(route[route.Count-2].x-route[route.Count-1].x,0, route[route.Count-1].y-route[route.Count-2].y));
		//controlled.Orient(new Vector3(route[moveOrder-1].x-route[moveOrder].x,0, route[moveOrder-1].y-route[moveOrder].y));
		if (m.moveT > .8f) {			
			if (m.moveOrder > 0) {
				m.unit.Orient(new Vector3(m.path[m.moveOrder-1].x-m.path[m.moveOrder].x,0, m.path[m.moveOrder-1].y-m.path[m.moveOrder].y));
				m.moveOrder--;
				m.moveT = 0;
			}
			else if (m.moveT >= 1) {
				m.unit.Orient(new Vector3(m.path[0].x-m.path[1].x, 0, m.path[0].y-m.path[1].y));
				MoveComplete (m);
			}
		}
		m.moveT += (Time.deltaTime*animationSpeed);
	}
	void PushComplete (Moving m) {
		
		m.unit.Pushed(m.pushedTo);

		if (m.endAbility != null) {
			m.unit.Orient(new Vector3(m.target.tile.x-m.unit.tile.x, 0, m.target.tile.y-m.unit.tile.y));
			m.endAbility.Use(m.unit, m.target);
			m.unit.animator.SetBool("Attack",true);
		}

		moving.Remove(m);

	}
	void MoveComplete (Moving m) {

		m.unit.Moved(m.weight, m.path[0]);
		m.unit.animator.SetBool("Moving", false);
		//unitMoving = false;
		//hoverOver = null;
		//TurnHandler.instance.TimeStep();

		// if RUSH
		if (m.endAbility != null) {
			m.unit.Orient(new Vector3(m.target.tile.x-m.unit.tile.x, 0, m.target.tile.y-m.unit.tile.y));
			m.endAbility.Use(m.unit, m.target);
			m.unit.animator.SetBool("Attack",true);
		}

		moving.Remove(m);
		/*
		switch (action) {
		case Action.Type.Rush:
			m.unit.Orient(new Vector3(targetUnit.tile.x-controlled.tile.x, 0, targetUnit.tile.y-controlled.tile.y));
			targetUnit.Strike(controlled.tile, controlled.strength*1.2f, controlled.strength*0.5f);
			moves-=1;
			break;
		}

		if (moves > 0) {
			Debug.Log("findagain");
			FindPathing(controlled.tile,controlled.tile.grid,range);
			ctrl = Ctrl.Orient;
		}
		else {
			Skip();
		}*/
	}

}

public class Moving {

	public bool pushed = false;
	public Tile pushedTo;

	public Ability endAbility;
	public Unit target;

	public Vector3 pushDir;
	public List<Tile> path;
	public Unit unit;

	public int moveOrder;
	public float moveT = 0;

	public float weight = 0;

	public Moving (Unit unit, List<Tile> path, float weight) {
		this.path = path;
		this.unit = unit;
		this.weight = weight;

		moveT = 0;
		moveOrder = path.Count-1;
	}
	public Moving (Unit unit, List<Tile> path, float weight, Ability endAbility, Unit target) {
		this.path = path;
		this.unit = unit;
		this.weight = weight;
		moveT = 0;
		moveOrder = path.Count-1;

		this.endAbility = endAbility;
		this.target = target;
	}
	public Moving (Unit unit, Vector3 pushDir, int distance) {
		pushed = true;
		this.unit = unit;
		this.pushDir = pushDir.normalized;
		moveOrder = distance;
		moveT = 0;
	}
	
}
