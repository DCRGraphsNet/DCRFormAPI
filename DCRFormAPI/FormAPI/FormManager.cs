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

        public static IActionResult ExecuteField(int formid, string activityid, string value, string userid, string role, string isNull)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }
                
            try
            {
                var path = new DCR.Path(activityid);
                Value valueinput;
                if (isNull == "true")
                {
                    valueinput = new Value();
                }
                else
                {
                    valueinput = graph.ParseToValue(path, value);
                }
                Form.execute(graph, path, valueinput, userid, role);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            return new OkResult();
        }

        public static IActionResult GetFormFields(int formid)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }
            StringWriter sw = new StringWriter();
            Form.getFormFields(graph, sw);
            return new OkObjectResult(sw.ToString());
        }

        public static IActionResult GetForm(int formid)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }
            StringWriter sw = new StringWriter();
            Form.getForm(graph, sw);
            return new OkObjectResult(sw.ToString());
        }

        public static IActionResult ExecuteAndFormFields(int formid, string activityid, string value, string userid, string role, string isNull)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }

            try
            {
                var path = new DCR.Path(activityid);
                Value valueinput;
                if (isNull == "true")
                {
                    valueinput = new Value();
                }
                else
                {
                    valueinput = graph.ParseToValue(path, value);
                }

                StringWriter sw = new StringWriter();
                Form.executeAndFormFields(graph, path, valueinput, userid, role, sw);
                return new OkObjectResult(sw.ToString());
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
        }

        public static IActionResult Submit(int formid)
        {
            if (ActiveForms == null)
            {
                ActiveForms = new Dictionary<int, Graph>();
            }
            var success = ActiveForms.TryGetValue(formid, out Graph graph);
            if (!success) { return new UnprocessableEntityObjectResult(formid + " is not active"); }

            XElement xml = Form.submit(graph);
            ActiveForms.Remove(formid);
            return new OkObjectResult(xml.ToString());

        }

    }
}
