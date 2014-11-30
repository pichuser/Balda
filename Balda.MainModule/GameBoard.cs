using Balda.DataAccess;
using Balda.DataAccess.Model;
using Balda.Services;
using Balda.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.MainModule
{
    public class GameBoard
    {
        private IDataRepository repository;
        private IUserService _userService;
        private IWordService _wordService;
        private IClock _clock;
        public GameBoard(IDataRepository repository, IUserService userService, IWordService wordService, IClock clock)
        {
            this._clock = clock;
            this._wordService = wordService;
            this.repository = repository;
            this._userService = userService;
        }
        public MainGameModel GetMainModel(string id)
        {
            return repository.GetMainBoard(id);
        }

        public void SetMainModel(MainGameModel mainGameModel, string id)
        {
            repository.SetMainModel(mainGameModel, id);
        }

        public void UpdateMainModel(Cell cell, string id)
        {
            var m = repository.GetMainBoard(id);
            var c = m.Cells.FirstOrDefault(p => p.Id == cell.Id);
            if (c != null)
            {
                c.Letter = cell.Letter;
            }
            repository.SetMainModel(m, id);
        }

        public void SetPlayersForGameId(List<string> testPlayers, string testGameName)
        {
            _userService.SetPlayersForGameId(testPlayers, testGameName);
        }

        public void SetTimeForGame(string gameId, DateTime now)
        {
            repository.SetTimeForGame(gameId, now);
        }

        public void CreateGame(List<string> players, string gameId, string pass)
        {
            if (GetAllGameIds().Contains(gameId))
            {
                throw new Exception();
            }
            Log.Debug("Create game" + gameId);
            repository.AddGuid(gameId, Guid.NewGuid());
            SetTimeForGame(gameId, _clock.Now);
            repository.SetPassword(gameId, pass);
            var game = new MainGameModel();
            SetPlayersForGameId(players, gameId);
            game.Users = _userService.GetPLayersByGameId(gameId);
            var t = Enumerable.Range(0, 25).Select(p => new Cell() { Id = p }).ToList();
            var word = _wordService.GetRandomWord(5);
            t[10].Letter = word[0].ToString();
            t[11].Letter = word[1].ToString();
            t[12].Letter = word[2].ToString();
            t[13].Letter = word[3].ToString();
            t[14].Letter = word[4].ToString();
            game.Cells = t;
            _userService.SetCurrentUser(players[0], gameId);
            SetMainModel(game, gameId);
        }

        public List<string> GetAllGameIds()
        {
            return repository.GetAllGameIds();
        }

        public bool ConnectToGame(string gameId, string userName, string gamePass)
        {
            if (!CheckPass(gameId, gamePass))
            {
                throw new Exception();
            }
            if (GetAllPlayersInGame(gameId).Count(p => p.UserName == userName) == 0)
            {
                _userService.AddPlayer(userName, gameId);
            }
            return true;
        }

        public bool CheckPass(string gameId, string pass)
        {
            return repository.CheckPass(gameId, pass);
        }

        public List<User> GetAllPlayersInGame(string GameId)
        {
            return _userService.GetAllPlayersInGame(GameId);
        }

        public void DeleteOldGames()
        {
            var games = repository.GetGamesWithTime();
            var oldGames = games.Where(p => p.Value <= _clock.Now.AddHours(-1)).Select(p => p.Key).ToList();
            foreach (var item in oldGames)
            {
                Log.Debug("delete " + item);
                DeleteGame(item);
            }
        }

        public Dictionary<string, DateTime> GetGamesWithTimes()
        {
            return repository.GetGamesWithTime();
        }

        public void DeleteGame(string GameId)
        {
            repository.DeleteGame(GameId);
            _userService.DeleteUsers(GameId);
            _wordService.ClearUsedWords(GameId);
        }

        public void ClearState()
        {
            repository.ClearState();
        }

        public IWordService GetWordService()
        {
            return _wordService;
        }

        public bool ConnectToGame(Guid GameGuid, string UserName)
        {
            if (GetGuids().Keys.Contains(GameGuid))
            {
                var gameId = GetGuids().FirstOrDefault(p => p.Key == GameGuid).Value;
                if (GetAllPlayersInGame(gameId).Count(p => p.UserName == UserName) == 0)
                {
                    _userService.AddPlayer(UserName, gameId);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<Guid, string> GetGuids()
        {
            return repository.GetGuids();
        }

        public Guid GetGuid(string GameId)
        {
            return repository.GetGuid(GameId);
        }
    }
}
