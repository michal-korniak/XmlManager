using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using XmlData.Models;
using XmlData.Repositories;
using XmlData.Services;
using XmlManager.ViewModels;
using XmlService;

namespace XmlManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMarkupsRepository _markupsRepository;
        private readonly IAttributesRepository _attributesRepository;
        private readonly IMarkupsService _markupsService;
        private List<AttributeViewModel> _currentAttributesList;
        private Markup _markupRoot;

        public MainWindow()
        {
            InitializeComponent();
            var connectionString = ConfigurationManager.ConnectionStrings["XmlDb"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            _markupsRepository = new MarkupsRepository(connection);
            _attributesRepository = new AttributesRepository(connection);
            _markupsService = new MarkupsService(_markupsRepository, _attributesRepository);


            var rootMarkups = _markupsService.LoadFromDb();
            if(rootMarkups.Count()>0)
                SetTreeViewData(rootMarkups.ElementAt(0));

        }
        private void SetTreeViewData(Markup markup)
        {
            trvMenu.Items.Clear();
            trvMenu.Items.Add(markup);
        }


        private void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            saveChangesBtn.IsEnabled = false;
            var selectedMarkup = (Markup) e.NewValue;
            if (selectedMarkup == null)
            {
                AttributesDg.Columns.Clear();
                return;
            }

            _currentAttributesList=new List<AttributeViewModel>();

            foreach (var attribute in selectedMarkup.Attributes)
            {
                _currentAttributesList.Add(AttributeViewModel.MapFromModel(attribute));
            }

            AttributesDg.ItemsSource = _currentAttributesList;
           
            AttributesDg.Columns[0].Width = 150;
            AttributesDg.Columns[1].Width = 200;
            AttributesDg.Columns[2].Visibility = Visibility.Hidden;
            AttributesDg.Columns[3].Visibility = Visibility.Hidden;
            AttributesDg.Columns[4].Visibility = Visibility.Hidden;
        }
        private void RowEditEndingHandler(object sender, DataGridRowEditEndingEventArgs e)
        {
            saveChangesBtn.IsEnabled = true;
            var editedAttribute = e.Row.Item as AttributeViewModel;
            if (editedAttribute.Flag == CrudFlag.Readed)
                editedAttribute.Flag = CrudFlag.Edited;
            else if (editedAttribute.Flag == CrudFlag.Created)
            {
                var selectedMarkup = trvMenu.SelectedItem as Markup;
                editedAttribute.MarkupId = selectedMarkup.Id;
            }
        }
        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            saveChangesBtn.IsEnabled = true;
            var grid = (DataGrid)sender;
            if (Key.Delete != e.Key)
                return;
            var deletedAttribute=grid.SelectedItem as AttributeViewModel;
            deletedAttribute.Flag = CrudFlag.Deleted;
            var deletedRow=grid.ItemContainerGenerator.ContainerFromItem(deletedAttribute) as DataGridRow;
            deletedRow.Visibility = Visibility.Collapsed;

            e.Handled=true;

        }
        private void SaveChangesHandler(object sender, RoutedEventArgs e)
        {
            if (_currentAttributesList.Count(x => x.Name == "innerText")>1)
            {
                MessageBox.Show("Znacznik nie może posiadać więcej niż jednego innerTexta");
                return;
            }
            foreach (var attributeVm in _currentAttributesList)
            {
                if(attributeVm.Flag==CrudFlag.Readed)
                    continue;
                var selectedMarkup = trvMenu.SelectedItem as Markup;

                switch (attributeVm.Flag)
                {
                    case CrudFlag.Edited:
                        var editedAttribute = selectedMarkup.Attributes.FirstOrDefault(x => x.Id == attributeVm.Id);
                        editedAttribute.Name = attributeVm.Name;
                        editedAttribute.Value = attributeVm.Value;
                        _attributesRepository.Update(editedAttribute);
                        break;
                    case CrudFlag.Deleted:
                        _attributesRepository.Delete(attributeVm.Id);
                        var removedAttribute = selectedMarkup.Attributes.FirstOrDefault(x => x.Id == attributeVm.Id);
                        selectedMarkup.Attributes.Remove(removedAttribute);
                        break;
                    case CrudFlag.Created:
                        var newAttribute = AttributeViewModel.MapToModel(attributeVm);
                        selectedMarkup.Attributes.Add(newAttribute);
                        _attributesRepository.Add(newAttribute);
                        break;
                }

            }

            MessageBox.Show("Zmiany zostały zapisane!");
        }

        private void ImportXmlClickHandler(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Import nowego pliku XML, spowoduje zastąpienie aktualnie przechowywanych danych. " +
                                                   "Czy chcesz kontynuować?", 
                "Potwierdzenie", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
                return;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter="Pliki z rozszerzeniem XML (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == false)
                return;
            var filePath = openFileDialog.FileName;
            var xmlImporter=new XmlImporter();
            var rootMarkup=xmlImporter.LoadFromFile(filePath);
            SetTreeViewData(rootMarkup);
            _attributesRepository.DeleteAll();
            _markupsRepository.DeleteAll();
            _markupsService.SaveToDb(rootMarkup);
            MessageBox.Show("Dane zostały wczytane");
        }


        private void ExportXmlClickHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki z rozszerzeniem XML (*.xml)|*.xml"
            };
            if (saveFileDialog.ShowDialog() == false)
                return;
            string filePath = saveFileDialog.FileName;
            var xmlExporter=new XmlExporter();
            xmlExporter.ExportToFile(_markupRoot,filePath);
            MessageBox.Show("Dane zostały wyeksportowane do pliku.");
        }
    }
    
}
