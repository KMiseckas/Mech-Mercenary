using UnityEngine;
using System.Collections;

public class Mech_Controls : MonoBehaviour 
{
	[Header("Controls Enabled ?")]
	public bool isControllingEnabled = true;

	[Header("Cameras")]
	public Transform mechMainCamHorizontal;
	public Transform mechMainCamVertical;
	public Transform mechBackUpCam;
	 
	[Header("Movement Settings")]
	public Transform movementDirector;
	public bool isMovementEnabled = true;
	public bool isAccelerationUsed = true;
	public float topSpeed;
	public float topReverseSpeed;
	public float walkSpeed;
	public float reverseWalkSpeed;
	public float maxForwardSpeedWhileRotating;
	public float maxReverseSpeedWhileRotating;
	private float currentSpeed = 0f;
	private float acceleration = 2;
	private float constantSpeedModifier = 0.28f;
	private float currentForwardSpeed = 0f;
	private float currentReverseSpeed = 0f;

	[Header("Rotation Settings")]
	public bool isCameraAhead = true;
	public bool isLowerBodyRotationEnabled = true;
	public float lowerBodyRotationSpeed;
	public bool isUpperBodyRotationEnabled = true;
	public float upperBodyRotationSensitivity;
	public float upperBodyRotationMaxSpeed;
	public bool isCameraVerticalRotationEnabled = true;
	public float verticalCameraRotationSensitivity;
	public float cameraVerticalMaxSpeed;
	public float cameraHorizontalMaxSpeed;
	public float maxVerticalAngle;
	public float minVerticalAngle;
	private bool isLowerBodyRotating = false;
	public bool canLeftUpperArmRotate = true;
	public bool canRightUpperArmRotate = true;


	[Header("Camera Free Look")]
	public bool isFreeLookEnabled = true;
	public float freeLookSpeed;
	public float maxVerticalFreeLookAngle;
	public float minVerticalFreeLookAngle;
	public float maxHorizontalFreeLookAngle;
	private bool isFreeLooking = false;
	
	[Header("Rotation Bones")]
	public Transform upperBody;
	public Transform lowerBody;
	public Transform upperLeftArm;
	public Transform upperRightArm;
	public Transform lowerLeftArm;
	public Transform lowerRightArm;

	[Header("AudioClips")]
	public AudioClip mechStep;

	[Header("AudioSource")]
	public GameObject mechFootRight;
	public GameObject mechFootLeft;

	private Vector3 defaultArmRot;

	private float nextIncreaseInSpeed = 0f;
	private float speedIncreaseRate = 0.05f;

	private bool movingForward = false;
	private bool movingBackward = false;

	private float lowerSpin = 90;
	private float upperSpin = 270;
	private float smoothSpin = 270;
	private float cameraVerticalSpin = 0;
	private float freeLookVerticalSpin = 0f;
	private float freeLookHorizontalSpin = 0f;

	private Vector3 moveDirection;

	private CharacterController cController;
	private Animator mechAnimator;


	[Header("Testing / Debugging")]
	[SerializeField] private float averageMouseSpeed = 0;
	public float speedDebug;

	void Start () 
	{
		cController = GetComponent<CharacterController> ();
		mechAnimator = gameObject.transform.FindChild ("Mech").GetComponent<Animator> ();

		defaultArmRot = upperLeftArm.transform.eulerAngles;

	}

	void Update () 
	{
		if(isControllingEnabled)
		{
			CheckInput();
			CheckLowerBodyRotation ();

			Move ();

			CheckUpperBodyRotation ();
			CheckVerticalCameraSpin ();
			CheckFreeLook ();

			//Debug.Log(isFreeLooking);
		}

		speedDebug = currentSpeed;
	}

