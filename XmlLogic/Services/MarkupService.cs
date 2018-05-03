using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlData.Models;
using XmlData.Repositories;

namespace XmlData.Services
{
    public class MarkupService
    {
        private readonly IMarkupsRepository _markupsRepository;
        private readonly IAttributesRepository _attributesRepository;

        public MarkupService(IMarkupsRepository markupsRepository, IAttributesRepository attributesRepository)
        {
            _markupsRepository = markupsRepository;
            _attributesRepository = attributesRepository;
        }
        public void SaveToDb(Markup markup)
        {
            _markupsRepository.Add(markup);
            foreach (var attribute in markup.Attributes)
            {
                _attributesRepository.Add(attribute);
            }
            foreach (var childMarkup in markup.ChildMarkups)
            {
                SaveToDb(childMarkup);
            }
        }

        public Markup LoadFromDb(string name)
        {
            var root = _markupsRepository.GetByName("root").FirstOrDefault();
            LoadChilds(root);
            return root;
        }

        private void LoadChilds(Markup parent)
        {
            parent.Attributes = _attributesRepository.GetByMarkupId(parent.Id).ToList();
            parent.ChildMarkups = _markupsRepository.GetByParentId(parent.Id).ToList();
            foreach (var childMarkup in parent.ChildMarkups)
            {
                LoadChilds(childMarkup);
            }
            
        }
    }
}
