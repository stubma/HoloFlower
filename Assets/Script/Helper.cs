using UnityEngine;
using System.Collections;

public class Helper {
	public static void TreeDisableRenderer(GameObject root) {
		if(root != null) {
			Renderer[] rList = root.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in rList) {
				r.enabled = false;
			}
		}
	}

	public static void TreeEnableRenderer(GameObject root) {
		if(root != null) {
			Renderer[] rList = root.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in rList) {
				r.enabled = true;
			}
		}
	}
}
