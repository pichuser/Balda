using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Balda.DataAccess.Model
{
    public class Cell
    {
        public string Letter { get; set; }
        public int Id { get; set; }
        public override bool Equals(object obj)
        {
            return ((Cell)obj).Letter == Letter && ((Cell)obj).Id == Id;
        }
    }
}