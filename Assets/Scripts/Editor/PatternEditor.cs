using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Pattern)),CanEditMultipleObjects]
public class PatternEditor : Editor {

	[MenuItem("Abilities/Pattern")]
	static void CreatePattern() {
		string path = EditorUtility.SaveFilePanel("Create Pattern", "Assets/Abilities/Patterns","default.asset","asset");

		if (path == "") return;

		path = FileUtil.GetProjectRelativePath(path);

		Pattern p = CreateInstance<Pattern>();
		AssetDatabase.CreateAsset(p, path);
		AssetDatabase.SaveAssets();
	}
		
	// EDITING
	public override void OnInspectorGUI ()
	{
		if (Event.current.type == EventType.Layout) return;

		Rect pos = new Rect(0,50,Screen.width,Screen.height);

		foreach (var item in targets) {
			if (pos.height < EditorGUIUtility.singleLineHeight*2) 
				continue;
			Pattern p = item as Pattern;				
			pos.y+=InspectPattern(pos, p);
		}
	}

	public float InspectPattern(Rect position, Pattern p) {

		float usedHeight = 0;

		int sizeX = EditorGUI.IntField(new Rect(position.x, position.y, position.width*0.5f, EditorGUIUtility.singleLineHeight), "Size X", p.sizeX);

		int sizeY = EditorGUI.IntField(new Rect(position.x + position.width*0.5f, position.y, position.width*0.5f, EditorGUIUtility.singleLineHeight), "Size Y", p.sizeY);

		position.y += EditorGUIUtility.singleLineHeight;

		int originX = EditorGUI.IntField(new Rect(position.x, position.y, position.width*0.5f, EditorGUIUtility.singleLineHeight), "Origin X", p.originX);

		int originY = EditorGUI.IntField(new Rect(position.x + position.width*0.5f, position.y, position.width*0.5f, EditorGUIUtility.singleLineHeight), "Origin Y", p.originY);
		//EditorGUI.IntField(new Rect(position.x, position.y, position.width*0.5, EditorGUIUtility.singleLineHeight), 

		position.y += EditorGUIUtility.singleLineHeight;
		usedHeight += EditorGUIUtility.singleLineHeight;

		if((sizeX != p.sizeX) || (sizeY != p.sizeY)) {
			GUI.changed = true;	
			p.Resize(sizeX,sizeY);
		}
		else GUI.changed = false;

		if((originX != p.originX) || (originY != p.originY)) {
			GUI.changed = true;	
			p.SetOrigin(originX, originY);
		}

		//////////////

		float xWidth = Mathf.Min(position.width / Mathf.Max(1, p.sizeX), position.height / Mathf.Max(1, p.sizeX));
		GUIStyle myFontStyle = new GUIStyle(EditorStyles.textField);
		myFontStyle.fontSize = Mathf.FloorToInt(xWidth * 0.6f);
		GUIStyle originFont = new GUIStyle(EditorStyles.textField);
		originFont.fontSize = Mathf.FloorToInt(xWidth * 0.8f);

		// edit bloxx
		for (int x = 0; x < p.sizeX; x++) {
			for (int y = 0; y < p.sizeY; y++) {

				if (x == originX && y == originY) {
					float f = EditorGUI.FloatField(new Rect(position.x + xWidth * x,
						position.y + xWidth * y,
						xWidth,
						xWidth),
						p.GetValue(x,y), originFont);
					p.SetValue(x,y,f);
				}
				else {
					float f = EditorGUI.FloatField(new Rect(position.x + xWidth * x,
						position.y + xWidth * y,
						xWidth,
						xWidth),
						p.GetValue(x,y), myFontStyle);
					p.SetValue(x,y,f);
				}				
			}
		}

		/////////////

		if (GUI.changed)
			EditorUtility.SetDirty(p);
		return usedHeight;
	}
}
