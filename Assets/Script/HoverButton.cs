using UnityEngine;
using System.Collections;

public class HoverButton : MonoBehaviour {
	[Tooltip("Normal image name")]
	public string normalImage;

	[Tooltip("Hover image name")]
	public string hoverImage;

	public void OnGazeEnter() {
		Texture2D tex = Resources.Load<Texture2D>(hoverImage);
		gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
	}

	public void OnGazeLeave() {
		Texture2D tex = Resources.Load<Texture2D>(normalImage);
		gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
	}
}
