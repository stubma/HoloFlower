﻿using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class DragFlowerHandler : MonoBehaviour {
	[Tooltip("How much to scale each axis of hand movement (camera relative) when manipulating the object")]
	public Vector3 handPositionScale = new Vector3(2.0f, 2.0f, 4.0f);

	private Vector3 initialHandPosition;
	private Vector3 initialObjectPosition;

	private Interpolator targetInterpolator;

	private bool IsManipulating { get; set; }

	private GameObject target;

	// is model entered
	private bool isModelEntered;

	// model stay timer
	private float modelStayTime;

	// is model printed
	private bool isPrinted;

	void Start() {
		isModelEntered = false;
		isPrinted = false;
	}

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

				// In order to ensure that any manipulated objects move with the user, we do all our math relative to the camera,
				// so when we save the initial hand position and object position we first transform them into the camera's coordinate space
				initialHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
				initialObjectPosition = Camera.main.transform.InverseTransformPoint(transform.position);
			}
		}
	}

	private void EndManipulation() {
		IsManipulating = false;
	}

	void Update() {
		if(IsManipulating) {
			// First step is to figure out the delta between the initial hand position and the current hand position
			Vector3 currentHandPosition = Camera.main.transform.InverseTransformPoint(GestureManager.Instance.ManipulationHandPosition);
			Vector3 handDelta = currentHandPosition - initialHandPosition;

			// When performing a manipulation gesture, the hand generally only translates a relatively small amount.
			// If we move the object only as much as the hand itself moves, users can only make small adjustments before
			// the hand is lost and the gesture completes.  To improve the usability of the gesture we scale each
			// axis of hand movement by some amount (camera relative).  This value can be changed in the editor or
			// at runtime based on the needs of individual movement scenarios.
			Vector3 scaledHandDelta = Vector3.Scale(handDelta, handPositionScale);

			// Once we've figured out how much the object should move relative to the camera we apply that to the initial
			// camera relative position.  This ensures that the object remains in the appropriate location relative to the camera
			// and the hand as the camera moves.  The allows users to use both gaze and gesture to move objects.  Once they
			// begin manipulating an object they can rotate their head or walk around and the object will move with them
			// as long as they maintain the gesture, while still allowing adjustment via hand movement.
			Vector3 localObjectPosition = initialObjectPosition + scaledHandDelta;
			Vector3 worldObjectPosition = Camera.main.transform.TransformPoint(localObjectPosition);

			// If the object has an interpolator we should use it, otherwise just move the transform directly
			if(targetInterpolator != null) {
				targetInterpolator.SetTargetPosition(worldObjectPosition);
			} else {
				target.transform.position = worldObjectPosition;
			}

			// check collision with printer
			MainController mc = Camera.main.GetComponent<MainController>();
			BoxCollider printerCollider = mc.neoboxPlaceholder.GetComponent<BoxCollider>();
			Bounds printerBounds = printerCollider.bounds;
			BoxCollider flowerCollider = gameObject.GetComponent<BoxCollider>();
			Bounds flowerBounds = flowerCollider.bounds;
			if(flowerBounds.Intersects(printerBounds)) {
				if(!isModelEntered) {
					Debug.Log("model entered!!!");
					isModelEntered = true;
					modelStayTime = 0;
				}
			} else {
				if(isModelEntered) {
					Debug.Log("model exit!!!");
					isModelEntered = false;
				}
			}
		}

		// if model entered printer
		if(isModelEntered && !isPrinted) {
			modelStayTime += Time.deltaTime;
			if(modelStayTime >= 1) {
				isPrinted = true;
				Debug.Log("print!!!!!!");
			}
		}
	}
}