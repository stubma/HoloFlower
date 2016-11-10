using UnityEngine;
using System.Collections;

public class ManipulatorUpdate : MonoBehaviour {
	void Update() {
		if(TargetManager.Instance.Target != null) {
			// get flower controller
			MainController mc = Camera.main.GetComponent<MainController>();
			SBPlaceholderController sbpc = mc.surfaceBookPlaceholder.GetComponent<SBPlaceholderController>();
			FlowerController fc = sbpc.flowerBox.GetComponent<FlowerController>();

			// set manipulator position
			Vector3 pos = sbpc.flowerBox.transform.position;
			pos.y += fc.FlowerBound.size.y / 2;
			gameObject.transform.position = pos;

			// scale by target bound
			Vector3 scale = Vector3.Scale(sbpc.flowerBox.transform.localScale, fc.FlowerBound.size);
			gameObject.transform.localScale = scale;

			// rotation
			Quaternion rotation = sbpc.flowerBox.transform.localRotation;
			rotation.x = 0;
			rotation.z = 0;
			gameObject.transform.localRotation = rotation;
		}
	}
}
