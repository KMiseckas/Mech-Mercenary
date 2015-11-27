using UnityEngine;
using System.Collections;

public class PowerSource : MonoBehaviour
{
    [Header("Power Settings")]
    [SerializeField] private PowerType powerType;
    [SerializeField] private float criticalPowerLevel;

    public enum PowerType
    {
        Nuclear,
        PowerCell
    };

    [Header("Nuclear Settings")]
    [SerializeField] private float nuclear_PowerLimit = 100;
    [SerializeField] private float nuclear_ReplenishSpeed = 0.25f;
    [SerializeField] private float nuclear_ReplenishDelay = 1.5f;

    [Header("Power Cell Settings")]
    [SerializeField] private float cell_PowerLimit = 250;
    [SerializeField] private float cell_ReplenishSpeed = 1f;
    [SerializeField] private float cell_ReplenishDelay = 1.5f;

    private float powerLimit;
    private float replenishRate;
    private float replenishDelay;

    private float currentPower;

    private bool isPowerActive = true;
    private bool isPowerEmpty = false;
    private bool isReplenishingPower = false;

    void Start()
    {
        //Set correct stats for the chosen power type
        switch(powerType)
        {
            case PowerType.Nuclear:
                SetChosenPowerTypeStats (nuclear_PowerLimit, nuclear_ReplenishSpeed, nuclear_ReplenishSpeed);
                break;
            case PowerType.PowerCell:
                SetChosenPowerTypeStats (cell_PowerLimit, cell_ReplenishSpeed, cell_ReplenishDelay);
                break;
        }

        currentPower = powerLimit;
    }

    void Update()
    {
        Debug.Log ("Current Power = " + currentPower);
    }

    //Set power stats
    void SetChosenPowerTypeStats(float powerLimit, float replenishRate, float replenishDelay)
    {
        this.powerLimit = powerLimit;
        this.replenishRate = replenishRate;
        this.replenishDelay = replenishDelay;
    }

    /// <summary>
    /// Set wheather power source is active or not
    /// </summary>
    /// <param name="isActive"></param>
    public void IsPowerActive(bool isActive)
    {
        isPowerActive = isActive;
    }

    /// <summary>
    /// Set what power type the mech is using as a power source
    /// </summary>
    /// <param name="powerType"></param>
    public void SetPowerType(PowerType powerType)
    {
        this.powerType = powerType;
    }

    /// <summary>
    /// Returns wheather all power is gone at the current time
    /// </summary>
    /// <returns></returns>
    public bool IsPowerEmpty()
    {
        return isPowerEmpty;
    }
    
    public void DecreasePower()
    {
        //If empty, stop power consumption
        if(!isPowerEmpty)
        {
            currentPower--;

            if(currentPower <= 0)
            {
                isPowerEmpty = true;
            }

            StopCoroutine ("ReplenishPower");
            CancelInvoke ("Replenish");

            StartCoroutine ("ReplenishPower");
        }
    }

    IEnumerator ReplenishPower()
    {
        float elapsedTime = 0;

        //Delay for power replenishing
        while(elapsedTime < replenishDelay)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        //Repeat power replenish every set rate
        InvokeRepeating ("Replenish", 0, replenishRate);
    }

    void Replenish()
    {
        //Increase power by 1
        currentPower++;

        //Only allow use of power again, after critical level has been filled up with power
        if(isPowerEmpty && currentPower > criticalPowerLevel)
        {
            isPowerEmpty = false;
        }

        if(currentPower > powerLimit)
        {
            currentPower = powerLimit;
            CancelInvoke ("Replenish");
        }
    }
}
