using UnityEngine;
using System.Collections;

public class PrintButtonHandler : MonoBehaviour {
	void OnSelect() {
		// get neobox controller
		MainController mc = Camera.main.GetComponent<MainController>();
		NeoboxController nc = mc.neoboxPlaceholder.GetComponent<NeoboxController>();

		// print
		nc.Print();
	}
}
