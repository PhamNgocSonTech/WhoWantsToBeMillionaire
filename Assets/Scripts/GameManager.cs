﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Gameplay
{
  
    //Dùng Enum để định nghĩa các State
    public enum GameState
    {
        HOME,
        GAMEPLAY,
        GAMEOVER,
        WINGAME
    }

    public class GameManager : MonoBehaviour
    {
        //khai báo trường câu hỏi
        [SerializeField] private TextMeshProUGUI textQuestion;

        //khai báo trường 4 đáp án trả lời
        [SerializeField] private TextMeshProUGUI textAnswerA;
        [SerializeField] private TextMeshProUGUI textAnswerB;
        [SerializeField] private TextMeshProUGUI textAnswerC;
        [SerializeField] private TextMeshProUGUI textAnswerD;

        //khai báo trường hình ảnh cho 4 đáp án
        [SerializeField] private Image imgAnswerA;
        [SerializeField] private Image imgAnswerB;
        [SerializeField] private Image imgAnswerC;
        [SerializeField] private Image imgAnswerD;

        [SerializeField] private TextMeshProUGUI Score;
        [SerializeField] private TextMeshProUGUI HighScoreGameplay;
        [SerializeField] private TextMeshProUGUI HighScoreMenu;
        [SerializeField] private TextMeshProUGUI HighScoreGameOver;
        [SerializeField] private TextMeshProUGUI HighScoreWin;
        [SerializeField] private TextMeshProUGUI TotalScore;
        [SerializeField] private TextMeshProUGUI ScoreOver;
        [SerializeField] private TextMeshProUGUI GameLives;
        [SerializeField] private TextMeshProUGUI CountQuest;
        [SerializeField] private TextMeshProUGUI TimeRemain;

        //khai báo trường button với các màu tương ứng
        [SerializeField] private Sprite buttonGreen;
        [SerializeField] private Sprite buttonYellow;
        [SerializeField] private Sprite buttonBlack;

        //khai báo trường quản lý âm thanh
        [SerializeField] private AudioSource audioSource;

        //khai báo trường để lưu audio tương ứng với mỗi trạng thái nhất định (đúng/sai/nhạc nền)
        [SerializeField] private AudioClip musicMain;
        [SerializeField] private AudioClip sfxWrongAnswer;
        [SerializeField] private AudioClip sfxCorrectAnswer;
            
        //[SerializeField] private QuestionData[] questionData;

        //khai báo mảng chứa các câu hỏi có kiểu dữ liệu là một ScriptableObject
        [SerializeField] private QuestionSciptableData[] questionData;

        //Khai báo các trường Panel sẽ được kích hoạt tương ứng với mỗi State
        [SerializeField] private GameObject homePanel, gameplayPanel, gameOverPanel, winGamePanel;

        //Tạo một biến để lưu vị trí câu hỏi hiện tại
        private int questionIndex;

        //Tạo một biến để lưu State hiện tại
        private GameState gameState;

        //Biến lưu số lần chơi 
        private int gameLives = 3;

        //Biến lưu số điểm
        private int gameScore = 0;

        //
        private float timeValue = 10;

        // Start is called before the first frame update
        void Start()
        {
            HomeState();
            ResetValueQuest(0);
            //showCountQuest();
            /* questionIndex = 0;
             CreateQuestion(0);*/
            /* int a = randomIndexQuest();
             Debug.Log("Random"+a);*/
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Index Start: " + questionIndex);
            //countTime();
            Invoke("CountTime", 2f);



        }

        public void ClickAnswer(string selectorAnswer)
        {
            bool isCorrect = false;
            //int countCorrectQuest = 0;
            int sizeQuestionData = questionData.Length;


            if (questionData[questionIndex].correctAnswer == selectorAnswer)
            {
                isCorrect = true;
                /* countCorrectQuest++;
                 Debug.Log("Count Correct: " + countCorrectQuest);*/
                if (questionData[questionIndex].isHardQuestion)
                {
                    audioSource.PlayOneShot(sfxCorrectAnswer);
                    AddBonusScore();
                }
                else
                {
                    audioSource.PlayOneShot(sfxCorrectAnswer);
                    AddScore();
                }
            }
            else
            {
                gameLives--;
                if (gameLives == 0)
                {
                    GameOverState();
                    audioSource.Stop();
                    Debug.Log("You Lose");
                    return;
                }
                else if(gameLives > 0 && questionIndex == (sizeQuestionData - 1)) 
                {
                    WinGameState();
                    Debug.Log("You Win");
                    return;
                }
                audioSource.PlayOneShot(sfxWrongAnswer);
                GameLives.text = "Lives: " + gameLives.ToString();
                isCorrect = false;
                SubScore(isCorrect, gameScore);
                Invoke("NextQuestion", 2f);
            }
            
            switch (selectorAnswer)
            {
                case "a":
                    imgAnswerA.sprite = isCorrect ? buttonGreen : buttonYellow; 
                   //imgAnswerA.color = isCorrect ? Color.green : Color.red;
                    break;
                case "b":
                    imgAnswerB.sprite = isCorrect ? buttonGreen : buttonYellow;
                    //imgAnswerB.color = isCorrect ? Color.green : Color.red;
                    break;
                case "c":
                    imgAnswerC.sprite = isCorrect ? buttonGreen : buttonYellow;
                    //imgAnswerC.color = isCorrect ? Color.green : Color.red;
                    break;
                case "d":
                    imgAnswerD.sprite = isCorrect ? buttonGreen : buttonYellow;
                    //imgAnswerD.color = isCorrect ? Color.green : Color.red;
                    break;
            }

            if (isCorrect)
            {
                if (questionIndex == (sizeQuestionData - 1))
                {
                    Invoke("WinGameState", 1f);
                    //WinGameState();
                    Debug.Log("You Win");
                    return;
                }
                //Invoke sẽ làm chậm quá trình chuyển đổi việc đổi câu hỏi
                Invoke("NextQuestion", 2f);
            }
        }

        private void NextQuestion()
        {
            questionIndex++;
            ShowCountQuest(questionIndex);
            CreateQuestion(questionIndex);
        }

        private void CreateQuestion(int paramIndex)
        {
            if (paramIndex < 0 || paramIndex >= questionData.Length)
                return;
            /* imgAnswerA.color = Color.white;
             imgAnswerB.color = Color.white;
             imgAnswerC.color = Color.white;
             imgAnswerD.color = Color.white;*/

            imgAnswerA.sprite = buttonBlack;
            imgAnswerB.sprite = buttonBlack;
            imgAnswerC.sprite = buttonBlack;
            imgAnswerD.sprite = buttonBlack;
            //textQuestion.text = questionData[questionIndex].question;
            if (!questionData[questionIndex].isHardQuestion)
            {
                textQuestion.text = questionData[questionIndex].question;
                textQuestion.color = Color.green;
            }
            else
            {
                textQuestion.text = questionData[questionIndex].question;
                textQuestion.color = Color.red;

            }
            textAnswerA.text = "A: " + questionData[questionIndex].answerA;
            textAnswerB.text = "B: " + questionData[questionIndex].answerB;
            textAnswerC.text = "C: " + questionData[questionIndex].answerC;
            textAnswerD.text = "D: " + questionData[questionIndex].answerD;
        }

        //Function cập nhật các state cho game
        public void SetGameState(GameState currentState)
        {
            gameState = currentState;
            gameLives = 3;

            homePanel.SetActive(gameState == GameState.HOME);
            gameplayPanel.SetActive(gameState == GameState.GAMEPLAY);
            gameOverPanel.SetActive(gameState == GameState.GAMEOVER);
            winGamePanel.SetActive(gameState == GameState.WINGAME);

            GameLives.text = "Lives: " + gameLives.ToString();
            Score.text = "Score: " + gameScore.ToString();
        }

        public void GameplayState()
        {
            gameLives = 3;
            gameScore = 0;
            SetGameState(GameState.GAMEPLAY);
            ResetValueQuest(0);
            SaveHighScore();
        }

        public void HomeState()
        {
            SetGameState(GameState.HOME);
            audioSource.clip = musicMain;
            audioSource.Play();
            SaveHighScore();
        }

        public void GameOverState()
        {
            SetGameState(GameState.GAMEOVER);
            //questionIndex = Random.Range(0, questionData.Length);
            CreateQuestion(0);
            ScoreOver.text = Score.text;
            SaveHighScore();

        }

        public void WinGameState()
        {
            SetGameState(GameState.WINGAME);
            TotalScore.text = Score.text;
            SaveHighScore();
        }

        private void AddScore()
        {
            gameScore += 5;
            Score.text = "Score: " + gameScore.ToString();
            PlayerPrefs.SetInt("HighScore", gameScore);
            //SaveHighScore(gameScore);
            Debug.Log("Correct");
        }

        private void AddBonusScore()
        {
            gameScore += 10;
            Score.text = "Score: " + gameScore.ToString();
            PlayerPrefs.SetInt("HighScore", gameScore);
            //SaveHighScore(gameScore);
            Debug.Log("Correct");
        }

        private void SubScore(bool checkCorrect, int checkScore)
        {
            if (checkCorrect == false && checkScore > 0)
            {
                gameScore -= 5;
                Debug.Log("Uncorrect");

            }
            Score.text = "Score: " + gameScore.ToString();
        }

        private void ResetValueQuest(int valueIndex)
        {
            questionIndex = valueIndex;
            ShowCountQuest(questionIndex);
            CreateQuestion(questionIndex);

            /* Code random Quest BUGGG ^^
             * questionIndex = Random.Range(valueIndex, questionData.Length);
            showCountQuest(questionIndex);
            CreateQuestion(questionIndex);
            */
        }

        private void SaveHighScore()
        {
            /*highScore = saveScore;
            HighScore.text = "High Score: " + highScore.ToString();*/
            HighScoreGameplay.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreMenu.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreGameOver.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreWin.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();

        }

        private void ShowCountQuest(int index)
        {
            int totalQuest = questionData.Length;
            CountQuest.text = (index + 1) + " / " + totalQuest;
        }

        private void CountTime()
        {
            if(timeValue > 0)
            {
                timeValue -= Time.deltaTime;
            }
            else
            {
                timeValue = 0;
            }

            //return timeValue;
            ShowCountTime(timeValue);
        }

        private void ShowCountTime(float displayTime)
        {
            if(displayTime < 0)
            {
                displayTime = 0;
            }
            float seconds = Mathf.FloorToInt(displayTime % 60);
            TimeRemain.text = string.Format("{00:00}", seconds);

        }

        private int randomIndexQuest()
        {
            int randomValue = Random.Range(0, questionData.Length);
            //int indexRandom = questionData[];

            return randomValue;
        }


       
    }
}