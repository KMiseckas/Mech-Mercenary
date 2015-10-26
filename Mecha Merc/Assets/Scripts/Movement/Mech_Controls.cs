using UnityEngine;
using System.Collections;

public class Mech_Controls : MonoBehaviour 
{
	private bool isControllingEnabled = true;

	[Header("Cameras")]
	public Transform mechMainCamHorizontal;
	public Transform mechMainCamVertical;
	public Transform mechBackUpCam;
	 
	[Header("Movement Settings")]
	public Transform movementDirector;
	public bool isAccelerationUsed = true;
	public float topSpeed;
	public float topReverseSpeed;
	public float walkSpeed;
	public float reverseWalkSpeed;
	public float maxForwardSpeedWhileRotating;
	public float maxReverseSpeedWhileRotating;

	private bool isMovementEnabled = true;

	private float currentSpeed = 0f;
	private float acceleration = 2;
	private float constantSpeedModifier = 0.28f;
	private float currentForwardSpeed = 0f;
	private float currentReverseSpeed = 0f;

	[Header("Rotation Settings")]
	public float lowerBodyRotationMaxSpeed;
	public float upperBodyRotationMaxSpeed;
	public float weaponVerticalMaxSpeed;
	public float weaponVerticalMaxAngle;
	public float weaponVerticalMinAngle;
	public float mouseSensitivity = 0.5f;

	private bool isLowerBodyRotationEnabled = true;
	private bool isUpperBodyRotationEnabled = true;
	private bool isCameraVerticalRotationEnabled = true;
	private bool canLeftUpperArmRotate = true;
	private bool canRightUpperArmRotate = true;
	private bool isLowerBodyRotating = false;
	
	[Header("Camera Free Look")]
	public bool isFreeLookEnabled = true;
	public float freeLookSensitivity = 0.5f;
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
	//private Animator mechAnimator;


	[Header("Testing / Debugging")]
	[SerializeField] private float averageMouseSpeed = 0;
	public float speedDebug;




    #region Properties ------
    public bool IsControllingEnabled
    {
        get{ return isControllingEnabled;}
        set{ isControllingEnabled = value;}
    }

    public bool IsLowerBodyRotationEnabled
    {
        get{return isLowerBodyRotationEnabled;}
        set {isLowerBodyRotationEnabled = value;}
    }

    public bool IsUpperBodyRotationEnabled
    {
        get{return isUpperBodyRotationEnabled;}
        set {isUpperBodyRotationEnabled = value;}
    }

    public bool IsCameraVerticalRotationEnabled
    {
        get{return isCameraVerticalRotationEnabled;}
        set{isCameraVerticalRotationEnabled = value;}
    }

    public bool CanLeftUpperArmRotate
    {
        get{ return canLeftUpperArmRotate;}
        set{canLeftUpperArmRotate = value;}
    }

    public bool CanRightUpperArmRotate
    {
        get{ return canRightUpperArmRotate;}
        set{canRightUpperArmRotate = value;}
    }

    public bool IsLowerBodyRotating
    {
        get{ return isLowerBodyRotating;}
        set{isLowerBodyRotating = value;}
    }
    #endregion

    void Start () 
	{
		cController = GetComponent<CharacterController> ();
		//mechAnimator = gameObject.transform.FindChild ("Mech").GetComponent<Animator> ();

		defaultArmRot = upperLeftArm.transform.eulerAngles;

	}

	void Update () 
	{
		if(IsControllingEnabled)
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

				//mechAnimator.SetBool("isWalking",true);

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
				//mechAnimator.SetBool("isWalking",false);
			}

			//mechAnimator.SetFloat("animation speed",currentSpeed * 0.07f);

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
				lowerSpin -= Input.GetAxis ("Horizontal") * Time.deltaTime * lowerBodyRotationMaxSpeed;
			}
			else
			{
				lowerSpin += Input.GetAxis ("Horizontal") * Time.deltaTime * lowerBodyRotationMaxSpeed;
			}
			
			if(Input.GetButton("Horizontal"))
			{
                Debug.Log (lowerSpin);
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
				float rotSpeed = Input.GetAxisRaw ("MouseX") * mouseSensitivity;

				float rotSpeedBoundary = upperBodyRotationMaxSpeed * Time.deltaTime;

				rotSpeed = Mathf.Clamp(rotSpeed,-rotSpeedBoundary,rotSpeedBoundary);

				upperSpin += rotSpeed;

				//Rotate the camera with the upper body rotation ( on the y axis )
				mechMainCamHorizontal.transform.eulerAngles = new Vector3(mechMainCamHorizontal.transform.eulerAngles.x,
				                                                          upperSpin - 270,mechMainCamHorizontal.transform.eulerAngles.z);

				//Smooth out the upper body movement (body follows camera smoothly)
				smoothSpin = Lerp(smoothSpin,upperSpin,8 * Time.deltaTime);

				//Debug.Log(Input.GetAxisRaw("MouseX") * 0.5f);

				//Rotate the upper body based on mouse input
				upperBody.transform.eulerAngles = new Vector3(upperBody.transform.eulerAngles.x, smoothSpin , upperBody.transform.eulerAngles.z);
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
				float rotSpeed = Input.GetAxisRaw ("MouseY") * mouseSensitivity;

				float rotSpeedBoundary = weaponVerticalMaxSpeed * Time.deltaTime;

				rotSpeed = Mathf.Clamp(rotSpeed,-rotSpeedBoundary,rotSpeedBoundary);

				cameraVerticalSpin -= rotSpeed;

				//Clamp vertical Rotation
				cameraVerticalSpin = Mathf.Clamp(cameraVerticalSpin,-weaponVerticalMaxAngle,weaponVerticalMinAngle);

				//Rotate the main camera based on mouse input ( on the x axis ), also keep body rotation on the y axis
				mechMainCamVertical.transform.eulerAngles = new Vector3(cameraVerticalSpin,mechMainCamHorizontal.transform.eulerAngles.y,0);


				//Rotate arms
				float targetRotationLeft = Coserp(upperLeftArm.transform.eulerAngles.z,-cameraVerticalSpin + defaultArmRot.z,0.3f);
				float targetRotationRight = Coserp(upperRightArm.transform.eulerAngles.z,-cameraVerticalSpin + defaultArmRot.z,0.3f);

				if(canLeftUpperArmRotate)
				{
					upperLeftArm.transform.eulerAngles = new Vector3(upperLeftArm.transform.eulerAngles.x,upperLeftArm.transform.eulerAngles.y,targetRotationLeft);
				}

				if(canRightUpperArmRotate)
				{
					upperRightArm.transform.eulerAngles = new Vector3(upperRightArm.transform.eulerAngles.x,upperRightArm.transform.eulerAngles.y,targetRotationRight);
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

				freeLookVerticalSpin -= Input.GetAxisRaw("MouseY") * freeLookSensitivity;;
				freeLookHorizontalSpin += Input.GetAxisRaw("MouseX") * freeLookSensitivity;;

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
