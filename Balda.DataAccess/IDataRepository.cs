using Balda.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.DataAccess
{
    public interface IDataRepository
    {
        List<string> GetWords();

        void AddWords(List<string> list);

        List<string> GetUsedWords(string gameId);

        void AddUsedWord(string word, string gameId);

        MainGameModel GetMainBoard(string id);

        void SetMainModel(MainGameModel mainGameModel, string id);

        List<string> GetAllGameIds();

        bool CheckPass(string gameId, string pass);

        void SetPassword(string gameId, string pass);
        
        Dictionary<string, DateTime> GetGamesWithTime();

        void DeleteGame(string GameId);

        void SetTimeForGame(string gameId, DateTime now);

        void ClearState();

        void ClearUsedWords(string GameId);

        void AddGuid(string GameId, Guid guid);

        Dictionary<Guid, string> GetGuids();

        Guid GetGuid(string GameId);
    }
}
