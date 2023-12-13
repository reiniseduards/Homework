using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace CsvXmlProcessor
{
    public partial class MainWindow : Window
    {
        // ... existing code

        private void ReadXml(string file)
        {
            var doc = XDocument.Load(file);
            TxtXmlCheck.Visibility = Visibility.Visible;
            TxtError.Visibility = Visibility.Hidden;
            DistrictInfoList = LoadDistrictsFromXml(doc);
        }

        private static List<DistrictInfo> LoadDistrictsFromXml(XDocument xmlDoc)
        {
            var entryNodes = xmlDoc.Descendants(XName.Get("entry", "http://www.w3.org/2005/Atom"));

            List<DistrictInfo> districtList = new List<DistrictInfo>();

            foreach (var entryNode in entryNodes)
            {
                var propertiesNode = entryNode.Descendants(XName.Get("properties", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")).FirstOrDefault();

                DistrictInfo district = new DistrictInfo
                {
                    DistrictAtvk = (string)propertiesNode.Element(XName.Get("district_atvk", "http://schemas.microsoft.com/ado/2007/08/dataservices")),
                    DistrictName = (string)propertiesNode.Element(XName.Get("district_name", "http://schemas.microsoft.com/ado/2007/08/dataservices"))
                };
                districtList.Add(district);
            }

            return districtList;
        }
    }
}