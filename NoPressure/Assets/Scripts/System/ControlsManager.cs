using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlsManager : MonoBehaviour {

    //public static PointEffector2D 
    public static Vector3 CurserPoint = new Vector3(0, 0, 0);

    public Transform TileSelectPrefab;
    public Transform CrosshairPrefab;

    Transform activeTileSelect;
    Transform activeCrosshair;

    TargetingMode targetingMode = TargetingMode.TileSelect;

    TargetingMode SaveStateTarget = TargetingMode.TileSelect;
    bool isSavingState = false;

    public MapController map;

    public enum TargetingMode
    {
        TileSelect,
        PushButtonSelect,
        Crosshair,
    }

    public enum Controller
    {
        MouseAndKeyboard,
        XboxController,
    }

    public enum MoveType
    {
        TwinStick,
        LookAtCurser,
    }

    public MoveType moveType = MoveType.TwinStick;

    private Player player;
	private CameraController mCamera;

	//Crosshair crosshair;
 	float lookSpeed = 20f; // turning speed for game controller
    float scrollDistance = 0f;
    float scrollSensitivity = 0.11f; 

	// Use this for initialization
	void Start () {
        activeCrosshair = Instantiate(CrosshairPrefab);
        activeTileSelect = Instantiate(TileSelectPrefab);

        SetTargetingMode(targetingMode);

        map = GetComponent<MapController>();
        if (map == null)
            Debug.LogError("Can not find map controller");

        setupPlayer();
        activeTileSelect.GetComponent<TileSelector>().player = player.transform;
        // Remove the mouse cursor
        Cursor.visible = false;
    }
	
    public void setupPlayer()
    {
        GameObject temp = GameObject.Find("Player");

        if (temp == null)
        {
            Debug.Log("Player Is null");
            return;
        }
        player = temp.GetComponent<Player>();

        player.mChangeCurser += ChangeCurserAction;

    }

    public void ChangeCurserAction(TargetingMode setTargetMode)
    {
        if (isSavingState)
            SaveStateTarget = setTargetMode;
        else
            SetTargetingMode(setTargetMode);
    }

	// Update is called once per frame
	void Update () {

		if (Input.GetJoystickNames().Contains("Controller (XBOX 360 For Windows)"))
			controlPlayerXboxController();
		else
			controlPlayerMouseAndKeyboard();

        Vector3 curserPosition = CurserPoint;
        switch (targetingMode)
        {
            case TargetingMode.Crosshair:
                activeCrosshair.position = curserPosition;
                break;

            case TargetingMode.TileSelect:
                activeTileSelect.position = new Vector3(
                    Mathf.Round(curserPosition.x),
                    1f,
                    Mathf.Round(curserPosition.z));
                break;

            case TargetingMode.PushButtonSelect:
                activeTileSelect.position = new Vector3(
                    Mathf.Round(curserPosition.x),
                    1f,
                    Mathf.Round(curserPosition.z));
                break;

        }

    }

    public void SetCameraController(CameraController lCamera)
    {
        mCamera = lCamera;
    }

    void FixedUpdate()
    {
        if (moveType == MoveType.LookAtCurser)
        {
            // Generate a plane that intersects the transform's position with an upwards normal.
            Plane playerPlane = new Plane(Vector3.up, player.transform.position);

            // Generate a ray from the cursor position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            float hitdist = 0.0f;
            // If the ray is parallel to the plane, Raycast will return false.
            if (playerPlane.Raycast(ray, out hitdist))
            {
                // Get the point along the ray that hits the calculated distance.
                Vector3 targetPoint = ray.GetPoint(hitdist);

                CurserPoint = targetPoint;

                // Determine the target rotation.  This is the rotation if the transform looks at the target point.
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - player.transform.position);

                // Smoothly rotate towards the target point.
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.mRotateSpeed * Time.deltaTime);

            }
        }
    }


    public void SetTargetingMode(TargetingMode newTargetingMode)
    {
        targetingMode = newTargetingMode;
        switch (newTargetingMode)
        {
            case TargetingMode.Crosshair:
                activeCrosshair.gameObject.SetActive(true);
                activeTileSelect.gameObject.SetActive(false);
                break;

            case TargetingMode.TileSelect:
                activeCrosshair.gameObject.SetActive(false);
                activeTileSelect.gameObject.SetActive(true);

                activeTileSelect.GetComponent<TileSelector>().selectMode = TileSelector.SelectMode.PlaceItem;
                break;

            case TargetingMode.PushButtonSelect:
                activeCrosshair.gameObject.SetActive(false);
                activeTileSelect.gameObject.SetActive(true);

                activeTileSelect.GetComponent<TileSelector>().selectMode = TileSelector.SelectMode.Search;
                break;
        }
    }

    private void controlPlayerMouseAndKeyboard()
	{
        //Movement controls

        if (moveType == MoveType.TwinStick)
        {
            Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (player == null)
            {
                setupPlayer();

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

                CurserPoint = point;

                // 5. turn the Player to face that point
                player.LookAt(point);
            }

        }

        else if(moveType == MoveType.LookAtCurser)
        {
            var x = Input.GetAxis("Horizontal") * Time.deltaTime * 0.0f;        //basic movement
            var z = Input.GetAxis("Vertical") * Time.deltaTime * player.mMoveSpeed;

            player.transform.Rotate(0, x, 0);
            player.transform.Translate(0, 0, z);
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //if (targetingMode == TargetingMode.TileSelect)
            //    targetingMode = TargetingMode.Crosshair;
            //else
            // targetingMode = TargetingMode.TileSelect;


            SaveStateTarget = targetingMode;
            isSavingState = true;

            SetTargetingMode(TargetingMode.PushButtonSelect);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SetTargetingMode(SaveStateTarget);
            isSavingState = false;
            // targetingMode = ;
        }

        //Shooting controls
        if (Input.GetMouseButton(0))
        {
            if (targetingMode == TargetingMode.Crosshair)
                player.OnTriggerHold();
            else 
            {
                // get Clicked Tile
                MapTile clickedTile = map.getTileAt(
                    new Coord((int)activeTileSelect.transform.position.x,
                    (int)activeTileSelect.transform.position.z));

                TileSelector tileSelected = activeTileSelect.GetComponent<TileSelector>();

                PowerSource ClickedObject = tileSelected.getPowerObject();

                player.OnSelectDown(clickedTile,ClickedObject);

                Crate ClickedItem = tileSelected.getItemCrate();

                player.OnItemClickDown(ClickedItem);


                DestructibleObstacle WallClicked = tileSelected.getObstacle();

                player.OnWallClick(WallClicked);
            } 
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (targetingMode == TargetingMode.Crosshair)
                player.OnTriggerRelease();
            else 
            {
                TileSelector tileSelected = activeTileSelect.GetComponent<TileSelector>();


                // get Clicked Tile
                MapTile clickedTile = map.getTileAt(
                    new Coord((int)activeTileSelect.transform.position.x,
                    (int)activeTileSelect.transform.position.z));

                PowerSource ClickedObject = activeTileSelect.GetComponent<TileSelector>().getPowerObject();

                player.OnSelectUp(clickedTile, ClickedObject);

                Crate ClickedItem = tileSelected.getItemCrate();

                if (player.OnItemClickUp(ClickedItem) == true)
                {
                    tileSelected.currentCrate.SetButtonInactive();
                    Destroy(tileSelected.currentCrate.gameObject);
                    tileSelected.currentCrate = null;
                }
            }
            
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

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            player.DropCurrentItem();
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
        //crosshair.Move(newPoint, lookSpeed);
        // 7. turn the Player to face the crosshair
        //player.LookAt(crosshair.transform.position);

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
