using UnityEngine;
using System.Collections;

public class NeoboxController : MonoBehaviour {
	[Tooltip("A hint canvas used to display hint message")]
	public GameObject hintCanvas;

	void Start () {
		// place hint canvas
		RectTransform tf = hintCanvas.GetComponent<RectTransform>();
		hintCanvas.transform.localPosition = new Vector3(0, tf.rect.height / 2 + 0.01f, 0);

		// hide hint canvas
		hintCanvas.SetActive(false);
	}

	public void Print() {
		// show hint canvas
		hintCanvas.SetActive(true);
	}
}
