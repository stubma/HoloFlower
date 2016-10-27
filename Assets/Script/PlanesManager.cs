using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;

public class PlanesManager : Singleton<PlanesManager> {
	[Tooltip("How much time (in seconds) that the SurfaceObserver will run after being started; used when 'Limit Scanning By Time' is checked.")]  
	public float scanTime = 15.0f;

	[Tooltip("Material to use when rendering Spatial Mapping meshes while the observer is running.")]  
	public Material defaultMaterial;

	[Tooltip("Optional Material to use when rendering Spatial Mapping meshes after the observer has been stopped.")]  
	public Material secondaryMaterial;
	  
	[Tooltip("Minimum number of floor planes required in order to exit scanning/processing mode.")]  
	public uint minimumFloors = 0;
	  
	[Tooltip("Minimum number of wall planes required in order to exit scanning/processing mode.")]  
	public uint minimumWalls = 0;

	[Tooltip("Minimum number of table planes required in order to exit scanning/processing mode.")]  
	public uint minimumTables = 0;
	  
	/// <summary>  
	/// is mesh processed 
	/// </summary>  
	private bool meshesProcessed = false;

	/// <summary>
	/// horizontal planes
	/// </summary>
	private List<GameObject> horizontalPlanes;

	/// <summary>
	/// vertical planes
	/// </summary>
	private List<GameObject> verticalPlanes;

	public PlanesManager() {
		horizontalPlanes = new List<GameObject>();
		verticalPlanes = new List<GameObject>();
	}

	public List<GameObject> HorizontalPlanes {
		get { 
			return horizontalPlanes;
		}
	}

	public List<GameObject> VerticalPlanes {
		get {
			return verticalPlanes;
		}
	}

	private void Start() {  
		// render mesh with default material  
		SpatialMappingManager.Instance.SetSurfaceMaterial(defaultMaterial);  

		// register for plane completed event 
		SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;  
	}

	private void Update() {
		if(!meshesProcessed) {
			if((Time.time - SpatialMappingManager.Instance.StartTime) >= scanTime) {
				// stop scanning
				if(SpatialMappingManager.Instance.IsObserverRunning()) {  
					SpatialMappingManager.Instance.StopObserver();  
				}  

				// create planes 
				CreatePlanes(); 

				// set flag  
				meshesProcessed = true;  
			}  
		}  
	}

	/// <summary>  
	/// plane complete event handler
	/// </summary>  
	/// <param name="source">事件源</param>  
	/// <param name="args">事件参数</param>  
	private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args) {  
		// clear old planes
		horizontalPlanes.Clear();
		verticalPlanes.Clear();

		// get planes
		if(minimumTables > 0) {
			horizontalPlanes.AddRange(SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table));
		}
		if(minimumFloors > 0) {
			horizontalPlanes.AddRange(SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor));
		}
		if(minimumWalls > 0) {
			verticalPlanes = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);  
		}

		// if we get enough planes, go ahead
		// if not, restart scanning
		if(horizontalPlanes.Count >= (minimumFloors + minimumTables) && verticalPlanes.Count >= minimumWalls) {  
			// remove vertices to make clear plane  
			RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);  

			// render plane with other material
			SpatialMappingManager.Instance.SetSurfaceMaterial(secondaryMaterial);  
		} else {  
			SpatialMappingManager.Instance.StartObserver();
			meshesProcessed = false;  
		}  
	}

	/// <summary>  
	/// create planes from mesh
	/// </summary>  
	private void CreatePlanes() {  
		SurfaceMeshesToPlanes surfaceToPlanes = SurfaceMeshesToPlanes.Instance;  
		if(surfaceToPlanes != null && surfaceToPlanes.enabled) {  
			surfaceToPlanes.MakePlanes();  
		}  
	}

	/// <summary>  
	/// remove vertices of plane
	/// </summary>  
	/// <param name="boundingObjects"></param>  
	private void RemoveVertices(IEnumerable<GameObject> boundingObjects) {  
		RemoveSurfaceVertices removeVerts = RemoveSurfaceVertices.Instance;  
		if(removeVerts != null && removeVerts.enabled) {  
			removeVerts.RemoveSurfaceVerticesWithinBounds(boundingObjects);  
		}  
	}

	/// <summary>
	/// get lowest horizontal plane
	/// </summary>
	/// <returns>The lowest horizontal plane or null if there is no horizontal plane</returns>
	public GameObject GetLowestHorizontalPlane() {
		float minY = float.MaxValue;
		GameObject lowest = null;
		foreach(GameObject plane in horizontalPlanes) {
			if(minY > plane.transform.position.y) {
				minY = plane.transform.position.y;
				lowest = plane;
			}
		}
		return lowest;
	}

	/// <summary>  
	/// unregister event handler when destory
	/// </summary>  
	private void OnDestroy() {  
		if(SurfaceMeshesToPlanes.Instance != null) {  
			SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;  
		}  
	}
}