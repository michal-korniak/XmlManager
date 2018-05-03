using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlData.Models;
using XmlData.Repositories;
using XmlData.Services;
using XmlService;

namespace XmlStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["XmlDb"].ConnectionString;
            var connection = new SqlConnection(connectionString);

            var markupRepository = new MarkupsRepository(connection);
            var attributesRepository = new AttributesRepository(connection);
            var markupService = new MarkupService(markupRepository, attributesRepository);

            //XmlLoader loader = new XmlLoader();
            //Markup root = loader.LoadFromFile("81.xml");
            //markupService.SaveToDb(root);
            var markup =markupService.LoadFromDb("root");
        }
    }
}
