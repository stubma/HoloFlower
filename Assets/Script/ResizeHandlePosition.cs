using UnityEngine;
using System.Collections;

public class ResizeHandlePosition : MonoBehaviour {
	public Vector3 RelativePosition;
	
	// Update is called once per frame
	void Update() {
		gameObject.transform.localPosition = RelativePosition / 2;
		Vector3 parentScale = gameObject.transform.parent.localScale;
		gameObject.transform.localScale = new Vector3(
			0.02f / parentScale.x,
			0.02f / parentScale.y,
			0.02f / parentScale.z);
	}

	public Vector3 GetResizeAnchorPosition() {
		return gameObject.transform.parent.TransformPoint(-RelativePosition / 2);
	}
}
