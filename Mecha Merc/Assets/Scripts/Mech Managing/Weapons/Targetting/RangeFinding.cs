using UnityEngine;
using System.Collections;

public class RangeFinding : MonoBehaviour
{

    private float range;
    private float maxRange = 2000f;

    private bool isRangeFindingEnabled = true;

    [SerializeField] private GameObject rangeFindingStart;

    private RaycastHit hit;

    void Start()
    {
        FindRange ();
    }

    /// <summary>
    /// Find range between mech and the aimed at point
    /// </summary>
    void FindRange()
    {
        if(Physics.Raycast(rangeFindingStart.transform.position, rangeFindingStart.transform.forward, out hit, maxRange))
        {
            range = Vector3.Distance (rangeFindingStart.transform.position, hit.point);
        }
        else
        {
            range = 0;
        }

        //Debug.Log ("Range = " + (int) range);

        //Update range every 0.5 seconds
        Invoke ("FindRange", 0.5f);
    }


    /// <summary>
    /// Get range between mech and aimed at point
    /// </summary>
    /// <returns></returns>
    public int GetRange()
    {
        return (int) range;
    }
}
