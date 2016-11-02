using UnityEngine;
using System.Collections;

public class GrowController : MonoBehaviour {
	[Tooltip("Flower object")]
	public GameObject flower;

	// Use this for initialization
	void Start () {
		flower.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnSelect() {
		if(!flower.activeSelf) {
			// get surface placeholder
			MainController mc = Camera.main.GetComponent<MainController>();
			GameObject surfaceBookPlaceholder = mc.surfaceBookPlaceholder;

			// place flower on it
			flower.SetActive(true);
			flower.transform.position = surfaceBookPlaceholder.transform.position;
			flower.transform.localRotation = surfaceBookPlaceholder.transform.localRotation;

			// play grow animation
		}
	}
}
