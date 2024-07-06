using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuestionGetter : MonoBehaviour
{
    public string URL;
    public GameObject QuizPanel;
    public Text QuestionText;
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;

    private List<SetQuestion> questionSets;
    private int currentQuestionIndex;
    private List<Question> currentQuestions;

    private void Start()
    {
        if (QuizPanel == null || QuestionText == null || OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần UI chưa được gán trong Inspector.");
            return;
        }

        string setID = GameSettings.SetID;
        if (string.IsNullOrEmpty(setID))
        {
            setID = "defaultSetID";  // Thay bằng SetID mặc định của bạn
        }
        GetData(setID);
    }

    public void GetData(string setID)
    {
        StartCoroutine(FetchData(setID));
        Debug.Log("****QuizManager receive your SetID:**** " + setID);
    }

    public IEnumerator FetchData(string setID)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL + setID))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                // Nếu có lỗi, sử dụng setID mặc định
                StartCoroutine(FetchData("defaultSetID"));  // Thay bằng SetID mặc định của bạn
            }
            else
            {
                questionSets = JsonConvert.DeserializeObject<List<SetQuestion>>(request.downloadHandler.text);
                if (questionSets != null && questionSets.Count > 0)
                {
                    currentQuestions = questionSets[0].questions;
                    currentQuestionIndex = Random.Range(0, currentQuestions.Count);
                    DisplayQuestion(currentQuestionIndex);
                }
                else
                {
                    Debug.Log("Không tìm thấy câu hỏi nào trong bộ câu hỏi.");
                    Debug.Log("****QuizManager receive your SetID:**** " + setID);
                }
            }
        }
    }

    private void DisplayQuestion(int index)
    {
        if (currentQuestions == null || currentQuestions.Count == 0 || index < 0 || index >= currentQuestions.Count)
        {
            Debug.LogWarning("Không có câu hỏi hoặc chỉ số không hợp lệ.");
            return;
        }

        if (QuizPanel == null || QuestionText == null || OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("QuizPanel hoặc một trong các thành phần không được gán.");
            return;
        }

        Question test = currentQuestions[index];
        QuestionText.text = test.Content;
        Debug.Log("Question: " + test.Content);

        Text optionAText = OptionA.GetComponentInChildren<Text>();
        Text optionBText = OptionB.GetComponentInChildren<Text>();
        Text optionCText = OptionC.GetComponentInChildren<Text>();
        Text optionDText = OptionD.GetComponentInChildren<Text>();

        if (optionAText == null || optionBText == null || optionCText == null || optionDText == null)
        {
            Debug.LogError("Không tìm thấy thành phần Text bên trong một hoặc nhiều nút Option.");
            return;
        }

        // Kiểm tra xem các tùy chọn có được lấy đúng cách không
        Debug.Log("Option1: " + test.Option1);
        Debug.Log("Option2: " + test.Option2);
        Debug.Log("Option3: " + test.Option3);
        Debug.Log("Option4: " + test.Option4);

        optionAText.text = test.Option1;
        optionBText.text = test.Option2;
        optionCText.text = test.Option3;
        optionDText.text = test.Option4;

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
            Debug.Log("Chính xác!");
            currentQuestions.Remove(question);
            if (currentQuestions.Count > 0)
            {
                currentQuestionIndex = Random.Range(0, currentQuestions.Count);
                DisplayQuestion(currentQuestionIndex);
            }
            else
            {
                //moi them cho nay nha
                currentQuestions = questionSets[0].questions;
                currentQuestionIndex = Random.Range(0, currentQuestions.Count);
                Debug.Log("Đã trả lời đúng tất cả các câu hỏi!");
                DisplayQuestion(currentQuestionIndex);
            }
        }
        else
        {
            Debug.Log("Trả lời sai");
            // Nếu sai, hiển thị lại câu hỏi hiện tại
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
