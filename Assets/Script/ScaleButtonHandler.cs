using UnityEngine;
using System.Collections;
using System;

public class ScaleButtonHandler : MonoBehaviour {
	[Tooltip("Scale percent")]
	public float percent;

	void OnSelect() {
		// get flower controller
		MainController mc = Camera.main.GetComponent<MainController>();
		SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();
		FlowerController fc = sbc.flowerBox.GetComponent<FlowerController>();

		// calculate start and end scale
		Vector3 startScale = sbc.flowerBox.transform.localScale;
		Vector3 endScale = startScale * percent;

		// call flower controller method
		fc.Scale(startScale, endScale);
	}
}
