using UnityEngine;
using System.Collections;

public class MechDestroyed : MonoBehaviour 
{

	private bool isMechDestroyed = false;

	public bool IsMechDestroyed 
	{
		get {return isMechDestroyed;}
	}

	public void DestroyMech ()
	{
			isMechDestroyed = true;

			Debug.Log("MECH DESTROYED --------------------");
	}
	
}
