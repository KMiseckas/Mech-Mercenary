using UnityEngine;
using System.Collections;

public class Mech_Controls : MonoBehaviour 
{
	private bool isControllingEnabled = true;

	[Header("Cameras")]
	[SerializeField] private Transform mechMainCamHorizontal;
	[SerializeField] private Transform mechMainCamVertical;
	[SerializeField] private Transform mechBackUpCam;
	 
	[Header("Movement Settings (At lowest weight)")]
	[SerializeField] private Transform movementDirector;
	[SerializeField] private bool isAccelerationUsed = true;
	[SerializeField] private float topSpeed;
	[SerializeField] private float topReverseSpeed;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float reverseWalkSpeed;
	[SerializeField] private float maxForwardSpeedWhileRotating;
	[SerializeField] private float maxReverseSpeedWhileRotating;

    private float defaultTopSpeed;

    private float nextIncreaseInSpeed = 0f;
    private float speedIncreaseRate = 0.05f;

    private bool movingForward = false;
    private bool movingBackward = false;
    private bool isMovementEnabled = true;

    [SerializeField] private float currentSpeed = 0f;
    private float acceleration = 2;
    private float constantSpeedModifier = 0.28f;
    private float currentForwardSpeed = 0f;
    private float currentReverseSpeed = 0f;

    [Header("Weight / Speed Settings")]
    [SerializeField] private float weightToSpeedRatio;
    [SerializeField] private float maxSpeedChange;

    private float speedDifferenceDueWeight = 0f;

	[Header("Rotation Settings")]
	[SerializeField] private float lowerBodyRotationMaxSpeed;
	[SerializeField] private float upperBodyRotationMaxSpeed;
	[SerializeField] private float weaponVerticalMaxSpeed;
	[SerializeField] private float weaponVerticalMaxAngle;
	[SerializeField] private float weaponVerticalMinAngle;
	[SerializeField] private float defaultMouseSensitivity = 0.5f;
    [SerializeField] private float defaultZoomSensitivity = 0.3f;

    private float mouseSensitivity;

	private bool isLowerBodyRotationEnabled = true;
	private bool isUpperBodyRotationEnabled = true;
	private bool isCameraVerticalRotationEnabled = true;
	private bool canLeftUpperArmRotate = true;
	private bool canRightUpperArmRotate = true;
	private bool isLowerBodyRotating = false;
	
	[Header("Camera Free Look")]
	[SerializeField] private bool isFreeLookEnabled = true;
	[SerializeField] private float freeLookSensitivity = 0.5f;
	[SerializeField] private float maxVerticalFreeLookAngle;
	[SerializeField] private float minVerticalFreeLookAngle;
	[SerializeField] private float maxHorizontalFreeLookAngle;

	private bool isFreeLooking = false;
	
	[Header("Rotation Bones")]
	[SerializeField] private Transform upperBody;
	[SerializeField] private Transform lowerBody;
	[SerializeField] private Transform upperLeftArm;
	[SerializeField] private Transform upperRightArm;
	[SerializeField] private Transform lowerLeftArm;
	[SerializeField] private Transform lowerRightArm;

	[Header("AudioClips")]
	[SerializeField] private AudioClip mechStep;

	[Header("AudioSource")]
	[SerializeField] private GameObject mechFootRight;
	[SerializeField] private GameObject mechFootLeft;

	private Vector3 defaultArmRot;

	private float lowerSpin = 90;
	private float upperSpin = 270;
	private float smoothSpin = 270;
	private float cameraVerticalSpin = 0;
	private float freeLookVerticalSpin = 0f;
	private float freeLookHorizontalSpin = 0f;

    [SerializeField] private bool isInMechEditor = false;

	private Vector3 moveDirection;

	private CharacterController cController;
    private MechWeight mechWeight;
    private GameState gameState;
	//private Animator mechAnimator;

	[Header("Testing / Debugging")]
	[SerializeField] private float averageMouseSpeed = 0;
	[SerializeField] private float speedDebug;

