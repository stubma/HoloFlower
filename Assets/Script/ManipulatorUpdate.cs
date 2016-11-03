using UnityEngine;
using System.Collections;

public class ManipulatorUpdate : MonoBehaviour {
	void Update() {
		if(TargetManager.Instance.Target != null) {
			gameObject.transform.parent = TargetManager.Instance.Target.transform;
			gameObject.transform.localPosition = new Vector3(0, 0.041f, 0);
			gameObject.transform.localScale = new Vector3(0.224f, 0.0824f, 0.25f);
		}
	}
}
