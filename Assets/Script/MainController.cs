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

	// is surface located
	public bool IsSBLocated {
		get;
		set;
	}

	// is neobox located
	public bool IsNeoboxLocated {
		get;
		set;
	}

	void Start () {
		// hide something
		Helper.TreeDisableRenderer(neoboxPlaceholder);
		Helper.TreeDisableRenderer(surfaceBookPlaceholder);

		// add delegate
		{
			TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingStart += MainController_onPlacingStart;
			ttp.PlacingEnd += MainController_onPlacingEnd;
		}
		{
			TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
			ttp.PlacingStart += MainController_onPlacingStart;
			ttp.PlacingEnd += MainController_onPlacingEnd;
		}

		// init state
		SetState(OpState.IDLE);
	}

	void OnBecameVisible() {
		// place locate panel before user
		float locatePanelDistance = locatePanel.transform.position.magnitude; 
		Vector3 dstPos = gameObject.transform.forward * locatePanelDistance;
		locatePanel.transform.position = dstPos;
		locatePanel.transform.localRotation = gameObject.transform.localRotation;
	}

	public void StartLocateSurfaceBook() {
		if(state != OpState.LOCATE_SURFACE_BOOK) {
			// change state
			SetState(OpState.LOCATE_SURFACE_BOOK);

			// start to place
			Helper.TreeEnableRenderer(surfaceBookPlaceholder);
			TapToPlace ttp = surfaceBookPlaceholder.GetComponent<TapToPlace>();
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void StartLocateNeobox() {
		if(state != OpState.LOCATE_NEOBOX) {
			// change state
			SetState(OpState.LOCATE_NEOBOX);

			// start to place
			Helper.TreeEnableRenderer(neoboxPlaceholder);
			TapToPlace ttp = neoboxPlaceholder.GetComponent<TapToPlace>();
			ttp.SendMessage("OnSelect", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void MainController_onPlacingStart(GameObject target) {
		// reset something when placing starts
		if(target == surfaceBookPlaceholder) {
			SetState(OpState.LOCATE_SURFACE_BOOK);
			IsSBLocated = false;
			PlaceholderController pc = surfaceBookPlaceholder.GetComponent<PlaceholderController>();
			pc.ResetBody();
		} else if(target == neoboxPlaceholder) {
			SetState(OpState.LOCATE_NEOBOX);
			IsNeoboxLocated = false;
			PlaceholderController pc = neoboxPlaceholder.GetComponent<PlaceholderController>();
			pc.ResetBody();
		}
	}

	private void MainController_onPlacingEnd() {
		// place
		switch(state) {
		case OpState.LOCATE_SURFACE_BOOK:
			{
				// fade out body
				PlaceholderController pc = surfaceBookPlaceholder.GetComponent<PlaceholderController>();
				pc.FadeOutBody();

				// flag
				IsSBLocated = true;

				// to idle state
				SetState(OpState.IDLE);

				break;
			}
		case OpState.LOCATE_NEOBOX:
			{
				// fade out body
				PlaceholderController pc = neoboxPlaceholder.GetComponent<PlaceholderController>();
				pc.FadeOutBody();

				// flag
				IsNeoboxLocated = true;

				// to idle state
				SetState(OpState.IDLE);

				break;
			}
		}

		// if end, hide locate panel
		if(IsSBLocated && IsNeoboxLocated) {
			// remove TapToPlace to disable placing function
			Destroy(surfaceBookPlaceholder.GetComponent<TapToPlace>());

			// remove TapToPlace to disable placing function
			Destroy(neoboxPlaceholder.GetComponent<TapToPlace>());

			// enable grow button
			SBController sbc = surfaceBookPlaceholder.GetComponent<SBController>();
			sbc.EnableGrowButton();

			// hide locate panel
			locatePanel.gameObject.SetActive(false);
		}
	}

	private void SetState(OpState s) {
		state = s;
	}
}
