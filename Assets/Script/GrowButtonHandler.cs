﻿using UnityEngine;
using System.Collections;

public class GrowButtonHandler : MonoBehaviour {
	public void OnSelect() {
		// grow the flower
		MainController mc = Camera.main.GetComponent<MainController>();
		GameObject sb = mc.surfaceBookPlaceholder;
		GrowController gc = sb.GetComponent<GrowController>();
		gc.GrowFlower();

		// hide button
		gameObject.SetActive(false);
	}
}
