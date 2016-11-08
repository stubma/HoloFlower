﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {
	[Tooltip("Tooltip text to be shown")]
	public string tooltip;

	// ui text to display tooltip
	private Text tooltipText;

	void Start () {
		// create text
		tooltipText = CreateUIText(tooltip);

		// initially set alpha to 0
		tooltipText.GetComponent<CanvasRenderer>().SetAlpha(0);

		// place tooltip above button
		Bounds b = gameObject.GetComponent<Collider>().bounds;
		Vector3 pos = Vector3.zero;
		pos.y += b.size.y / 2;
		pos.y += tooltipText.GetComponent<RectTransform>().rect.height / 2 * tooltipText.transform.localScale.y;
		tooltipText.transform.localPosition = pos;
	}

	public void OnGazeEnter() {
		tooltipText.CrossFadeAlpha(1, 0.2f, true);
	}

	public void OnGazeLeave() {
		tooltipText.CrossFadeAlpha(0, 0.2f, true);
	}

	private Text CreateUIText(string str) {
		// create game object
		GameObject obj = new GameObject("tooltip");
		obj.transform.parent = transform;

		// create text component
		Text t = obj.AddComponent<Text>();
		t.text = str;
		t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		t.fontSize = 14;
		t.transform.localScale = new Vector3(0.004f, 0.0025f, 1);
		t.alignment = TextAnchor.MiddleCenter;

		// adjust size
		RectTransform rt = t.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(160, 30);

		// return
		return t;
	}
}
