using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiConfig : ScriptableObject
{
  public static string url = "https://sw2hs7ufb5gevarvuyswhrndjm.appsync-api.eu-west-1.amazonaws.com/graphql";
  public static string apiKey = "da2-5aipqbxwx5eyfkoain3jhxtigm";
  public static string updateTrainerData = "query updateTrainerData {updateTrainer(trainerKey: $key^, trainerData: $data^)}";
  public static string downloadTrainerData = "query update {downloadTrainer(trainerKey: $key^)}";
}