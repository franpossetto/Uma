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
    public class Element
    {
        private Element() { }

        [NodeCategory("Actions")]
        public static RevitElements.Element GetRoom(Revit.Elements.Element Element)
        {
            List<RevitElements.Element> result = new List<RevitElements.Element>();
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<DB.Element> _rooms = collector.OfClass(typeof(DB.SpatialElement)).WhereElementIsNotElementType().Where(x => x.GetType() == typeof(DB.Architecture.Room)).ToList();

            List<DB.Architecture.Room> rooms = _rooms.Select(r => r as DB.Architecture.Room).ToList();

            DB.Element _element = (DB.Element)Element.InternalElement;
            
            DB.FamilyInstance familyInstance = (DB.FamilyInstance)Element.InternalElement;

            LocationPoint familyInstanceLocationPoint = familyInstance.Location as LocationPoint;
            XYZ point = familyInstanceLocationPoint.Point;

            DB.Architecture.Room room = rooms.Where(x => x.IsPointInRoom(point)).FirstOrDefault();
            return room.ToDSType(true);
 
        }
    }
}