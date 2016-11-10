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
			Animation anim = flower.GetComponent<Animation>();
			return !anim.isPlaying && IsGrowed;
		}
	}

	// is flower growed
	public bool IsGrowed {
		get;
		set;
	}

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

	void Start () {
		// init
		IsGrowed = false;

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
		if(!IsGrowed) {
			// get placeholder
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject placeholder = mc.surfaceBookPlaceholder;

			// place flower on it
			gameObject.transform.localRotation = placeholder.transform.localRotation;
			Vector3 pos = placeholder.transform.position;
			BoxCollider c = placeholder.GetComponent<BoxCollider>();
			pos.y += c.bounds.extents.y;
			gameObject.transform.position = pos;

			// play grow animation
			Animation anim = flower.GetComponent<Animation>();
			anim.Play("Take 001");

			// set flag
			IsGrowed = true;
		}
	}
}
