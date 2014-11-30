using Balda.DataAccess.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Balda.DataAccess
{
    public class InSessionDataRepository : IDataRepository
    {
        //private HttpSessionState items;
        private IApplicationState items;
        public InSessionDataRepository(IApplicationState items)
        {
            //items = System.Web.HttpContext.Current.Session;
            this.items = items;
            if (items == null || items["Words"] == null)
            {
                items["Words"] = new List<string>() { "test", "ado", "hello" };
                items["UsedWords"] = new List<string>();
                //var t = Enumerable.Range(0, 25).Select(p => new Cell() { Id = p }).ToList();
                //t[0].Letter = "T";
                //t[1].Letter = "E";
                //t[2].Letter = "S";
                //t[3].Letter = "T";
                //items["GameBoard"] = new MainGameModel() { Cells = t };
            }
        }
        public List<string> GetWords()
        {
            if (items["Words"] == null)
            {
                return new List<string>();
            }
            return items["Words"] as List<string>;
        }


        public void AddWords(List<string> list)
        {
            if (items["Words"] == null)
            {
                items["Words"] = new List<string>();
            }
            (items["Words"] as List<string>).AddRange(list);
        }


        public List<string> GetUsedWords(string gameId)
        {
            if (items["UsedWords" + gameId] == null)
            {
                items["UsedWords" + gameId] = new List<String>();
            }
            return (items["UsedWords" + gameId] as List<string>);
        }

        public void AddUsedWord(string word, string gameId)
        {
            (items["UsedWords" + gameId] as List<string>).Add(word);
        }


        public Model.MainGameModel GetMainBoard(string id)
        {
            return items["GameBoard" + id] as MainGameModel;
        }


        public void SetMainModel(MainGameModel mainGameModel, string id)
        {
            if (items["AllGameBoards"] == null)
            {
                items["AllGameBoards"] = new List<string>();
            }
            (items["AllGameBoards"] as List<string>).Add(id);
            items["GameBoard" + id] = mainGameModel;
        }


        public List<string> GetAllGameIds()
        {
            var result = (items["AllGameBoards"] as List<string>);
            if (result == null)
            {
                return new List<string>();
            }
            return result;
        }


        public bool CheckPass(string gameId, string pass)
        {
            return (items["Password" + gameId] as string) == pass;
        }


        public void SetPassword(string gameId, string pass)
        {
            items["Password" + gameId] = pass;
        }


        public Dictionary<string, DateTime> GetGamesWithTime()
        {
            var games = (items["GamesWithTime"] as Dictionary<string, DateTime>);
            if (games == null)
            {
                games = new Dictionary<string, DateTime>();
            }
            return games;
        }


        public void DeleteGame(string GameId)
        {
            (items["AllGameBoards"] as List<string>).Remove(GameId);
            items.Remove("GameBoard" + GameId);
            items.Remove("Password" + GameId);
            var guid = GetGuids().Where(p => p.Value == GameId).Select(p => p.Key).FirstOrDefault();
            if (guid != new Guid())
            {
                GetGuids().Remove(guid);
            }
            var games = (items["GamesWithTime"] as Dictionary<string, DateTime>);
            if (games.Keys.Contains(GameId))
            {
                games.Remove(GameId);
            }
            items.Remove("Password" + GameId);
        }


        public void SetTimeForGame(string gameId, DateTime now)
        {
            var games = (items["GamesWithTime"] as Dictionary<string, DateTime>);
            if (games == null)
            {
                games = new Dictionary<string, DateTime>();
                items["GamesWithTime"] = games;
            }
            games[gameId] = now;
        }


        public void ClearState()
        {
            items.Clear();
        }

        public void ClearUsedWords(string GameId)
        {
            items.Remove("UsedWords" + GameId);
        }

        public void AddGuid(string GameId, Guid guid)
        {
            GetGuids().Add(guid, GameId);
        }

        public Dictionary<Guid, string> GetGuids()
        {
            var guids = (items["Guids"] as Dictionary<Guid, string>);
            if (guids == null)
            {
                guids = new Dictionary<Guid, string>();
                items["Guids"] = guids;
            }
            return guids;
        }
        public Guid GetGuid(string GameId)
        {
            return GetGuids().Where(p => p.Value == GameId).Select(p => p.Key).FirstOrDefault();
        }
    }
}
