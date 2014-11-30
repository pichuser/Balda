
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DataAccess
{
    public interface IUserDataStorage
    {
        List<User> GetAllPlayersInGame(string gameId);
        void SetPlayers(List<string> players, string gameId);
        User GetCurrentUser(string gameId);
        void SetCurrentUSer(string userName, string gameId);
        string GetGameIdByUserName(string userName);
        void AddPlayer(string player, string gameName);
        List<User> GetPlayersByGameId(string gameId);
        void DeleteUsers(string GameId);
    }
}
