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
		Helper.TreeDisableRenderer(neoboxPlaceholder);
		Helper.TreeDisableRenderer(surfaceBookPlaceholder);

		// init state
		SetState(OpState.IDLE);
	}

	public void StartLocateSurfaceBook() {
		if(state != OpState.LOCATE_SURFACE_BOOK) {
			SetState(OpState.LOCATE_SURFACE_BOOK);
			Helper.TreeEnableRenderer(surfaceBookPlaceholder);
			TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingEnd += MainController_onPlacingEnd;
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void StartLocateNeobox() {
		if(state != OpState.LOCATE_NEOBOX) {
			SetState(OpState.LOCATE_NEOBOX);
			Helper.TreeEnableRenderer(neoboxPlaceholder);
			TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingEnd += MainController_onPlacingEnd;
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void MainController_onPlacingEnd() {
		lock(this) {
			switch(state) {
			case OpState.LOCATE_SURFACE_BOOK:
				{
					// remove TapToPlace to disable placing function
					TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
					Destroy(ttp);

					// fade out body
					PlaceholderController pc = surfaceBookPlaceholder.GetComponent<PlaceholderController>();
					pc.FadeOutBody();

					break;
				}
			case OpState.LOCATE_NEOBOX:
				{
					// remove TapToPlace to disable placing function
					TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
					Destroy(ttp);

					// fade out body
					PlaceholderController pc = neoboxPlaceholder.GetComponent<PlaceholderController>();
					pc.FadeOutBody();

					// enable grow button
					SBPlaceholderController gc = surfaceBookPlaceholder.GetComponent<SBPlaceholderController>();
					gc.EnableGrowButton();

					// hide locate panel
					locatePanel.gameObject.SetActive(false);

					// to idle state
					SetState(OpState.IDLE);

					break;
				}
			}
		}
	}

	private void SetState(OpState s) {
		state = s;
	}
}
