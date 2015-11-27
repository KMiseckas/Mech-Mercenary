using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Camera mechCamera;
    [SerializeField] private int zoomSpeed = 5;

    private bool isZoomedIn = false;

    private float zoomFOV = 30;
    private float normalFOV = 0;
    private float currentFOV;

    Mech_Controls mechControls;

    void Awake()
    {
        normalFOV = mechCamera.fieldOfView;
        currentFOV = normalFOV;

        mechControls = GetComponent<Mech_Controls> ();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {

            StopCoroutine ("TimedZoom");

            //Debug.Log ("Zoom activated");
            if(isZoomedIn)
            {
                isZoomedIn = false;
                mechControls.ChangeMouseSensitivity ();
                StartCoroutine ("TimedZoom", normalFOV);
            }
            else
            {
                isZoomedIn = true;
                mechControls.ChangeMouseSensitivity ();
                StartCoroutine ("TimedZoom", zoomFOV);
            }

            //Debug.Log (isZoomedIn);

        }
    }

    IEnumerator TimedZoom(float targetFOV)
    {
        while(currentFOV != targetFOV)
        {
            if(targetFOV > currentFOV)
            {
                currentFOV += zoomSpeed;

                if (currentFOV > targetFOV)
                {
                    currentFOV = targetFOV;
                }
            }
            else if(targetFOV < currentFOV)
            {
                currentFOV -= zoomSpeed;

                if(currentFOV < targetFOV)
                {
                    currentFOV = targetFOV;
                }
            }

            mechCamera.fieldOfView = currentFOV;

            yield return null;

            //Debug.Log ("Working");
        }
    }

    /// <summary>
    /// Change zoom in FOV (zoom in amount)
    /// </summary>
    /// <param name="fov"></param>
    public void SetZoomFOV(float fov)
    {
        zoomFOV = fov;
    }

}
