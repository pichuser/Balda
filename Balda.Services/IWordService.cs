using Balda.DataAccess;
using Balda.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.Services
{
    public interface IWordService
    {
        bool CheckWord(string word, string gameId);
        UserService GetUserService();
        List<string> GetUsedWords(string gameId);
        string GetRandomWord(int length);
        void AddWords(List<string> words);
        void ClearUsedWords(string GameId);
    }
}
