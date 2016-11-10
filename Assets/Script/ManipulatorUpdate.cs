using UnityEngine;
using System.Collections;

public class ManipulatorUpdate : MonoBehaviour {
	// is manipulator placed
	private bool isManipulatorPlaced = false;

	void Update() {
		if(TargetManager.Instance.Target != null) {
			if(!isManipulatorPlaced) {
				// set target as manipulator's parent
				gameObject.transform.parent = TargetManager.Instance.Target.transform;

				// get flower controller
				MainController mc = Camera.main.GetComponent<MainController>();
				SBPlaceholderController gc = mc.surfaceBookPlaceholder.GetComponent<SBPlaceholderController>();
				FlowerController fc = gc.flowerBox.GetComponent<FlowerController>();

				// set manipulator position
				gameObject.transform.localPosition = new Vector3(0, fc.FlowerBound.size.y / 2, 0);

				// scale by target bound
				gameObject.transform.localScale = fc.FlowerBound.size;

				// set flag
				isManipulatorPlaced = true;
			}
		}
	}
}
