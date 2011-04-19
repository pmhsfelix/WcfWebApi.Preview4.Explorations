using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfWebApi.Preview4.Explorations.First
{
    interface IToDoRepository
    {
        IQueryable<ToDo> ToDos { get; }
        void Add(ToDo t);
        void Remove(ToDo t);
    }
}
