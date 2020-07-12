using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.DB;
using Revit.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Linq;

using DB = Autodesk.Revit.DB;

namespace Dynarok
{
    public class Material
    {
        private Material() { }

        [NodeCategory("Create")]
        public static Revit.Elements.Element Create(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            if (Exists(name))
                return null;

            ElementId materialId = null;
            using (Transaction t = new Transaction(doc, "Create Material"))
            {
                t.Start();
                materialId = DB.Material.Create(doc, name);
                t.Commit();
            }

            return doc.GetElement(materialId).ToDSType(true);
        }

        [NodeCategory("Actions")]
        public static bool Exists(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<DB.Element> materials = collector.OfClass(typeof(DB.Material)).WhereElementIsNotElementType().ToElements();
            
            return materials.Any(x => x.Name == name);
        }

        [NodeCategory("Actions")]
        public static Revit.Elements.Element SetProperties(Revit.Elements.Material material, string name)
        {

            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.Material mat = (DB.Material) material.InternalElement;

            using (Transaction t = new Transaction(doc, "Set properties in material"))
            {
                t.Start();
                    if (name != null) mat.Name = name;
                t.Commit();
            }
            return mat.ToDSType(true);
        }


        //public static Revit.Elements.Element Duplicate(Revit.Elements.Material material, string name)
        //{
        //    //material.
        //    //DB.Material newMaterial = material.Duplicate(name);
        //    //if (newMaterial != null) return newMaterial;
        //    //while (newMaterial == null)
        //    //{
        //    //    int n = 1;
        //    //    name = name + "Copy" + " " + name.ToString();
        //    //    newMaterial = material.Duplicate(name);
        //    //    n++;
        //    //}
        //    //return newMaterial.ToDSType(true); ;

        //}
    }
}
