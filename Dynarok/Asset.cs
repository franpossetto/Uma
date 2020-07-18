using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Linq;
using DB = Autodesk.Revit.DB;
using RevitElements = Revit.Elements;

namespace Revit
{
    public class AppearanceAssetElement
    {
        private AppearanceAssetElement() { }
        [NodeCategory("Create")]
        public static RevitElements.Element Create(string name, string AssetName)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            if (Exists(name))
                return null;

            List<DB.Visual.Asset> assetList= doc.Application.GetAssets(DB.Visual.AssetType.Appearance).ToList();
            int assetIndex = 0;

            DB.Visual.AssetProperty assetProp = assetList[0].FindByName("BaseSchema");
            DB.Visual.AssetProperty assetProp2 = assetList[0].FindByName("SatinBaseSchema");



            DB.AppearanceAssetElement appearanceAssetElement = null;

            TransactionManager.Instance.EnsureInTransaction(doc);
            appearanceAssetElement = DB.AppearanceAssetElement.Create(doc, name, assetList[assetIndex]);
            TransactionManager.Instance.TransactionTaskDone();

            return appearanceAssetElement.ToDSType(true);
        }

        [NodeCategory("Query")]
        public static bool Exists(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<DB.Element> materials = collector.OfClass(typeof(DB.AppearanceAssetElement)).WhereElementIsNotElementType().ToElements();

            return materials.Any(x => x.Name == name);
        }

    }
}
