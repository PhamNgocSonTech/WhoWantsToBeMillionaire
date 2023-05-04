using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

   /* public class Question
    {
        public string question;
        public string answerA;
        public string answerB;
        public string answerC;
        public string answerD;
        public string correctAnswer;
        public bool isHardQuestion = false;
    }*/

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
        //[SerializeField] private QuestionSciptableData[] questionData;

        //Khai báo các trường Panel sẽ được kích hoạt tương ứng với mỗi State
        [SerializeField] private GameObject homePanel, gameplayPanel, gameOverPanel, winGamePanel;

        //Tạo một biến để lưu vị trí câu hỏi hiện tại
        private int questionIndex = 1;

        //Tạo một biến để lưu State hiện tại
        private GameState gameState;

        //Biến lưu số lần chơi 
        private int gameLives = 3;

        //Biến lưu số điểm
        private int gameScore = 0;

        //Biến lưu giá trị thời gian và thời gian hiện tại (currentTime)
        private float timeValue = 45;
        private float currentTime;
        private int sizeQuestionForPlayer = 10;

        //private QuestionSciptableData randomIndexQuestionData = QuestionManager.Ins.GetRandomQuestion();
        private QuestionSciptableData randomIndexQuestionData;

        // Start is called before the first frame update
        void Start()
        {
            HomeState();
            ResetValueQuest(0);
        }

        // Update is called once per frame
        void Update()
        {
            CountTime();
        }

        public void ClickAnswer(string selectorAnswer)
        {
            bool isCorrect = false;
            //int sizeQuestionData = questionData.Length;
            //int sizeQuestionData = QuestionManager.Ins.questionData.Count();
            int sizeQuestionData = sizeQuestionForPlayer;
            if (randomIndexQuestionData.correctAnswer == selectorAnswer)
            {
                isCorrect = true;
                /* countCorrectQuest++;
                 Debug.Log("Count Correct: " + countCorrectQuest);*/
                if (randomIndexQuestionData.isHardQuestion)
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
                else if(gameLives > 0 && questionIndex == sizeQuestionData) 
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
                if (questionIndex == sizeQuestionData)
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
            CreateQuestion();
        }

        private void CreateQuestion()
        {
            /*if (paramIndex < 0 || paramIndex >= questionData.Length)
                return;*/
            /* imgAnswerA.color = Color.white;
             imgAnswerB.color = Color.white;
             imgAnswerC.color = Color.white;
             imgAnswerD.color = Color.white;*/

            imgAnswerA.sprite = buttonBlack;
            imgAnswerB.sprite = buttonBlack;
            imgAnswerC.sprite = buttonBlack;
            imgAnswerD.sprite = buttonBlack;
            //textQuestion.text = questionData[questionIndex].question;
            QuestionSciptableData randIndex = RandomIndexQuestion();
            if (!randIndex.isHardQuestion)
            {
                textQuestion.text = randIndex.question;
                textQuestion.color = Color.green;
            }
            else
            {
                textQuestion.text = randIndex.question;
                textQuestion.color = Color.red;
            }
            textAnswerA.text = "A: " + randIndex.answerA;
            textAnswerB.text = "B: " + randIndex.answerB;
            textAnswerC.text = "C: " + randIndex.answerC;
            textAnswerD.text = "D: " + randIndex.answerD;
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
            currentTime = timeValue;
            SetGameState(GameState.GAMEPLAY);
            ResetValueQuest(0);
            SaveHighScore();
            CountTime();
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
            CreateQuestion();
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
        }

        private void AddBonusScore()
        {
            gameScore += 10;
            Score.text = "Score: " + gameScore.ToString();
            PlayerPrefs.SetInt("HighScore", gameScore);
            //SaveHighScore(gameScore);
        }

        private void SubScore(bool checkCorrect, int checkScore)
        {
            if (checkCorrect == false && checkScore > 0)
            {
                gameScore -= 5;
            }
            Score.text = "Score: " + gameScore.ToString();
        }

        private void ResetValueQuest(int valueIndex)
        {
            valueIndex = questionIndex;
            ShowCountQuest(valueIndex);
            CreateQuestion();
        }

        private void SaveHighScore()
        {
            HighScoreGameplay.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreMenu.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreGameOver.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
            HighScoreWin.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
        }

        private void ShowCountQuest(int index)
        {
            int totalQuest = sizeQuestionForPlayer;
            index = questionIndex;
            CountQuest.text = (index) + " / " + totalQuest;
        }

        //Rewrite count time func
         private void CountTime()
         {
            if (gameState == GameState.GAMEPLAY)
            {
                currentTime -= Time.deltaTime;
                SetTime(timeValue);
            }
         }
        //Set time and format to second
        private void SetTime(float valueCurrentTime )
        {
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            TimeRemain.text = time.ToString("mm':'ss");
            if (currentTime <= 0 )
            {
                GameOverState();     
            }
        }

        /* private void CountTime()
         {
             if(timeValue > 0)
             {
                 timeValue -= Time.deltaTime;
             }
             else
             {
                 timeValue = 0;
             }
             ShowCountTime(timeValue);
         }

         private void ShowCountTime(float displayTime)
         {
             if(displayTime < 0)
             {
                 displayTime = 0;
             }
             float seconds = Mathf.FloorToInt(displayTime % 60);
             TimeRemain.text = string.Format("00:0"+seconds);

         }*/

        public void RestartEvent()
        {
            GameplayState();
        }

        public void MenuEvent()
        {
            HomeState();
        }
        private QuestionSciptableData RandomIndexQuestion()
        {
            randomIndexQuestionData = QuestionManager.Ins.GetRandomQuestion();
            return randomIndexQuestionData;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}