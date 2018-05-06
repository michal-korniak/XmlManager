using System;
using System.Xml;
using XmlData.Models;
using Attribute = XmlData.Models.Attribute;

namespace XmlService
{
    public interface IXmlImporter {
        Markup LoadFromFile(string filePath);
    }

    public class XmlImporter : IXmlImporter
    {
        public Markup LoadFromFile(string filePath)
        {
            var doc=new XmlDocument();
            doc.Load(filePath);
            Markup rootMarkup = new Markup(Guid.NewGuid(), "root");
            XmlNode rootNode=doc.SelectSingleNode("/root");
            LoadChildesAndAttributes(rootNode,rootMarkup,rootMarkup.Id);
            return rootMarkup;
        }

        private void LoadChildesAndAttributes(XmlNode parentNode, Markup parentMarkup, Guid? parentId)
        {
            if (parentNode.Attributes != null)
            {
                foreach(XmlAttribute attributeNode in parentNode.Attributes)
                {
                    var attributeObject = new Attribute(Guid.NewGuid(),attributeNode.Name, attributeNode.Value,parentId.Value);
                    parentMarkup.Attributes.Add(attributeObject);
                }
                    
            }
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.Name == "#text")
                {
                    var innerTextAttribute = new Attribute(Guid.NewGuid(),"innerText", childNode.Value,parentId.Value);
                    parentMarkup.Attributes.Add(innerTextAttribute);
                    continue;
                }
                var childMarkup = new Markup(Guid.NewGuid(),childNode.Name,parentId);
                LoadChildesAndAttributes(childNode,childMarkup,childMarkup.Id);
                parentMarkup.ChildMarkups.Add(childMarkup);
            }
            
        }
    }
}