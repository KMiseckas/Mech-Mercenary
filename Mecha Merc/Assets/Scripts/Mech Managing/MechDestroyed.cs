using UnityEngine;
using System.Collections;

public class MechDestroyed : MonoBehaviour 
{

	private bool isMechDestroyed = false;

    void OnEnable()
    {
        MechComponentManager.OnDestroyHuman += DestroyMech;
    }

    void OnDisable()
    {
        MechComponentManager.OnDestroyHuman -= DestroyMech;
    }

	public bool IsMechDestroyed 
	{
		get {return isMechDestroyed;}
	}

	public void DestroyMech ()
	{
		isMechDestroyed = true;
        MechComponentManager.OnDestroyHuman -= DestroyMech;

        Debug.Log("MECH DESTROYED --------------------");
	}
	
}
