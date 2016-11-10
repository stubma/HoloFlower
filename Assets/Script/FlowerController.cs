using UnityEngine;
using System.Collections;

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
