using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public Transform canvas;
	public RectTransform actionBar;

	public GameObject buttonPrefab;



	// // // // // // // // //
	public static UI instance;

	List<GameObject> actionButtons;
	void Awake () {
		if (instance == null) {
			instance = this as UI;
			DontDestroyOnLoad(gameObject);
		}			
		else Destroy(gameObject);
	}

	// Use this for initialization
	void Start () {

		actionButtons = new List<GameObject>();
	
	}
	
	// Update is called once per frame
	void Update () {


	
	}

	public void UpdateButtons (List<Ability> abilities) {

		Debug.Log("updating buttons, "+abilities.Count+" (amount)");

		actionButtons.ForEach(Destroy);
		actionButtons.Clear();

		foreach (Ability ability in abilities) {
			GameObject btn = (GameObject)Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
			Text txt = btn.GetComponentInChildren<Text>();
			btn.transform.SetParent(actionBar);
			btn.transform.localScale = Vector3.one;
			btn.transform.rotation = actionBar.rotation;
			txt.text = ability.name;
			actionButtons.Add(btn);
		}
		
	}
}
