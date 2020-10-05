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
using Autodesk.Revit.DB.Visual;
using Dynamo.Core;
using Autodesk.DesignScript.Geometry;

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

        [NodeCategory("Action")]
        public static List<string> SetTintColor(Revit.Elements.Element AppearanceAsset, DSCore.Color color)
        {
            
            List<string> output = new List<string>();

            DB.AppearanceAssetElement _appearanceAsset = (DB.AppearanceAssetElement)AppearanceAsset.InternalElement;
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);
            AppearanceAssetEditScope editScope = new Autodesk.Revit.DB.Visual.AppearanceAssetEditScope(doc);
            Asset editableAsset = editScope.Start(_appearanceAsset.Id);
            
            try
            {
            ElementId aid = _appearanceAsset.Id;

            AssetPropertyDoubleArray4d tintColorProp = editableAsset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;

            byte r = color.Red;
            byte g = color.Green;
            byte b = color.Blue;

            Color _color = new Color(r, g, b);
           
            if(tintColorProp.IsEditable()) tintColorProp.SetValueAsColor(_color);

            }
            catch(Exception e)
            {
                output.Add(e.ToString());
            }

            editScope.Commit(false);
            TransactionManager.Instance.TransactionTaskDone();

            return output;
        }

        [NodeCategory("Action")]
        public static List<string> SetTintColorByMat (Revit.Elements.Element Material, DSCore.Color color)
        {

            List<string> output = new List<string>();

            DB.Material material = (DB.Material)Material.InternalElement;
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);
            ElementId _appearanceAssetId = material.AppearanceAssetId;
            if (_appearanceAssetId.IntegerValue.Equals(-1)) return output;
            AppearanceAssetEditScope editScope = new Autodesk.Revit.DB.Visual.AppearanceAssetEditScope(doc);
            Asset editableAsset = editScope.Start(_appearanceAssetId);

            try
            {
                AssetPropertyDoubleArray4d tintColorProp = editableAsset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;

                byte r = color.Red;
                byte g = color.Green;
                byte b = color.Blue;

                Color _color = new Color(r, g, b);

                if (tintColorProp.IsEditable()) tintColorProp.SetValueAsColor(_color);

            }
            catch (Exception e)
            {
                output.Add(e.ToString());
            }

            editScope.Commit(false);
            TransactionManager.Instance.TransactionTaskDone();

            return output;
        }
    }
}
