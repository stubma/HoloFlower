using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MainController : MonoBehaviour {
	// lowest table
	private GameObject table;

	// Use this for initialization
	void Start () {
		// register for plane completed event 
		SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;  
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>  
	/// unregister event handler when destory
	/// </summary>  
	private void OnDestroy() {  
		if(SurfaceMeshesToPlanes.Instance != null) {  
			SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;  
		}  
	}

	private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args) {  
		// show lowest horizontal plane
		table = PlanesManager.Instance.GetLowestHorizontalPlane();
		if(table != null) {
			SurfacePlane plane = table.GetComponent<SurfacePlane>();
			plane.IsVisible = true;
		}
	}
}
