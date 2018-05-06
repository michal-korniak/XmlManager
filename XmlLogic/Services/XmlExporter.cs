using System.Security.Policy;
using System.Text;
using System.Xml;
using XmlData.Models;

namespace XmlData.Services
{
    public class XmlExporter
    {
        public void ExportToFile(Markup rootMarkup,string filePath)
        {
            var document =CreateXml(rootMarkup);
            using (XmlTextWriter writer = new XmlTextWriter(filePath, null)) {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
            }

        }

        private XmlDocument CreateXml(Markup rootMarkup)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement header = document.DocumentElement;
            document.InsertBefore(xmlDeclaration, header);
            FillXml(document,null,rootMarkup);
            return document;
        }

        private void FillXml(XmlDocument document, XmlElement parentElement,Markup markup)
        {
            XmlElement element=document.CreateElement(markup.Name);
            if (parentElement == null)
                document.AppendChild(element);
            else
                parentElement.AppendChild(element);

            foreach (var attribute in markup.Attributes)
            {
                if (attribute.Name == "innerText")
                {
                    element.InnerText = attribute.Value;
                    continue;
                }
                var xmlAttribute = document.CreateAttribute(attribute.Name);
                xmlAttribute.Value = attribute.Value;
                element.Attributes.Append(xmlAttribute);
            }
            foreach (var childMarkup in markup.ChildMarkups)
            {
                FillXml(document,element,childMarkup);
            }
        }
    }
}