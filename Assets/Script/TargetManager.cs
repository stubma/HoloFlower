using UnityEngine;
using System.Collections;

public class TargetManager : MonoBehaviour {

    public GameObject Manipulator;

    public static TargetManager Instance { get; private set; }
    private GameObject target;

	// Use this for initialization
	void Start () {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        Manipulator.SetActive(target != null);
    }

    public void SetTarget(GameObject t)
    {
        target = t;
    }

    public GameObject GetTarget()
    {
        return target;
    }
}
