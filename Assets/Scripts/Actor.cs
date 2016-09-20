using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
	public bool controllable = false;
	public bool timeStep = true;

    public float delay;
    public float actionDelay;
    public Tile tile;

    public int x, y;

    void Start()
    {
        tile = FindObjectOfType<Grid>().grid[x, y];
		//gameObject.transform.position = tile.gameObject.transform.position;
		Initialize();
    }
    public virtual void Act() {
        Debug.Log("Actor doing Actor things");
        delay = actionDelay;
    }
	public virtual void Initialize() {
		
	}
}