	void CheckInput()
	{
		if(isMovementEnabled)
		{
			moveDirection.z = constantSpeedModifier * Input.GetAxis("Vertical");

			if(!isLowerBodyRotating)
			{
				if(Input.GetButton("Sprint"))
				{
					currentForwardSpeed = topSpeed;
					currentReverseSpeed = topReverseSpeed;
				}
				else
				{
					if(currentReverseSpeed > reverseWalkSpeed)
					{
						currentReverseSpeed = Lerp(currentReverseSpeed,reverseWalkSpeed,0.4f * Time.deltaTime);
					}
					else
					{
						currentReverseSpeed = reverseWalkSpeed;
					}

					if(currentForwardSpeed > walkSpeed)
					{
						currentForwardSpeed = Lerp(currentForwardSpeed,walkSpeed,0.4f * Time.deltaTime);
					}
					else
					{
						currentForwardSpeed = walkSpeed;
					}

				}
			}
			else
			{
				currentReverseSpeed = Sinerp(currentReverseSpeed,maxReverseSpeedWhileRotating,0.5f * Time.deltaTime);
				currentForwardSpeed = Sinerp(currentForwardSpeed,maxForwardSpeedWhileRotating,0.5f * Time.deltaTime);
			}

			if(Input.GetAxis("Vertical") > 0)
			{
				if(isAccelerationUsed)
				{
					if(Time.time > nextIncreaseInSpeed)
					{
						speedIncreaseRate = 0.1f * (currentSpeed / 10 + 1f);

						nextIncreaseInSpeed = Time.time + speedIncreaseRate;
						
						currentSpeed += (acceleration);
						//currentSpeed = Mathf.RoundToInt(currentSpeed);

						currentSpeed = Mathf.Clamp(currentSpeed,0,currentForwardSpeed);
					}
				}
				else
				{
					currentSpeed = currentForwardSpeed;
				}

				mechAnimator.SetBool("isWalking",true);

			}
			else if(Input.GetAxis("Vertical") < 0)
			{
				if(isAccelerationUsed)
				{
					if(Time.time > nextIncreaseInSpeed)
					{
						speedIncreaseRate = 0.1f * (currentSpeed / 10 + 1f);

						nextIncreaseInSpeed = Time.time + speedIncreaseRate;
						
						currentSpeed += (acceleration);
						//currentSpeed = Mathf.RoundToInt(currentSpeed);

						currentSpeed = Mathf.Clamp(currentSpeed,0,currentReverseSpeed);
					}
				}
				else
				{
					currentSpeed = currentReverseSpeed;
				}

			}

			if(!Input.GetButton("Vertical"))
			{
				currentSpeed = 0;
				mechAnimator.SetBool("isWalking",false);
			}

			mechAnimator.SetFloat("animation speed",currentSpeed * 0.07f);

			//Debug.Log(mechAnimator.GetFloat("animation speed"));
			//Debug.Log ("CurrentSpeed: " + currentSpeed + "  " + (moveDirection.z * currentSpeed));

			moveDirection.z *= currentSpeed;

			//Debug.Log("kph: " + currentSpeed);
		}

	}

	void CheckLowerBodyRotation()
	{
		if(isLowerBodyRotationEnabled && isMovementEnabled)
		{
			if(Input.GetAxis("Vertical") < 0)
			{
				lowerSpin -= Input.GetAxis ("Horizontal") * Time.deltaTime * lowerBodyRotationSpeed;
			}
			else
			{
				lowerSpin += Input.GetAxis ("Horizontal") * Time.deltaTime * lowerBodyRotationSpeed;
			}
			
			if(Input.GetButton("Horizontal"))
			{
				lowerBody.transform.eulerAngles = new Vector3(transform.eulerAngles.x, lowerSpin, lowerBody.transform.eulerAngles.z);
				isLowerBodyRotating = true;
			}
			else
			{
				isLowerBodyRotating = false;
			}
		}
	}

