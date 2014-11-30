using System;
using System.Collections.Generic;
using System.Linq;
namespace Balda.DataAccess.Model
{
    public class MainGameModel
    {
        public List<Cell> Cells { get; set; }
        public List<string> UsedWords { get; set; }
        public User CurrentUser { get; set; }
        public List<User> Users { get; set; }
    }
}