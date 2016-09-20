using UnityEngine;
using System.Collections;

[System.Serializable]
public class Pattern : ScriptableObject {

	public int originX, originY;
	public int sizeX, sizeY;
	public float[] values;

	// Use this for initialization
	void Start () {
		
		originX = 0;
		originY = 0;
		sizeX = 4;
		sizeY = 4;
		values = new float[sizeX*sizeY];
	
	}
	public void SetValue(int x, int y, float value) {
		if ((x >= 0) && (x < sizeX) && (y >= 0) && (y < sizeY))
			values[x + y * sizeX] = value;
	}
	public float GetValue(int x, int y) {
		if ((x >= 0) && (x < sizeX) && (y >= 0) && (y < sizeY))
			return values[x + y * sizeX];
		else return 0;
	}

	public void Resize (int sizeX, int sizeY) {

		sizeX = Mathf.Max(sizeX, 1);
		sizeY = Mathf.Max(sizeY, 1);

		float[] newValues = new float[sizeX * sizeY];

		for (int x = 0; (x < sizeX) && (x < this.sizeX); x++) {
			for (int y = 0; (y < sizeY) && (y < this.sizeY); y++) {
				newValues[x+y*sizeX] = values[x+y*this.sizeX];
			}
		}

		this.sizeX = sizeX;
		this.sizeY = sizeY;

		if (originX > sizeX) originX = sizeX;
		if (originY > sizeY) originY = sizeY;

		values = newValues;
		
	}
	public void SetOrigin (int originX, int originY) {
		if (originX < sizeX && originX >= 0)
			this.originX = originX;
		if (originY < sizeY && originY >= 0)
			this.originY = originY;
	}

}
