using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class FlowerController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

	// a clone flower used to play animation
	private GameObject dupBox;

	// a clone which is not rotated, to get correct final rotation for printing
	private GameObject fixedDup;

	// flower bound
	private Bounds flowerBound;
	public Bounds FlowerBound {
		get {
			return flowerBound;
		}
	}

	// is flower grow animation finished
	public bool IsGrowAnimationDone {
		get {
			return !isGrowing && isGrowed;
		}
	}

	// is flower growed
	private bool isGrowed = false;
	private bool isGrowing = false;

	// for rotation
	private bool isRotating = false;
	private float rotateDuration = 0.2f;
	private float rotateTime = 0;
	private Quaternion startRotation;
	private Quaternion endRotation;

	// for scale
	private bool isScaling = false;
	private float scaleDuration = 0.2f;
	private float scaleTime = 0;
	private Vector3 startScale;
	private Vector3 endScale;

	// for printing animation
	private bool isPrinting = false;
	private int printAnimPhase = 0;
	private float printTime = 0;
	private float printDuration = 1;
	private Color startColor;
	private Color endColor;
	private Vector3 startPrintScale;
	private Vector3 endPrintScale;
	public event UnityAction PrintAnimationEnd;

	void Start() {
		// get flower bounds
		// remove collider after get bounds, we don't need collision on flower
		BoxCollider flowerCollider = gameObject.GetComponent<BoxCollider>();
		flowerBound = flowerCollider.bounds;
		Destroy(flowerCollider);

		// duplicate without rotation
		fixedDup = new GameObject();
		fixedDup.transform.position = Vector3.zero;
		fixedDup.transform.localRotation = Quaternion.identity;
	}

	void OnDestroy() {
		Destroy(fixedDup);
	}

	void Update () {
		// rotate
		if(isRotating) {
			rotateTime += Time.deltaTime;
			float t = Math.Min(1, rotateTime / rotateDuration);
			gameObject.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
			if(t >= 1) {
				isRotating = false;
			}
		}

		// scale
		if(isScaling) {
			scaleTime += Time.deltaTime;
			float t = Math.Min(1, scaleTime / scaleDuration);
			gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, t);
			if(t >= 1) {
				isScaling = false;
				isGrowed = true;
				isGrowing = false;
			}
		}

		// printing animation
		if(isPrinting) {
			if(dupBox != null) {
				if(printAnimPhase == 0) {
					PrintAnimationUpPhase();
				} else if(printAnimPhase == 1) {
					PrintAnimationDownPhase();
				} else if(printAnimPhase == 2) {
					// wait for 0.2 second to avoid print animation stuck
					printTime += Time.deltaTime;
					if(printTime >= 0.2f) {
						isPrinting = false;
						if(PrintAnimationEnd != null) {
							PrintAnimationEnd();
						}
					}
				}
			}
		}
	}

	private void PrintAnimationUpPhase() {
		printTime += Time.deltaTime;
		float t = Math.Min(1, printTime / printDuration);
		dupBox.transform.localScale = Vector3.Lerp(startPrintScale, endPrintScale, t);
		dupBox.GetComponentInChildren<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
		if(t >= 1) {
			// now move dup box to print placeholder
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject placeholder = mc.neoboxPlaceholder;
			dupBox.transform.localRotation = placeholder.transform.localRotation;
			Vector3 pos = placeholder.transform.position;
			BoxCollider c = placeholder.GetComponent<BoxCollider>();
			pos.y += c.bounds.extents.y;
			dupBox.transform.position = pos;

			// scale back
			startPrintScale = dupBox.transform.localScale;
			endPrintScale = gameObject.transform.localScale;

			// fade in, so swap colors
			Color tmp = startColor;
			startColor = endColor;
			endColor = tmp;

			// reset time and increase phase
			printTime = 0;
			printAnimPhase++;
		}
	}

	private void PrintAnimationDownPhase() {
		printTime += Time.deltaTime;
		float t = Math.Min(1, printTime / printDuration);
		dupBox.transform.localScale = Vector3.Lerp(startPrintScale, endPrintScale, t);
		dupBox.GetComponentInChildren<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
		if(t >= 1) {
			printTime = 0;
			printAnimPhase++;
		}
	}

	public void Rotate(Quaternion start, Quaternion end, float duration = 0.2f) {
		if(!isRotating) {
			startRotation = start;
			endRotation = end;
			rotateDuration = duration;
			rotateTime = 0;
			isRotating = true;
		}
	}

	public void Scale(Vector3 start, Vector3 end, float duration = 0.2f) {
		if(!isScaling) {
			startScale = start;
			endScale = end;
			scaleTime = duration;
			scaleTime = 0;
			isScaling = true;
		}
	}

	public void RotateFixedDup(Vector3 axis, float angle) {
		Quaternion r = fixedDup.transform.localRotation;
		Vector3 localAxis = fixedDup.transform.InverseTransformPoint(axis);
		fixedDup.transform.localRotation = r * Quaternion.AngleAxis(angle, localAxis);
	}

	public Quaternion GetFixedDupRotation() {
		return fixedDup.transform.localRotation;
	}

	public void Grow() {
		if(!isGrowed && !isGrowing) {
			// get placeholder
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject placeholder = mc.surfaceBookPlaceholder;

			// place flower on it
			gameObject.transform.localRotation = placeholder.transform.localRotation;
			Vector3 pos = placeholder.transform.position;
			BoxCollider c = placeholder.GetComponent<BoxCollider>();
			pos.y += c.bounds.extents.y;
			gameObject.transform.position = pos;

			// play scale animation
			Scale(Vector3.zero, Vector3.one, 1);

			// set flag
			isGrowing = true;
		}
	}

	public void ChangeColor(Color c) {
		MeshRenderer[] rList = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer r in rList) {
			r.material.color = c;
		}
	}

	public void PlayPrintAnimation() {
		if(!isPrinting) {
			// clone flower
			dupBox = UnityEngine.Object.Instantiate(gameObject);
			Destroy(dupBox.GetComponent<FlowerController>());
			dupBox.transform.localRotation = Quaternion.identity;

			// for tint
			startColor = dupBox.GetComponentInChildren<Renderer>().material.color;
			endColor = startColor;
			endColor.a = 0;

			// for scaling
			startPrintScale = dupBox.transform.localScale;
			endPrintScale = startPrintScale;
			endPrintScale.x = 0;
			endPrintScale.y *= 7;
			endPrintScale.z = 0;

			// set flag
			isPrinting = true;
			printAnimPhase = 0;
			printTime = 0;
		}
	}
}
