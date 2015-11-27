using UnityEngine;
using System.Collections;

public class ArmorPlate : MonoBehaviour 
{
    [SerializeField] private GameObject myMech;

    [Header("x plate thickness to 1 tonne weight")]
    [SerializeField] private float plateThicknessToWeightRatio;

    private float minWeight;
    private float startWeight;

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
    private float lastThickness;

    public float GetPlateThickness 
	{
		get {return currentPlateThickness;}
	}

    public delegate void Shake(float duration, float magnitude, GameObject thisMech);
    public static event Shake OnArmorHit;

    MechWeight mechWeight;
    GameState gameState;

    void Awake()
    {
        mechWeight = myMech.GetComponent<MechWeight> ();
        gameState = myMech.GetComponent<GameState> ();

        minWeight = minPlateThickness / plateThicknessToWeightRatio;
        startWeight = plateThickness / plateThicknessToWeightRatio;

        mechWeight.IncreaseLowestWeight (minWeight);
        mechWeight.IncreaseWeight (startWeight);
    }

    void Start()
	{
		currentDurability = maxDurability;
		currentPlateThickness = plateThickness;

		//Calculate how much loss in durability reduces armor thickness
		durabilityToArmorRatio = (plateThickness - minPlateThickness) / (maxDurability - minDurability);

        lastThickness = plateThickness;
	}

    void Update()
    {
        if(gameState.GetLocalGameState () == GameState.CurrentLocalGameState.Editor)
        {
            if (lastThickness != plateThickness)
            {
                mechWeight.IncreaseWeight (-(lastThickness / plateThicknessToWeightRatio));
                startWeight = plateThickness / plateThicknessToWeightRatio;
                mechWeight.IncreaseWeight (startWeight);

                lastThickness = plateThickness;
            }
        }
    }

	public void DamageDurability(float damage)
	{
        //Camera Shake
        OnArmorHit (0.05f, 0.1f, myMech);

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
		//Debug.Log (currentPlateThickness);
	}

}
