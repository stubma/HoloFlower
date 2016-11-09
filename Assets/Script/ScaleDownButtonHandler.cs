using UnityEngine;
using System.Collections;
using System;

public class ScaleDownButtonHandler : MonoBehaviour {
	// busy flag
	private bool isScaling = false;
	private float duration = 0.2f;
	private float time = 0;
	private Vector3 startScale;
	private Vector3 endScale;

	void OnSelect() {
		MainController mc = Camera.main.GetComponent<MainController>();
		GrowController gc = mc.surfaceBookPlaceholder.GetComponent<GrowController>();
		if(!gc.IsEditing) {
			isScaling = true;
			gc.IsEditing = true;
			startScale = gc.flowerBox.transform.localScale;
			endScale = startScale * 0.9f;
			time = 0;
		}
	}

	void Update() {
		if(isScaling) {
			time += Time.deltaTime;
			float t = Math.Min(1, time / duration);
			MainController mc = Camera.main.GetComponent<MainController>();
			GrowController gc = mc.surfaceBookPlaceholder.GetComponent<GrowController>();
			gc.flowerBox.transform.localScale = Vector3.Lerp(startScale, endScale, t);
			if(t >= 1) {
				isScaling = false;
				gc.IsEditing = false;
			}
		}
	}
}
