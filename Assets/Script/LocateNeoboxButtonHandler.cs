using UnityEngine;
using System.Collections;

public class LocateNeoboxButtonHandler : MonoBehaviour {
	public void OnSelect() {
		MainController mc = Camera.main.GetComponent<MainController>();
		mc.StartLocateNeobox();
	}
}
