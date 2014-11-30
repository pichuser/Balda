using Balda.DataAccess.Model;
using Balda.DI;
using Balda.MainModule;
using Balda.Models;
using Balda.Services;
using Balda.UserManagement;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace Balda.Controllers
{
    public class BaldaGameApiController : ApiController
    {
        StandardKernel kernel;
        private WordService wordService;
        private GameBoard game;
        private UserService userSerivce;
        public BaldaGameApiController()
        {
            kernel = new StandardKernel(new CoreModule());
            wordService = kernel.Get<WordService>();
            userSerivce = kernel.Get<UserService>();
            game = kernel.Get<GameBoard>();
        }
        //
        // GET: /BaldaGame/
        [HttpGet]
        public string Test()
        {
            return "hhhhhh";
        }
        [HttpPost]
        public RegisterWordResponse RegisterWord(TestModel model)
        {
            try
            {
                if (wordService.CheckWord(model.word, model.GameId))
                {
                    game.UpdateMainModel(model.NewCell, model.GameId);
                    userSerivce.AddPointToCurrentUser(wordService.GetPoints(model.word), model.GameId);
                    userSerivce.NextUser(model.GameId);
                    return new RegisterWordResponse() { Status = "success", Word = model.word };
                }
                else
                {
                    return new RegisterWordResponse() { Status = "not exist", Word = model.word };
                }
            }
            catch (Exception)
            {
                return new RegisterWordResponse() { Status = "was used", Word = model.word };
            }
        }
        [HttpPost]
        public void AddWordToLibrary(TestModel model)
        {
            wordService.AddWord(model.word);
            wordService.CheckWord(model.word, model.GameId);
            userSerivce.AddPointToCurrentUser(wordService.GetPoints(model.word), model.GameId);
            var c = userSerivce.GetCurrentUser(model.GameId);
            game.UpdateMainModel(model.NewCell, model.GameId);
            userSerivce.NextUser(model.GameId);
        }

        public class TestModel
        {
            public string word
            {
                get
                {
                    return string.Join("", Word.Select(p => p.Letter).ToList());
                }
            }
            public List<Cell> Word { get; set; }
            public Cell NewCell { get; set; }
            public string GameId { get; set; }
        }

        public MainGameModel GetMainModel(string gameId)
        {
            var model = game.GetMainModel(gameId);
            model.CurrentUser = userSerivce.GetCurrentUser(gameId);
            model.Users = userSerivce.GetAllPlayersInGame(gameId);
            model.UsedWords = wordService.GetUsedWords(gameId);
            return model;
        }
        
    }
}
