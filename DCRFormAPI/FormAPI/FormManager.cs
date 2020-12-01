using DCR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FormAPI.FormAPI
{
    public static class FormManager
    {

        private static Dictionary<int, Graph> ActiveForms;
        private static int currentIndex = 0;

        /*Takes a form, adds it to the map of active forms*/
        public static IActionResult InitialiseForm(string input)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }

            int index = currentIndex++;
            XDocument xmlInput = XDocument.Parse(input);
            Graph graph = Graph.FromXml(xmlInput);
            graph.Initialise();
            ActiveForms.Add(index, graph);
            return new OkObjectResult(index);
        }

        public static IActionResult ExecuteField(int formid, string activityid, string value, string userid, string role)
        {
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }

            try
            {
                Form.execute(graph, new DCR.Path(activityid), Value.Parse(value), userid, role);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.InnerException.Message);
            }
            return new OkResult();
        }

        public static IActionResult GetFormFields(int formid)
        {
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }
            StringWriter sw = new StringWriter();
            Form.getFormFields(graph, sw);
            return new OkObjectResult(sw.ToString());
        }

        public static IActionResult GetForm(int formid)
        {
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }
            StringWriter sw = new StringWriter();
            Form.getForm(graph, sw);
            return new OkObjectResult(sw.ToString());
        }

        public static IActionResult ExecuteAndFormFields(int formid, string activityid, string value, string userid, string role)
        {
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }

            try
            {
                StringWriter sw = new StringWriter();
                Form.executeAndFormFields(graph, new DCR.Path(activityid), Value.Parse(value), userid, role, sw);
                return new OkObjectResult(sw.ToString());
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.InnerException.Message);
            }
            
        }

        public static IActionResult Submit(int formid)
        {
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }

            XElement xml = Form.submit(graph);
            ActiveForms.Remove(formid);
            return new OkObjectResult(xml.ToString());

        }

    }
}
