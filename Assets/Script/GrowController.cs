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

	// flag indicating all placeholders are placed
	public bool IsPlaced {
		get;
		set;
	}

	// is flower growed
	private bool IsGrowed {
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
		// place grow button
		Bounds b = growButton.GetComponent<Collider>().bounds;
		PlaceholderResizer pr = gameObject.GetComponent<PlaceholderResizer>();
		float length = pr.length;
		float width = pr.width;
		growButton.transform.localPosition = new Vector3(0, b.size.y / 2, width / 2 + b.size.z / 2 + 0.01f);

		// hide flower
		flowerAnchor.SetActive(false);

		// init flag
		IsPlaced = false;
	}
	
	// Update is called once per frame
	void Update () {
		// make flower target if grow animation is done
		if(IsGrowAnimationDone) {
			TargetManager.Instance.Target = flowerAnchor;
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
}
