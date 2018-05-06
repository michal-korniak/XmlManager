using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlData.Models;
using XmlData.Repositories;

namespace XmlData.Services
{
    public interface IMarkupsService {
        void SaveToDb(Markup markup);
        IEnumerable<Markup> LoadFromDb();
    }

    public class MarkupsService : IMarkupsService
    {
        private readonly IMarkupsRepository _markupsRepository;
        private readonly IAttributesRepository _attributesRepository;

        public MarkupsService(IMarkupsRepository markupsRepository, IAttributesRepository attributesRepository)
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

        public IEnumerable<Markup> LoadFromDb()
        {
            var roots = _markupsRepository.GetRootMarkups();
            foreach (var root in roots)
            {
                LoadChilds(root);
            }
            return roots;
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
