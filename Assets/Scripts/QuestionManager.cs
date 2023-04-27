using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Ins;
    public QuestionSciptableData[] questionData;

    List<QuestionSciptableData> questionsDataList;
    QuestionSciptableData currentQuestion;

    public QuestionSciptableData CurrentQuestion { get => currentQuestion; set => currentQuestion = value; }

    private void Awake()
    {
        questionsDataList = questionData.ToList();

        //Debug.Log("Get Random Question Function: " + GetRandomQuestion().question);
        MakeSingleton();
    }

    public QuestionSciptableData GetRandomQuestion()
    {
        if (questionsDataList != null && questionsDataList.Count() > 0)
        {
            int randomIndex = Random.Range(0, questionsDataList.Count());

            currentQuestion= questionsDataList[randomIndex];

            questionsDataList.RemoveAt(randomIndex);

        }

        return currentQuestion;
    }

    public void MakeSingleton()
    {
        if (Ins == null)
            Ins = this;
        else
            Destroy(gameObject);
    }
}
