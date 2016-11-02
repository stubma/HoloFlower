using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GrowController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

	[Tooltip("A operation canvas which displays when user gazes surface book")]
	public Canvas surfaceOpCanvas;

	[Tooltip("Grow text label")]
	public Text growText;

	// flag indicating placeholder is placed
	public bool IsPlaced {
		get;
		set;
	}

	// gaze timer
	private bool isGazed;
	private float gazeLeaveTime;
	private bool isOpShown;

	// Use this for initialization
	void Start () {
		// hide
		flower.SetActive(false);
		surfaceOpCanvas.gameObject.SetActive(false);

		// init flag
		IsPlaced = false;
		isOpShown = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(IsPlaced && !isGazed) {
			// fade out if leave 5 seconds
			gazeLeaveTime += Time.deltaTime;
			if(gazeLeaveTime >= 5) {
				FadeOutSurfaceOpCanvas();
			}
		}
	}

	public void OnGazeEnter() {
		if(IsPlaced) {
			// place operation canvas
			PlaceSurfaceOpCanvas();

			// fade in
			FadeInSurfaceOpCanvas();

			// set flag
			isGazed = true;
		}
	}

	public void OnGazeLeave() {
		if(IsPlaced) {
			// reset time
			isGazed = false;
			gazeLeaveTime = 0;
		}
	}

	public void OnSelect() {
		if(!flower.activeSelf) {
			// get surface placeholder
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject surfaceBookPlaceholder = mc.surfaceBookPlaceholder;

			// place flower on it
			flower.SetActive(true);
			flower.transform.position = surfaceBookPlaceholder.transform.position;
			flower.transform.localRotation = surfaceBookPlaceholder.transform.localRotation;

			// play grow animation
		}
	}

	private void PlaceSurfaceOpCanvas() {
		surfaceOpCanvas.gameObject.SetActive(true);
		Quaternion r = Camera.main.transform.localRotation;
		r.x = 0;
		r.z = 0;
		surfaceOpCanvas.transform.localRotation = r;
		Vector3 pos = gameObject.transform.position;
		pos.y += 0.15f;
		surfaceOpCanvas.transform.position = pos;
	}

	private void FadeInSurfaceOpCanvas() {
		if(!isOpShown) {
			growText.GetComponent<CanvasRenderer>().SetAlpha(0);
			growText.CrossFadeAlpha(1f, 0.2f, true);
			isOpShown = true;
		}
	}

	private void FadeOutSurfaceOpCanvas() {
		if(isOpShown) {
			growText.GetComponent<CanvasRenderer>().SetAlpha(1);
			growText.CrossFadeAlpha(0, 0.2f, true);
			isOpShown = false;
		}
	}
}
