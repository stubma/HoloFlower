using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadModel : MonoBehaviour {
    public Text MessageText;

	// Use this for initialization
	void Start () {
        Mesh mesh = Instantiate(Resources.Load("teddy2", typeof(Mesh))) as Mesh;
       
        Debug.Log(mesh.vertexCount);

        GameObject instance = new GameObject();
        
        MeshFilter meshFilter = instance.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        //instance.transform.localScale = new Vector3(.1f, .1f, .1f);
        instance.transform.position = new Vector3(1, 0f, 2.0f);

        MeshRenderer meshRenderer = instance.AddComponent<MeshRenderer>();
        Material material = Instantiate(Resources.Load("Materials/defaultMat")) as Material;
        meshRenderer.material = material;

        BoxCollider boxCollider = instance.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.10f, 0.08f, 0.12f); // local space
        boxCollider.center = new Vector3(0, 0, 0); // local space

        //Interaction interaction = instance.AddComponent<Interaction>();
        //interaction.MessageText = MessageText;

        PlaceToBed placeToBed = instance.AddComponent<PlaceToBed>();
        placeToBed.MessageText = MessageText;

        //HoloToolkit.Unity.GestureManipulator manipulator = instance.AddComponent<HoloToolkit.Unity.GestureManipulator>();
    }
	
	// Update is called once per frame
	void Update () {
	}
}
