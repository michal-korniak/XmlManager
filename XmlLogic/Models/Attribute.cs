using System;
using System.ComponentModel;

namespace XmlData.Models
{
    public class Attribute
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid MarkupId { get; set; }

        public Attribute()
        {
        }

        public Attribute(Guid id,string name, string value, Guid markupId)
        {
            Id = id;
            Name = name;
            Value = value;
            MarkupId = markupId;
        }
    }
}