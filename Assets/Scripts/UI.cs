using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public RectTransform canvas;
	public RectTransform actionBar;
	public RectTransform healthBarParent;

	public GameObject buttonPrefab;
	public GameObject healthBarPrefab;

	public Camera uiCam;



	// // // // // // // // //
	public static UI instance;

	List<GameObject> actionButtons;
	List<HealthBar> healthBars;

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
		if (healthBars == null)
			healthBars = new List<HealthBar>();
	
	}
	
	// Update is called once per frame
	void Update () {

		UpdateHealthBars();


	
	}

	public void UpdateButtons (List<Ability> abilities) {

		Debug.Log("updating buttons, "+abilities.Count+" (amount)");

		actionButtons.ForEach(Destroy);
		actionButtons.Clear();

		foreach (Ability ability in abilities) {
			GameObject btn = (GameObject)Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
			Text txt = btn.GetComponentInChildren<Text>();
			btn.transform.SetParent(actionBar, false);
			btn.transform.localScale = Vector3.one;
			btn.transform.rotation = actionBar.rotation;
			txt.text = ability.name;
			actionButtons.Add(btn);
		}
		
	}
	public void AddHealthBar(Unit u) {
		GameObject hbObj = (GameObject) Instantiate(healthBarPrefab, healthBarParent, false);
		HealthBar h = new HealthBar(u, hbObj);

		if (healthBars == null)
			healthBars = new List<HealthBar>();
		
		healthBars.Add(h);
	}
	void UpdateHealthBars () {

		foreach (HealthBar healthBar in healthBars) {
			healthBar.bar.fillAmount = healthBar.unit.health/healthBar.unit.maxHealth;
			Vector2 viewportPoint;// = Camera.main.WorldToScreenPoint(healthBar.unit.transform.position);
			RectTransform rectTransform = healthBar.canvasObject.GetComponent<RectTransform>();
			//viewportPoint.x -= (canvas.sizeDelta.x * canvas.pivot.x);
			//rectTransform.anchoredPosition = viewportPoint;

			viewportPoint = WorldToCanvasPosition(canvas, Camera.main, healthBar.unit.transform.position+Vector3.up*5f);
			rectTransform.anchoredPosition = viewportPoint;
		}
		
	}



	private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position) {
		//Vector position (percentage from 0 to 1) considering camera size.
		//For example (0,0) is lower left, middle is (0.5,0.5)
		Vector2 temp = camera.WorldToViewportPoint(position);

		//Calculate position considering our percentage, using our canvas size
		//So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
		temp.x *= canvas.sizeDelta.x;
		temp.y *= canvas.sizeDelta.y;

		//The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
		//But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
		//We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
		//returned value will still be correct.

		temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
		temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

		return temp;
	}
}

public class HealthBar {

	public Unit unit;
	public GameObject canvasObject;
	public Image bar;

	public HealthBar (Unit unit, GameObject uiObj) {
		this.unit = unit;
		canvasObject = uiObj;
		bar = (Image) canvasObject.GetComponentInChildren<Image>();
	}


}
