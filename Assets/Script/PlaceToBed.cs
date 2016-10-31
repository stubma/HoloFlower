using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaceToBed : MonoBehaviour {

    public Text MessageText;
	private GameObject bed;

    // Use this for initialization
    void Start () {
		bed = GameObject.Find("Bed");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {	
        GameObject duplicate = Instantiate<GameObject>(gameObject);
        Destroy(duplicate.GetComponent<PlaceToBed>());

        Bounds bounds = duplicate.GetComponent<Renderer>().bounds;
        //bounds.min
        Vector3 modelBottomCenter = bounds.center;
        modelBottomCenter.y = bounds.min.y;
        Bounds bedBounds = bed.GetComponent<Renderer>().bounds;
        Vector3 bedTopCenter = bedBounds.center;
        bedTopCenter.y = bedBounds.min.y;
        duplicate.transform.position = duplicate.transform.position + (bedTopCenter - modelBottomCenter);

        duplicate.transform.parent = bed.transform;

        //duplicate.GetComponent<BoxCollider>().enabled = false;
        Interaction interaction = duplicate.AddComponent<Interaction>();
        interaction.MessageText = MessageText;
        TargetManager.Instance.SetTarget(duplicate);
    }
}
