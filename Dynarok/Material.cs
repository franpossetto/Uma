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
        [MultiReturn(new[] { "Name", "Class" })]
        public static Dictionary<string, string> GetIdentityProperties(Revit.Elements.Material material)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            result["Name"] = material.Name;
            result["Class"] = material.MaterialClass;
            

            return result;
        }

        [NodeCategory("Actions")]
        public static RevitElements.Element SetIdentityProperties(RevitElements.Material material, string name = "", string @class= "")
        {

            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            DB.Material mat = (DB.Material) material.InternalElement;

            TransactionManager.Instance.EnsureInTransaction(doc);

            if (name != null && name != "") mat.Name = name;
            if ( @class != null) mat.MaterialClass = @class;

            TransactionManager.Instance.TransactionTaskDone();

            return mat.ToDSType(true);
        }


    }
}
