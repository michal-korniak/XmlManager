using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XmlData.Extensions;
using XmlData.Models;

namespace XmlData.Repositories
{
    public interface IMarkupsRepository {
        void Add(Markup markup);
        void Update(Markup markup);
        Markup GetById(Guid id);
        IEnumerable<Markup> GetByName(string name);
        IEnumerable<Markup> GetByParentId(Guid parentId);

    }

    public class MarkupsRepository : BaseRepository<Markup>,IMarkupsRepository
    {
        private readonly IDbConnection _connection;

        public MarkupsRepository(IDbConnection connection)
        {
            this._connection = connection;
        }

        public void Add(Markup markup)
        {
            _connection.Open();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Markups VALUES (@MarkupId,@Name,@ParentId)";
                command.AddParameter("MarkupId",markup.Id);
                command.AddParameter("Name",markup.Name);
                command.AddParameter("ParentId",markup.ParentId);
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public void Update(Markup markup)
        {
            _connection.Open();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Users SET ParentId = @ParentId, Name=@Name WHERE Id = @MarkupId";
                command.AddParameter("MarkupId",markup.Id);
                command.AddParameter("Name",markup.Name);
                command.AddParameter("ParentId",markup.ParentId);
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public Markup GetById(Guid id)
        {
            _connection.Open();
            Markup result;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT FROM Markups WHERE MarkupId=@MarkupId";
                command.AddParameter("MarkupId",id);
                var markups=ExecuteQueryAndGetResults(command);
                 result= markups.FirstOrDefault();
            }
            _connection.Close();
            return result;
        }

        public IEnumerable<Markup> GetByName(string name)
        {
            _connection.Open();
            IEnumerable<Markup> result;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Markups WHERE Name=@Name";
                command.AddParameter("Name",name);
                result=ExecuteQueryAndGetResults(command);
            }
            _connection.Close();
            return result;
        }

        public IEnumerable<Markup> GetByParentId(Guid parentId)
        {
            _connection.Open();
            IEnumerable<Markup> result;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Markups WHERE ParentId=@ParentId";
                command.AddParameter("ParentId",parentId);
                result=ExecuteQueryAndGetResults(command);
            }
            _connection.Close();
            return result;
        }

        protected override Markup Map(IDataReader reader)
        {
            var markupId = reader.GetGuid(0);
            var name = reader.GetString(1);
            Guid? parentId = null;
            if (!reader.IsDBNull(2))
                parentId = reader.GetGuid(2);
            var markup=new Markup(markupId,name,parentId);
            return markup;
        }

    }
}