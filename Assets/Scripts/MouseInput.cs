using UnityEngine;
using System.Collections;

public class MouseInput : MonoBehaviour {


	public LayerMask mouseLayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99999, mouseLayer)) {
			if (hit.transform.parent.name == "Grid") {

				Grid grid = hit.transform.parent.GetComponent<Grid>();
				Tile t = grid.GetTile(hit.point);
				Debug.Log(t.x+":"+t.y);
				
			}
		}
	}

}
