using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnHandler : MonoBehaviour {

    List<Actor> actors;
    public int factionTurn;
    public float timeCounter = 0;
    public bool initCombat = true; //Switch to false after implementation of exploration/non-combat mode (Optional stuff)
    public bool combatMode = false;


	public static TurnHandler instance;
	void Awake() {
		if (instance == null) {
			DontDestroyOnLoad(gameObject);
			instance = this as TurnHandler;
		}            
		else Destroy(gameObject);
	}

    
    // Use this for initialization
    void Start () {
        factionTurn = 0;
        if (initCombat == true)
        {
            //Debug.Log("Initiating Combat");
            InitiateCombat();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("b") == true)
        {
			TimeStep();
        }
    }
    void DebugCheck()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            Debug.Log("Actorlist: " + actors[i] + "Delay = " + actors[i].delay);
        }
        //Debug.Log("Current Actor is " + actors[0].name);
    }
    void InitiateCombat()
    {
        if(combatMode == false)
        {
            combatMode = true;
            initCombat = false;
            actors = new List<Actor>();
            actors.AddRange(FindObjectsOfType<Actor>());
            actors = actors.OrderBy(d => d.delay).ToList();
            //DebugCheck();
        }
    }

	public void TimeStep () {
		if (combatMode)
		{
			actors = actors.OrderBy(d => d.delay).ToList(); //Listsortering med Linq
			float timestep = actors[0].delay;
			foreach (Actor a in actors)
			{
				a.delay -= timestep;
			}
			//Debug.Log("\n Object " + actors[0].name + " acting");

			actors[0].Act();
			//DebugCheck();
		}
	}
		
}
