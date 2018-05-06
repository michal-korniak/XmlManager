using System;

namespace XmlManager.ViewModels
{
    public enum CrudFlag
    {
        Created,
        Readed,
        Edited,
        Deleted
    }
    public class AttributeViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid Id { get; set; }
        public Guid MarkupId { get; set; }
        public CrudFlag Flag { get; set; }

        public AttributeViewModel()
        {
            Flag = CrudFlag.Created;
            Id = Guid.NewGuid();
        }

        public static AttributeViewModel MapFromModel(XmlData.Models.Attribute attribute)
        {
            var attributeVm = new AttributeViewModel
            {
                Id = attribute.Id,
                MarkupId = attribute.MarkupId,
                Name = attribute.Name,
                Value = attribute.Value,
                Flag = CrudFlag.Readed
            };
            return attributeVm;
        }
        public static XmlData.Models.Attribute MapToModel(AttributeViewModel attributeVm)
        {
            var attribute = new XmlData.Models.Attribute(attributeVm.Id, attributeVm.Name, attributeVm.Value, attributeVm.MarkupId);
            return attribute;
        }

    }
}