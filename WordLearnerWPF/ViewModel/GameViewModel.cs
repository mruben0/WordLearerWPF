﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WordLearnerWPF.Core.Abstract;
using WordLearnerWPF.Params.Abstract;
using WordLearnerWPF.Params.Impl;
using WordLearnerWPF.Services.Abstract;

namespace WordLearnerWPF.ViewModel
{
    public class GameViewModel : CoreViewModel
    {
        private IStaticParams _staticParams;
        private IDocumentService _documentService;
        private ICoreNavigationServie _navigationService;
        private FileDto _documetDTO;
        private Dictionary<string, string> _wordDictionary;
        private int? _startind;
        private int _endInd;
        private string _statLabel;
        private string _endLabel;
        private string _askWord;
        private string _answer;
        private bool? _singleResult;
        private string _rightAnswer;
        private string _submitCommandName;
        private SubmitType _submitType;
        private int _rightCount;
        private int _falseCount;

        public GameViewModel(ICoreNavigationServie navigationService, 
                            IStaticParams staticParams,
                            IDocumentService documentService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _staticParams = staticParams ?? throw new ArgumentNullException(nameof(staticParams));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public override Task Initialize<T>(T param)
        {
            DocumentDto = param as FileDto;
            RightAnswered = new List<string>();
            RightCount = 0;
            FalseCount = 0;
            RaisePropertyChanged(nameof(DocumentDto));
            return Task.FromResult(0);           
        }

        public ICommand OpenFile => new RelayCommand(() =>
        {
            Process.Start(DocumentDto.Path);
        });

        public ICommand StartCommand => new RelayCommand(() =>
        {
            if (AllNeedadPopsSelected)
            {
                getMyDictionary();
                step();
            }
            RaisePropertyChanged(nameof(AllNeedadPopsSelected));
            RaisePropertyChanged(nameof(TotalCount));
        });
       
        public ICommand SubmitAnswerCommand => new RelayCommand(() =>
        {            
            if (SubmitType == SubmitType.Submit)
            {
                SingleResult = CheckAnswer();
                RightAnswer = WordDictionary[AskWord];
                if (SingleResult == true)
                {                   
                    SubmitType = SubmitType.Next;
                    RightCount++;
                }
                else
                {
                    FalseCount++;
                    if (RightAnswered.Contains(WordDictionary[AskWord]))
                    {
                        RightAnswered.Remove(WordDictionary[AskWord]);
                    }                    
                }
            }
            else
            {              
                if (SingleResult == true)
                {
                    CheckRights(WordDictionary[AskWord]);
                    RightAnswer = string.Empty;
                    Answer = string.Empty;
                    step();
                    SubmitType = SubmitType.Submit;
                }
            }
            RaisePropertyChanged(nameof(TotalCount));
        });

        private bool CheckAnswer()
        {
            if (WordDictionary[AskWord].Last() == ' ')
            {
                var answerWithSpace = Answer + ' ';
                return answerWithSpace == WordDictionary[AskWord];
            }
            return Answer == WordDictionary[AskWord];
        }

        private void getMyDictionary()
        {
            
          WordDictionary = _documentService.GetDictionaryFromXls(DocumentDto.Path, StartInd.Value, EndInd, StartLabel, EndLabel);

            if (WordDictionary != null)
            {
                RaisePropertyChanged(nameof(WordDictionary));
            }
        }

        private void step()
        {
            if (WordDictionary.Count != 0)
            {
                Random rand = new Random();
                var r = rand.Next(0, WordDictionary.Count());
                AskWord = WordDictionary.ElementAt(r).Key;
            }
            else
            {
                AskWord = "Պրծ, սաղ բառերը արել ես, տի դեմք";
                EndLabel = null;
                RaisePropertyChanged(nameof(AllNeedadPopsSelected));
            }
        }

        public Dictionary<string, string> WordDictionary
        {
            get { return _wordDictionary; }
            set { _wordDictionary = value;
                RaisePropertyChanged(nameof(WordDictionary));
            }
        }
        
        public FileDto DocumentDto
        {
            get { return _documetDTO; }
            set { _documetDTO = value;
                RaisePropertyChanged(nameof(DocumentDto));
            }
        }
    
        public int? StartInd
        {
            get { return _startind; }
            set { _startind = value;
                RaisePropertyChanged(nameof(StartInd));
            }
        }

        public int EndInd
        {
            get { return _endInd; }
            set { _endInd = value;
                RaisePropertyChanged(nameof(EndInd));
            }
        }

        public string StartLabel
        {
            get { return _statLabel; }
            set { _statLabel = value;
                RaisePropertyChanged(nameof(StartLabel));
            }
        }
        
        public string EndLabel
        {
            get { return _endLabel; }
            set { _endLabel = value;
                RaisePropertyChanged(nameof(EndLabel));
            }
        }

        public string AskWord
        {
            get { return _askWord; }
            set { _askWord = value;
                RaisePropertyChanged(nameof(AskWord));
            }
        }

        public string Answer
        {
            get { return _answer; }
            set { _answer = value;
                RaisePropertyChanged(nameof(Answer));
            }
        }

        public bool? SingleResult
        {
            get { return _singleResult; }
            set { _singleResult = value;
                RaisePropertyChanged(nameof(SingleResult));
            }
        }

        public string RightAnswer
        {
            get { return _rightAnswer; }
            set { _rightAnswer = value;
                RaisePropertyChanged(nameof(RightAnswer));
            }
        }

        public string SubmitCommandName
        {
            get { return _submitCommandName; }
            set { _submitCommandName = value; }
        }

        public SubmitType SubmitType
        {
            get { return _submitType; }
            set { _submitType = value;
                RaisePropertyChanged(nameof(SubmitType));
            }
        }

        public int RightCount
        {
            get { return _rightCount; }
            set { _rightCount = value;
                RaisePropertyChanged(nameof(RightCount));
            }
        }

        public int FalseCount
        {
            get { return _falseCount; }
            set { _falseCount = value;
                RaisePropertyChanged(nameof(FalseCount));
            }
        }

        private void CheckRights(string word)
        {
            var rightCount = RightAnswered.Where(e=> e== word).Count();
            if (rightCount <= 1)
            {
                RightAnswered.Add(word);
            }
            else
            {
                var rightKey = WordDictionary.FirstOrDefault(e=>e.Value == word).Key;
                WordDictionary.Remove(rightKey);
            }
        }

        public int TotalCount => WordDictionary.Count();

        private List<string> RightAnswered { get; set; }

        public bool AllNeedadPopsSelected => EndLabel != null && StartLabel != null && StartInd != null && EndInd != 0;
    }
}
