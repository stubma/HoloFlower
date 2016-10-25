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
	public uint minimumFloors = 1;
	  
	[Tooltip("Minimum number of wall planes required in order to exit scanning/processing mode.")]  
	public uint minimumWalls = 1;
	  
	/// <summary>  
	/// is mesh processed 
	/// </summary>  
	private bool meshesProcessed = false;

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
		// plane list
		List<GameObject> horizontal = new List<GameObject>();  
		List<GameObject> vertical = new List<GameObject>();  

		// get planes
		horizontal = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table | PlaneTypes.Floor);  
		vertical = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);  

		// if we get enough planes, go ahead
		// if not, restart scanning
		if(horizontal.Count >= minimumFloors && vertical.Count >= minimumWalls) {  
			// remove vertices to make clear plane  
			RemoveVertices(SurfaceMeshesToPlanes.Instance.ActivePlanes);  

			// render plane with other material
			SpatialMappingManager.Instance.SetSurfaceMaterial(secondaryMaterial);  

			// generate items in found plane 
			ObjectCollectionManager.Instance.GenerateItemsInWorld(horizontal, vertical);  
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
	/// unregister event handler when destory
	/// </summary>  
	private void OnDestroy() {  
		if(SurfaceMeshesToPlanes.Instance != null) {  
			SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= SurfaceMeshesToPlanes_MakePlanesComplete;  
		}  
	}
}