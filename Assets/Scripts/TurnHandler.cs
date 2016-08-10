using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnHandler : MonoBehaviour {

    List<Actor> actors;

    public int factionTurn;
    public float timeCounter = 0;
    
    // Use this for initialization
    void Start () {
        factionTurn = 0;
        actors = new List<Actor>();

        actors[0].Act();
    }
        
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
