using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GrowController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

	[Tooltip("Flower container to hold flower animation and set custom scale anchor")]
	public GameObject flowerBox;

	[Tooltip("Grow button, user click to grow flower")]
	public GameObject growButton;

	[Tooltip("Canvas which holds edit buttons for flower")]
	public GameObject editCanvas;

	// flag indicating flower editing is ongoing or not
	public bool IsEditing {
		get;
		set;
	}

	// flower bound
	private Bounds flowerBound;
	public Bounds FlowerBound {
		get {
			return flowerBound;
		}
	}

	// is flower growed
	public bool IsGrowed {
		get {
			return flowerBox.activeSelf;
		}
	}

	// is flower grow animation finished
	private bool IsGrowAnimationDone {
		get {
			Animation anim = flower.GetComponent<Animation>();
			return !anim.isPlaying && IsGrowed;
		}
	}

	// Use this for initialization
	void Start () {
		// init
		IsEditing = false;

		// place and hide grow button
		Collider growCollider = growButton.GetComponent<Collider>();
		Bounds growBound = growCollider.bounds;
		PlaceholderResizer pr = gameObject.GetComponent<PlaceholderResizer>();
		float length = pr.length;
		float width = pr.width;
		growButton.transform.localPosition = new Vector3(0, growBound.size.y / 2, width / 2 + growBound.size.z / 2 + 0.02f);
		growButton.transform.localScale = new Vector3(length / growBound.size.x, 1, 1);
		growCollider.enabled = false;
		growButton.SetActive(false);

		// hide edit canvas
		editCanvas.SetActive(false);

		// get flower bounds
		// remove collider after get bounds, we don't need collision on flower
		BoxCollider flowerCollider = flowerBox.GetComponent<BoxCollider>();
		flowerBound = flowerCollider.bounds;
		Destroy(flowerCollider);

		// hide flower
		flowerBox.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(IsGrowAnimationDone) {
			// make flower target if grow animation is done
			TargetManager.Instance.Target = flowerBox;

			// show edit canvas
			if(!editCanvas.activeSelf) {
				editCanvas.SetActive(true);
				Helper.TreeEnableRenderer(editCanvas);
			}
		}
	}

	void LateUpdate() {
		if(IsGrowAnimationDone && editCanvas.activeSelf) {
			// get flower size, scaled
			// multiply with sqrt 2 for 45 degree situation
			Vector3 flowerScale = flowerBox.transform.localScale;
			float flowerSize = flowerScale.x * flowerBound.size.x;
			flowerSize *= (float)Math.Sqrt(2);

			// update edit canvas position
			PlaceholderResizer pr = gameObject.GetComponent<PlaceholderResizer>();
			float length = pr.length;
			RectTransform editTransform = editCanvas.GetComponent<RectTransform>();
			editCanvas.transform.localPosition = new Vector3(Math.Max(length, flowerSize) / 2 + editTransform.rect.width / 2 + 0.05f, 
				editTransform.rect.height / 2, 0);
		}
	}

	public void GrowFlower() {
		if(!IsGrowed) {
			// place flower on it
			flowerBox.SetActive(true);
			flowerBox.transform.localRotation = gameObject.transform.localRotation;
			Vector3 pos = gameObject.transform.position;
			BoxCollider c = gameObject.GetComponent<BoxCollider>();
			pos.y += c.bounds.extents.y;
			flowerBox.transform.position = pos;

			// play grow animation
			Animation anim = flower.GetComponent<Animation>();
			anim.Play("Take 001");
		}
	}

	public void EnableGrowButton() {
		growButton.SetActive(true);
		Collider growCollider = growButton.GetComponent<Collider>();
		growCollider.enabled = true;
		Helper.TreeEnableRenderer(growButton);
	}
}
