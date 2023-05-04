using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Ins;
    public QuestionSciptableData[] questionData;
    private List<QuestionSciptableData> questionsDataList;
    private QuestionSciptableData currentQuestion;
    //private int suffleTimes = 2;

    QuestionSciptableData CurrentQuestion { get => currentQuestion; set => currentQuestion = value; }

    private void Awake()
    {
        questionsDataList = questionData.ToList();
        MakeSingleton();
    }

    public QuestionSciptableData GetRandomQuestion()
    {

     /*   for(int i  = 0; i <  questionsDataList.Count; i++ )
        {
            currentQuestion = questionsDataList[i];
            int randomIndex = Random.Range(i, questionsDataList.Count);
            questionsDataList[i] = questionsDataList[randomIndex];
            questionsDataList[randomIndex] = currentQuestion;
        }*/

            int randomNumber = Random.Range(0, questionsDataList.Count);
            currentQuestion = questionsDataList[randomNumber];
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
