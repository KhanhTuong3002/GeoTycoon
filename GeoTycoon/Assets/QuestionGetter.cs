using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuestionGetter : MonoBehaviour
{
    public string URL;
    public InputField Qid;
    public GameObject QuestionPanel;
    public void GetData()
    {
        StartCoroutine(FetchData());
    }
    public IEnumerator FetchData()
    {
        // using (UnityWebRequest request = UnityWebRequest.Get(URL + Qid.text))
        using (UnityWebRequest request = UnityWebRequest.Get("https://localhost:7170/api/Set/default"))
        {
            yield return request.SendWebRequest();
            Debug.Log(request.result);
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var set = new List<SetQuestion>();
                set = JsonConvert.DeserializeObject<List<SetQuestion>>(request.downloadHandler.text);
                
                foreach (SetQuestion s in set){
                    Debug.Log(s.SetName);
                    var q = new List<Question>();
                    q = s.questions;
                    foreach (Question q2 in q){
                        Debug.Log(q2.Content);
                    }
                }
                // string outPutHere = request.downloadHandler.text;
                // Debug.Log(outPutHere);
                // question = JsonConvert.DeserializeObject<Question>(request.downloadHandler.text);
                // QuestionPanel.transform.GetChild(2).GetComponent<Text>().text = question.Content;
                // QuestionPanel.transform.GetChild(3).GetComponent<Text>().text = "A : " + question.Option1;
                // QuestionPanel.transform.GetChild(4).GetComponent<Text>().text = "B : " + question.Option2;
                // QuestionPanel.transform.GetChild(5).GetComponent<Text>().text = "C : " + question.Option3;
                // QuestionPanel.transform.GetChild(6).GetComponent<Text>().text = "D : " + question.Option4;
                // QuestionPanel.transform.GetChild(7).GetComponent<Text>().text = "Answer : " + question.answer;
                // QuestionPanel.transform.GetChild(8).GetComponent<Text>().text = "Description : " + question.description;
            }
        }
    }
}
