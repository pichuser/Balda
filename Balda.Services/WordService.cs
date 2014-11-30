using Balda.DataAccess;
using Balda.UserManagement;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.Services
{
    public class WordService : IWordService
    {
        private UserService _userService;
        IDataRepository _repository;
        private static List<string> _words;
        public List<string> GetWords()
        {
            if (_words == null)
            {
                _words = _repository.GetWords();
            }
            return _words;
        }
        public WordService(IDataRepository repository, UserService service)
        {
            _userService = service;
            _repository = repository;
        }
        public bool CheckWord(string existWord, string gameId)
        {
            if (GetUsedWords(gameId).Any(p => existWord.ToLower() == p.ToLower()))
            {
                throw new Exception();
            }
            if (_repository.GetWords().Any(p => p.ToLower() == existWord.ToLower()))
            {
                _repository.AddUsedWord(existWord, gameId);
                return true;
            }
            return false;
        }

        public List<string> GetUsedWords(string gameId)
        {
            return _repository.GetUsedWords(gameId);
        }

        public int GetPoints(string p)
        {
            return p.Length;
        }
        public UserManagement.UserService GetUserService()
        {
            return _userService;
        }

        public void AddWord(string word)
        {
            _repository.AddWords(new List<string>() { word });
        }
        public void AddWords(List<string> words)
        {
            _repository.AddWords(words);
        }

        public string GetRandomWord(int length)
        {
            var count = _repository.GetWords().Where(p => p.Length == length).Count();
            if (count != 0)
            {
                var rand = new Random().Next(count);
                return _repository.GetWords().Where(p => p.Length == length).Skip(rand).Take(1).FirstOrDefault();
            }
            return "ВЕДРО";
        }

        public void ClearState()
        {
            _repository.ClearState();
        }


        public void ClearUsedWords(string GameId)
        {
            _repository.ClearUsedWords(GameId);
        }
    }
}
