using UnityEngine;
using System.Collections;

public class MechWeight : MonoBehaviour
{
    [Header("Mech Weight Settings")]
    [SerializeField] private float totalWeight;
    [SerializeField] private float maxWeightIncrease;

    [SerializeField] private float lowestWeight;

    /// <summary>
    /// Increase the current mech weight (done as parts are added or armor thickness is increased)
    /// </summary>
    /// <param name="weight"></param>
    public void IncreaseWeight(float weight)
    {
        totalWeight += weight;
    }

    /// <summary>
    /// Increase the lowest possible mech weight (done at start)
    /// </summary>
    /// <param name="weight"></param>
    public void IncreaseLowestWeight(float weight)
    {
        lowestWeight += weight;
    }

    /// <summary>
    /// Return the difference in weight between lowest possible mech weight and current total mech weight
    /// </summary>
    /// <returns></returns>
    public float GetWeightDifference()
    {
        return (totalWeight - lowestWeight);
    }
}
