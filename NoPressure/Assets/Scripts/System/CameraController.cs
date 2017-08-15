using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour {

	private enum CameraMode
	{
		Focus,
		Manual,//can move the camera via keyboard
	}

	private GameObject mCameraObject; 
	public GameObject Focus;//The current focus point of the viewport
	public Canvas HUDDisplay; //The hud displayed over the camera.

	public int xBoundMin = 0; // camera will stop scrolling if focus.x <= this value
	public int xBoundMax = 100; //camera will stop scrolling if focus.x + screen.width is >= this value
	public int zBoundMin = 0; // camera will stop scrolling if the focus.z <= this value
    public int zBoundMax = 100; //camera will stop scrolling if the focus.z + screen.height is >= this value

	private float cameraWidth;
	private float cameraHeight;

	private float viewMoveBufferX = 2;
	private float viewMoveBufferZ = 2;

	private Camera mCamera;
	// Use this for initialization
	void Start () {
        mCameraObject = GameObject.Find("Main Camera");
        mCamera = mCameraObject.GetComponent<Camera> ();

        cameraWidth = mCamera.transform.position.y / 2;
        cameraHeight = mCamera.transform.position.y / 2;

        if (Focus == null)
            Focus = GameObject.Find("Player");
        //DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (mCameraObject == null)
            mCameraObject = GameObject.Find("Main Camera");
        if (Focus == null)
            Focus = GameObject.Find("Player");

        MoveCameraToFocus ();
		keepWithinBounds ();
		MoveFocusToCameraCenter ();
	}

	public void MoveCameraToFocus ()
	{
        if (mCameraObject == null)
            return;

        if (Focus == null)
            return;

        mCameraObject.transform.position = new Vector3(
            Focus.transform.position.x,
            mCameraObject.transform.position.y,
            Focus.transform.position.z);
	}

	public void keepWithinBounds ()
	{

	}

	public void MoveFocusToCameraCenter ()
	{

	}

	public Ray ScreenPointToRay(Vector3 screenPoint)
	{
        if (mCamera == null)
            mCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            //return new Ray(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

        Ray returnVal = mCamera.ScreenPointToRay(screenPoint);
        //Debug.Log(returnVal);

        return returnVal;
        //new Vector3(0, 0, 0);

        //return new Ray(new Vector3(0,0,0),new Vector3(0,0,0));
    }

}
