using Balda.DataAccess.Model;
using Balda.DI;
using Balda.MainModule;
using Balda.Services;
using Balda.UserManagement;
using Microsoft.AspNet.SignalR;
using Ninject;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Balda.Hubs
{
    public class BaldaHub : Hub
    {
        StandardKernel kernel;
        private WordService wordService;
        private GameBoard game;
        private UserService userSerivce;
        public BaldaHub()
        {
            kernel = new StandardKernel(new CoreModule());
            wordService = kernel.Get<WordService>();
            userSerivce = kernel.Get<UserService>();
            game = kernel.Get<GameBoard>();
        }
        public void SendGameState(string gameId)
        {
            var model = game.GetMainModel(gameId);
            model.CurrentUser = userSerivce.GetCurrentUser(gameId);
            model.UsedWords = wordService.GetUsedWords(gameId);
            model.Users = game.GetAllPlayersInGame(gameId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            var data = js.Serialize(model);
            foreach (var item in model.Users)
            {
                Clients.Group(gameId + item.UserName).GameStateChange(data);
            }
        }
        public void Register(string userId)
        {
            Groups.Add(Context.ConnectionId, userId.ToString());
        }
    }
}