using UnityEngine;
using System.Collections;

public class SimpleScreenBloodSplat : MonoBehaviour
{

    [Header("Blood Splatter Settings")]
    [SerializeField] private Material[] bloodSplatMaterials;
    [SerializeField] private GameObject bloodSplatPanel;
    [SerializeField] private float fadeOutTime = 2.2f;
    [SerializeField] private float fadeOutAmount = 0.05f;

    private float currentFadeOut = 0f;
    private bool isBloodSplatActive = false;
    private int currentBloodSplatMat = 0;

    private float updateFadeRate = 0f;

    private Color fullAlpha;
    private Color targetAlpha;

    void OnEnable()
    {
        MechComponentManager.OnDestroyHuman += SplatBloodOnCamera;
        MechComponentManager.OnDamageHuman += SplatBloodOnCamera;
    }

    void OnDisable()
    {
        MechComponentManager.OnDestroyHuman -= SplatBloodOnCamera;
        MechComponentManager.OnDamageHuman -= SplatBloodOnCamera;
    }

    void Start()
    {
        fullAlpha = Color.white;
        fullAlpha.a = 1;

        updateFadeRate = 1 / fadeOutAmount;
        updateFadeRate = fadeOutTime / updateFadeRate;
    }

    void Update()
    {
        if (Input.GetKeyDown (KeyCode.F))
        {
            SplatBloodOnCamera ();
        }
    }

    void SplatBloodOnCamera()
    {
        if(isBloodSplatActive)
        {

            CancelInvoke ("FadeBloodSplat");

            currentFadeOut = 0f;
            bloodSplatPanel.GetComponent<MeshRenderer> ().material.color = fullAlpha;
            targetAlpha = fullAlpha;
            FadeBloodSplat ();
        }
        else
        {
            currentFadeOut = 0;

            targetAlpha = fullAlpha;

            if(bloodSplatMaterials.Length > 1)
            {
                int random = Random.Range (0, bloodSplatMaterials.Length - 1);

                bloodSplatPanel.GetComponent<MeshRenderer> ().material = bloodSplatMaterials[random];
            }
            else
            {
                bloodSplatPanel.GetComponent<MeshRenderer> ().material = bloodSplatMaterials[0];
                bloodSplatPanel.GetComponent<MeshRenderer> ().material.color = fullAlpha;
                FadeBloodSplat ();
            }

            isBloodSplatActive = true;
        }
    }

    void FadeBloodSplat()
    {
        targetAlpha.a -= fadeOutAmount;

        bloodSplatPanel.GetComponent<MeshRenderer> ().material.color = targetAlpha;

        if (bloodSplatPanel.GetComponent<MeshRenderer> ().material.color.a > 0)
        {
            Invoke ("FadeBloodSplat", updateFadeRate);
        }
        else
        {
            isBloodSplatActive = false;
        }
    }


}
