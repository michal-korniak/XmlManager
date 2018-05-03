using System;
using System.Collections.Generic;

namespace XmlData.Models
{
    public class Markup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public List<Markup> ChildMarkups { get; set; }
        public List<Attribute> Attributes { get; set; }

        public Markup(Guid id,string name,Guid? parentId=null)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
            ChildMarkups=new List<Markup>();
            Attributes=new List<Attribute>();
        }
    }
}