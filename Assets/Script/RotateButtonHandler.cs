using UnityEngine;
using System.Collections;
using System;

public class RotateButtonHandler : MonoBehaviour {
	[Tooltip("Rotation axis")]
	public Vector3 axis;

	[Tooltip("Rotation angle in degree")]
	public float angle;

	void OnSelect() {
		// get flower controller
		MainController mc = Camera.main.GetComponent<MainController>();
		SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();
		FlowerController fc = sbc.flowerBox.GetComponent<FlowerController>();

		// calculate start and end rotation
		Quaternion startRotation = sbc.flowerBox.transform.localRotation;
		Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, axis);

		// call flower controller method
		fc.Rotate(startRotation, endRotation);
	}
}
