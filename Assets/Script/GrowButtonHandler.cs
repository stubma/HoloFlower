using UnityEngine;
using System.Collections;

public class GrowButtonHandler : MonoBehaviour {
	// is flower growed
	private bool isGrowed;

	void Start () {
		// reset flag
		isGrowed = false;
	}

	public void OnGazeEnter() {
		// if gazed, keep grow button visible
		MainController mc = Camera.main.GetComponent<MainController>();
		GameObject sb = mc.surfaceBookPlaceholder;
		GrowController gc = sb.GetComponent<GrowController>();
		gc.ShouldKeepOpCanvas = true;
	}

	public void OnGazeLeave() {
		// if not gazed, grow button can be hide
		MainController mc = Camera.main.GetComponent<MainController>();
		GameObject sb = mc.surfaceBookPlaceholder;
		GrowController gc = sb.GetComponent<GrowController>();
		gc.ShouldKeepOpCanvas = false;
	}

	public void OnSelect() {
		// if not growed yet
		if(!isGrowed) {
			// grow the flower
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject sb = mc.surfaceBookPlaceholder;
			GrowController gc = sb.GetComponent<GrowController>();
			gc.GrowFlower();
			isGrowed = true;

			// hide operation canvas immediately
			gc.ShouldKeepOpCanvas = false;
			gc.DisableSurfaceOpCanvas();
		}
	}
}
