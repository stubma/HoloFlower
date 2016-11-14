using UnityEngine;
using System.Collections;
using System.IO;

#if !UNITY_EDITOR
using System.Net.Http;
using System.Threading.Tasks;
#endif

public class NeoboxController : MonoBehaviour {
	[Tooltip("A hint canvas used to display hint message")]
	public GameObject hintCanvas;

	// custom log message
	string message;

	void Start () {
		// place hint canvas
		RectTransform tf = hintCanvas.GetComponent<RectTransform>();
		hintCanvas.transform.localPosition = new Vector3(0, tf.rect.height / 2 + 0.01f, 0);

		// hide hint canvas
		hintCanvas.SetActive(false);
	}

	public void Print() {
		// show hint canvas
		hintCanvas.SetActive(true);

		// send model to print
		SendModel();
	}

	private void SendModel() {
#if !UNITY_EDITOR
		string url = PrinterDetector.Instance.getUrl();
        message = "Start upload to " + url;
        UploadAndPrint(getContent(), url, getTransform());
#endif
	}

#if !UNITY_EDITOR
    [System.Serializable]
    public class DetectRespond
    {
        public int mesh_id;
    }

    async void UploadAndPrint(byte[] data, string url, PrintRequest request)
    {
        string respondString = await Upload(data, url);
        DetectRespond detectRespond = JsonUtility.FromJson<DetectRespond>(respondString);
        message = respondString + " decode: " + detectRespond.mesh_id.ToString();
        
        // wait for a while to ensure the model is processed
        await Task.Delay(3000);
        request.meshId = detectRespond.mesh_id;
        string json = JsonUtility.ToJson(request);
        string printRespond = await Print(json);
        message = message + printRespond;
    }

    async static Task<string> Upload(byte[] data, string url)
    {
        using (var client = new HttpClient())
        {
            using (var content = new MultipartFormDataContent("abcdefgabcdefgabcdefg"))
            {
                TextAsset asset = Resources.Load("teddydata") as TextAsset;

                content.Add(new StreamContent(new MemoryStream(asset.bytes)), "file", "teddy.stl");

                using (var message = await client.PostAsync(url, content))
                {
                    var input = await message.Content.ReadAsStringAsync();
                    return input;
                }
            }
        }
    }

    static byte[] getContent()
    {
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
    }

    IEnumerator sendModel(string url)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", getContent(), "filename.stl", "application/octet-stream");
        WWW www = new WWW(url, form);
        Debug.Log("upload");
        yield return www;
        if (www.error != null)
        {
            message = "Upload error " + www.error;
            Debug.Log(message);
        } else
        {
            message = "Uploaded";
            Debug.Log(message);
        }
    }

    [System.Serializable]
    public class PrintRequest
    {
        public int meshId;
        public double scale;
        public double[] translation;
        public double[] orientation;
    }

    private static PrintRequest getTransform()
    {
        PrintRequest request = new PrintRequest();
        GameObject target = TargetManager.Instance.Target;
        request.scale = target.transform.localScale.x;
        request.translation = new double[] { target.transform.localPosition.x, target.transform.localPosition.y, target.transform.localPosition.z };
        request.orientation = new double[] { target.transform.localRotation.w, target.transform.localRotation.x,
            target.transform.localRotation.y, target.transform.localRotation.z };
        return request;
    }

    // curl -d '{"meshId":1,"scale":0.2,"translation":[20,-50,0],"orientation":[0.6,0.8,0,0]}' http://192.168.1.116:8080/print/printer/print
    async static Task<string> Print(string json)
    {
        Debug.Log(json);
        string url = "http://" + PrinterDetector.Instance.address + ":8080/print/printer/print";
        Debug.Log(url);

        //*
        using (var client = new HttpClient())
        {
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
            {
                //Debug.Log(content.wr);
                string myContent = await content.ReadAsStringAsync();
                Debug.Log(myContent);
                using (var message = await client.PostAsync(url, content))
                {
                    var input = await message.Content.ReadAsStringAsync();
                    return input;
                }
            }
        }
        //*/

        /*
        //string jsonContent = "{\"color\":\"green\",\"message\":\"My first notification (yey)\",\"notify\":false,\"message_format\":\"text\"}";
        byte[] jsonBytes = System.Text.Encoding.ASCII.GetBytes(json);
        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://organization.hipchat.com/v2/room/2388080/notification?auth_token=authKeyHere");
        request.ContentType = "application/json";
        request.Method = "POST";
        request.ContentLength = jsonBytes.Length;

        using (var reqStream = request.GetRequestStream())
        {
            reqStream.Write(jsonBytes, 0, jsonBytes.Length);
            reqStream.Close();
        }

        System.Net.WebResponse response = request.GetResponse();
        */
    }

#endif
}
