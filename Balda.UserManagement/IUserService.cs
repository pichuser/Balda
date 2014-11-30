using Balda.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.UserManagement
{
    public interface IUserService
    {
        List<User> GetAllPlayersInGame(string gameId);
        User GetCurrentUser(string gameId);
        void NextUser(string gameId);
        void SetPlayers(List<string> players, string gameId);
        void AddPointToCurrentUser(int points, string gameId);
        string GetGameIdByUserName(string testUserName);
        void SetPlayersForGameId(List<string> players, string gameId);
        List<User> GetPLayersByGameId(string gameId);
        void AddPlayer(string player, string gameName);
        void SetCurrentUser(string userName, string gameId);
        void DeleteUsers(string GameId);
    }
}
