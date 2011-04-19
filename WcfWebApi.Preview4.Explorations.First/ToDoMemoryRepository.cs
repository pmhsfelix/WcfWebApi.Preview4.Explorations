using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfWebApi.Preview4.Explorations.First
{
    class ToDoMemoryRepository : IToDoRepository
    {
        private int _nextId = 0;
        private List<ToDo> _todos = new List<ToDo>();


        public IQueryable<ToDo> ToDos
        {
            get { return _todos.AsQueryable(); }
        }

        public void Add(ToDo t)
        {
            t.Id = _nextId++;
            _todos.Add(t);
        }

        public void Remove(ToDo t)
        {
            _todos.Remove(t);
        }
    }
}
