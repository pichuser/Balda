using Automatonymous;
using Balda.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.UserManagement
{
    public class UserService : IUserService
    {
        IUserDataStorage _dataStorage;
        public UserService(IUserDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }
        public List<User> GetAllPlayersInGame(string gameId)
        {
            return _dataStorage.GetAllPlayersInGame(gameId);
        }

        public User GetCurrentUser(string gameId)
        {
            return _dataStorage.GetCurrentUser(gameId);
        }

        public void NextUser(string gameId)
        {
            var next = nextUser(gameId);
            _dataStorage.SetCurrentUSer(next, gameId);
        }

        private string nextUser(string gameId)
        {
            var players = GetAllPlayersInGame(gameId);
            var nextIndex = ((players.IndexOf(GetCurrentUser(gameId)) + 1) % players.Count);
            return players[nextIndex].UserName;
        }
        public void SetPlayers(List<string> players, string gameId)
        {
            _dataStorage.SetPlayers(players, gameId);
        }

        public void SetCurrentUser(string userName, string gameId)
        {
            _dataStorage.SetCurrentUSer(userName, gameId);
        }


        public void AddPointToCurrentUser(int points, string gameId)
        {
            GetCurrentUser(gameId).Points += points;
        }

        public void AddPointToUser(string userName, int points)
        {
            var user = _dataStorage.GetAllPlayersInGame(GetGameIdByUserName(userName)).FirstOrDefault(p => p.UserName == userName);
            user.Points += points;
        }

        public int GetUserPoint(string userName, string gameId)
        {
            return _dataStorage.GetAllPlayersInGame(gameId).Where(p => p.UserName == userName).Select(p => p.Points).FirstOrDefault();
        }


        public string GetGameIdByUserName(string testUserName)
        {
            return _dataStorage.GetGameIdByUserName(testUserName);
        }


        public void SetPlayersForGameId(List<string> players, string gameId)
        {
            foreach (var item in players)
            {
                _dataStorage.AddPlayer(item, gameId);
            }
        }

        public List<User> GetPLayersByGameId(string gameId)
        {
            return _dataStorage.GetPlayersByGameId(gameId);
        }


        public void AddPlayer(string player, string gameName)
        {
            _dataStorage.AddPlayer(player, gameName);
        }


        public void DeleteUsers(string GameId)
        {
            _dataStorage.DeleteUsers(GameId);
        }
    }
}
