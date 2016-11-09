using UnityEngine;
using System.Collections;
using System;

public class PlaceholderController : MonoBehaviour {
	[Tooltip("body object")]
	public GameObject body;

	// for fading out body
	private bool isFadingOutBody = false;
	private float duration = 1.0f;
	private Color startColor;
	private Color endColor;
	private float startTime = 0;

	void Update() {
		// fade out body if flag is set
		if(isFadingOutBody) {
			startTime += Time.deltaTime;
			startTime = Math.Min(startTime, duration);
			body.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, startTime / duration);
			if(startTime >= duration) {
				isFadingOutBody = false;
			}
		}
	}

	/// <summary>
	/// Fade out body part
	/// </summary>
	public void FadeOutBody() {
		isFadingOutBody = true;
		startColor = body.GetComponent<Renderer>().material.color;
		endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
	}
}
