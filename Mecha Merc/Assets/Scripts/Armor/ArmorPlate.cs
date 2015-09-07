using UnityEngine;
using System.Collections;

public class ArmorPlate : MonoBehaviour 
{

	[Header("Armor Properties")]
	public float plateThickness;
	public float maxDurability;
	public float minDurability;

	//Default values for Rolled Homogenous Armor ( default for all mechs at start )
	[Header("Plate Thickness Limits On RHA (Default)")]
	public float maxPlateThickness;
	public float minPlateThickness;

	private float currentDurability;
	private float currentPlateThickness;
	private float durabilityToArmorRatio;

	public float GetPlateThickness 
	{
		get {return currentPlateThickness;}
	}

	void Start()
	{
		currentDurability = maxDurability;
		currentPlateThickness = plateThickness;

		//Calculate how much loss in durability reduces armor thickness ratio
		durabilityToArmorRatio = (maxDurability - minDurability) / (maxPlateThickness - minPlateThickness);
	}

	void DamageDurability(float damage)
	{
		if(currentDurability > minDurability)
		{
			//Reduce durability and recalculate armor plate thickness
			currentDurability -= damage;
			CalculateNewPlateThickness();
		}
	}

	void CalculateNewPlateThickness()
	{
		currentPlateThickness = plateThickness - ((maxDurability - currentDurability) * durabilityToArmorRatio);
	}

}
