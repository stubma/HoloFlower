using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class RotateHandleManipulator : MonoBehaviour {
	// last hand position in camera space
	private Vector3 lastHandPosition;

	// interpolator of target, if has
	private Interpolator targetInterpolator;

	// user is manipulating
	private bool IsManipulating { get; set; }

	// target object
	private GameObject target;

	private void OnEnable() {
		if(GestureManager.Instance != null) {
			GestureManager.Instance.ManipulationStarted += BeginManipulation;
			GestureManager.Instance.ManipulationCompleted += EndManipulation;
			GestureManager.Instance.ManipulationCanceled += EndManipulation;
		} else {
			Debug.LogError(string.Format(
				"GestureManipulator enabled on {0} could not find GestureManager instance, manipulation will not function",
				name));
		}
	}

	private void OnDisable() {
		if(GestureManager.Instance) {
			GestureManager.Instance.ManipulationStarted -= BeginManipulation;
			GestureManager.Instance.ManipulationCompleted -= EndManipulation;
			GestureManager.Instance.ManipulationCanceled -= EndManipulation;
		}

		IsManipulating = false;
	}

	private void BeginManipulation() {
		if(GestureManager.Instance != null && GestureManager.Instance.ManipulationInProgress) {
			if(GestureManager.Instance.FocusedObject == gameObject && TargetManager.Instance.Target != null) {
				IsManipulating = true;

				// get target and interpolator if has
				target = TargetManager.Instance.Target;
				targetInterpolator = target.GetComponent<Interpolator>();

				// In order to ensure that any manipulated objects move with the user, we do all our math relative to the camera
				lastHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
			}            
		}
	}

	private void EndManipulation() {
		IsManipulating = false;
	}

	void Update() {
		if(IsManipulating) {
			// get surface book controller
			MainController mc = Camera.main.GetComponent<MainController>();
			SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();

			// get hand delta
			Vector3 currentHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
			Vector3 handDelta = currentHandPosition - lastHandPosition;

			// save current position as last position
			lastHandPosition = currentHandPosition;

			// convert delta length to angle by a ratio
			float angle = handDelta.magnitude * 300 * (handDelta.x > 0 ? -1 : 1);

			// calculate rotation axis in flower space
			Vector3 globalPoint = Vector3.up + sbc.flowerBox.transform.position;
			Vector3 axis = sbc.flowerBox.transform.InverseTransformPoint(globalPoint);

			// calculate final rotation
			Quaternion startRotation = sbc.flowerBox.transform.localRotation;
			Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, axis);

			// If the object has an interpolator we should use it, otherwise just move the transform directly
			if(targetInterpolator != null) {
				targetInterpolator.SetTargetRotation(endRotation);
			} else {
				target.transform.localRotation = endRotation;
			}
		}
	}
}
