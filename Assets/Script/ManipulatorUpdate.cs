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

				// get grow controller
				MainController mc = Camera.main.GetComponent<MainController>();
				GrowController gc = mc.surfaceBookPlaceholder.GetComponent<GrowController>();

				// set manipulator position
				gameObject.transform.localPosition = new Vector3(0, gc.FlowerBound.size.y / 2, 0);

				// scale by target bound
				gameObject.transform.localScale = gc.FlowerBound.size;

				// set flag
				isManipulatorPlaced = true;
			}
		}
	}
}
