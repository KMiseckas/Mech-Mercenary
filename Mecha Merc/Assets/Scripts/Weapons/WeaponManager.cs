using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour 
{
	[Header("Weapon Settings")]
	public GameObject bullet;
	public GameObject muzzle;

	[Header("Firing Settings")]
	[Tooltip("how much time between each shot")] public float fireRate;

	private float nextFire;
	private bool rightWeapon = true;

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

	void Update()
	{
		if(rightWeapon)
		{
			if(Input.GetButton("Fire2"))
			{
				if(Time.time > nextFire)
				{
					nextFire = Time.time + fireRate;

					GameObject bulletClone = Instantiate(bullet,muzzle.transform.position,Camera.main.transform.parent.rotation) as GameObject;
				}
			}
		}
		else
		{
			if(Input.GetButton("Fire1"))
			{
				if(Time.time > nextFire)
				{
					nextFire = Time.time + fireRate;
					
					GameObject bulletClone = Instantiate(bullet,muzzle.transform.position,Camera.main.transform.parent.rotation) as GameObject;
				}
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
