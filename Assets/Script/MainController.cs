using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	[Tooltip("A canvas which holds hint message text")]
	public Canvas hintCanvas;

	[Tooltip("A text control which displays hint message")]
	public Text hintText;

	[Tooltip("A rectangular used to locate surface book position")]
	public GameObject surfaceBookPlaceholder;

	[Tooltip("A model used to locate neobox")]
	public GameObject neoboxPlaceholder;

	[Tooltip("A operation canvas which displays when user gazes surface book")]
	public Canvas surfaceOpCanvas;

	[Tooltip("Grow text label")]
	public Text growText;

	// canvas distance
	private float hintCanvasDistance;

	// hint move speed
	private float hintMoveSpeed = 0.5f;

	// is placing started?
	private bool isPlacing = false;

	/// <summary>
	/// current state of app 
	/// </summary>
	private enum OpState {
		IDLE = 0,
		LOCATE_SURFACE_BOOK = 1,
		LOCATE_NEOBOX = 2,
		GROW_FLOWER = 3
	}

	/// <summary>
	/// state of app
	/// </summary>
	private OpState state;

	void Start () {
		// distance of hint canvas
		hintCanvasDistance = hintCanvas.transform.position.magnitude; 

		// hide something
		HideNeoboxPlaceholder();

		// force remove old anchor so that we will re-locate them
		WorldAnchorManager.Instance.RemoveAnchor(surfaceBookPlaceholder);
		WorldAnchorManager.Instance.RemoveAnchor(neoboxPlaceholder);

		// init state
		SetState(OpState.LOCATE_SURFACE_BOOK);
	}

	void Update () {
		switch(state) {
		case OpState.GROW_FLOWER:
			// raycast to test whether hit surface book
			RaycastHit hitInfo;
			bool hit = Physics.Raycast(gameObject.transform.position, 
				           gameObject.transform.forward, 
				           out hitInfo,
				           surfaceBookPlaceholder.transform.position.magnitude * 2,
				           surfaceBookPlaceholder.layer);

			// if hit, show operation button
			// if not hit, hide operation buttion
			if(hit && hitInfo.collider.gameObject == surfaceBookPlaceholder) {
				Quaternion r = gameObject.transform.localRotation;
				r.x = 0;
				r.z = 0;
				surfaceOpCanvas.transform.localRotation = r;
				Vector3 pos = surfaceBookPlaceholder.transform.position;
				pos.y += 0.5f;
				surfaceOpCanvas.transform.position = pos;
				FadeInSurfaceOpCanvas();
			} else {
				FadeOutSurfaceOpCanvas();
			}
			break;
		default:
			// let hint canvas follow camera
			if(hintCanvas.gameObject.activeSelf) {
				Vector3 dstPos = gameObject.transform.forward * hintCanvasDistance;
				dstPos.y += 3f;
				Vector3 srcPos = hintCanvas.transform.position;
				float dist = (dstPos - srcPos).magnitude;
				if(dist > 0) {
					hintCanvas.transform.position = Vector3.Lerp(srcPos, dstPos, hintMoveSpeed / dist);
				}
				hintCanvas.transform.localRotation = gameObject.transform.localRotation;
			}
			break;
		}
	}

	void LateUpdate() {
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			if(!isPlacing) {
				TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
				ttp.PlacingEnd += MainController_onPlacingEnd;
				ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
				isPlacing = true;
			}
			break;
		case OpState.LOCATE_NEOBOX:
			if(!isPlacing) {
				TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
				ttp.PlacingEnd += MainController_onPlacingEnd;
				ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
				isPlacing = true;
			}
			break;
		}
	}

	private void HideSurfaceBookPlaceholder() {
		Renderer r = surfaceBookPlaceholder.GetComponent<Renderer>();
		r.enabled = false;
	}

	private void HideNeoboxPlaceholder() {
		neoboxPlaceholder.SetActive(false);
	}

	private void ShowNeoboxPlaceholder() {
		neoboxPlaceholder.SetActive(true);
	}

	private void HideSurfaceOpCanvas() {
		surfaceOpCanvas.gameObject.SetActive(false);
	}

	private void FadeInSurfaceOpCanvas() {
		surfaceOpCanvas.gameObject.SetActive(true);
		growText.GetComponent<CanvasRenderer>().SetAlpha(0);
		growText.CrossFadeAlpha(1f, 0.2f, true);
	}

	private void FadeOutSurfaceOpCanvas() {
		growText.GetComponent<CanvasRenderer>().SetAlpha(1);
		growText.CrossFadeAlpha(0, 0.2f, true);
	}

	private void MainController_onPlacingEnd() {
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			{
				// reset flag and remove listener
				isPlacing = false;
				TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
				ttp.PlacingEnd -= MainController_onPlacingEnd;
				Destroy(ttp);
				ShowNeoboxPlaceholder();

				// switch state to locate neobox
				SetState(OpState.LOCATE_NEOBOX);
				break;
			}
		case OpState.LOCATE_NEOBOX:
			{
				// reset flag and remove listener
				isPlacing = false;
				TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
				ttp.PlacingEnd -= MainController_onPlacingEnd;
				Destroy(ttp);

				// to idle state
				SetState(OpState.GROW_FLOWER);
				break;
			}
		}
	}

	private void SetState(OpState s) {
		state = s;
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			// show hint
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Your Surface Book";
			isPlacing = false;
			break;
		case OpState.LOCATE_NEOBOX:
			hintCanvas.gameObject.SetActive(true);
			hintText.text = "Locate Neobox";
			isPlacing = false;
			break;
		default:
			hintCanvas.gameObject.SetActive(false);
			break;
		}
	}
}
