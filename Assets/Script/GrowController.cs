using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GrowController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

	[Tooltip("Flower scale anchor as a flower container")]
	public GameObject flowerAnchor;

	[Tooltip("Grow button, user click to grow flower")]
	public GameObject growButton;

	[Tooltip("Canvas which holds edit buttons for flower")]
	public GameObject editCanvas;

	// is flower growed
	public bool IsGrowed {
		get {
			return flowerAnchor.activeSelf;
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

		// place and hide edit canvas
		RectTransform editTransform = editCanvas.GetComponent<RectTransform>();
		editCanvas.transform.localPosition = new Vector3(length / 2 + editTransform.rect.width / 2 + 0.05f, editTransform.rect.height / 2, 0);
		editCanvas.SetActive(false);

		// hide flower
		flowerAnchor.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(IsGrowAnimationDone) {
			// make flower target if grow animation is done
			TargetManager.Instance.Target = flowerAnchor;

			// show edit canvas
			editCanvas.SetActive(true);
			Helper.TreeEnableRenderer(editCanvas);
		}
	}

	public void GrowFlower() {
		if(!IsGrowed) {
			// place flower on it
			flowerAnchor.SetActive(true);
			flowerAnchor.transform.localRotation = gameObject.transform.localRotation;
			Vector3 pos = gameObject.transform.position;
			BoxCollider c = gameObject.GetComponent<BoxCollider>();
			pos.y += c.bounds.extents.y;
			flowerAnchor.transform.position = pos;

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
