using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

#if !UNITY_EDITOR
using System.Net.Http;
using System.Threading.Tasks;
#endif

public class NeoboxController : MonoBehaviour {
	[Tooltip("A hint canvas used to display hint message")]
	public GameObject hintCanvas;

	[Tooltip("A text to display more detail hint message")]
	public Text hintText;

	[Tooltip("A text to display hint title")]
	public Text hintTitle;

	// hint message string
	private string hintMsg = "";
	private int hintTitleDots = 0;
	private float hintTitleTime = 0;

	// is printing?
	private bool isPrinting = false;

	void Start() {
		// init 
		hintTitle.text = "";

		// place hint canvas
		RectTransform tf = hintCanvas.GetComponent<RectTransform>();
		hintCanvas.transform.localPosition = new Vector3(0, tf.rect.height / 2 + 0.01f, 0);

		// listen print animation event
		MainController mc = Camera.main.GetComponent<MainController>();
		SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();
		FlowerController fc = sbc.flowerBox.GetComponent<FlowerController>();
		fc.PrintAnimationEnd += NeoboxController_PrintAnimationEnd;
	}

	void Update() {
		// update hint message
		// if no hint message, show printer ip if detected and located
		hintText.text = hintMsg;
		if(hintMsg.Length <= 0) {
			MainController mc = Camera.main.GetComponent<MainController>();
			if(mc.IsNeoboxLocated && PrinterDetector.Instance.IsDetectionSuccess) {
				hintText.text = "printer detected: " + PrinterDetector.Instance.PrinterAddress;
			}
		}

		// update hint title
		if(isPrinting) {
			hintTitleTime += Time.deltaTime;
			if(hintTitleTime >= 1) {
				hintTitle.text = "Printing";
				for(int i = 0; i < hintTitleDots; i++) {
					hintTitle.text += ".";
				}
				hintTitleDots++;
				hintTitleDots %= 4;
				hintTitleTime = 0;
			}
		}
	}

	void OnDestroy() {
		// remove self from delegate list
		MainController mc = Camera.main.GetComponent<MainController>();
		SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();
		FlowerController fc = sbc.flowerBox.GetComponent<FlowerController>();
		fc.PrintAnimationEnd -= NeoboxController_PrintAnimationEnd;
	}

	public void Print() {
		// play start animation
		MainController mc = Camera.main.GetComponent<MainController>();
		SBController sbc = mc.surfaceBookPlaceholder.GetComponent<SBController>();
		FlowerController fc = sbc.flowerBox.GetComponent<FlowerController>();
		fc.PlayPrintAnimation();
	}

	private void NeoboxController_PrintAnimationEnd() {
		// send model to print
		isPrinting = true;
		hintTitle.text = "Printing";
		SendModel();
	}

	private void SendModel() {
#if !UNITY_EDITOR
		UploadAndPrint(GetTargetSTL(), PrinterDetector.Instance.ModelUploadUrl, BuildPrintRequest());
#endif
	}

#if !UNITY_EDITOR
    [System.Serializable]
    public class DetectRespond {
        public int mesh_id;
    }

    async void UploadAndPrint(byte[] data, string url, PrintRequest request) {
		// upload model data, in stl format
		hintMsg = "uploading model";
        string respondString = await Upload(data, url);
        DetectRespond detectRespond = JsonUtility.FromJson<DetectRespond>(respondString);
		
		// log
		hintMsg = "model uploaded, mesh id is: " + detectRespond.mesh_id.ToString();

        // wait for a while to ensure the model is uploaded, then send print request
		// the print request should carry a mesh id which is returned from uploading
        await Task.Delay(3000);
        request.meshId = detectRespond.mesh_id;
        string json = JsonUtility.ToJson(request);
		hintMsg = "sending print request";
        string printRespond = await Print(json);
		
		// log
		hintMsg = "printing in progress";
    }

    async Task<string> Upload(byte[] data, string url) {
        using (var client = new HttpClient()) {
            using (var content = new MultipartFormDataContent("abcdefgabcdefgabcdefg")) {
				content.Add(new StreamContent(new MemoryStream(data)), "file", "flower.stl");
                using (var message = await client.PostAsync(url, content)) {
                    var input = await message.Content.ReadAsStringAsync();
                    return input;
                }
            }
        }
    }

