using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MechComponentManager : MonoBehaviour
{
	private MechDestroyed mechDestroyedManager;
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

	public enum MechComponent
	{
		Human,
		Camera_Unit,
		Left_Upper_Arm_Rotator,
		Left_Lower_Arm_Rotator,
		Right_Upper_Arm_Rotator,
		Right_Lower_Arm_Rotator,
		Left_Upper_Leg_Rotator,
		Left_Lower_Leg_Rotator,
		Right_Upper_Leg_Rotator,
		Right_Lower_Leg_Rotator,
		Raycast_Weapon_Barrel,
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

	void Start()
	{
		mechDestroyedManager = myMechMainParent.GetComponent<MechDestroyed> (); 
	}

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
	/// Apply damage effects to the mech depending on what the damage level is ( 1 = damaged, 2 = destroyed component )
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
                    DisableMechControlling(); //Disable mech controls

                    Invoke ("EnableMechConrolling", 4f);
                }
                else
                {
                    // + ADD BLOOD EFFECT
                    // + ADD SOUND EFFECT

                    CancelInvoke ("EnableMechControlling");

                    DestroyMech();
                }
			break;
		case MechComponent.Camera_Unit:
                if(damageLevel == 1)
                {
                    //CAMERA IS GLITCHY UNTIL REPAIRED
                }
                else
                {
                    //DISABLE CAMERA ( DISCONNECT SCREEN )
                    
                }
                break;
		case MechComponent.Ammo_Storage:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //REPAIR TO STOP FIRE - IF NOT IT EXPLODES
                }
                else
                {
                    //EXPLODES AND TAKES LIMBS OFF THE MECH
                }
                break;
		case MechComponent.Body_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW UPPER BODY ROTATION
                }
                else
                {
                    //NO UPPER BODY ROTATION
                }
                break;
		case MechComponent.Communicator:
                if (damageLevel == 1)
                {
                    //NOTHING
                }
                else
                {
                    //NO COMMUNICATIONS
                }
                break;
		case MechComponent.CPU:
                if (damageLevel == 1)
                {
                    //ERROR CODES ON SCREEN
                }
                else
                {
                    //REBOOT CPU REQUIRED - ALL CONTROLS LOST
                    //REBOOT AND GET CONTROL
                }
                break;
		case MechComponent.Left_Lower_Arm_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else
                {
                    //NO ROTATION
                }
                break;
		case MechComponent.Left_Lower_Leg_Rotator:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED
                }
                else
                {
                    //CANT MOVE FORWARD
                }
                break;
		case MechComponent.Left_Upper_Arm_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else
                {
                    //NO ROTATION
                }
                break;
		case MechComponent.Left_Upper_Leg_Rotator:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED, LOWER BODY ROTATION SLOWED
                }
                else
                {
                    //CANT MOVE FORWARD OR ROTATE
                }
                break;
		case MechComponent.Monitor_Display:
                if (damageLevel == 1)
                {
                    //SCREEN CRACKS
                }
                else
                {
                    //REMOVES SCREEN AND REPLACES IF HAVE BACKUP
                }
                break;
		case MechComponent.Nuclear_Generator:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //POWER PRODUCED SLOWLY
                }
                else
                {
                    //EXPLODES, INSTANT DEATH
                }
                break;
		case MechComponent.PDU:
                if (damageLevel == 1)
                {
                    //SETS ON FIRE
                    //FASTER POWER USAGE
                }
                else
                {
                    //POWER USAGE NOT CONTROLLED TO ITS POTENTIAL
                }
                break;
		case MechComponent.Power_Cell:
                if (damageLevel == 1)
                {
                    //LEAKS , POWER USAGE HIGHER
                }
                else
                {
                    //NO POWER OUTPUT, MECH STOPS WHEN POWER EMPTY
                }
                break;
		case MechComponent.Raycast_Weapon_Barrel:
                if (damageLevel == 1)
                {
                    //LESS ACCURATE SHOOTING
                    //CHANCE TO DESTROY BARREL
                }
                else
                {
                    //BARREL CANT BE USED UNTIL REPAIR
                }
                break;
		case MechComponent.Right_Lower_Arm_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else
                {
                    //NO ROTATION
                }
                break;
		case MechComponent.Right_Lower_Leg_Rotator:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED
                }
                else
                {
                    //CANT MOVE FORWARD
                }
                break;
		case MechComponent.Right_Upper_Arm_Rotator:
                if (damageLevel == 1)
                {
                    //SLOW ROTATION
                }
                else
                {
                    //NO ROTATION
                }
                break;
		case MechComponent.Right_Upper_Leg_Rotator:
                if (damageLevel == 1)
                {
                    //MOVEMENT SLOWED, LOWER BODY ROTATION SLOWED
                }
                else
                {
                    //CANT MOVE FORWARD OR ROTATE
                }
                break;
		case MechComponent.Stabilization_Unit:
                if (damageLevel == 1)
                {
                    //SLOWER MOVEMENT FORWARD
                }
                else
                {
                    //SLOW MOVEMENT TO KEEP STABLE
                }
                break;
		case MechComponent.Targeting_Unit:
                if (damageLevel == 1)
                {
                    //NOTHING
                }
                else
                {
                    //NO CROSSHAIR - RANGE FINDING
                }
                break;
		}
	}

    void DestroyMech()
    {
        mechDestroyedManager.DestroyMech ();
    }

    void EnableMechControlling()
    {
        mechControlScript.IsControllingEnabled = true;
    }

    void DisableMechControlling()
    {
        mechControlScript.IsControllingEnabled = false;
    }
}
