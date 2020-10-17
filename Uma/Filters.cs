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
                    string ruleType = CastFilterRule(r);
                    filterRulesEvaluatorType.Add(ruleType);
                }
            }
            return filterRulesEvaluatorType;
        }
        [NodeCategory("Actions")]
        public static List<string> GetFilterRuleMethod(RevitElements.Element ParameterFilterElement)
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
                    string ruleMethod = FilterRuleMethod(CastFilterRule(r));
                    filterRulesEvaluatorType.Add(ruleMethod);
                }
            }
            return filterRulesEvaluatorType;
        }

        public static string CreateFilterRule(string Method, string[] Parameters)
        {
            try
            {
                object filterRule = typeof(ParameterFilterRuleFactory)
                        .GetMethod(Method)
                        .Invoke(null, Parameters);
                if (filterRule != null) return "Filter Rule created";
                else return "Something happened";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        //Private Method
        private static string CastFilterRule(FilterRule r)
        {
            if (r is FilterStringRule)
            {
                FilterStringRule filterRule = r as FilterStringRule;
                FilterStringRuleEvaluator evaluator = filterRule.GetEvaluator();
                if (filterRule.GetEvaluator() is FilterStringBeginsWith) return "FilterStringBeginsWith";
                else if (filterRule.GetEvaluator() is FilterStringContains) return "FilterStringContains";
                else if (filterRule.GetEvaluator() is FilterStringEndsWith) return "FilterStringEndsWith";
                else if (filterRule.GetEvaluator() is FilterStringEquals) return "FilterStringEquals";
                else if (filterRule.GetEvaluator() is FilterStringGreaterOrEqual) return "FilterStringGreaterOrEqual";
                else if (filterRule.GetEvaluator() is FilterStringLess) return "FilterStringLess";
                else if (filterRule.GetEvaluator() is FilterStringLessOrEqual) return "FilterStringLessOrEqual";
            }
            else if (r is FilterIntegerRule || r is FilterDoubleRule || r is FilterGlobalParameterAssociationRule)
            {
                FilterIntegerRule filterRule = r as FilterIntegerRule;
                FilterNumericRuleEvaluator evaluator = filterRule.GetEvaluator();
                if (filterRule.GetEvaluator() is FilterNumericEquals) return"FilterNumericEquals";
                else if (filterRule.GetEvaluator() is FilterNumericGreater) return "FilterNumericGreater";
                else if (filterRule.GetEvaluator() is FilterNumericGreaterOrEqual) return "FilterNumericGreaterOrEqual";
                else if (filterRule.GetEvaluator() is FilterNumericLess) return "FilterNumericLess";
                else if (filterRule.GetEvaluator() is FilterNumericLessOrEqual) return "FilterNumericLessOrEqual";
            }
            else if (r is FilterInverseRule)
            {
                FilterInverseRule filterRule = r as FilterInverseRule;
                FilterRule innerRule = filterRule.GetInnerRule();
                return CastFilterRule(innerRule) + "_InnerRule";
            }
            else if (r is SharedParameterApplicableRule)
            {
                return "SharedParameterApplicableRule";
            };
            return null;
        }

        private static string FilterRuleMethod(string FilterRuleEvaluatorType)
        {
            if (FilterRuleEvaluatorType      == "FilterStringBeginsWith") return "CreateBeginsWithRule"; // 1
            else if (FilterRuleEvaluatorType == "FilterStringContains") return "CreateContainsRule";
            else if (FilterRuleEvaluatorType == "FilterStringEndsWith") return "CreateEndsWithRule";
            else if (FilterRuleEvaluatorType == "FilterStringGreaterOrEqual") return "CreateGreaterOrEqualRule";
            else if (FilterRuleEvaluatorType == "FilterStringLess") return "CreateLessRule";
            else if (FilterRuleEvaluatorType == "FilterStringLessOrEqual") return "CreateLessOrEqualRule";
            else if (FilterRuleEvaluatorType == "FilterStringEquals") return "CreateEqualsRule";
            
            else if (FilterRuleEvaluatorType == "FilterNumericEquals") return "CreateEqualsRule";
            else if (FilterRuleEvaluatorType == "FilterNumericGreater") return "CreateGreaterRule";
            else if (FilterRuleEvaluatorType == "FilterNumericGreaterOrEqual") return "CreateGreaterOrEqualRule";
            else if (FilterRuleEvaluatorType == "FilterNumericLess") return "CreateLessRule";
            else if (FilterRuleEvaluatorType == "FilterNumericLessOrEqual") return "CreateLessOrEqualRule";

            //Inverse Method
            else if (FilterRuleEvaluatorType == "FilterStringBeginsWith_InnerRule") return "CreateNotBeginsWithRule";
            else if (FilterRuleEvaluatorType == "FilterStringContains_InnerRule") return "CreateNotContainsRule";
            else if (FilterRuleEvaluatorType == "FilterStringEndsWith_InnerRule") return "CreateNotEndsWithRule";
            else if (FilterRuleEvaluatorType == "FilterNumericEquals_InnerRule") return "CreateNotEqualsRule";
            else if (FilterRuleEvaluatorType == "FilterStringLess_InnerRule") return "CreateLessRule";
            else if (FilterRuleEvaluatorType == "FilterStringLessOrEqual_InnerRule") return "CreateLessOrEqualRule";

            else if (FilterRuleEvaluatorType == "SharedParameterApplicableRule") return "CreateSharedParameterApplicableRule";
            else if (FilterRuleEvaluatorType == "SharedParameterApplicableRule_InnerRule") return "CreateLessOrEqualRule";
            
            return null;
        }


    }
}
