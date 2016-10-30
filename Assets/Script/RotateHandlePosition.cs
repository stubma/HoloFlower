using UnityEngine;
using System.Collections;

public class RotateHandlePosition : MonoBehaviour {

    public Vector3 RelativePosition;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localPosition = RelativePosition / 2;
        Vector3 parentScale = gameObject.transform.parent.localScale;
        gameObject.transform.localScale = new Vector3(0.02f / parentScale.x, 0.02f / parentScale.y, 0.02f / parentScale.z);
    }
}
