using UnityEngine;
using System.Collections;

public class speedMeasurement : MonoBehaviour 
{

	public bool enableMeasuring = false;
	public bool isFinished = false;
	private bool startPosTaken = false;

	private Vector3 startPos;
	private Vector3 endPos;

	private float timeTaken = 0f;

	private float metresPerSecond = 0f;

	Mech_Controls mechC;

	void Start()
	{
		mechC = GetComponent<Mech_Controls> ();
	}

	void Update()
	{
		if(enableMeasuring && !isFinished)
		{
			if(!startPosTaken)
			{
				startPos = transform.position;
				startPosTaken = true;
			}

			endPos = transform.position;

			timeTaken += Time.deltaTime;

			if(timeTaken >= 2)
			{
				mechC.IsControllingEnabled = false;

				isFinished = true;

				metresPerSecond = Vector3.Distance(startPos,endPos);
				
				metresPerSecond = metresPerSecond / timeTaken;

				Debug.Log(" metres per second: " + metresPerSecond + " |||||| seconds " + timeTaken);
			}

		}
	}

}
