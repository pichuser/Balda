using Balda.DataAccess;
using Balda.DI;
using Balda.MainModule;
using Balda.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Balda.Controllers
{
    public class BaldaController : Controller
    {
        //
        // GET: /Balda/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EnterToGame()
        {
            return View();
        }
        [HttpGet]
        public ActionResult EnterToGameThroughGuid(Guid guid)
        {
            return View(new EnterToGameThroughGuidModel() { GameGuid = guid });
        }
        [HttpPost]
        public ActionResult EnterToGameThroughGuid(EnterToGameThroughGuidModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var kernel = new StandardKernel(new CoreModule());
            var game = kernel.Get<GameBoard>();
            var result = game.ConnectToGame(model.GameGuid, model.UserName);
            if (result)
            {
                var gameId = game.GetGuids().FirstOrDefault(p => p.Key == model.GameGuid).Value;
                return RedirectToAction("ConnectToGame", "angular/app", new { UserName = model.UserName, GameId = gameId, GameGuid = model.GameGuid });
            }
            else
            {
                ModelState.AddModelError("", "Неверная ссылка");
                return View();
            }
        }

        [HttpPost]
        public ActionResult EnterToGame(EnterToGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var kernel = new StandardKernel(new CoreModule());
            var game = kernel.Get<GameBoard>();
            try
            {
                var result = game.ConnectToGame(model.GameId, model.UserName, model.GamePassword);
                var guid = game.GetGuid(model.GameId);
                return RedirectToAction("ConnectToGame", "angular/app", new { UserName = model.UserName, GameId = model.GameId, GameGuid = guid });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Не удалось войти");
                return View();
            }
        }
        public ActionResult CreateGame()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGame(CreateGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var db = new SqLiteDataContext())
            {
                Visit v = new Visit()
                {
                    GameName = model.GameId,
                    IpAddress = GetUser_IP(),
                    Time = DateTime.Now,
                    Pass = model.Password,
                    UserName = model.Player1
                };
                db.Visits.Add(v);
                db.SaveChanges();
            }
            var kernel = new StandardKernel(new CoreModule());
            var game = kernel.Get<GameBoard>();
            try
            {
                game.DeleteOldGames();
                game.CreateGame(new List<string>() { model.Player1 }, model.GameId, model.Password);
                return RedirectToAction("ConnectToGame", "angular/app", new { UserName = model.Player1, GameId = model.GameId, GameGuid = game.GetGuid(model.GameId) });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Не удалось создать игру. Возможно имя совпадает с уже существующей игрой");
                return View(model);
            }
        }

        protected string GetUser_IP()
        {
            string VisitorsIPAddr = string.Empty;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }
    }
}
