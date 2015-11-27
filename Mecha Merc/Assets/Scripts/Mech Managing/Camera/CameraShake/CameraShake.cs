using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
    [Header("Test values for camera shake")]
	[SerializeField] private float duration = 0.25f;
	[SerializeField] private float magnitude = 0.13f;

    [Header("Mech Camera")]
    [SerializeField] private GameObject thisCamerasMech;
	
	public bool test = false;

    float elapsed = 0.0f;

    void OnEnable()
    {
        ArmorPlate.OnArmorHit += ShakeCamera;
    }

    void OnDisable()
    {
        ArmorPlate.OnArmorHit -= ShakeCamera;
    }

    void Update()
    {
        if (test)
        {
            test = false;
            PlayShake ();
        }
    }

    //Test shake
    void PlayShake()
    {
        //StopCoroutine ("Shake");
		StartCoroutine(Shake(this.duration,this.magnitude));
    }

    //Events call for this method
    void ShakeCamera(float duration,float magnitude, GameObject mech)
    {
        if(mech == thisCamerasMech)
        {
            //StopCoroutine ("Shake");
            StartCoroutine (Shake (duration, magnitude));
        }
    }

	IEnumerator Shake(float duration, float magnitude)
    {
		
        //Get original local camera position
		Vector3 originalCamPos = Camera.main.transform.localPosition;

        //While time elapsed is still not more than shake duration...
        while (elapsed < duration)
        {
            //Increment time each frame
			elapsed += Time.deltaTime;			
			
			float percComplete = elapsed / duration;			
			float damper = 1.0f - Mathf.Clamp(4.0f * percComplete - 3.0f, 0.0f, 1.0f);
			
			// map noise to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			Camera.main.transform.localPosition = new Vector3(x, y, originalCamPos.z);
				
			yield return null;
		}

        elapsed = 0.0f;

        //After camera shake is complete, return to original local camera position
        Camera.main.transform.localPosition = originalCamPos;
	}
}
