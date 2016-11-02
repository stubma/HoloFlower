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

	// flag indicating if we should keep op canvas visible
	public bool ShouldKeepOpCanvas {
		get;
		set;
	}

	// gaze timer
	private bool isGazed;
	private float gazeLeaveTime;
	private bool isOpShown;

	// is flower growed
	private bool IsGrowed {
		get {
			return flower.activeSelf;
		}
	}

	// Use this for initialization
	void Start () {
		// hide
		flower.SetActive(false);
		surfaceOpCanvas.gameObject.SetActive(false);

		// init flag
		IsPlaced = false;
		isOpShown = false;
		ShouldKeepOpCanvas = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(IsPlaced && !isGazed) {
			if(ShouldKeepOpCanvas) {
				gazeLeaveTime = 0;
			} else {
				// fade out if leave 5 seconds
				gazeLeaveTime += Time.deltaTime;
				if(gazeLeaveTime >= 5) {
					FadeOutSurfaceOpCanvas();
				}
			}
		}
	}

	public void OnGazeEnter() {
		if(IsPlaced && !IsGrowed) {
			// place operation canvas
			PlaceSurfaceOpCanvas();

			// fade in
			FadeInSurfaceOpCanvas();

			// set flag
			isGazed = true;
		}
	}

	public void OnGazeLeave() {
		if(IsPlaced && !IsGrowed) {
			// reset time
			isGazed = false;
			gazeLeaveTime = 0;
		}
	}

	public void GrowFlower() {
		if(!IsGrowed) {
			// place flower on it
			flower.SetActive(true);
			flower.transform.localRotation = gameObject.transform.localRotation;
			Vector3 pos = gameObject.transform.position;
			Renderer r = gameObject.GetComponent<Renderer>();
			pos.y += r.bounds.extents.y;
			flower.transform.position = pos;

			// play grow animation
			Animation anim = flower.GetComponent<Animation>();
			anim.Play("Take 001");
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

	public void FadeOutSurfaceOpCanvas() {
		if(isOpShown) {
			growText.GetComponent<CanvasRenderer>().SetAlpha(1);
			growText.CrossFadeAlpha(0, 0.2f, true);
			isOpShown = false;
		}
	}
}
