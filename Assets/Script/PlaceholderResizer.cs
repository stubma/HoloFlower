using UnityEngine;
using System.Collections;

public class PlaceholderResizer : MonoBehaviour {
	[Tooltip("bottom left corner")]
	public GameObject cornerBL;

	[Tooltip("top left corner")]
	public GameObject cornerTL;

	[Tooltip("bottom right corner")]
	public GameObject cornerBR;

	[Tooltip("top right corner")]
	public GameObject cornerTR;

	[Tooltip("body object")]
	public GameObject body;

	[Tooltip("placeholder length")]
	public float length;

	[Tooltip("Placeholder width")]
	public float width;

	void Start() {
		// relayout
		cornerBL.transform.localPosition = new Vector3(length / 2, 0, -width / 2);
		cornerTL.transform.localPosition = new Vector3(-length / 2, 0, -width / 2);
		cornerTL.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
		cornerBR.transform.localPosition = new Vector3(length / 2, 0, width / 2);
		cornerBR.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
		cornerTR.transform.localPosition = new Vector3(-length / 2, 0, width / 2);
		cornerTR.transform.localRotation = Quaternion.Euler(new Vector3(0, -180, 0));
		body.transform.localPosition = Vector3.zero;
		body.transform.localRotation = Quaternion.identity;
		body.transform.localScale = new Vector3(length, 0.01f, width);

		// adjust box collider
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		collider.size = new Vector3(length, 0.01f, width);
	}
}
