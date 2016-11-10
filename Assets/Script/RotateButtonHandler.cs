using UnityEngine;
using System.Collections;
using System;

public class RotateButtonHandler : MonoBehaviour {
	[Tooltip("Rotation axis")]
	public Vector3 axis;

	[Tooltip("Rotation angle in degree")]
	public float angle;

	// busy flag
	private bool isRotating = false;
	private float duration = 0.2f;
	private float time = 0;
	private Quaternion startRotation;
	private Quaternion endRotation;

	void OnSelect() {
		MainController mc = Camera.main.GetComponent<MainController>();
		GrowController gc = mc.surfaceBookPlaceholder.GetComponent<GrowController>();
		if(!gc.IsEditing) {
			startRotation = gc.flowerBox.transform.localRotation;
			endRotation = startRotation * Quaternion.AngleAxis(angle, axis);
			isRotating = true;
			gc.IsEditing = true;
			time = 0;
		}
	}

	void Update() {
		if(isRotating) {
			time += Time.deltaTime;
			float t = Math.Min(1, time / duration);
			MainController mc = Camera.main.GetComponent<MainController>();
			GrowController gc = mc.surfaceBookPlaceholder.GetComponent<GrowController>();
			gc.flowerBox.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
			if(t >= 1) {
				isRotating = false;
				gc.IsEditing = false;
			}
		}
	}
}
