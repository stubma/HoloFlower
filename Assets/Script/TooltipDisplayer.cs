using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TooltipDisplayer : MonoBehaviour {
	[Tooltip("A text control to display tooltip")]
	public Text tooltipText;

	[Tooltip("Tooltip text to be shown")]
	public string tooltip;

	void Start () {
		tooltipText.text = tooltip;
		tooltipText.GetComponent<CanvasRenderer>().SetAlpha(0);
		Bounds b = gameObject.GetComponent<Collider>().bounds;
		Vector3 pos = gameObject.transform.position;
		pos.y += b.size.y / 2;
		pos.y += tooltipText.GetComponent<RectTransform>().rect.height / 2 * tooltipText.transform.localScale.y;
		tooltipText.transform.position = pos;
	}

	public void OnGazeEnter() {
		tooltipText.CrossFadeAlpha(1, 0.2f, true);
	}

	public void OnGazeLeave() {
		tooltipText.CrossFadeAlpha(0, 0.2f, true);
	}
}
