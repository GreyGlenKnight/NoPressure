using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: Create control mode enum that will allow control of the 
//       camera independant of the player
//public enum ControlMode
//{
//    PlayerControl,
//    CameraControl,//allow direct control of camera 
//}
// ControlMode controlMode = ControlMode.PlayerControl;


public class ControlsManager : MonoBehaviour {

    // create a keycode list of all user actions
    KeyCode ShootKey;
    bool RebindingShoot = false;

    KeyCode ReloadKey;
    bool RebindingReload = false;

    KeyCode MoveUp;
    bool RebindingMoveUp = false;

    KeyCode MoveDown;
    bool RebindingMoveDown = false;
    //...
    KeyCode LastKeyPressed;
    bool newKeyPressed = false;

    // Call from gui button "Rebind reload"
    public void StartListenRebindKeyReload()
    {
        RebindingMoveDown = true;
        newKeyPressed = false;
    }
    // Call in update()
    public void RebindReloadKey()
    {
        if (newKeyPressed == false)
            return;// waiting on user to imput a key in OnGUI();
        else
        {
            ReloadKey = LastKeyPressed;
            RebindingReload = true;
        }
    }

    // in update()
    //...
    // if (RebindingReload == true)
    //    RebindReloadKey();
    //...
    // if Input.GetKey(ReloadKey) {
    //    player.Reload();
    // }
    //...
    // 

    //public void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.isKey)
    //        Debug.Log("Detected key code: " + e.keyCode);
    //    LastKeyPressed = e.keyCode;
    //    newKeyPressed = true;
    //}

    public enum Controller
    {
        MouseAndKeyboard,
        XboxController,
    }

    public Player player;
	private CameraController mCamera;

	Crosshair crosshair;
 	float lookSpeed = 20f; // turning speed for game controller
    float scrollDistance = 0f;
    float scrollSensitivity = 0.11f; 

	// Use this for initialization
	void Start () {
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Crosshair>();
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetJoystickNames().Contains("Controller (XBOX 360 For Windows)"))
			controlPlayerXboxController();
		else
			controlPlayerMouseAndKeyboard();
	}

    public void SetCameraController(CameraController lCamera)
    {
        mCamera = lCamera;
    }

	private void controlPlayerMouseAndKeyboard()
	{
		//Movement controls
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (player == null)
		{
            GameObject temp = GameObject.Find("Player");
            
            if (temp == null)
            {
                Debug.Log("Player Is null");
                return;
            }
            player = temp.GetComponent<Player>();

        }
        player.Move(moveInput.normalized);

        //Aiming controls
        // Turn Player to face the direction of the cursor
        // The screen is a 2d world but the game is in a 3d world so we need raycasting to get the
        // accurate position of the mouse cursor in the game world.

        // 1. Cast a ray from the camera through the mouse cursor's position
        if (mCamera == null)
            Debug.Log("Camera is null");
        
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
        // 2. draw a plane that's perpendicular to the y-axis and passes through a point at the gun height.
        // this would be a plane just above the ground and slicing the player at gun height.
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * 1); // 1 here represents the height of the gun
        float rayDistance;

        // 3. if the ray drawn earlier intersects the plane
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // 4. get the point where the ray intersects the plane
            Vector3 point = ray.GetPoint(rayDistance);
            // 5. turn the Player to face that point
            player.LookAt(point);
            // 6. move the crosshair to the position of the cursor
            //Debug.Log("Looking for curser");

            if (crosshair == null)
                crosshair = GameObject.Find("Crosshair").GetComponent<Crosshair>();
            if (crosshair == null)
                Debug.Log("can not find crosshair!");

            crosshair.transform.position = point;
        }

		//Shooting controls
        if (Input.GetMouseButton(0))
        {
            
            player.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            player.OnTriggerRelease();
        }

        //reloading controls
        if (Input.GetKey(KeyCode.R))
        {
            player.Reload();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            player.AbortReload();
        }

        //dashing controls
        if (Input.GetMouseButtonUp(1))
        {
            //TODO reimplement Dash
            //player.Dash();
        }

		//switch Guns
        if (Input.GetKeyUp(KeyCode.F))
        {
            player.CycleInventory(1);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            player.SetSelection(0);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            player.SetSelection(1);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        { 
            player.SetSelection(2);
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            player.SetSelection(3);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.DropCurrentItem();
        }
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            player.SetSelection(3);
        }

        scrollDistance = scrollDistance + Input.GetAxis("Mouse ScrollWheel");

        if (scrollDistance <= -scrollSensitivity)
        {
            scrollDistance = scrollDistance + 0.11f;
            player.CycleInventory(1);

        }

        if (scrollDistance >= scrollSensitivity)
        {
            scrollDistance = scrollDistance - 0.11f;
            player.CycleInventory(-1);
        }

    }
	private void controlCameraXboxController()
	{
		//TODO
	}
	private void controlCameraMouseKeyboard()
	{
		//TODO
	}

    // TODO: update and fix xbox controller
    private void controlPlayerXboxController()
    {
        //Movement controls

        // Move Player based on input from the keyboard
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.Move(moveInput.normalized);

        //Aiming controls
        // Turn Player

        // If an XBOX 360 pad is attached, turn the Player according to
        // the movement of the right analog stick.
        Vector3 lookPoint = new Vector3(Input.GetAxisRaw("XBOXRightAnalogH"), 0, Input.GetAxisRaw("XBOXRightAnalogV"));
        Vector3 newPoint = lookPoint.normalized * lookSpeed;
        // 6. move the crosshair to the position of the cursor
        crosshair.Move(newPoint, lookSpeed);
        // 7. turn the Player to face the crosshair
        player.LookAt(crosshair.transform.position);

        //Shooting controls

        // Shoot
        if (Input.GetAxis("XBOXRightT") > 0)
        {
            player.OnTriggerHold();
        }
        else
        {
            player.OnTriggerRelease();
        }
        //dashing controls
        if (Input.GetButtonUp("XBOXRightB"))
        {
            //player.Dash();
        }
        //switch Guns
        if (Input.GetButtonUp("XBOXLeftB"))
        {
            //player.SwitchGuns();
        }
    }

}
