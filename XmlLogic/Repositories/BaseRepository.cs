using System;
using System.Collections.Generic;
using System.Data;
using XmlData.Models;

namespace XmlData.Repositories
{
    public abstract class BaseRepository<T>
    {
        protected IEnumerable<T> ExecuteQueryAndGetResults(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                var items = new List<T>();
                while (reader.Read())
                {
                    var item=Map(reader);
                    items.Add(item);
                }
                return items;
            }
        }

        protected abstract T Map(IDataReader reader);
    }
}