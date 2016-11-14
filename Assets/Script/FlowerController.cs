using UnityEngine;
using System.Collections;
using System;

public class FlowerController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

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

	void Start() {
		// get flower bounds
		// remove collider after get bounds, we don't need collision on flower
		BoxCollider flowerCollider = gameObject.GetComponent<BoxCollider>();
		flowerBound = flowerCollider.bounds;
		Destroy(flowerCollider);
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
			Scale(Vector3.zero, Vector3.one, 0.5f);

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
}
