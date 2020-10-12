using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.DB;
using Revit.Elements;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Linq;
using DB = Autodesk.Revit.DB;
using RevitElements = Revit.Elements;



namespace Revit
{
    public class Room
    {
        private Room() { }

        [NodeCategory("Actions")]
        public static List<Revit.Elements.Element> GetElementsByCategory(Revit.Elements.Element Room, Revit.Elements.Category category)
        {
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            ElementId categoryId = new ElementId(category.Id);
            DB.Category internalRevitCat = Autodesk.Revit.DB.Category.GetCategory(doc, categoryId);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<DB.Element> AllElementOfCategory = collector.OfCategoryId(internalRevitCat.Id).WhereElementIsNotElementType().ToElements().ToList();

            DB.Element _room = (DB.Element)Room.InternalElement;
            DB.Architecture.Room room = _room as DB.Architecture.Room;
            List<Revit.Elements.Element> elems = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> _elems = new List<Revit.Elements.Element>();

            foreach (DB.Element element in AllElementOfCategory)
            {
                DB.FamilyInstance familyInstance = (DB.FamilyInstance)element;
                LocationPoint familyInstanceLocationPoint = familyInstance.Location as LocationPoint;
                XYZ point = familyInstanceLocationPoint.Point;
                if (room.IsPointInRoom(point)) _elems.Add(element.ToDSType(true));
            }
            return _elems;
        }
    }
}
