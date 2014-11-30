using Balda.DataAccess;
using Balda.DI;
using Balda.Services;
using Balda.UserManagement;
using Moq;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda.Test
{
    [TestFixture]
    public class WordServiceTest
    {
        WordService wordService;
        string existWord = "hello";
        string notExistWord = "hhhhh";
        string testGameId = "testGame";
        Mock<IWordService> mockWordService = new Mock<IWordService>();
        [SetUp]
        public void Setup()
        {
            var kernel = new StandardKernel(new TestDIModule());
            wordService = kernel.Get<WordService>();
            wordService.ClearState();
            wordService.AddWord(existWord);
            var service = wordService.GetUserService();
            service.SetPlayers(new List<string>() { "player1", "player2" }, testGameId);
            service.SetCurrentUser("player1", testGameId);
        }
        //Проверка слов
        [Test]
        public void TestCheckService()
        {
            var result = wordService.CheckWord(existWord, testGameId);
            Assert.IsTrue(result);
        }
        //Проверка несуществующего слова
        [Test]
        public void TestNotExistWord()
        {
            var result = wordService.CheckWord(notExistWord, testGameId);
            Assert.IsFalse(result);
        }
        //Проверка количества очков за слово
        [Test]
        public void TestGetPoints()
        {
            Assert.AreEqual(existWord.Length, wordService.GetPoints(existWord));
        }
        //Если слово уже было использовано, то ексепшн
        [Test]
        public void IfWordWasUsedThenException()
        {
            wordService.CheckWord(existWord, testGameId);
            Assert.Throws(typeof(Exception), delegate { wordService.CheckWord(existWord, testGameId); });
        }
        //Если слово было использовано в одной игре, то оно может использоваться в другой
        [Test]
        public void IfWordWasUsedInOneGameThenItCanBeUsedInAnother()
        {
            Assert.IsTrue(wordService.CheckWord(existWord, testGameId));
            Assert.IsTrue(wordService.CheckWord(existWord, "anotherGame"));
        }
        //Если добавить слово в словарь, то оно существует
        [Test]
        public void IfAddNotExistThenItExist()
        {
            wordService.AddWord(notExistWord);
            Assert.IsTrue(wordService.CheckWord(notExistWord, testGameId));
        }
        //Получить историю слов
        [Test]
        public void TestGetHistoryWord()
        {
            var gameId = "testGame";
            wordService.CheckWord(existWord, testGameId);
            var history = wordService.GetUsedWords(gameId);
            Assert.IsTrue(history.Any(p => p == existWord));
        }
        //Получить случайное 5буквенное слово 
        [Test]
        public void GetRandom5LetterWord()
        {
            wordService.AddWords(new List<string>() { "ВЕДРО", "МУСОР" });
            var randomWord = wordService.GetRandomWord(5);
            Assert.IsTrue(randomWord.Length == 5);
        }
    }
}
