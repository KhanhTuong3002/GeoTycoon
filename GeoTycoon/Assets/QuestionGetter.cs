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
        using (UnityWebRequest request = UnityWebRequest.Get(URL + Qid.text))
        // using (UnityWebRequest request = UnityWebRequest.Get("https://localhost:7170/api/Question/id?id=4ba60f93-5f26-42a1-ba04-76330d77c2dc"))
        {
            yield return request.SendWebRequest();
            Debug.Log(request.result);
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Question question = new Question();
                string outPutHere = request.downloadHandler.text;
                Debug.Log(outPutHere);
                question = JsonConvert.DeserializeObject<Question>(request.downloadHandler.text);
                QuestionPanel.transform.GetChild(2).GetComponent<Text>().text = question.content;
                QuestionPanel.transform.GetChild(3).GetComponent<Text>().text = "A : " + question.option1;
                QuestionPanel.transform.GetChild(4).GetComponent<Text>().text = "B : " + question.option2;
                QuestionPanel.transform.GetChild(5).GetComponent<Text>().text = "C : " + question.option3;
                QuestionPanel.transform.GetChild(6).GetComponent<Text>().text = "D : " + question.option4;
                // QuestionPanel.transform.GetChild(7).GetComponent<Text>().text = "Answer : " + question.answer;
                // QuestionPanel.transform.GetChild(8).GetComponent<Text>().text = "Description : " + question.description;
            }
        }
    }
}
