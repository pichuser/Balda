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
    public class AngularController : Controller
    {
        GameBoard _gameBoard;
        public AngularController(GameBoard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        public ActionResult Redirect()
        {
            return Redirect("/angular/app/Home");
        }

        public ActionResult ConnectToGame(string UserName, string GameId, Guid GameGuid)
        {
            ViewBag.UserName = UserName;
            ViewBag.GameId = GameId;
            ViewBag.GameGuid = GameGuid;
            var res = _gameBoard.ConnectToGame(GameGuid, UserName);
            if (res)
            {
                return Index();
            }
            else
            {
                return Redirect("/Balda/EnterToGame");
            }
        }

        public ActionResult Index()
        {
            if (ViewBag.UserName == null || ViewBag.GameId == null)
            {
                return RedirectToAction("EnterToGame", "Angular");
            }
            return View("Index");
        }


        [AllowAnonymous]
        public ActionResult GetHtmlPage(string path)
        {
            return new FilePathResult(path, "text/html");
        }
        public ActionResult TestView()
        {
            return View();
        }
    }
}
