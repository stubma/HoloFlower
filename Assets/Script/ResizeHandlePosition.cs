using UnityEngine;
using System.Collections;

public class ResizeHandlePosition : MonoBehaviour {
	public Vector3 RelativePosition;

	void Update() {
		gameObject.transform.localPosition = RelativePosition / 2;
		Vector3 parentScale = gameObject.transform.parent.localScale;
		gameObject.transform.localScale = new Vector3(
			0.02f / parentScale.x,
			0.02f / parentScale.y,
			0.02f / parentScale.z);
	}
}
