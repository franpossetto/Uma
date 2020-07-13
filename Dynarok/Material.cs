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
    public class Material
    {
        private Material() { }

        [NodeCategory("Create")]
        public static RevitElements.Element Create(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            if (Exists(name))
                return null;

            ElementId materialId = null;

            TransactionManager.Instance.EnsureInTransaction(doc);
            materialId = DB.Material.Create(doc, name);
            TransactionManager.Instance.TransactionTaskDone();

            return doc.GetElement(materialId).ToDSType(true);
        }

        [NodeCategory("Query")]
        public static bool Exists(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<DB.Element> materials = collector.OfClass(typeof(DB.Material)).WhereElementIsNotElementType().ToElements();
            
            return materials.Any(x => x.Name == name);
        }

        [NodeCategory("Query")]
        [MultiReturn(new[] { "Name", "Class", "Category", "AppearenceAssetId", "Color" })]
        public static Dictionary<string, dynamic> GetProperties(RevitElements.Material Material)
        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            DB.Material mat = (DB.Material)Material.InternalElement;

            result["Name"] = mat.Name;
            result["Class"] = mat.MaterialClass;
            result["Category"] = mat.MaterialCategory;
            result["AppearenceAssetId"] = mat.AppearanceAssetId;

            Dictionary<string, double> colors = new Dictionary<string, double>();
            colors["Red"] = mat.Color.Red;
            colors["Green"] = mat.Color.Green;
            colors["Blue"] = mat.Color.Blue;
            result["Color"] = colors;

            return result;
        }

        [NodeCategory("Actions")]
        public static RevitElements.Element SetProperties(RevitElements.Material Material, string Name = "", string @Class= "", string Category = "")
        {

            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            DB.Material mat = (DB.Material) Material.InternalElement;

            TransactionManager.Instance.EnsureInTransaction(doc);

            if (Name != null && Name != "") mat.Name = Name;
            if ( @Class != null) mat.MaterialClass = @Class;
            if (@Class != null) mat.MaterialCategory = Category;

            TransactionManager.Instance.TransactionTaskDone();

            return mat.ToDSType(true);
        }


    }
}