    #region Properties ------
    public bool IsMechControllingEnabled
    {
        get { return isControllingEnabled;}
        set { isControllingEnabled = value;}
    }

    public bool IsLowerBodyRotationEnabled
    {
        get {return isLowerBodyRotationEnabled;}
        set {isLowerBodyRotationEnabled = value;}
    }

    public bool IsUpperBodyRotationEnabled
    {
        get {return isUpperBodyRotationEnabled;}
        set {isUpperBodyRotationEnabled = value;}
    }

    public bool IsCameraVerticalRotationEnabled
    {
        get {return isCameraVerticalRotationEnabled;}
        set {isCameraVerticalRotationEnabled = value;}
    }

    public bool CanLeftUpperArmRotate
    {
        get { return canLeftUpperArmRotate;}
        set {canLeftUpperArmRotate = value;}
    }

    public bool CanRightUpperArmRotate
    {
        get { return canRightUpperArmRotate;}
        set {canRightUpperArmRotate = value;}
    }

    public bool IsLowerBodyRotating
    {
        get {return isLowerBodyRotating;}
        set {isLowerBodyRotating = value;}
    }

    public float TopSpeed
    {
        get {return topSpeed;}
        set {topSpeed = value;}
    }

    public float WalkSpeed
    {
        get {return walkSpeed;}
        set {walkSpeed = value;}
    }

    public bool IsMovementEnabled
    {
        get{return isMovementEnabled;}
        set{isMovementEnabled = value;}
    }

    public float Acceleration
    {
        get{return acceleration;}
        set{acceleration = value;}
    }

    public float LowerBodyRotationMaxSpeed
    {
        get{return lowerBodyRotationMaxSpeed;}
        set {lowerBodyRotationMaxSpeed = value;}
    }

    public float UpperBodyRotationMaxSpeed
    {
        get{return upperBodyRotationMaxSpeed;}
        set{upperBodyRotationMaxSpeed = value;}
    }

    public bool IsFreeLookEnabled
    {
        get {return isFreeLookEnabled;}
        set {isFreeLookEnabled = value;}
    }


    #endregion

    void OnEnable()
    {
        MechComponentManager.OnDestroyHuman += DisableMechControls;
    }

    void OnDisable()
    {
        MechComponentManager.OnDestroyHuman -= DisableMechControls; 
    }

    void Awake()
    {
        defaultTopSpeed = topSpeed;
        mouseSensitivity = defaultMouseSensitivity;
    }

    void Start () 
	{
		cController = GetComponent<CharacterController> ();
        mechWeight = GetComponent<MechWeight> ();
        gameState = GetComponent<GameState> ();
		//mechAnimator = gameObject.transform.FindChild ("Mech").GetComponent<Animator> ();

		defaultArmRot = upperLeftArm.transform.eulerAngles;

        UpdateSpeeds ();
	}

	void Update () 
	{
        if(gameState.GetLocalGameState() == GameState.CurrentLocalGameState.Editor)
        {
            UpdateSpeeds ();
        }

		if(IsMechControllingEnabled)
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

    /// <summary>
    /// Update the mech movement speeds based on weight of the mech
    /// </summary>
    void UpdateSpeeds()
    {
        walkSpeed = defaultTopSpeed / 2;

        speedDifferenceDueWeight = (mechWeight.GetWeightDifference () / weightToSpeedRatio);

        //Speed is different depending on mech weight
        walkSpeed -= speedDifferenceDueWeight / 2;
        topSpeed = defaultTopSpeed - speedDifferenceDueWeight;
    }

    /// <summary>
    /// Check all the button input for controls
    /// </summary>
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

    public void DisableMechControls()
    {
        isControllingEnabled = false;
    }

    public void EnableMechControls()
    {
        isControllingEnabled = true;
    }

    public void ChangeMouseSensitivity()
    {
        if(mouseSensitivity != defaultMouseSensitivity)
        {
            mouseSensitivity = defaultMouseSensitivity;
        }
        else
        {
            mouseSensitivity = defaultZoomSensitivity;
        }

        Debug.Log (mouseSensitivity);
    }

}
