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
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;

    private List<SetQuestion> questionSets;
    private int currentQuestionIndex;
    private List<Question> currentQuestions;
    private List<Question> duplicateQuestions;

    public void GetData()
    {
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL + Qid.text))
        {
            yield return request.SendWebRequest();
            Debug.Log(request.result);
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                questionSets = JsonConvert.DeserializeObject<List<SetQuestion>>(request.downloadHandler.text);
                // Debug.Log(request.downloadHandler.text);
                // foreach (SetQuestion s in questionSets)
                // {
                //     Debug.Log(s.SetName);
                //     foreach (Question q in s.questions)
                //     {
                //         Debug.Log(q.Title);
                //     }
                // }
                if (questionSets.Count > 0)
                {
                    
                    currentQuestions = questionSets[0].questions;
                    currentQuestionIndex = Random.Range(0, currentQuestions.Count);
                    duplicateQuestions = currentQuestions;
                    Debug.Log("question remaining: "+currentQuestions.Count);
                    Debug.Log(currentQuestions[currentQuestionIndex].Title);
                    Debug.Log("Random index: "+currentQuestionIndex);
                    DisplayQuestion(currentQuestionIndex);
                }
            }
        }
    }

    private void DisplayQuestion(int index)
    {
        if (currentQuestions == null || currentQuestions.Count == 0 || index < 0 || index >= currentQuestions.Count)
            return;

        Question test = currentQuestions[index];
        QuestionPanel.transform.GetChild(2).GetComponent<Text>().text = test.Content;
        OptionA.GetComponentInChildren<Text>().text = test.Option1;
        OptionB.GetComponentInChildren<Text>().text = test.Option2;
        OptionC.GetComponentInChildren<Text>().text = test.Option3;
        OptionD.GetComponentInChildren<Text>().text = test.Option4;

        OptionA.onClick.RemoveAllListeners();
        OptionB.onClick.RemoveAllListeners();
        OptionC.onClick.RemoveAllListeners();
        OptionD.onClick.RemoveAllListeners();

        OptionA.onClick.AddListener(() => CheckAnswerWrapper(test.Option1));
        OptionB.onClick.AddListener(() => CheckAnswerWrapper(test.Option2));
        OptionC.onClick.AddListener(() => CheckAnswerWrapper(test.Option3));
        OptionD.onClick.AddListener(() => CheckAnswerWrapper(test.Option4));
    }

    private void CheckAnswer(Question question, string selectedAnswer)
    {
        if (question.Answer == selectedAnswer)
        {
            Debug.Log("correct!");
            currentQuestions.Remove(question);
            currentQuestionIndex = Random.Range(0, currentQuestions.Count);
            Debug.Log("question remaining: " + currentQuestions.Count);
            Debug.Log(currentQuestions[currentQuestionIndex].Title);
            Debug.Log("Random index: "+currentQuestionIndex);
            // currentQuestionIndex++;
            if (currentQuestionIndex < currentQuestions.Count)
            {
                DisplayQuestion(currentQuestionIndex);
            }
            else
            {
                currentQuestions = duplicateQuestions;
                currentQuestionIndex = Random.Range(0, currentQuestions.Count);
                //Debug.Log("All questions answered correctly!");
                // Add any end of quiz logic here
            }
        }
        else
        {
            // Incorrect answer, so redisplay the current question
            DisplayQuestion(currentQuestionIndex);
        }
    }

    public void CheckAnswerWrapper(string selectedAnswer)
    {
        if (currentQuestions == null || currentQuestions.Count == 0 || currentQuestionIndex < 0 || currentQuestionIndex >= currentQuestions.Count)
            return;

        CheckAnswer(currentQuestions[currentQuestionIndex], selectedAnswer);
    }
}
