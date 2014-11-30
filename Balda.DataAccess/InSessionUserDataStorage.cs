using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Balda.DataAccess
{
    public class InSessionUserDataStorage : IUserDataStorage
    {
        //private HttpSessionState items;
        private IApplicationState items;
        public InSessionUserDataStorage(IApplicationState items)
        {
            this.items = items;
        }
        public List<User> GetAllPlayersInGame(string gameId)
        {
            var result = items["AllPlayers" + gameId] as List<User>;
            if (result == null)
            {
                return new List<User>();
            }
            else
            {
                return result;
            }
        }

        public void SetPlayers(List<string> players, string gameId)
        {
            foreach (var item in players)
            {
                items["UsersInGame" + item] = gameId;
            }
            items["AllPlayers" + gameId] = players.Select(p => new User() { UserName = p }).ToList();
        }

        public User GetCurrentUser(string gameId)
        {
            return (items["AllPlayers" + gameId] as List<User>).FirstOrDefault(p => p.UserName == (items["CurrentUser" + gameId] as User).UserName);
        }

        public void SetCurrentUSer(string userName, string gameId)
        {
            items["CurrentUser" + gameId] = new User() { UserName = userName };
        }


        public string GetGameIdByUserName(string testUserName)
        {
            return (items["UsersInGame" + testUserName] as string);
        }


        public void AddPlayer(string player, string gameId)
        {
            if (items["AllPlayers" + gameId] == null)
            {
                items["AllPlayers" + gameId] = new List<User>();
            }
            (items["AllPlayers" + gameId] as List<User>).Add(new User() { UserName = player });
            items["UsersInGame" + player] = gameId;
        }


        public List<User> GetPlayersByGameId(string gameId)
        {
            return items["AllPlayers" + gameId] as List<User>;
        }


        public void DeleteUsers(string GameId)
        {
            items.Remove("AllPlayers" + GameId);
            items.Remove("CurrentUser" + GameId);
        }
    }
}
