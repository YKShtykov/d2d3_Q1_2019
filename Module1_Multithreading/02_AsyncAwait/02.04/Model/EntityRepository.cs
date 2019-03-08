using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _02._04.Model
{
    using System.Threading;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class EntityRepository
    {
        private readonly List<Entity> _store = new List<Entity>
        {
            new Entity {Age = 10, FirstName = "Bob", LastName = "Bobson"},
            new Entity {Age = 20, FirstName = "Sam", LastName = "Samson"},
            new Entity {Age = 30, FirstName = "Dan", LastName = "Danson"},
            new Entity {Age = 40, FirstName = "Jim", LastName = "Jimson"},
        };

        public async Task<IEnumerable<Entity>> GetAsync()
        {
            return await Task.Run(() =>
            {
                var copy = new Entity[_store.Count];
                _store.CopyTo(copy);
                Thread.Sleep(2000);
                return copy;
            }).ConfigureAwait(false);
        }

        public async Task PostAsync(Entity entity)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10000);
                _store.Add(entity);
            }).ConfigureAwait(false);
        }

        public async Task PutAsync(Entity entity)
        {
            var entityForChange = _store.FirstOrDefault(e => e.Age == entity.Age &&
                                                             e.FirstName == entity.FirstName &&
                                                             e.LastName == entity.LastName);
            await Task.Run(() =>
            {
                Thread.Sleep(10000);
                if (entityForChange != null)
                {
                    entityForChange.Age = entity.Age;
                    entityForChange.FirstName = entity.FirstName;
                    entityForChange.LastName = entity.LastName;
                }

                ;
            }).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Entity entity)
        {
            var entityForDelete = _store.FirstOrDefault(e => e.Age == entity.Age &&
                                                             e.FirstName == entity.FirstName &&
                                                             e.LastName == entity.LastName);
            await Task.Run(() =>
            {
                Thread.Sleep(10000);
                if (entityForDelete != null)
                {
                    _store.Remove(entityForDelete);
                }

                ;
            }).ConfigureAwait(false);
        }
    }
}
