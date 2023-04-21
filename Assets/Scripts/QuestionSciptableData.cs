using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="QuestionData")]
public class QuestionSciptableData : ScriptableObject
{
    public string question;
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    public string correctAnswer;
    public bool isHardQuestion = false;
}
