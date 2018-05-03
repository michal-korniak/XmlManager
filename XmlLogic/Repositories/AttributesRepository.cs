using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlData.Extensions;
using XmlData.Models;

namespace XmlData.Repositories
{
    public interface IAttributesRepository {
        void Add(Models.Attribute attribute);
        IEnumerable<Models.Attribute> GetByMarkupId(Guid markupId);
    }

    public class AttributesRepository : BaseRepository<Models.Attribute>, IAttributesRepository
    {
        private readonly IDbConnection _connection;

        public AttributesRepository(IDbConnection connection)
        {
            this._connection = connection;
        }

        public void Add(Models.Attribute attribute)
        {
            _connection.Open();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Attributes VALUES (@AttributeId,@Name,@Value,@MarkupId)";
                command.AddParameter("AttributeId",attribute.Id);
                command.AddParameter("Name",attribute.Name);
                command.AddParameter("Value",attribute.Value);
                command.AddParameter("MarkupId",attribute.MarkupId);
                command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public IEnumerable<Models.Attribute> GetByMarkupId(Guid markupId)
        {
            _connection.Open();
            IEnumerable<Models.Attribute> result;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Attributes WHERE MarkupId=@MarkupId";
                command.AddParameter("MarkupId",markupId);
                result = ExecuteQueryAndGetResults(command).ToList();
            }
            _connection.Close();
            return result;
        }

        protected override Models.Attribute Map(IDataReader reader)
        {
            var attributeId = reader.GetGuid(0);
            var name = reader.GetString(1);
            var value = reader.GetString(2);
            var markupId = reader.GetGuid(3);
            var attribute=new Models.Attribute(attributeId,name,value,markupId);
            return attribute;
        }
    }
}
