using System;
using System.Data;

namespace XmlData.Extensions
{
    public static class CommandExtensions
    {
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            if (command == null || name==null) 
                throw new ArgumentNullException();
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}