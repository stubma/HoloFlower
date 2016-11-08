using UnityEngine;
using System.Collections;

public class GrowButtonHandler : MonoBehaviour {
	// is flower growed
	private bool isGrowed;

	void Start () {
		// reset flag
		isGrowed = false;
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
		}
	}
}
