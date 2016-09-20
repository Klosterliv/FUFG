using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public RectTransform canvas;
	public RectTransform actionBar;
	public RectTransform healthBarParent;
	public RectTransform turnOrderParent;
	public RectTransform actionPreviewTooltip;

	public GameObject buttonPrefab;
	public GameObject healthBarPrefab;
	public GameObject turnOrderPrefab;

	public Camera uiCam;



	// // // // // // // // //
	public static UI instance;

	List<GameObject> actionButtons;
	List<HealthBar> healthBars;
	List<TurnOrderIcon> turnOrderIcons;

	void Awake () {
		if (instance == null) {
			instance = this as UI;
			DontDestroyOnLoad(gameObject);
		}			
		else Destroy(gameObject);

		if (actionButtons == null)
			actionButtons = new List<GameObject>();
		if (healthBars == null)
			healthBars = new List<HealthBar>();
		if (turnOrderIcons == null)
			turnOrderIcons = new List<TurnOrderIcon>();	
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		UpdateHealthBars();
		UpdateTurnOrder();
	
	}
	public void SkipClick () {
		MouseInput.instance.Skip();
	}

	public void UpdateButtons (List<Ability> abilities, Unit unit) {

		//Debug.Log("updating buttons, "+abilities.Count+" (amount)");

		actionButtons.ForEach(Destroy);
		actionButtons.Clear();

		foreach (Ability ability in abilities) {
			GameObject btn = (GameObject)Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
			Text txt = btn.GetComponentInChildren<Text>();
			btn.transform.SetParent(actionBar,false);
			btn.transform.localScale = Vector3.one;
			btn.transform.rotation = actionBar.rotation;
			txt.text = ability.name;
			btn.GetComponent<ActionButton>().ability = ability;
			btn.GetComponent<ActionButton>().user = unit;
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
			healthBar.guardText.text = (healthBar.unit.guardPercent*100)+"%";
			Vector2 viewportPoint;// = Camera.main.WorldToScreenPoint(healthBar.unit.transform.position);
			RectTransform rectTransform = healthBar.canvasObject.GetComponent<RectTransform>();
			//viewportPoint.x -= (canvas.sizeDelta.x * canvas.pivot.x);
			//rectTransform.anchoredPosition = viewportPoint;

			viewportPoint = WorldToCanvasPosition(canvas, Camera.main, healthBar.unit.transform.position+Vector3.up*5f);
			rectTransform.anchoredPosition = viewportPoint;
		}
		
	}
	public void UpdateTurnOrder (List<Actor> actors) {
		//int maxLength = 6;
		int o = 0;
		foreach (Actor a in actors) {
			// TODO :: maybe is bad, but perhaps show some actors as visible in turn order without control
			if (a.GetType() != typeof(Unit)) continue;
			Unit u = a as Unit;

			// TODO :: All this is crap
			bool found = false;
			foreach (TurnOrderIcon icon in turnOrderIcons) {

				if (icon.unit == u) {
					icon.turn = o;
					icon.icon.SetSiblingIndex(actors.Count-o);
					icon.Update();
					found = true;
				}
			}
			if (!found) {
				GameObject iconObj = (GameObject) Instantiate(turnOrderPrefab);
				TurnOrderIcon reqIcon = new TurnOrderIcon(turnOrderParent, iconObj.GetComponent<RectTransform>(), u);
				reqIcon.icon.SetParent(turnOrderParent, false);
				reqIcon.icon.SetSiblingIndex(actors.Count-o);
				turnOrderIcons.Add(reqIcon);
				reqIcon.turn = o;
				reqIcon.Update();
			}
			o++;
		}
	}
	void UpdateTurnOrder () {
		foreach (TurnOrderIcon icon in turnOrderIcons) {
			//icon.icon.position = Vector2.Lerp(icon.icon.position, icon.desiredOffset, Time.deltaTime*15);
			icon.icon.anchoredPosition = Vector2.Lerp(icon.icon.anchoredPosition, icon.desiredOffset, Time.deltaTime*15);
			if (icon.turn == 0) {
				icon.icon.localScale = Vector3.Lerp(icon.icon.localScale, Vector3.one*1.4f, Time.deltaTime*3);
			}
			else icon.icon.localScale = Vector3.Lerp(icon.icon.localScale, Vector3.one, Time.deltaTime*3);

			//icon.icon.localPosition = Vector2.Lerp(icon.icon.localPosition, icon.desiredOffset, Time.deltaTime*15);
			//icon.icon.position = Vector3.zero;
			//icon.icon.localScale = Vector3.one;

			//icon.icon.sizeDelta = Vector3.one;
		}

	}

	public void UpdateActionPreviewTooltip (Vector3 mousePos, string text) {
		actionPreviewTooltip.anchoredPosition = WorldToCanvasPosition(canvas, Camera.main, mousePos);
		actionPreviewTooltip.GetComponentInChildren<Text>().text = text;
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

	/*
	void DrawWeights (float[,] wts) {
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
	void DrawRoute (List<Tile> route) {
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
	void DrawOutline (float[,] wts) {
		ClearOutline();
		for (int x = 0; x < wts.GetLength(0); x++) {
			for (int y = 0; y < wts.GetLength(1); y++) {
				if (wts[x,y] <= range) {
					GameObject obj = (GameObject) Instantiate(moveOutlineObject, new Vector3(x,0.6f,y), Quaternion.identity);
					moveOutline.Add(obj);
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
		mouseOverObject.transform.position = hoverOver.gameObject.transform.position + Vector3.up*0.7f;
		mouseOverObject.SetActive(true);
	}
	void ClearRoute () {
		routeRenderer.SetVertexCount(0);
	}
	void ClearOutline () {
		moveOutline.ForEach(Destroy);
	}
	*/

}

public class HealthBar {

	public Unit unit;
	public GameObject canvasObject;
	public Image bar;
	public Text guardText;

	public HealthBar (Unit unit, GameObject uiObj) {
		this.unit = unit;
		canvasObject = uiObj;
		bar = (Image) canvasObject.GetComponentInChildren<Image>();
		guardText = (Text) canvasObject.GetComponentInChildren<Text>();
	}

}

public class TurnOrderIcon {

	public Unit unit;

	public RectTransform parent;
	public RectTransform icon;
	public Text delayText;
	public Text nameText;
	public Vector2 desiredOffset;
	public int turn;

	public TurnOrderIcon (RectTransform parent, RectTransform icon, Unit unit) {

		this.parent = parent;
		this.icon = icon;
		this.unit = unit;

		nameText = icon.GetComponentInChildren<Text>();
		Text[] texts = icon.GetComponentsInChildren<Text>();
		for (int i = 0; i < texts.Length; i++) {
			if (texts[i].name == "Name") nameText = texts[i];
			else if (texts[i].name == "Delay") delayText = texts[i];
		}

	}
	public void Update () {
		delayText.text = unit.delay.ToString("0.00");
		nameText.text = unit.name;
		//icon.SetSiblingIndex(turn);
		desiredOffset = new Vector2(0, turn*55+25);

	}

}
