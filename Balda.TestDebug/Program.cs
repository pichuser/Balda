using Balda.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Balda.DataAccess;

namespace Balda.TestDebug
{
    class Program
    {
        static void Main(string[] args)
        {        

            var u = new GameBoardTest();
            u.Setup();
            u.IfCreateGameThenYouCanGetItGuid();
        }
    }
}
