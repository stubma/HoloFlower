using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	[Tooltip("A canvas which holds hint message text")]
	public Canvas hintCanvas;

	[Tooltip("A text control which displays hint message")]
	public Text hintText;

	[Tooltip("A rectangular used to locate surface book position")]
	public GameObject surfaceBookPlaceholder;

	// canvas distance
	private float hintCanvasDistance;

	// hint move speed
	private float hintMoveSpeed = 0.5f;

	// is placing started?
	private bool isPlacing = false;

	/// <summary>
	/// current state of app 
	/// </summary>
	private enum OpState {
		IDLE = 0,
		LOCATE_SURFACE_BOOK = 1,
		LOCATE_NEOBOX = 2
	}

	/// <summary>
	/// state of app
	/// </summary>
	private OpState state;

	void Start () {
		// distance of hint canvas
		hintCanvasDistance = hintCanvas.transform.position.magnitude; 

		// listen place end event
		TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
		ttp.PlacingEnd += MainController_onPlacingEnd;

		// init state
		setState(OpState.LOCATE_SURFACE_BOOK);
	}

	void Update () {
		// let hint canvas follow camera
		if(hintCanvas.gameObject.activeSelf) {
			Vector3 dstPos = gameObject.transform.forward * hintCanvasDistance;
			dstPos.y += 3f;
			Vector3 srcPos = hintCanvas.transform.position;
			float dist = (dstPos - srcPos).magnitude;
			if(dist > 0) {
				hintCanvas.transform.position = Vector3.Lerp(srcPos, dstPos, hintMoveSpeed / dist);
			}
			hintCanvas.transform.localRotation = gameObject.transform.localRotation;
		}
	}

	void LateUpdate() {
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			if(!isPlacing) {
				TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
				ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
				isPlacing = true;
			}
			break;
		}
	}

	private void MainController_onPlacingEnd() {
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			isPlacing = false;
			setState(OpState.LOCATE_NEOBOX);
			break;
		}
	}

	private void setState(OpState s) {
		state = s;
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			// show hint
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Your Surface Book";
			isPlacing = false;
			break;
		case OpState.LOCATE_NEOBOX:
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Neobox";
			isPlacing = false;
			break;
		default:
			hintCanvas.gameObject.SetActive(false);
			break;
		}
	}
}
