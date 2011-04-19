using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfWebApi.Preview4.Explorations.First
{
    class ToDo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public ToDo(string desc)
        {
            Description = desc;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}\n", Id, Description);
        }
    }
}
