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
    public class Filters
    {
        private Filters() { }

        [NodeCategory("Actions")]
        public static List<RevitElements.Element> GetFiltersFromActiveView(bool Run)
        {
            if (!Run) return null;

            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            List<RevitElements.Element> filters = new List<RevitElements.Element>();
            ICollection<ElementId> filterIds = doc.ActiveView.GetFilters();
            List<DB.Element> parameterFilterElements = filterIds.Select(id => doc.GetElement(id)).ToList();


            foreach (DB.Element pfe in parameterFilterElements)
            {
                ParameterFilterElement _pfe = pfe as ParameterFilterElement;
                filters.Add(_pfe.ToDSType(true));
            }

            return filters;
        }
        [NodeCategory("Actions")]
        public static List<RevitElements.Element> GetFiltersFromView(Revit.Elements.Element View)
        {
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            List<RevitElements.Element> filters = new List<RevitElements.Element>();
            View _view = View.InternalElement as View;
            ICollection<ElementId> filterIds = _view.GetFilters();
            List<DB.Element> parameterFilterElements = filterIds.Select(id => doc.GetElement(id)).ToList();


            foreach (DB.Element pfe in parameterFilterElements)
            {
                ParameterFilterElement _pfe = pfe as ParameterFilterElement;
                filters.Add(_pfe.ToDSType(true));
            }

            return filters;
        }
        [NodeCategory("Actions")]
        public static List<RevitElements.Element> GetallFilters(bool Run)
        {
            if (!Run) return null;

            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<RevitElements.Element> filters = new List<RevitElements.Element>();
            ICollection<ElementId> filterIds = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).ToElementIds();
            List<DB.Element> parameterFilterElements = filterIds.Select(id => doc.GetElement(id)).ToList();


            foreach (DB.Element pfe in parameterFilterElements)
            {
                ParameterFilterElement _pfe = pfe as ParameterFilterElement;
                filters.Add(_pfe.ToDSType(true));
            }
            return filters;
        }
        [NodeCategory("Actions")]
        public static List<string> GetFilterRuleType(RevitElements.Element ParameterFilterElement)
        {
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<string> filterRulesType = new List<string>();

            DB.ParameterFilterElement parameterFilterElement = (DB.ParameterFilterElement)ParameterFilterElement.InternalElement;
            ElementLogicalFilter elementFilter = parameterFilterElement.GetElementFilter() as ElementLogicalFilter;

            IList<ElementFilter> efs = elementFilter.GetFilters();

            foreach (ElementFilter ef in efs)
            {
                ElementParameterFilter epf = ef as ElementParameterFilter;
                foreach (FilterRule r in epf.GetRules())
                {
                    if (r is FilterDoubleRule) filterRulesType.Add("FilterDoubleRule");
                    else if (r is FilterStringRule) filterRulesType.Add("FilterStringRule");
                    else if (r is FilterIntegerRule) filterRulesType.Add("FilterIntegerRule");
                    else if (r is FilterGlobalParameterAssociationRule) filterRulesType.Add("FilterGlobalParameterAssociationRule");
                    else if (r is FilterInverseRule) filterRulesType.Add("FilterInverseRule");
                    else if (r is SharedParameterApplicableRule) filterRulesType.Add("SharedParameterApplicableRule");
                }
            }
            return filterRulesType;
        }
        [NodeCategory("Actions")]
        public static List<string> GetFilterRuleEvaluatorType(RevitElements.Element ParameterFilterElement)
        {
            DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<string> filterRulesEvaluatorType = new List<string>();

            DB.ParameterFilterElement parameterFilterElement = (DB.ParameterFilterElement)ParameterFilterElement.InternalElement;
            ElementLogicalFilter elementFilter = parameterFilterElement.GetElementFilter() as ElementLogicalFilter;

            IList<ElementFilter> efs = elementFilter.GetFilters();

            foreach (ElementFilter ef in efs)
            {
                ElementParameterFilter epf = ef as ElementParameterFilter;
                foreach (FilterRule r in epf.GetRules())
                {
                    if (r is FilterStringRule)
                    {
                        FilterStringRule filterRule = r as FilterStringRule;
                        FilterStringRuleEvaluator evaluator = filterRule.GetEvaluator();
                        if (filterRule.GetEvaluator() is FilterStringBeginsWith) filterRulesEvaluatorType.Add("FilterStringBeginsWith");
                        else if (filterRule.GetEvaluator() is FilterStringContains) filterRulesEvaluatorType.Add("FilterStringContains");
                        else if (filterRule.GetEvaluator() is FilterStringEndsWith) filterRulesEvaluatorType.Add("FilterStringEndsWith");
                        else if (filterRule.GetEvaluator() is FilterStringEquals) filterRulesEvaluatorType.Add("FilterStringEquals");
                        else if (filterRule.GetEvaluator() is FilterStringGreaterOrEqual) filterRulesEvaluatorType.Add("FilterStringGreaterOrEqual");
                        else if (filterRule.GetEvaluator() is FilterStringLess) filterRulesEvaluatorType.Add("FilterStringLess");
                        else if (filterRule.GetEvaluator() is FilterStringLessOrEqual) filterRulesEvaluatorType.Add("FilterStringLessOrEqual");
                    }
                    else if (r is FilterIntegerRule || r is FilterDoubleRule || r is FilterGlobalParameterAssociationRule)
                    {
                        FilterIntegerRule filterRule = r as FilterIntegerRule;
                        FilterNumericRuleEvaluator evaluator = filterRule.GetEvaluator();
                        if (filterRule.GetEvaluator() is FilterNumericEquals) filterRulesEvaluatorType.Add("FilterNumericEquals");
                        else if (filterRule.GetEvaluator() is FilterNumericGreater) filterRulesEvaluatorType.Add("FilterNumericGreater");
                        else if (filterRule.GetEvaluator() is FilterNumericGreaterOrEqual) filterRulesEvaluatorType.Add("FilterNumericGreaterOrEqual");
                        else if (filterRule.GetEvaluator() is FilterNumericLess) filterRulesEvaluatorType.Add("FilterNumericLess");
                        else if (filterRule.GetEvaluator() is FilterNumericLessOrEqual) filterRulesEvaluatorType.Add("FilterNumericLessOrEqual");
                    }
                    else if (r is FilterInverseRule)
                    {
                        FilterInverseRule filterRule = r as FilterInverseRule;
                        FilterRule innerRule = filterRule.GetInnerRule();
                        filterRulesEvaluatorType.Add("InnerRule");
                    }
                    else if (r is SharedParameterApplicableRule)
                    {
                        filterRulesEvaluatorType.Add("SharedParameterApplicableRule");
                    };
                }
            }
            return filterRulesEvaluatorType;
        }
       

    }
}
