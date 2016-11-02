using UnityEngine;
using System.Collections;

public class GrowButtonHandler : MonoBehaviour {
	// is flower growed
	private bool isGrowed;

	void Start () {
		isGrowed = false;
	}

	public void OnGazeEnter() {
		MainController mc = Camera.main.GetComponent<MainController>();
		GameObject sb = mc.surfaceBookPlaceholder;
		GrowController gc = sb.GetComponent<GrowController>();
		gc.ShouldKeepOpCanvas = true;
	}

	public void OnGazeLeave() {
		MainController mc = Camera.main.GetComponent<MainController>();
		GameObject sb = mc.surfaceBookPlaceholder;
		GrowController gc = sb.GetComponent<GrowController>();
		gc.ShouldKeepOpCanvas = false;
	}

	public void OnSelect() {
		// grow the flower
		if(!isGrowed) {
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject sb = mc.surfaceBookPlaceholder;
			GrowController gc = sb.GetComponent<GrowController>();
			gc.GrowFlower();
			isGrowed = true;

			// hide operation canvas immediately
			gc.ShouldKeepOpCanvas = false;
			gc.FadeOutSurfaceOpCanvas();
		}
	}
}
