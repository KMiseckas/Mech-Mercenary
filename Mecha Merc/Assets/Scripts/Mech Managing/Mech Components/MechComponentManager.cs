using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MechComponentManager : MonoBehaviour
{
    private Mech_Controls mechControlScript;

	[Header("Component Properties")]
	public MechComponent mechComponent;
	public float health;
	public float damagedHealth;
	public bool repairsAllowed = false;

    [Header("Main Parent")]
	public GameObject myMechMainParent;

	private bool isDestroyed = false;
	private bool isDamaged = false;
	private bool canRepair = true;
	private bool isRepaired = false;

    #region EVENTS - Component Hit Events
    //Delegates
    public delegate void ComponentHit();
    public delegate void Shake(float duration, float magnitude);

    //Shake events
    public static event Shake OnComponentHit;

    //Camera events
    public static event ComponentHit OnDamageCamera;
    public static event ComponentHit OnDestroyCamera;
    public static event ComponentHit OnRepairCamera;

    //Human events
    public static event ComponentHit OnDamageHuman;
    public static event ComponentHit OnDestroyHuman;
    public static event ComponentHit OnRepairHuman;

    //Left Upper Arm events
    public static event ComponentHit OnDamageLeftUpperArm;
    public static event ComponentHit OnDestroyLeftUpperArm;
    public static event ComponentHit OnRepairLeftUpperArm;

    //Left Lower Arm events
    public static event ComponentHit OnDamageLeftLowerArm;
    public static event ComponentHit OnDestroyLeftLowerArm;
    public static event ComponentHit OnRepairLeftLowerArm;

    //Right Upper Arm events
    public static event ComponentHit OnDamageRightUpperArm;
    public static event ComponentHit OnDestroyRightUpperArm;
    public static event ComponentHit OnRepairRightUpperArm;

    //Right Lower Arm events
    public static event ComponentHit OnDamageRightLowerArm;
    public static event ComponentHit OnDestroyRightLowerArm;
    public static event ComponentHit OnRepairRightLowerArm;

    //Left Upper Leg events
    public static event ComponentHit OnDamageLeftUpperLeg;
    public static event ComponentHit OnDestroyLeftUpperLeg;
    public static event ComponentHit OnRepairLeftUpperLeg;

    //Left Lower Leg events
    public static event ComponentHit OnDamageLeftLowerLeg;
    public static event ComponentHit OnDestroyLeftLowerLeg;
    public static event ComponentHit OnRepairLeftLeftLowerLeg;

    //Right Upper Leg events
    public static event ComponentHit OnDamageRightUpperLeg;
    public static event ComponentHit OnDestroyRightUpperLeg;
    public static event ComponentHit OnRepairRightUpperLeg;

    //Right Lower Leg events
    public static event ComponentHit OnDamageRightLowerLeg;
    public static event ComponentHit OnDestroyRightLowerLeg;
    public static event ComponentHit OnRepairRightLowerLeg;

    //Left Weapon events
    public static event ComponentHit OnDamageLeftWeapon;
    public static event ComponentHit OnDestroyLeftWeapon;
    public static event ComponentHit OnRepairLeftWeapon;

    //Right Weapon events
    public static event ComponentHit OnDamageRightWeapon;
    public static event ComponentHit OnDestroyRightWeapon;
    public static event ComponentHit OnRepairRightWeapon;

    //Ammo_Storage events
    public static event ComponentHit OnDamageAmmoStorage;
    public static event ComponentHit OnDestroyAmmoStorage;
    public static event ComponentHit OnRepairAmmoStorage;

    //Body Rotator events
    public static event ComponentHit OnDamageBodyRotator;
    public static event ComponentHit OnDestroyBodyRotator;
    public static event ComponentHit OnRepairBodyRotator;

    //Communication Unit events
    public static event ComponentHit OnDamageCommunicator;
    public static event ComponentHit OnDestroyCommunicator;
    public static event ComponentHit OnRepairCommunicator;

    //CPU events
    public static event ComponentHit OnDamageCPU;
    public static event ComponentHit OnDestroyCPU;
    public static event ComponentHit OnRepairCPU;

    //Display events
    public static event ComponentHit OnDamageDisplay;
    public static event ComponentHit OnDestroyDisplay;
    public static event ComponentHit OnRepairDisplay;

    //Power Cell events
    public static event ComponentHit OnDamagePowerCell;
    public static event ComponentHit OnDestroyPowerCell;
    public static event ComponentHit OnRepairPowerCell;

    //Nuclear Generator events
    public static event ComponentHit OnDamageNuclearGenerator;
    public static event ComponentHit OnDestroyNuclearGenerator;
    public static event ComponentHit OnRepairNuclearGenerator;

    //PDU events (power distribution unit)
    public static event ComponentHit OnDamagePDU;
    public static event ComponentHit OnDestroyPDU;
    public static event ComponentHit OnRepairPDU;

    //Targeting Systems events
    public static event ComponentHit OnDamageTargeting;
    public static event ComponentHit OnDestroyTargeting;
    public static event ComponentHit OnRepairTargeting;

    //Stabilization Unit events
    public static event ComponentHit OnDamageStabilizationUnit;
    public static event ComponentHit OnDestroyStabilizationUnit;
    public static event ComponentHit OnRepairStabilizationUnit;

    //public static event OnComponentHit OnDamageHuman;
    //public static event OnComponentHit OnDestroyHuman;
    //public static event OnComponentHit OnRepairHuman;
    #endregion

    //Components that can exist on any mech
    public enum MechComponent
	{
		Human,
		Camera_Unit,
		Left_Upper_Arm,
		Left_Lower_Arm,
		Right_Upper_Arm,
		Right_Lower_Arm,
		Left_Upper_Leg,
		Left_Lower_Leg,
		Right_Upper_Leg,
		Right_Lower_Leg,
		Left_Weapon,
        Right_Weapon,
		Ammo_Storage,
		Body_Rotator,
		Communicator,
		CPU,
		Monitor_Display,
		Power_Cell,
		Nuclear_Generator,
		PDU,
		Targeting_Unit,
		Stabilization_Unit
	};


	public void DamageComponent(float damage)
	{
		if(!isDestroyed)
		{
			health -= damage;

			if(health > 0)
			{
				if(health <= damagedHealth)
				{
					isDamaged = true;
					DamageEffectsOnComponent(1); // 1 = damaged, not destroyed
					//Debug.Log("Mech Component Damaged");
				}
			}
			else
			{
				isDestroyed = true;
				isDamaged = false;
				canRepair = false;

				DamageEffectsOnComponent(2); // 2 = destroyed
			}
		}
	}

	/// <summary>
	/// Apply damage effects to the mech depending on what the damage level is ( 1 = damaged, 2 = destroyed component, 3 = repair component )
	/// </summary>
	void DamageEffectsOnComponent(int damageLevel)
	{
		switch(mechComponent)
		{
		case MechComponent.Human:
                if (damageLevel == 1)
                {
                    // + ADD BLOOD EFFECT
                    // + ADD SOUND EFFECT
                    Invoke ("RepairComponent", 4f);
                }
                else if( damageLevel == 2)
                {
                    // + ADD BLOOD EFFECT
                    // + ADD SOUND EFFECT
                    CancelInvoke ("RepairComponent");
                    
                }
                else
                {

                }
                break;
		case MechComponent.Camera_Unit:
                if(damageLevel == 1)
                {
                    //CAMERA IS GLITCHY UNTIL REPAIRED
                }
                else if (damageLevel == 2)
                {
                    //DISABLE CAMERA ( DISCONNECT SCREEN )
                    
                }
                else
                {

                }
                break;
		case MechComponent.Ammo_Storage:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //REPAIR TO STOP FIRE - IF NOT IT EXPLODES
                }
                else if (damageLevel == 2)
                {
                    //EXPLODES AND TAKES LIMBS OFF THE MECH
                }
                else
                {

                }
                break;
		case MechComponent.Body_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW UPPER BODY ROTATION
                }
                else if (damageLevel == 2)
                {
                    //NO UPPER BODY ROTATION
                }
                else
                {

                }
                break;
		case MechComponent.Communicator:
                if (damageLevel == 1)
                {
                    //NOTHING
                }
                else if (damageLevel == 2)
                {
                    //NO COMMUNICATIONS
                }
                else
                {

                }
                break;
		case MechComponent.CPU:
                if (damageLevel == 1)
                {
                    //ERROR CODES ON SCREEN
                }
                else if (damageLevel == 2)
                {
                    //REBOOT CPU REQUIRED - ALL CONTROLS LOST
                    //REBOOT AND GET CONTROL
                }
                else
                {

                }
                break;
		case MechComponent.Left_Lower_Arm:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else if (damageLevel == 2)
                {
                    //NO ROTATION
                }
                else
                {

                }
                break;
		case MechComponent.Left_Lower_Leg:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED
                }
                else if (damageLevel == 2)
                {
                    //CANT MOVE FORWARD
                }
                else
                {

                }
                break;
		case MechComponent.Left_Upper_Arm:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else if (damageLevel == 2)
                {
                    //NO ROTATION
                }
                else
                {

                }
                break;
		case MechComponent.Left_Upper_Leg:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED, LOWER BODY ROTATION SLOWED
                }
                else if (damageLevel == 2)
                {
                    //CANT MOVE FORWARD OR ROTATE
                }
                else
                {

                }
                break;
		case MechComponent.Monitor_Display:
                if (damageLevel == 1)
                {
                    //SCREEN CRACKS
                }
                else if (damageLevel == 2)
                {
                    //REMOVES SCREEN AND REPLACES IF HAVE BACKUP
                }
                else
                {

                }
                break;
		case MechComponent.Nuclear_Generator:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //POWER PRODUCED SLOWLY
                }
                else if (damageLevel == 2)
                {
                    //EXPLODES, INSTANT DEATH
                }
                else
                {

                }
                break;
		case MechComponent.PDU:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //FASTER POWER USAGE
                }
                else if (damageLevel == 2)
                {
                    //POWER USAGE NOT CONTROLLED TO ITS POTENTIAL
                }
                else
                {

                }
                break;
		case MechComponent.Power_Cell:
                if (damageLevel == 1)
                {
                    //LEAKS , POWER USAGE HIGHER
                }
                else if (damageLevel == 2)
                {
                    //NO POWER OUTPUT, MECH STOPS WHEN POWER EMPTY
                }
                else
                {

                }
                break;
		case MechComponent.Left_Weapon:
                if (damageLevel == 1)
                {
                    //LESS ACCURATE SHOOTING
                    //CHANCE TO DESTROY BARREL
                }
                else if (damageLevel == 2)
                {
                    //BARREL CANT BE USED UNTIL REPAIR
                }
                else
                {

                }
                break;
            case MechComponent.Right_Weapon:
                if (damageLevel == 1)
                {
                    //LESS ACCURATE SHOOTING
                    //CHANCE TO DESTROY BARREL
                }
                else if (damageLevel == 2)
                {
                    //BARREL CANT BE USED UNTIL REPAIR
                }
                else
                {

                }
                break;
		case MechComponent.Right_Lower_Arm:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else if (damageLevel == 2)
                {
                    //NO ROTATION
                }
                else
                {

                }
                break;
		case MechComponent.Right_Lower_Leg:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED
                }
                else if (damageLevel == 2)
                {
                    //CANT MOVE FORWARD
                }
                else
                {

                }
                break;
		case MechComponent.Right_Upper_Arm:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else if (damageLevel == 2)
                {
                    //NO ROTATION
                }
                else
                {

                }
                break;
		case MechComponent.Right_Upper_Leg:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED, LOWER BODY ROTATION SLOWED
                }
                else if (damageLevel == 2)
                {
                    //CANT MOVE FORWARD OR ROTATE
                }
                else
                {

                }
                break;
		case MechComponent.Stabilization_Unit:
                if (damageLevel == 1)
                {
                    //SLOWER MOVEMENT FORWARD
                }
                else if (damageLevel == 2)
                {
                    //SLOW MOVEMENT TO KEEP STABLE
                }
                else
                {

                }
                break;
		case MechComponent.Targeting_Unit:
                if (damageLevel == 1)
                {
                    //NOTHING
                }
                else if (damageLevel == 2)
                {
                    //NO CROSSHAIR - RANGE FINDING
                }
                else
                {

                }
                break;
		}
	}

    void RepairComponent()
    {
        DamageEffectsOnComponent (3); // 3 = component repaired
    }

}
