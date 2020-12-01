using Microsoft.AspNetCore.Mvc;
using System;

namespace FormAPI.Controllers
{
    public class InitializeFormInput
    {
        public string Xml { get; set; }
    }

    public class ExecuteFieldInput
    {
        public int Formid { get; set; }
        
        public string Activityid { get; set; }
        public string Value { get; set; }
        public string Userid { get; set; }
        public string Role { get; set; }
    }

    public class SubmitInput
    {
        public int Formid { get; set; }
    }

    public class GetFormFieldsInput
    {
        public int Formid { get; set; }
    }

    [ApiController]
    [Route("form")]
    public class FormController : ControllerBase
    {
        [Route("InitialiseForm"), HttpPost]
        public IActionResult InitialiseForm(InitializeFormInput input)
        {
            /*InitializeForm(<dcr xml>) -> formid (integer) – 
             * save the graph in an array with the formid being the index in the map*/
            var xml = input.Xml;
            IActionResult FormId = FormAPI.FormManager.InitialiseForm(xml);
            return FormId;
        }

        [Route("GetForm/{id}"), HttpGet]
        public IActionResult GetForm(int id)
        {
            //GetForm(formid) -> form(JSON)
            return FormAPI.FormManager.GetForm(id);
        }

        [Route("ExecuteField"), HttpPost]
        public IActionResult ExecuteField(ExecuteFieldInput input)
        {
            //ExecuteField(formid, activity id, value, user id, role) -> void
            var success = FormAPI.FormManager.ExecuteField(input.Formid, input.Activityid, input.Value, input.Userid, input.Role);
            if (success != null) { return success; }
            return success;
        }

        [Route("GetFormFields/{id}"), HttpGet]
        public IActionResult GetFormFields(int id)
        {
            //GetFormFields(formid) -> form fields (JSON) – like GetEnabledOrPending in a task list
            return FormAPI.FormManager.GetFormFields(id);
            
        }

        [Route("Submit"), HttpPost]
        public IActionResult Submit(SubmitInput input)
        {
            //Submit(formid) -> dcr xml log (xml) – also remove the form id from a list of active form ids
            return FormAPI.FormManager.Submit(input.Formid);
        }

        [Route("ExecuteAndFormFields"), HttpPost]
        public IActionResult ExecuteAndFormFields(ExecuteFieldInput input)
        {
            //ExecuteAndFormFields(formid, activity id, value, user id, role)
            return FormAPI.FormManager.ExecuteAndFormFields(input.Formid, input.Activityid, input.Value, input.Userid, input.Role);
        }
    }
}