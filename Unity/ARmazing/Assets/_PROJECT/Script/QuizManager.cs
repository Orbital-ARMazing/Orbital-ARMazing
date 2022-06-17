using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using System.IO;
using Newtonsoft.Json;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;
    public string filename;

    public TMP_Text QuestionTxt;

    private void Start()
    {
        filename = "testquiz.json";
        QnA = ReadQuizFromJSON(filename);
        GenerateQuestion();
    }

    public void NextQuestion()
    {
        QnA.RemoveAt(currentQuestion);
        if (QnA.Count <= 0)
        {
            LoaderUtility.Initialize();
            SceneManager.LoadScene("AR", LoadSceneMode.Single);
        }
        GenerateQuestion();
        EnableOptions();
    }

    public void DisableOptions()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<Button>().interactable = false;
        }
    }

    public void EnableOptions()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<Button>().interactable = true;
        }
    }

    void SetAnswers()
    {
        options[0].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].option1;
        options[1].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].option2;
        options[2].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].option3;
        options[3].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].option4;

        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerButtonBehaviour>().isCorrect = false;

            if(QnA[currentQuestion].answer == i + 1)
            {
                options[i].GetComponent<AnswerButtonBehaviour>().isCorrect = true;
            }
        }
    }

    void GenerateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count);

        QuestionTxt.text = QnA[currentQuestion].question;
        SetAnswers();
    }

    List <QuestionAndAnswers> ReadQuizFromJSON(string filename)
    {
        string content = ReadFile(GetPath(filename));
        Debug.Log(content);
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return null;
        }

        QuizJSONObj res = JsonConvert.DeserializeObject<QuizJSONObj>(content);
        return res.msg;
    }

    string GetPath(string filename)
    {
        return "Assets/_PROJECT/Data/JSON/" + filename;
    }

    string ReadFile(string path)
    {
        if(File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
    
}

public class QuizJSONObj
{
    public bool status;
    public string error;
    public List<QuestionAndAnswers> msg;
}

public class QuestionAndAnswers
{
    public string id;
    public string eventName;
    public string asset;
    public string eventID;
    public string assetID;
    public string question;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    public int answer;
    public int points;
    public bool visible;
}
