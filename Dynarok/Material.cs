using System;

using Autodesk.Revit.DB;
using RevitServices.Persistence;

using DB = Autodesk.Revit.DB;

namespace Dynarok
{
    public class Material
    {
        public static Element Create(string name)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            ElementId materialId = null;
            using (Transaction t = new Transaction(doc, "Create Material"))
            {
                t.Start();
                materialId = DB.Material.Create(doc, name);
                t.Commit();
            }

            return doc.GetElement(materialId);
        }
        public static Element Duplicate(Autodesk.Revit.DB.Material material, string name)
        {

            DB.Material newMaterial = material.Duplicate(name);
            if (newMaterial != null) return newMaterial;
            while (newMaterial == null)
            {
                int n = 1;
                name = name + "Copy" + " " + name.ToString();
                newMaterial = material.Duplicate(name);
                n++;
            }
            return newMaterial;

        }
    }
}
