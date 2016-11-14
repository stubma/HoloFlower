using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SBController : MonoBehaviour {
	[Tooltip("Flower container to hold flower animation and set custom scale anchor")]
	public GameObject flowerBox;

	[Tooltip("Grow button, user click to grow flower")]
	public GameObject growButton;

	[Tooltip("Canvas which holds edit buttons for flower")]
	public GameObject editCanvas;

	[Tooltip("Canvas on which user select color for flower")]
	public GameObject colorCanvas;

	// Use this for initialization
	void Start () {
		// place and hide grow button
		Collider growCollider = growButton.GetComponent<Collider>();
		Bounds growBound = growCollider.bounds;
		PlaceholderResizer pr = gameObject.GetComponent<PlaceholderResizer>();
		float length = pr.length;
		float width = pr.width;
		growButton.transform.localPosition = new Vector3(0, growBound.size.y / 2, width / 2 + growBound.size.z / 2 + 0.02f);
		growButton.transform.localScale = new Vector3(length / growBound.size.x, length / growBound.size.x / 2, 1);
		growCollider.enabled = false;
		growButton.SetActive(false);

		// hide something
		editCanvas.SetActive(false);
		colorCanvas.SetActive(false);

		// hide flower
		flowerBox.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		FlowerController fc = flowerBox.GetComponent<FlowerController>();
		if(fc.IsGrowAnimationDone) {
			// make flower target if grow animation is done
			TargetManager.Instance.Target = flowerBox;

			// show edit canvas
			if(!editCanvas.activeSelf) {
				editCanvas.SetActive(true);
				Helper.TreeEnableRenderer(editCanvas);
			}

			// show color canvas
			if(!colorCanvas.activeSelf) {
				colorCanvas.SetActive(true);
				Helper.TreeEnableRenderer(colorCanvas);
			}
		}
	}

	void LateUpdate() {
		FlowerController fc = flowerBox.GetComponent<FlowerController>();
		if(fc.IsGrowAnimationDone) {
			// get flower size, scaled
			// multiply with sqrt 2 for 45 degree situation
			Vector3 flowerScale = flowerBox.transform.localScale;
			float flowerSize = flowerScale.x * fc.FlowerBound.size.x;
			flowerSize *= (float)Math.Sqrt(2);

			// get placeholder length
			PlaceholderResizer pr = gameObject.GetComponent<PlaceholderResizer>();
			float length = pr.length;

			// update edit canvas position
			if(editCanvas.activeSelf) {
				RectTransform tf = editCanvas.GetComponent<RectTransform>();
				editCanvas.transform.localPosition = new Vector3(Math.Max(length, flowerSize) / 2 + tf.rect.width / 2 + 0.05f, 
					tf.rect.height / 2, 0);
			}

			// update color canvas position
			if(colorCanvas.activeSelf) {
				RectTransform tf = colorCanvas.GetComponent<RectTransform>();
				colorCanvas.transform.localPosition = new Vector3(-Math.Max(length, flowerSize) / 2 - tf.rect.width / 2 - 0.05f, 
					tf.rect.height / 2, 0);
			}
		}
	}

	public void GrowFlower() {
		flowerBox.SetActive(true);
		FlowerController fc = flowerBox.GetComponent<FlowerController>();
		fc.Grow();
	}

	public void EnableGrowButton() {
		growButton.SetActive(true);
		Collider growCollider = growButton.GetComponent<Collider>();
		growCollider.enabled = true;
		Helper.TreeEnableRenderer(growButton);
	}
}
