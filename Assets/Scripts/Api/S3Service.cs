using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.CognitoIdentity;
using System.Collections;
using Amazon;
using EasyBuildSystem.Runtimes.Internal.Storage;
using System.Collections;
using UnityEngine.Networking;
public class S3Service : MonoBehaviour
{
  public string S3Region = RegionEndpoint.USEast1.SystemName;
  private RegionEndpoint _S3Region
  {
    get { return RegionEndpoint.GetBySystemName(S3Region); }
  }
  public string S3BucketName = "newborn-trainer-data";

  void Awake()
  {
    UnityInitializer.AttachToGameObject(this.gameObject);
    AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
  }

  #region private members

  private IAmazonS3 _s3Client;
  private AWSCredentials _credentials;

  private AWSCredentials Credentials
  {
    get
    {
      if (_credentials == null)
        _credentials = new CognitoAWSCredentials(
            "eu-west-1:7eb0715c-0ce5-476b-9ffc-b60dec05e8ab", // ID du groupe d'identités
            RegionEndpoint.EUWest1 // Région
        );
      return _credentials;
    }
  }

  private IAmazonS3 Client
  {
    get
    {
      if (_s3Client == null)
      {
        _s3Client = new AmazonS3Client(Credentials, _S3Region);
      }
      //test comment
      return _s3Client;
    }
  }

  #endregion
  public IEnumerator GetObject()
  {
    UnityWebRequest www = UnityWebRequest.Get("https://newborn-trainer-data.s3-eu-west-1.amazonaws.com/test.txt");
    yield return www.SendWebRequest();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.error);
    }
    else
    {
      Debug.Log(www.downloadHandler.data);
      Debug.Log(www.downloadHandler.text);
      StartCoroutine(transform.GetComponent<BuildStorage>().LoadDataFile(www.downloadHandler.text));
    }
  }

  public IEnumerator PostObject(Stream stream, string fileName)
  {

    Debug.Log(stream.Length);

    stream.Position = 0;


    var request = new PostObjectRequest()
    {
      Bucket = S3BucketName,
      Key = fileName,
      InputStream = stream,
      Region = _S3Region
    };

    Client.PostObjectAsync(request, (responseObj) =>
    {
      Debug.Log(responseObj.Response);
      Debug.Log(responseObj.Exception);
      responseObj.Response.HttpStatusCode.ToString();
    });

    yield return null;
  }

  #region helper methods


  private string GetPostPolicy(string bucketName, string key, string contentType)
  {
    bucketName = bucketName.Trim();

    key = key.Trim();
    // uploadFileName cannot start with /
    if (!string.IsNullOrEmpty(key) && key[0] == '/')
    {
      throw new ArgumentException("uploadFileName cannot start with / ");
    }

    contentType = contentType.Trim();

    if (string.IsNullOrEmpty(bucketName))
    {
      throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
    }
    if (string.IsNullOrEmpty(key))
    {
      throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
    }
    if (string.IsNullOrEmpty(contentType))
    {
      throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
    }

    string policyString = null;
    int position = key.LastIndexOf('/');
    if (position == -1)
    {
      policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
          bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
    }
    else
    {
      policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
          bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
    }

    return policyString;
  }

}

#endregion
