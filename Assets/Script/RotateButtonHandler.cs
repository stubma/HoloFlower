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

		// axis is in world space and not transformed, so we need transform it
		// with camera y roation first, then convert it to flower space
		Quaternion axisRotate = Quaternion.Euler(new Vector3(0, Camera.main.transform.localRotation.eulerAngles.y, 0));
		Vector3 globalPoint = axisRotate * axis;
		globalPoint += sbc.flowerBox.transform.position;
		Vector3 localAxis = sbc.flowerBox.transform.InverseTransformPoint(globalPoint);

		// calculate rotation by local axis
		Quaternion startRotation = sbc.flowerBox.transform.localRotation;
		Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, localAxis);

		// call flower controller method
		fc.Rotate(startRotation, endRotation);

		// rotate fixed duplication also
		// neobox uses right-hand coordinate
		fc.RotateFixedDup(axis, -angle);
	}
}
