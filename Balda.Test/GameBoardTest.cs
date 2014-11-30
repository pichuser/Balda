using Balda.DataAccess;
using Balda.DataAccess.Model;
using Balda.DI;
using Balda.MainModule;
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
    public class GameBoardTest
    {
        MainGameModel model;
        GameBoard gameBoard;
        Mock<IClock> mockClock;
        string testGameName = "testGame";
        string testGameName2 = "newGame";
        string testUserName = "testUser";
        string testUserName2 = "testUser2";
        string pass = "pass";
        //Вызывается перед каждым тестом
        [SetUp]
        public void Setup()
        {
            var kernel = new StandardKernel(new TestDIModule());
            mockClock = new Mock<IClock>();
            kernel.Rebind<IClock>().ToConstant(mockClock.Object);
            gameBoard = kernel.Get<GameBoard>();
            gameBoard.ClearState();

            var t = Enumerable.Range(0, 25).Select(p => new Cell() { Id = p }).ToList();
            t[0].Letter = "T";
            t[1].Letter = "O";
            t[2].Letter = "D";
            t[3].Letter = "A";
            t[4].Letter = "Y";
            model = new MainGameModel() { Cells = t };
            gameBoard.SetMainModel(model, testGameName);
        }
        //Взять модель игры по её идентификатору
        [Test]
        public void TestGetMainModelById()
        {
            var result = gameBoard.GetMainModel(testGameName);
            Assert.IsNotNull(result);
        }
        //Установить модель игры по идентификатору
        [Test]
        public void TestSetMainModel()
        {
            model.Cells[0].Letter = "A";
            gameBoard.SetMainModel(model, testGameName);
            Assert.AreEqual(model, gameBoard.GetMainModel(testGameName));
        }
        //Изменить модель игры по идентификатору
        [Test]
        public void UpdateMainModel()
        {
            var cell = new Cell() { Id = 0, Letter = "X" };
            gameBoard.UpdateMainModel(cell, testGameName);
            Assert.AreEqual(cell, gameBoard.GetMainModel(testGameName).Cells[0]);
        }
        //Нельзя создать игру с идентификатором, который был использован
        [Test]
        public void YouCantCreateGameWithTheSameName()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, "newGame", pass);
            Assert.Throws(typeof(Exception), delegate { gameBoard.CreateGame(new List<string>() { testUserName }, "newGame", pass); });
        }
        //Выбрав все игры мы должны получить игру, которая была создана
        [Test]
        public void GetAllGames()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.IsTrue(gameBoard.GetAllGameIds().Any(p => p == testGameName2));
        }
        //Присоединение к игре с верным паролем должно вернуть true
        [Test]
        public void ConnectToGameValid()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var result = gameBoard.ConnectToGame(testGameName2, testUserName2, pass);
            Assert.IsTrue(result);
        }
        //Если кто-то присоединился к игре, то он должен быть добавлен в список игроков
        [Test]
        public void IfConnectToGameThanAddThisUser()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var result = gameBoard.ConnectToGame(testGameName2, testUserName2, pass);
            Assert.IsTrue(gameBoard.GetAllPlayersInGame(testGameName2).Any(p => p.UserName == testUserName2));
        }
        //Если присоединиться к игре с неверным паролем, то должно быть выброшено исключение
        [Test]
        public void ConnectToGameWithIrregularPassword()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.Throws(typeof(Exception), delegate { gameBoard.ConnectToGame(testGameName2, testUserName2, "baddPass"); });
        }
        //Проверка пароля
        [Test]
        public void CheckGamePass()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var res = gameBoard.CheckPass(testGameName2, pass);
            Assert.IsTrue(res);
            Assert.IsFalse(gameBoard.CheckPass(testGameName2, "baddPass"));
        }
        //Если присоединился игрок с именем, которое уже есть, то не нужно добавлять повторно это имя
        [Test]
        public void IfConnectAlreadyRegisteredPlayer()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var before = gameBoard.GetMainModel(testGameName2).Users.Count(p => p.UserName == testUserName);
            gameBoard.ConnectToGame(testGameName2, testUserName, pass);
            var after = gameBoard.GetMainModel(testGameName2).Users.Count(p => p.UserName == testUserName);
            Assert.AreEqual(before, 1);
            Assert.AreEqual(after, 1);
        }
        //Если задачи старше чем 1 час, то они удаляются
        [Test]
        public void IfTaskIsOlderThen1HourThenItMustBeDelete()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            mockClock.Setup(p => p.Now).Returns(new DateTime(2014, 11, 10, 0, 30, 0));
            Assert.IsTrue(gameBoard.GetAllGameIds().Count == 2);
            mockClock.Setup(p => p.Now).Returns(new DateTime(2014, 11, 10, 1, 0, 0));
            gameBoard.DeleteOldGames();
            Assert.AreEqual(1, gameBoard.GetAllGameIds().Count);
        }
        //Когда создаем игру, то запоминаем время создания (для последующего удаления)
        [Test]
        public void WhenCreateGameThenItInGamesWithTimes()
        {
            mockClock.Setup(p => p.Now).Returns(new DateTime(2014, 11, 10, 0, 30, 0));
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.IsTrue(gameBoard.GetGamesWithTimes().Any(p => p.Key == testGameName2));
        }
        //Когда игра удалена, то должны быть удалены её игроки, использованные в игре слова и сама игра
        [Test]
        public void WhenDeleteGameThenTheyDontExistAndPlayersNotExist_AndUsedWordsNotExist()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.IsTrue(gameBoard.GetAllGameIds().Count == 2);
            var wordServ = gameBoard.GetWordService();
            wordServ.AddWords(new List<string>() { "testWord" });
            wordServ.CheckWord("testWord", testGameName2);
            gameBoard.DeleteGame(testGameName2);
            Assert.AreEqual(1, gameBoard.GetAllGameIds().Count);
            Assert.AreEqual(0, gameBoard.GetAllPlayersInGame(testGameName2).Count);
            Assert.AreEqual(0, wordServ.GetUsedWords(testGameName2).Count);
        }
        //Если игра удалена, то можно повторно создать игру с тем же идентификатором
        [Test]
        public void IfIDeleteGameThenICanCreateItAgain()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.IsTrue(gameBoard.GetAllGameIds().Count == 2);
            gameBoard.DeleteGame(testGameName2);
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            Assert.IsTrue(gameBoard.GetAllGameIds().Count == 2);
        }
        //Если игра удалена, то к ней нельзя присоединиться
        [Test]
        public void IfYouDeleteGameThenYouCantConnect()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            gameBoard.DeleteGame(testGameName2);
            Assert.Throws(typeof(Exception), delegate
            {
                gameBoard.ConnectToGame(testGameName2, testUserName, pass);
            });
        }
        //Можно присоединиться к игре зная её Guid
        [Test]
        public void ConnectToGameThroughGuid()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var guid = gameBoard.GetGuid(testGameName2);
            var result = gameBoard.ConnectToGame(guid, testUserName);
            Assert.IsTrue(result);
        }
        //Нельзя присоединиться к игре по неверному Guid
        [Test]
        public void ConnectToGameNotValidGuid()
        {
            var guid = Guid.NewGuid();
            var result = gameBoard.ConnectToGame(guid, testUserName);
            Assert.IsFalse(result);
        }
        //если создана игра, то создается её уникальный Guid
        [Test]
        public void IfCreateGameThenYouCanGetItGuid()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            var guids = gameBoard.GetGuids();
            Assert.IsTrue(guids.Count != 0);
        }
        //Если удалил игру, то больше не существует её уникального Guid
        [Test]
        public void IfYouDeleteGameThenItGuidIsNull()
        {
            gameBoard.CreateGame(new List<string>() { testUserName }, testGameName2, pass);
            gameBoard.DeleteGame(testGameName2);
            var guids = gameBoard.GetGuids();
            Assert.IsTrue(!guids.Values.Contains(testGameName2));
        }
    }
}
