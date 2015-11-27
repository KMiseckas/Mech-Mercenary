using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	[Header("Forces")]
	public float gravityForce = 1000;

	[Header("Bullet Properties")]
	public float bulletSpeed;
	public float deflectedBulletSpeed;
	public float deflectionAngle;
	public float maxArmorPen;
	public float minArmorPen;
	public float timeToMinPen;
	public float lifeTime;
	public float durabilityDmgPenHit;
	public float durabilityDmgDirectHit;
	public float durabilityDmgDeflectHit;
	public float mechComponentDamage;

	private float currentArmorPen;

	[Header("Raycast Collider Check")]
	public LayerMask layersToCheck;

	private Vector3 oldPos;
	private Vector3 newPos;
	private Vector3 gravity;
	private Vector3 direction;
	private Vector3 velocity;

	private RaycastHit hit;

	private ArmorPlate armorPlateHit;
	private MechComponentManager component;

	void Start()
	{
		oldPos = transform.position;

		//Bullet velocity forward
		velocity = bulletSpeed * transform.forward;

		//Bullet down force
		gravity = new Vector3 (0, -gravityForce, 0);

		currentArmorPen = maxArmorPen;

        //Replace with pooling later !!!! <--------------
		Destroy (this.gameObject, lifeTime);
	}

	void Update()
	{
		//Translate bullet forward and down using gravity and velocity
		velocity +=  gravity * Time.deltaTime;
		newPos =  transform.position + velocity * Time.deltaTime;

		//Get direction and distance for raycast check
		direction = newPos - oldPos;
		float dist = direction.magnitude;

		if(Physics.Raycast(oldPos,direction,out hit,dist,layersToCheck))
		{
			//Debug.Log("hit Object name: " + hit.collider.gameObject.name);

			//Get the angle the bullet hit a surface at
			float hitAngle = 90 - Vector3.Angle(hit.normal,-direction);
			//Debug.Log(90 - hitAngle);

			//Check what type of object has been hit
			switch(hit.collider.gameObject.tag)
			{
			case "MechArmor":

				//Check if object contains armor script, and reference it if it does
				if(armorPlateHit = hit.collider.gameObject.GetComponent<ArmorPlate>())
				{
					//Check if bullet is deflected
					if(hitAngle >= deflectionAngle)
					{
						//Calculate armor plate thickness at hit angle, using the sine law equations (thickness at angle = normal thickness / sin(Opposite angle) )
						float armorThicknessAtHitAngle = armorPlateHit.GetPlateThickness / (Mathf.Sin(Mathf.Deg2Rad * hitAngle));
						//Debug.Log(armorThicknessAtHitAngle);

						//If bullet penetrates
						if(armorThicknessAtHitAngle < currentArmorPen)
						{
							armorPlateHit.DamageDurability(durabilityDmgPenHit);
							layersToCheck = ~(1<<9);
						}
						else //If bullet doesnt penetrate
						{
							//Debug.Log(hit.collider.gameObject.name);

							armorPlateHit.DamageDurability(durabilityDmgDirectHit);
							Destroy(this.gameObject);
						}
					}
					else //If bullet is deflected
					{
						//Debug.Log("Deflected Bullet");

						armorPlateHit.DamageDurability(durabilityDmgDeflectHit);

						//Deflect bullet using Vector3.Reflect method
						Vector3 newForward = Vector3.Reflect(transform.forward,hit.normal);
						velocity = deflectedBulletSpeed * newForward;

						oldPos = transform.position;
						transform.position = hit.point; // Set new position as hit point, so it deflects of the point last raycast hit
					}
				}
				else
				{
					Destroy(this.gameObject);
				}
				break;
			case "MechComponent":

				//Debug.Log("Component Hit: " + hit.collider.gameObject.name);

				if(component = hit.collider.gameObject.GetComponent<MechComponentManager>())
				{
					component.DamageComponent(mechComponentDamage);
				}

				Destroy(this.gameObject);

				break;
			default:
				//Debug.Log(hit.collider.gameObject.name);

				Destroy(this.gameObject);
				break;
			}

		}
		else
		{
			oldPos = transform.position;
			transform.position = newPos;
		}

	}
}
