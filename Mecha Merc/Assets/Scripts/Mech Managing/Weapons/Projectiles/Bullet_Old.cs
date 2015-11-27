using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet_Old : MonoBehaviour 
{

	//OLD BULLET SCRIPT, GOES WITH OVERCOMPLICATED FIRING METHOD USING REAL MECHANICS MATHS

	[Header("Projectile Properties")]
	public float bulletSpeed;
	public float projectileDamage;
	public float maxLifeTime;

	[Header("Raycast Layer")]
	public LayerMask layersToCheck;

	private List<Vector3> nodePositions;

	private bool hasNodes = false;
	private bool isFacingCorrectForward = false;

	private int currentNodeID = 0;

	private float raycastCheckDistance = 0;

	RaycastHit hit;

	void Start()
	{
		Destroy (this.gameObject, maxLifeTime);
		raycastCheckDistance = bulletSpeed / 30;
	}

	void Update()
	{

		if(hasNodes)
		{
			if(Physics.Raycast(transform.position,transform.forward,out hit,raycastCheckDistance,layersToCheck))
			{
				Debug.Log("hit Object name: " + hit.collider.gameObject.name);
				Destroy(gameObject);
			}

			if(!isFacingCorrectForward)
			{
				FaceTowardsNode(nodePositions[currentNodeID ]);
			}


			transform.Translate(transform.forward * Time.deltaTime * bulletSpeed,Space.World);

			//float dist = Vector3.Distance(transform.position,nodePositions[currentNodeID]);

			if(Vector3.Distance(transform.position,nodePositions[currentNodeID]) < 25f)
			{
				isFacingCorrectForward = false;
				currentNodeID ++;
			}

			if(currentNodeID >= 20)
			{
				Destroy(this.gameObject);
			}
		}

	}

	void FaceTowardsNode(Vector3 nodePos)
	{
		transform.LookAt (nodePos);
		isFacingCorrectForward = true;
	}

	public void TravelToPosition(List<Vector3> nodePos)
	{
		nodePositions = nodePos;
		//Debug.Log (nodePositions[0]);
		hasNodes = true;
	}
}