	// get stl format for target to be printed
	byte[] GetTargetSTL(){
		// get flower data, in stl file
		TextAsset asset = Resources.Load("rose.stl") as TextAsset;
		return asset.bytes;

		// get hardcoded test data, stl format, it is a cube
		/*
        string c = "solid block100" +
            "   facet normal -1.000000e+000 0.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 1.000000e+002 1.000000e+002" +
            "         vertex 0.000000e+000 1.000000e+002 0.000000e+000" +
            "         vertex 0.000000e+000 0.000000e+000 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "   facet normal -1.000000e+000 0.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 0.000000e+000 1.000000e+002" +
            "         vertex 0.000000e+000 1.000000e+002 0.000000e+000" +
            "         vertex 0.000000e+000 0.000000e+000 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 0.000000e+000 1.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 1.000000e+002 1.000000e+002" +
            "         vertex 0.000000e+000 1.000000e+002 1.000000e+002" +
            "         vertex 1.000000e+002 0.000000e+000 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 0.000000e+000 1.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 0.000000e+000 1.000000e+002" +
            "         vertex 0.000000e+000 1.000000e+002 1.000000e+002" +
            "         vertex 0.000000e+000 0.000000e+000 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 1.000000e+000 0.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 1.000000e+002 0.000000e+000" +
            "         vertex 1.000000e+002 1.000000e+002 1.000000e+002" +
            "         vertex 1.000000e+002 0.000000e+000 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 1.000000e+000 0.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 0.000000e+000 0.000000e+000" +
            "         vertex 1.000000e+002 1.000000e+002 1.000000e+002" +
            "         vertex 1.000000e+002 0.000000e+000 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 0.000000e+000 -1.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 1.000000e+002 0.000000e+000" +
            "         vertex 1.000000e+002 1.000000e+002 0.000000e+000" +
            "         vertex 0.000000e+000 0.000000e+000 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 0.000000e+000 -1.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 0.000000e+000 0.000000e+000" +
            "         vertex 1.000000e+002 1.000000e+002 0.000000e+000" +
            "         vertex 1.000000e+002 0.000000e+000 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 1.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 1.000000e+002 1.000000e+002" +
            "         vertex 1.000000e+002 1.000000e+002 0.000000e+000" +
            "         vertex 0.000000e+000 1.000000e+002 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 1.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 1.000000e+002 1.000000e+002" +
            "         vertex 1.000000e+002 1.000000e+002 0.000000e+000" +
            "         vertex 0.000000e+000 1.000000e+002 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 -1.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 1.000000e+002 0.000000e+000 0.000000e+000" +
            "         vertex 1.000000e+002 0.000000e+000 1.000000e+002" +
            "         vertex 0.000000e+000 0.000000e+000 0.000000e+000" +
            "      endloop" +
            "   endfacet" +
            "   facet normal 0.000000e+000 -1.000000e+000 0.000000e+000" +
            "      outer loop" +
            "         vertex 0.000000e+000 0.000000e+000 0.000000e+000" +
            "         vertex 1.000000e+002 0.000000e+000 1.000000e+002" +
            "         vertex 0.000000e+000 0.000000e+000 1.000000e+002" +
            "      endloop" +
            "   endfacet" +
            "endsolid";
       	return System.Text.Encoding.ASCII.GetBytes(c);
		*/
    }

    [System.Serializable]
    public class PrintRequest {
        public int meshId;
        public double scale;
        public double[] translation;
        public double[] orientation;
    }

    private static PrintRequest BuildPrintRequest() {
        PrintRequest request = new PrintRequest();
        GameObject target = TargetManager.Instance.Target;
        request.scale = target.transform.localScale.x;
        request.translation = new double[] { target.transform.localPosition.x, target.transform.localPosition.y, target.transform.localPosition.z };
		request.orientation = new double[] { Helper.d2r(target.transform.localRotation.eulerAngles.x), 
			Helper.d2r(target.transform.localRotation.eulerAngles.y), 
			Helper.d2r(target.transform.localRotation.eulerAngles.z),
			0 };
        return request;
    }

    async Task<string> Print(string json) {
		// the print api entry url
		// usage:
		// curl -d '{"meshId":1,"scale":0.2,"translation":[20,-50,0],"orientation":[0.6,0.8,0,0]}' http://192.168.1.116:8080/print/printer/print
		string url = PrinterDetector.Instance.PrintUrl;

		// send print request
        using (var client = new HttpClient()) {
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")) {
                string myContent = await content.ReadAsStringAsync();
                using (var message = await client.PostAsync(url, content)){
                    var input = await message.Content.ReadAsStringAsync();
                    return input;
                }
            }
        }
    }

#endif
}