	void CheckUpperBodyRotation()
	{
		if(isUpperBodyRotationEnabled)
		{
			if(!isFreeLooking)
			{
				float rotSpeed = Input.GetAxisRaw ("MouseX") * upperBodyRotationSensitivity;

				rotSpeed = Mathf.Clamp(rotSpeed,-upperBodyRotationMaxSpeed,upperBodyRotationMaxSpeed);

				upperSpin += rotSpeed;

				//Rotate the camera with the upper body rotation ( on the y axis )
				mechMainCamHorizontal.transform.eulerAngles = new Vector3(mechMainCamHorizontal.transform.eulerAngles.x,
				                                                          upperSpin - 270,mechMainCamHorizontal.transform.eulerAngles.z);

				//Smooth out the upper body movement ( seems as body follows camera smoothly )
				smoothSpin = Coserp(smoothSpin,upperSpin,0.3f);

				//Rotate the upper body based on mouse input
				upperBody.transform.eulerAngles = new Vector3(upperBody.transform.eulerAngles.x, smoothSpin, upperBody.transform.eulerAngles.z);
			}
		}
		else if(!isFreeLooking)
		{
			//Rotate the camera with the upper body rotation ( on the y axis )
			mechMainCamHorizontal.transform.eulerAngles = new Vector3(mechMainCamHorizontal.transform.eulerAngles.x,
			                                                          upperBody.transform.eulerAngles.y - 270,mechMainCamHorizontal.transform.eulerAngles.z);
		}
	}

	void CheckVerticalCameraSpin()
	{
		if(isCameraVerticalRotationEnabled)
		{
			if(!isFreeLooking)
			{
				float rotSpeed = Input.GetAxisRaw ("MouseY") * verticalCameraRotationSensitivity;

				rotSpeed = Mathf.Clamp(rotSpeed,-cameraVerticalMaxSpeed,cameraVerticalMaxSpeed);

				cameraVerticalSpin -= rotSpeed;

				//Clamp vertical Rotation
				cameraVerticalSpin = Mathf.Clamp(cameraVerticalSpin,-maxVerticalAngle,minVerticalAngle);

				//Rotate the main camera based on mouse input ( on the x axis ), also keep body rotation on the y axis
				mechMainCamVertical.transform.eulerAngles = new Vector3(cameraVerticalSpin,mechMainCamHorizontal.transform.eulerAngles.y,0);

				float targetRotation = Coserp(upperLeftArm.transform.eulerAngles.z,-cameraVerticalSpin + defaultArmRot.z,0.3f);

				if(canLeftUpperArmRotate)
				{
					upperLeftArm.transform.eulerAngles = new Vector3(upperLeftArm.transform.eulerAngles.x,upperLeftArm.transform.eulerAngles.y,targetRotation);
				}

				if(canRightUpperArmRotate)
				{
					upperRightArm.transform.eulerAngles = new Vector3(upperRightArm.transform.eulerAngles.x,upperRightArm.transform.eulerAngles.y,targetRotation);
				}
			}
		}
		else if(!isFreeLooking)
		{
			//Rotate the main camera based on mouse input ( on the x axis ), also keep body rotation on the y axis
			mechMainCamVertical.transform.eulerAngles = new Vector3(cameraVerticalSpin,mechMainCamHorizontal.transform.eulerAngles.y,0);
		}
	}

	void CheckFreeLook()
	{
		if(isFreeLookEnabled)
		{
			if(Input.GetButton("Free Look"))
			{
				isFreeLooking = true;

				freeLookVerticalSpin -= Input.GetAxisRaw("MouseY") * freeLookSpeed;
				freeLookHorizontalSpin += Input.GetAxisRaw("MouseX") * freeLookSpeed;

				//Clamp rotations for free look
				freeLookHorizontalSpin = Mathf.Clamp(freeLookHorizontalSpin,-maxHorizontalFreeLookAngle,maxHorizontalFreeLookAngle);
				freeLookVerticalSpin = Mathf.Clamp(freeLookVerticalSpin,-minVerticalFreeLookAngle,maxVerticalFreeLookAngle);

				mechMainCamVertical.transform.eulerAngles = new Vector3(freeLookVerticalSpin,mechMainCamHorizontal.transform.eulerAngles.y + freeLookHorizontalSpin,0);

			}
			else
			{
				isFreeLooking = false;

				freeLookVerticalSpin = cameraVerticalSpin;
				freeLookHorizontalSpin = 0;
			}
		}
	}

	void Move()
	{
		//Make the forward movement direction, the forward of lower body transform
		moveDirection = moveDirection.z * transform.TransformDirection (movementDirector.forward);

		//Gravity
		moveDirection.y = -10f;

		cController.Move (moveDirection * Time.deltaTime);

	}

	public float Lerp(float start, float end, float value)
	{
		return ((1.0f - value) * start) + (value * end);
	}

	public float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}

	public float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
	}

}
