using UnityEngine;
using System.Collections;

public class ManipulatorUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject target = TargetManager.Instance.GetTarget();
	    if (target == null)
        {
            // TODO:
        }
        else
        {
            gameObject.transform.parent = target.transform;
            MeshFilter meshFilter = target.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            gameObject.transform.localPosition = mesh.bounds.center;
            gameObject.transform.localScale = mesh.bounds.size;
        }
	}
}
