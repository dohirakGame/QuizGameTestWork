using Struct;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using System.Collections.Generic;

namespace Core
{
    enum ResultType
    {
        Correct,
        Uncorrect
    }

	public class GameLogic : MonoBehaviour
    {
        [Header("Questions")]
        private QuestionsStruct[] questions;

        [Header("Save Config")]
        private string _savePath;
        private string _saveFileName = "data.json";
        private string _backgroundPath = "backgrounds/";

		private int _countQuestions;
        private int _currentQuestion;

        private string _question;
        private Answers[] _answer;
        private string _background;

        private int _countCorrectAnswers;

        [Header("Панели для отбражения")]
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private GameObject resultPanelBetweenQuestions;
        [SerializeField] private GameObject questionsAndAnswersPanel;

        [Header("Текстовые поля")]
        [SerializeField] private TextMeshProUGUI CountCorrectAnswersText;
        [SerializeField] private TextMeshProUGUI correctQuestionsText;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TextMeshProUGUI resultTextBetweenQuestions;

        [Header("Текстовые поля для ответов")]
        [SerializeField] private List<TextMeshProUGUI> answersText;

        [Header("Изображения")]
        [SerializeField] private Image backgroundImage;

        /*Для сохранения данных в Json
        public void SaveToFile()
        {
            GameCoreDataStruct gameCore = new GameCoreDataStruct
            {
                questions = this.questions
            };

            string json = JsonUtility.ToJson(gameCore, true);

            try
            {
                File.WriteAllText(savePath, json);
            }
            catch (Exception e)
            {
                Debug.Log("{GameLog} => [GameCore] - (<color=red>Error</color>) - SaveToFile -> " + e.Message);
            }
        }*/

        /*При выходе из приложения сохраняются данные
        private void OnApplicationQuit()
        {
            SaveToFile();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                SaveToFile();
            }
        }*/

        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
#else
            _savePath = Path.Combine(Application.dataPath, _saveFileName);
#endif
            LoadFromFile();
        }
        private void Start()
        {
            _countQuestions = questions.Length;
            _currentQuestion = 0;
            LoadQuestion();

        }
        public void LoadQuestion()
        {
            if (_currentQuestion + 1 <= _countQuestions)
            {
                ShowQuestionPanels();

                LoadQuestionData();

                LoadBackground();

                _currentQuestion += 1;

                UpdateTextFields();
            }
            else
            {
                FinishTheQuiz();
            }
        }
        public void CheckAnswer(int answerNumber)
        {
			if (_answer[answerNumber].correct)
			{
				_countCorrectAnswers += 1;
				CheckResult(ResultType.Correct);
			}
			else
			{
				CheckResult(ResultType.Uncorrect);
			}
			questionsAndAnswersPanel.SetActive(false);
            resultPanelBetweenQuestions.SetActive(true);
		}

        private void LoadFromFile()
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log("{GameLog} => [GameCore] - LoadFromFile -> File Not Found!");
                return;
            }

            try
            {
                string json = File.ReadAllText(_savePath);

                GameCoreDataStruct gameCoreFromJson = JsonUtility.FromJson<GameCoreDataStruct>(json);
                questions = gameCoreFromJson.questions;

            }
            catch (Exception e)
            {
                Debug.Log("{GameLog} - [GameCore] - (<color=red>Error</color>) - LoadFromFile -> " + e.Message);
            }
        }
        private void ShowQuestionPanels()
        {
			questionsAndAnswersPanel.SetActive(true);
			resultPanelBetweenQuestions.SetActive(false);
		}
        private void LoadQuestionData()
        {
			_question = questions[_currentQuestion].queston;
			_answer = questions[_currentQuestion].answers;
			_background = Path.GetFileNameWithoutExtension(questions[_currentQuestion].background);
		}
        private void LoadBackground()
        {
            {
                _background = _backgroundPath + _background;
                Sprite newImage = Resources.Load<Sprite>(_background);

                if (newImage != null)
                {
                    backgroundImage.sprite = newImage;
                }
                else
                {
                    Debug.LogError($"Ошибка загрузки фона: {_background}");
                }
            }
        }
        private void UpdateTextFields()
        {
			for (int i = 0; i < answersText.Count; i++)
			{
				answersText[i].text = _answer[i].text;
			}
			questionText.text = _question;
			correctQuestionsText.text = String.Format("Вопрос: {0}/{1}", _currentQuestion, _countQuestions);
		}
        private void FinishTheQuiz()
        {
			Debug.Log("Вопросы закончились");
			resultsPanel.SetActive(true);
			CountCorrectAnswersText.text = String.Format("{0}/{1}", _countCorrectAnswers, _countQuestions);
		}
        private void CheckResult(ResultType resultType)
        {
            switch (resultType)
            {
                case ResultType.Correct:
					resultTextBetweenQuestions.text = "Верно";
					resultTextBetweenQuestions.color = new Color(0.75f, 1f, 0.75f);
					break;
                case ResultType.Uncorrect:
					resultTextBetweenQuestions.text = "Неверно";
					resultTextBetweenQuestions.color = new Color(1f, 0.75f, 0.75f);
					break;
            }
        }
    }
}