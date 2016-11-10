using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HoloToolkit.Unity;

public class GazeableColorPicker : MonoBehaviour {
	// color picker object
	public Renderer rendererComponent;

	// color callback definition
	[System.Serializable]
	public class PickedColorCallback : UnityEvent<Color> {
	}

	// callback when gaze a color
	public PickedColorCallback OnGazedColor = new PickedColorCallback();

	// callback when select a color
	public PickedColorCallback OnPickedColor = new PickedColorCallback();

	private bool gazing = false;

	void OnGazeEnter() {
		gazing = true;
	}

	void OnGazeLeave() {
		gazing = false;
	}

	void OnSelect() {
		UpdatePickedColor(OnPickedColor);
	}

	void Update() {
		if(gazing == false)
			return;
		UpdatePickedColor(OnGazedColor);
	}

	void UpdatePickedColor(PickedColorCallback cb) {
		// if not hit, return
		RaycastHit hit = GazeManager.Instance.HitInfo;
		if(hit.transform.gameObject != rendererComponent.gameObject) {
			return;
		}
            
		// get color from texture pixel and callback
		Texture2D texture = rendererComponent.material.mainTexture as Texture2D;
		Vector2 pixelUV = hit.textureCoord;
		pixelUV.x *= texture.width;
		pixelUV.y *= texture.height;
		Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
		cb.Invoke(col);
	}
}