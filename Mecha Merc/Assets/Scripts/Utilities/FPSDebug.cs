using UnityEngine;
using System.Collections;

public class FPSDebug : MonoBehaviour 
{
	public int maxFPS = 60;

	void Awake() 
	{
		Application.targetFrameRate = maxFPS;
	}
}
