using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	[Tooltip("A canvas which holds locate operation buttons")]
	public Canvas locatePanel;

	[Tooltip("A rectangular used to locate surface book position")]
	public GameObject surfaceBookPlaceholder;

	[Tooltip("A model used to locate neobox")]
	public GameObject neoboxPlaceholder;

	/// <summary>
	/// current state of app 
	/// </summary>
	private enum OpState {
		IDLE = 0,
		LOCATE_SURFACE_BOOK = 1,
		LOCATE_NEOBOX = 2
	}

	/// <summary>
	/// state of app
	/// </summary>
	private OpState state;

	void Start () {
		// place locate panel before user
		float locatePanelDistance = locatePanel.transform.position.magnitude; 
		Vector3 dstPos = gameObject.transform.forward * locatePanelDistance;
		locatePanel.transform.position = dstPos;
		locatePanel.transform.localRotation = gameObject.transform.localRotation;

		// hide something
		HideNeoboxPlaceholder();
		HideSurfaceBookPlaceholder();

		// init state
		SetState(OpState.IDLE);
	}

	public void StartLocateSurfaceBook() {
		if(state != OpState.LOCATE_SURFACE_BOOK) {
			SetState(OpState.LOCATE_SURFACE_BOOK);
			ShowSurfaceBookPlaceholder();
			TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingEnd += MainController_onPlacingEnd;
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void StartLocateNeobox() {
		if(state != OpState.LOCATE_NEOBOX) {
			SetState(OpState.LOCATE_NEOBOX);
			ShowNeoboxPlaceholder();
			TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingEnd += MainController_onPlacingEnd;
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void TreeDisableRenderer(GameObject root) {
		Renderer[] rList = root.GetComponentsInChildren<Renderer>();
		foreach(Renderer r in rList) {
			r.enabled = false;
		}
	}

	private void TreeEnableRenderer(GameObject root) {
		Renderer[] rList = root.GetComponentsInChildren<Renderer>();
		foreach(Renderer r in rList) {
			r.enabled = true;
		}
	}

	private void HideSurfaceBookPlaceholder() {
		if(surfaceBookPlaceholder != null) {
			TreeDisableRenderer(surfaceBookPlaceholder);
		}
	}

	private void ShowSurfaceBookPlaceholder() {
		if(surfaceBookPlaceholder != null) {
			TreeEnableRenderer(surfaceBookPlaceholder);
		}
	}

	private void HideNeoboxPlaceholder() {
		if(neoboxPlaceholder != null) {
			TreeDisableRenderer(neoboxPlaceholder);
		}
	}

	private void ShowNeoboxPlaceholder() {
		if(neoboxPlaceholder != null) {
			TreeEnableRenderer(neoboxPlaceholder);
		}
	}

	private void MainController_onPlacingEnd() {
		lock(this) {
			switch(state) {
			case OpState.LOCATE_SURFACE_BOOK:
				{
					// remove TapToPlace to disable placing function
					TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
					ttp.PlacingEnd -= MainController_onPlacingEnd;
					Destroy(ttp);
					break;
				}
			case OpState.LOCATE_NEOBOX:
				{
					// remove TapToPlace to disable placing function
					TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
					ttp.PlacingEnd -= MainController_onPlacingEnd;
					Destroy(ttp);

					// to idle state
					SetState(OpState.IDLE);

					// hide locate panel
					locatePanel.gameObject.SetActive(false);

					break;
				}
			}
		}
	}

	private void SetState(OpState s) {
		state = s;
	}
}
