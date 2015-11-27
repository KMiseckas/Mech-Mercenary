using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour 
{
    [SerializeField] private GameObject myMech;

    [Header("Bullet Direction GameObject")]
    [SerializeField] private Transform bulletDirection;

	[Header("General Settings")]
    [SerializeField] private bool rightWeapon = true;
	[SerializeField] private GameObject bullet;
	[SerializeField] private GameObject muzzle;
    [SerializeField] private float powerUsageSpeed;

    [Header("Accuracy Settings")]
    [Tooltip("In Unity Metres")]
    [SerializeField] private float maxWeaponInaccuracy;

    [Header("Bullet Cross Distance")]
    [SerializeField] private Transform centre;
    [SerializeField] private float bulletIntersectionDist;

    private float distanceToCentre;
    private float angleToIntersection;

    private float distanceOfInaccuracy = 1000;  //Distance of max weapon inaccuracy in unity metres
    private float maxInaccuracyAngle;

	[Header("Fire Rate Settings")]
	[Tooltip("how much time between each shot")] public float fireRate;

	private float nextFire;

    private bool isUsingWeapon = false;

    private float nextPowerReduction;

    private PowerSource power;

	/* ----------------------- For the old Overcomplicated method only
	[Header("Testing")]
	public bool visualTestOn = false;
	public GameObject testObjectForCurve;

	[Header("Shot Properties")]
	public float initialVelocity;
	public float gravity;
	

	[Header("Node Properties")]
	[Tooltip("Time difference between each node / projectile travel")] public float nodeFrequency;
	[Tooltip("Nodes to calculate , maxNodes * nodeFrequency = total time of projectile")] public int maxNodes;

	private float shotAngle;

	List<Vector3> nodePos;
	*/

    void Start()
    {
        //Calculate angle of inaccuracy for bullet rotation when instantiating
        maxInaccuracyAngle = Mathf.Rad2Deg * Mathf.Atan ((maxWeaponInaccuracy / distanceOfInaccuracy));
        //Debug.Log ("max inaccuracy angle = " + maxInaccuracyAngle);

        distanceToCentre = Vector3.Distance (muzzle.transform.position, centre.position);
        angleToIntersection = Mathf.Rad2Deg * Mathf.Atan ((distanceToCentre / bulletIntersectionDist));

        power = myMech.GetComponent<PowerSource> ();

        //Debug.Log (distanceToCentre);
    }

	void Update()
	{
		if(rightWeapon)
		{
			if(Input.GetButton("Fire2"))
			{
                isUsingWeapon = true;

				if(Time.time > nextFire)
				{
                    float offsetAngleY = Random.Range (-maxInaccuracyAngle, maxInaccuracyAngle);
                    float offsetAngleX = Random.Range (-maxInaccuracyAngle, maxInaccuracyAngle);

                    //Add offset to new rotation for bullet
                    Quaternion bulletRotation = bulletDirection.rotation;
                    bulletRotation.eulerAngles = new Vector3 (bulletDirection.rotation.eulerAngles.x + offsetAngleX, bulletDirection.rotation.eulerAngles.y + offsetAngleY - angleToIntersection, 0); 

					nextFire = Time.time + fireRate;

                    GameObject bulletClone = Instantiate (bullet, muzzle.transform.position, bulletRotation) as GameObject;
				}
			}
            else
            {
                isUsingWeapon = false;
            }
		}
		else
		{
			if(Input.GetButton("Fire1"))
			{
                isUsingWeapon = true;

				if(Time.time > nextFire)
				{
                    float offsetAngleY = Random.Range (-maxInaccuracyAngle, maxInaccuracyAngle);
                    float offsetAngleX = Random.Range (-maxInaccuracyAngle, maxInaccuracyAngle);

                    //Add offset to new rotation for bullet
                    Quaternion bulletRotation = bulletDirection.rotation;
                    bulletRotation.eulerAngles = new Vector3 (bulletDirection.rotation.eulerAngles.x + offsetAngleX, bulletDirection.rotation.eulerAngles.y + offsetAngleY + angleToIntersection, 0);

                    nextFire = Time.time + fireRate;
					
					GameObject bulletClone = Instantiate(bullet, muzzle.transform.position, bulletRotation) as GameObject;
				}
			}
            else
            {
                isUsingWeapon = false;
            }
		}

        //Power reduction
        if (isUsingWeapon)
        {
            if (Time.time > nextPowerReduction)
            {
                nextPowerReduction = Time.time + powerUsageSpeed;

                power.DecreasePower ();
            }
        }
    }

	/*Old method ( *OVERCOMPLICATED*) uses real mechanics maths to determine bullet position at given time intervals
	void CalculateBulletTrajectory(GameObject bullet)
	{

		shotAngle = Vector3.Angle(defaultVector.transform.forward,muzzle.transform.forward);

		float sign = Mathf.Sign(Vector3.Dot (-muzzle.transform.right,Vector3.Cross(defaultVector.transform.forward,muzzle.transform.forward)));
		shotAngle = shotAngle * sign;

		//Debug.Log (shotAngle);
		
		nodePos = new List<Vector3>();
		
		float currentFrequency = 0f;
		
		float sinA = Mathf.Sin(Mathf.Deg2Rad * shotAngle);
		float cosA = Mathf.Cos(Mathf.Deg2Rad * shotAngle);

		//Debug.Log (sinA + " : " + cosA);
		
		for(int i = 0; i < maxNodes; i++)
		{
			float forward;
			float up;
			
			currentFrequency += nodeFrequency;
			
			forward = initialVelocity * currentFrequency * cosA ;
			up = (initialVelocity * currentFrequency * sinA ) + (0.5f * -gravity * (currentFrequency * currentFrequency));
			
			//Debug.Log(forward + "," + up);
			//Debug.Log(muzzle.transform.position);
			
			Vector3 thisNodePos = (muzzle.transform.forward * forward) + muzzle.transform.position;
			Vector3 upPos = up * muzzle.transform.up;
			thisNodePos.y = upPos.y + muzzle.transform.position.y;
			
			nodePos.Add(thisNodePos);

			if(visualTestOn)
			{
				GameObject testObject = Instantiate(testObjectForCurve,thisNodePos,Quaternion.identity) as GameObject;
			}
		}

		Bullet bulletScript = bullet.GetComponent<Bullet>();

		bulletScript.TravelToPosition(nodePos);
		//Debug.Log (nodePos);

	}
	*/

}
