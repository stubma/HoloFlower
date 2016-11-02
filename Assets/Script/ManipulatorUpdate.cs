using UnityEngine;
using System.Collections;

public class ManipulatorUpdate : MonoBehaviour {
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
		if(TargetManager.Instance.Target != null) {
			gameObject.transform.parent = TargetManager.Instance.Target.transform;
			MeshFilter meshFilter = TargetManager.Instance.Target.GetComponent<MeshFilter>();
			Mesh mesh = meshFilter.mesh;
			gameObject.transform.localPosition = mesh.bounds.center;
			gameObject.transform.localScale = mesh.bounds.size;
		}
	}
}
