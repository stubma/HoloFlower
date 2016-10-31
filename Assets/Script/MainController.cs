using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	[Tooltip("A canvas which holds hint message text")]
	public Canvas hintCanvas;

	[Tooltip("A text control which displays hint message")]
	public Text hintText;

	// canvas distance
	private float hintCanvasDistance;

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
		// init state
		setState(OpState.LOCATE_SURFACE_BOOK);

		// distance of hint canvas
		hintCanvasDistance = hintCanvas.transform.position.magnitude;
	}

	void Update () {
		// let hint canvas follow camera
		if(hintCanvas.gameObject.activeSelf) {
			hintCanvas.transform.position = gameObject.transform.forward * hintCanvasDistance;
			hintCanvas.transform.localRotation = gameObject.transform.localRotation;
		}
	}

	private void setState(OpState s) {
		state = s;
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Your Surface Book";
			break;
		case OpState.LOCATE_NEOBOX:
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Neobox";
			break;
		default:
			hintCanvas.gameObject.SetActive(false);
			break;
		}
	}
}
