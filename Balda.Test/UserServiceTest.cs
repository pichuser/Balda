using Balda.DataAccess;
using Balda.DI;
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
    public class UserServiceTest
    {
        List<string> testPlayers;
        UserService userService;
        string testGameName = "testGame";
        [SetUp]
        public void Setup()
        {
            var kernel = new StandardKernel(new TestDIModule());
            userService = kernel.Get<UserService>();
            testPlayers = new List<string>() { "player1", "player2", "player3" };
            userService.SetPlayers(testPlayers, testGameName);
        }
        //Проверка цикла прохода по всем игрокам (смена хода)
        [Test]
        public void TestNextUser()
        {
            userService.SetCurrentUser(testPlayers[0], testGameName);
            Assert.AreEqual(testPlayers[0], userService.GetCurrentUser(testGameName).UserName);
            userService.NextUser(testGameName);
            Assert.AreEqual(testPlayers[1], userService.GetCurrentUser(testGameName).UserName);
        }
        //Установка игроков на игру
        [Test]
        public void TestSetUsers()
        {
            userService.SetPlayers(testPlayers, testGameName);
            var players = userService.GetAllPlayersInGame(testGameName).Select(p => p.UserName).ToList();
            Assert.AreEqual(testPlayers, players);
        }
        //Установка игрока, который должен совершать ход
        [Test]
        public void TestSetCurrentUser()
        {
            userService.SetCurrentUser(testPlayers[0], testGameName);
            Assert.AreEqual(testPlayers[0], userService.GetCurrentUser(testGameName).UserName);
        }
        //Проверка добавления баллов игроку
        [Test]
        public void TestAddPointToUser()
        {
            userService.AddPointToUser(testPlayers[0], 5);
            Assert.AreEqual(5, userService.GetUserPoint(testPlayers[0], testGameName));
        }
        //Если к игре был добавлен пользователь, то он существует
        [Test]
        public void IfAddPlayerThenItExist()
        {
            var testUserName = "test1";
            var testPlayers = new List<string>() { testUserName, "test2" };
            userService.AddPlayer("test3", testGameName);
            var players = userService.GetAllPlayersInGame(testGameName);
            Assert.IsTrue(players.Count(p => p.UserName == "test3") == 1);
        }

        [Test]
        public void IfSetPlayersInGameThenTheyExist()
        {
            var testUserName = "test1";
            var testPlayers = new List<string>() { testUserName, "test2" };
            userService.SetPlayersForGameId(testPlayers, testGameName);
            var players = userService.GetAllPlayersInGame(testGameName);
            Assert.IsTrue(players.Count(p => p.UserName == testUserName) == 1);
        }
    }
}
