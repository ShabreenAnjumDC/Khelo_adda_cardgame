using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;

public class L_WebServices : MonoBehaviour
{
    public static L_WebServices instance;

    void Awake()
    {
        instance = this;
    }

	#region API
	//DEV_CODE
	public IEnumerator GETRequestData(string uri, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			{ "token", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : "") }
		};

		UnityWebRequest uwr = UnityWebRequest.Get(uri);

		Debug.Log("Request : " + uri);

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
		headerData.Clear();
		uwr.Dispose();
	}

	public IEnumerator POSTRequestData(string uri, string json, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			{ "token", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : "") }
		};

		UnityWebRequest uwr = new UnityWebRequest(uri, "POST");

		Debug.Log("Request : " + uri);

		if (json != "")
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		}

		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

		//uwr.SetRequestHeader("Authorization", (GlobalGameManager.playerToken.Length > 0 ? GlobalGameManager.playerToken : ""));
		uwr.SetRequestHeader("Content-Type", "application/json");

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}

	public IEnumerator PUTRequestData(string uri, string json, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			{ "token", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : "") }
		};

		UnityWebRequest uwr = new UnityWebRequest(uri, "PUT");

		Debug.Log("Request : " + uri);

		if (json != "")
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		}

		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

		uwr.SetRequestHeader("Authorization", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : ""));
		uwr.SetRequestHeader("Content-Type", "application/json");

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}

	public IEnumerator DeleteRequestData(string uri, System.Action<string, bool, string> callbackOnFinish)
	{
		Dictionary<string, string> headerData = new Dictionary<string, string>
		{
			{ "Authorization", (L_GlobalGameManager.playerToken.Length > 0 ? L_GlobalGameManager.playerToken : "") }
		};

		UnityWebRequest uwr = UnityWebRequest.Delete(uri);
		uwr.downloadHandler = new DownloadHandlerBuffer();
		Debug.Log("Request : " + uri);

		if (headerData.Count > 0)
		{
			foreach (var param in headerData)
			{
				uwr.SetRequestHeader(param.Key, param.Value);
			}
		}
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
		{
			callbackOnFinish("", true, uwr.error);
		}
		else
		{
			callbackOnFinish(uwr.downloadHandler.text, false, "");
		}
	}
	#endregion

	public IEnumerator LoadImageFromUrl(string URL, Image image, System.Action<bool, string> callbackOnFinish = null)
	{
		UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
		yield return unityWebRequest.SendWebRequest();

		if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.LogError("Download failed");
			callbackOnFinish(true, "Download failed");
		}
		else
		{
			Debug.Log("Texture Download Successfull");
			var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
			Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

			try
			{
				image.sprite = sprite;
				image.color = new Color32(255, 255, 255, 255);
				callbackOnFinish(false, "Download Successfull");
			}
			catch
			{
				Debug.Log("Error in set Image");
				callbackOnFinish(true, "Error in Download");
			}
		}
		unityWebRequest.Dispose();
	}

	public IEnumerator DownloadVideo(string url, string fileName)
	{
		UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
		Debug.Log("Downloading Video..");
		yield return unityWebRequest.SendWebRequest();

		if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(unityWebRequest.error);
		}
		else
		{
			if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
				Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

			File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "Videos", fileName), unityWebRequest.downloadHandler.data);
		}
		unityWebRequest.Dispose();
	}

	public bool IsInternetAvailable()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}

		return true;
	}
}
