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
using Autodesk.Revit.DB.PointClouds;
using RevitElements = Revit.Elements;

namespace Revit
{
    public class PointCloud
    {
        private PointCloud() { }

        [NodeCategory("Actions")]
        public static Boolean SetColorMode(int ColorMode)
        {
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            //add selection

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<PointCloudInstance> pointClouds = collector.OfCategory(BuiltInCategory.OST_PointClouds)
                                                                    .WhereElementIsNotElementType().ToList()
                                                                    .Cast<PointCloudInstance>();
            PointCloudOverrides pco = doc.ActiveView.GetPointCloudOverrides();


            foreach (PointCloudInstance pointCloud in pointClouds)
            {
                PointCloudOverrideSettings pt_cloud_settings = pco.GetPointCloudScanOverrideSettings(pointCloud.Id);

                switch (ColorMode)
                {
                    case 0:
                        pt_cloud_settings.ColorMode = PointCloudColorMode.Elevation;
                        break;
                    case 1:
                        pt_cloud_settings.ColorMode = PointCloudColorMode.FixedColor;
                        break;
                    case 2:
                        pt_cloud_settings.ColorMode = PointCloudColorMode.Intensity;
                        break;
                    case 3:
                        pt_cloud_settings.ColorMode = PointCloudColorMode.NoOverride;
                        break;
                    case 4:
                        pt_cloud_settings.ColorMode = PointCloudColorMode.Normals;
                        break;

                    default:
                        break;
                }

                try
                {
                View activeView = doc.ActiveView;
                PointCloudOverrides overrides = activeView.GetPointCloudOverrides();
                PointCloudColorSettings pointCloudColorSettings = new PointCloudColorSettings();
                TransactionManager.Instance.EnsureInTransaction(doc);
                overrides.SetPointCloudScanOverrideSettings(pointCloud.Id, pt_cloud_settings);
                TransactionManager.Instance.TransactionTaskDone();
                return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        [NodeCategory("Actions")]
        public static Boolean Hide(Boolean Run)
        {
            if (!Run) return false;
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            Categories categories = doc.Settings.Categories;
            DB.Category pointCloudCategory = categories.get_Item(BuiltInCategory.OST_PointClouds);
            View activeView = doc.ActiveView;

            PointCloudOverrides pc_ovverides = activeView.GetPointCloudOverrides();

            if (activeView.ViewTemplateId == ElementId.InvalidElementId)
            {
                try
                {
                    TransactionManager.Instance.EnsureInTransaction(doc);
                    activeView.SetCategoryHidden(pointCloudCategory.Id, true);
                    TransactionManager.Instance.TransactionTaskDone();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        [NodeCategory("Actions")]
        public static Boolean Show(Boolean Run) 
        {
            if (!Run) return false;
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            Categories categories = doc.Settings.Categories;
            DB.Category pointCloudCategory = categories.get_Item(BuiltInCategory.OST_PointClouds);
            View activeView = doc.ActiveView;

            PointCloudOverrides pc_ovverides = activeView.GetPointCloudOverrides();

            if (activeView.ViewTemplateId == ElementId.InvalidElementId)
            {
                try
                {
                    TransactionManager.Instance.EnsureInTransaction(doc);
                    activeView.SetCategoryHidden(pointCloudCategory.Id, false);
                    TransactionManager.Instance.TransactionTaskDone();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        [NodeCategory("Actions")]
        public static Boolean Switch(Boolean Run)
        {
            if (!Run) return false;
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            Categories categories = doc.Settings.Categories;
            DB.Category pointCloudCategory = categories.get_Item(BuiltInCategory.OST_PointClouds);
            View activeView = doc.ActiveView;

            bool hide = activeView.GetCategoryHidden(pointCloudCategory.Id) ? false : true;
            PointCloudOverrides pc_ovverides = activeView.GetPointCloudOverrides();

            if (activeView.ViewTemplateId == ElementId.InvalidElementId)
            {
                try
                {
                    TransactionManager.Instance.EnsureInTransaction(doc);
                    activeView.SetCategoryHidden(pointCloudCategory.Id, hide);
                    TransactionManager.Instance.TransactionTaskDone();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;


        }

        [NodeCategory("Actions")]
        public static Boolean HideInView(Revit.Elements.Element view, Boolean Run)
        {
            
            return false;
        }
}
