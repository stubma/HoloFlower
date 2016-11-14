using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {
	[Tooltip("Tooltip text to be shown")]
	public string tooltip;

	[Tooltip("Tooltip text y axis offset")]
	public float offsetY;

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
		pos.y += b.size.y / gameObject.transform.localScale.y / 2;
		pos.y += tooltipText.GetComponent<RectTransform>().rect.height / 2 * tooltipText.transform.localScale.y;
		pos.y += offsetY;
		tooltipText.transform.localPosition = pos;
	}

	public void OnGazeEnter() {
		tooltipText.CrossFadeAlpha(1, 0.2f, true);
	}

	public void OnGazeLeave() {
		tooltipText.CrossFadeAlpha(0, 0.2f, true);
	}

	void OnEnable() {
		// initially set alpha to 0
		if(tooltipText != null) {
			tooltipText.GetComponent<CanvasRenderer>().SetAlpha(0);
		}
	}

	private Text CreateUIText(string str) {
		// create game object
		GameObject obj = new GameObject("tooltip");
		obj.transform.parent = transform;
		obj.transform.localRotation = Quaternion.Inverse(obj.transform.parent.localRotation);
		Vector3 parentScale = obj.transform.parent.localScale;

		// create text component
		Text t = obj.AddComponent<Text>();
		t.text = str;
		t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		t.fontSize = 14;
		t.transform.localScale = new Vector3(0.002f / parentScale.x, 0.002f / parentScale.y, 1 / parentScale.z);
		t.alignment = TextAnchor.MiddleCenter;

		// adjust size
		RectTransform rt = t.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(160, 30);

		// return
		return t;
	}
}
