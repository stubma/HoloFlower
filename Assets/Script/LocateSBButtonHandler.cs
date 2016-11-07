using UnityEngine;
using System.Collections;

public class LocateSBButtonHandler : MonoBehaviour {
	public void OnSelect() {
		MainController mc = Camera.main.GetComponent<MainController>();
		mc.StartLocateSurfaceBook();
	}
}
