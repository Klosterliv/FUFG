using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outlines : MonoBehaviour {

	// PREFABS
	public GameObject moveOutlineObject;
	public GameObject mouseOverPrefab;
	public GameObject previewPrefab;

	// RENDER ITEMS
	public LineRenderer routeRenderer;
	List<GameObject> moveOutline;
	GameObject mouseOverObject;
	List<GameObject> previewObjects;

	List<Tile> previewTargets;
	List<Tile> route;
	float[,] wts;
	int moves;
	float range;
	Grid grid;

	// FLAGS
	bool renderOutline = false;
	bool renderRoute = false;
	bool renderMouseOver = false;
	bool renderPreviews = false;

	bool clearOutline = false;
	bool clearRoute = false;
	bool clearMouseOver = false;
	bool clearPreviews = false;

	void Start () {
		mouseOverObject = (GameObject) Instantiate(mouseOverPrefab, transform.position, Quaternion.identity);
		mouseOverObject.SetActive(false);
		routeRenderer = GetComponent<LineRenderer>();
		previewObjects = new List<GameObject>();
		moveOutline = new List<GameObject>();
		previewTargets = new List<Tile>();
		route = new List<Tile>();
	}

	void LateUpdate() {
		
		if (renderOutline)
			DrawOutline();
		if (renderRoute)
			DrawRoute();
		if (renderMouseOver)
			DrawMouseOver();
		if (renderPreviews)
			DrawPreviews();	

		renderOutline = false;
		renderMouseOver = false;
		renderPreviews = false;
		renderRoute = false;

		if (clearRoute)
			ClearRoute();
		if (clearPreviews)
			ClearPreviews();
		//if (clearMouseOver)
		if (clearOutline)
			ClearOutline();

		clearRoute = true;
		clearPreviews = true;
		//clearMouseOver = true;
		clearOutline = true;

	}

	public void DrawOutline (float[,] wts, float range, Grid grid, int moves) {
		clearOutline = false;
		// If not same then redraw
		if (this.range != range || this.wts != wts || this.grid != grid || this.moves != moves || moveOutline.Count < 1) {
			this.range = range;
			this.grid = grid;
			this.moves = moves;
			this.wts = wts;
			renderOutline = true;	
		}	
	}
	public void DrawMouseOver (Tile hoverOver) {
		mouseOverObject.transform.position = hoverOver.gameObject.transform.position + Vector3.up*0.6f;
		renderMouseOver = true;
	}
	public void DrawPreviews (List<Tile> previewTargets) {
		clearPreviews = false;
		// If not same then redraw
		if (this.previewTargets != previewTargets) {
			this.previewTargets = previewTargets;
			renderPreviews = true;
		}
	}
	public void DrawRoute(List<Tile> route) {
		clearRoute = false;
		// If not same then redraw
		if (this.route != route) {
			this.route = route;
			renderRoute = true;
		}
	}
	
	void DrawRoute () {
		//Debug.Log(route.Count);
		if (route.Count > 0) {
			//Debug.Log(wts[route[0].x,route[0].y]);
		}
		ClearRoute();
		routeRenderer.SetVertexCount(route.Count);
		for (int i = 0; i < route.Count; i++) {
			//Debug.DrawLine(new Vector3(route[i].x, .5f, route[i].y), new Vector3(route[i+1].x, .5f, route[i+1].y), Color.blue);
			routeRenderer.SetPosition(i, new Vector3(route[i].x, .6f, route[i].y));
			//routeRenderer.SetPosition(i+1, new Vector3(route[i+1].x, .5f, route[i+1].y));
		}
	}
	void DrawOutline () {
		ClearOutline();
		for (int x = 0; x < wts.GetLength(0); x++) {
			for (int y = 0; y < wts.GetLength(1); y++) {
				//TODO : ultraplaceholder
				if (grid.grid[x,y].effects.Count > 0) {
					GameObject obj = (GameObject) Instantiate(moveOutlineObject, new Vector3(x,0.6f,y), Quaternion.identity);
					moveOutline.Add(obj);
					obj.GetComponentInChildren<Renderer>().material.SetColor("_TintColor",Color.red);
				}
				else if (wts[x,y] <= range) {
					GameObject obj = (GameObject) Instantiate(moveOutlineObject, new Vector3(x,0.6f,y), Quaternion.identity);
					moveOutline.Add(obj);
					if (moves == 1) obj.GetComponentInChildren<Renderer>().material.SetColor("_TintColor",Color.yellow);
				}
				else if (moves>1 && wts[x,y] <= range*2) {
					GameObject obj = (GameObject) Instantiate(moveOutlineObject, new Vector3(x,0.6f,y), Quaternion.identity);
					obj.GetComponentInChildren<Renderer>().material.SetColor("_TintColor",Color.yellow);
					moveOutline.Add(obj);					
				}
			}
		}
	}
	void DrawMouseOver () {
		mouseOverObject.SetActive(true);
	}
	void ClearRoute () {
		routeRenderer.SetVertexCount(0);
	}
	void ClearOutline () {
		moveOutline.ForEach(Destroy);
		moveOutline.Clear();
	}
	void ClearPreviews() {
		previewObjects.ForEach(Destroy);
		previewObjects.Clear();
	}
	void DrawPreviews() {
		ClearPreviews();
		foreach (Tile t in previewTargets) {
			GameObject obj = (GameObject) Instantiate(previewPrefab, new Vector3(t.x,0.6f,t.y), Quaternion.identity);
			previewObjects.Add(obj);
		}
	}
}
