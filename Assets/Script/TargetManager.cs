using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class TargetManager : Singleton<TargetManager> {
	[Tooltip("A box which contains handler to scale/rotate model")]
	public GameObject manipulator;

	// model to be printed
	private GameObject target;

	// property
	public GameObject Target {
		get {
			return target;
		}
		set {
			target = value;
			manipulator.SetActive(target != null);
		}
	}

	// Use this for initialization
	void Start() {
		manipulator.SetActive(false);
	}
	
	// Update is called once per frame
	void Update() {
	}
}
