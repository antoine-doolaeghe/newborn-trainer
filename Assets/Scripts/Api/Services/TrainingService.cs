using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using EasyBuildSystem.Runtimes.Internal.Storage;

[ExecuteInEditMode]
public class TrainingService : MonoBehaviour
{
  public string responseUuid;

  public static Dictionary<string, string> variable = new Dictionary<string, string>();
  public static Dictionary<string, string[]> array = new Dictionary<string, string[]>();

  private static String graphQlInput;

  public delegate void PostModelCallback(Transform transform, WWW www, GameObject agent);

  public IEnumerator GetObject(string trainerKey)
  {
    byte[] postData;
    Dictionary<string, string> postHeader;
    TrainingService.variable["key"] = "\"" + trainerKey + "\"";

    WWW www;
    ServiceHelpers.graphQlApiRequest(variable, array, out postData, out postHeader, out www, out graphQlInput, ApiConfig.downloadTrainerData, ApiConfig.apiKey, ApiConfig.url);
    yield return www;
    if (www.error != null)
    {
      throw new Exception("There was an error sending request: " + www.error + www.text);
    }
    else
    {
      JSONNode responseData = JSON.Parse(www.text)["data"]["downloadTrainer"];
      if (responseData == null)
      {
        throw new Exception("There was an error sending request: " + www.text);
      }
      Debug.Log("Training Instance successfully launched");
      StartCoroutine(transform.GetComponent<BuildStorage>().LoadDataFile(responseData));
    }
  }

  public static IEnumerator UpdateTrainerData(string trainerKey, string trainerData)
  {
    byte[] postData;
    Dictionary<string, string> postHeader;
    trainerData = trainerData.Replace("\"", "'");
    TrainingService.variable["key"] = "\"" + trainerKey + "\"";
    TrainingService.variable["data"] = "\"" + trainerData + "\"";

    WWW www;
    ServiceHelpers.graphQlApiRequest(variable, array, out postData, out postHeader, out www, out graphQlInput, ApiConfig.updateTrainerData, ApiConfig.apiKey, ApiConfig.url);
    Debug.Log(graphQlInput);
    yield return www;
    if (www.error != null)
    {
      throw new Exception("There was an error sending request: " + www.error + www.text);
    }
    else
    {
      JSONNode responseData = JSON.Parse(www.text)["data"]["updateTrainer"];
      if (responseData == null)
      {
        throw new Exception("There was an error sending request: " + www.text);
      }
      Debug.Log("Training Instance successfully launched");
      yield return "hello";
    }
  }
}
