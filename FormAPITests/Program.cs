using System;
using FormAPI.FormAPI;
using Microsoft.AspNetCore.Mvc;

namespace FormAPITests
{
    class Program
    {
        static void Main(string[] args)
        {
            FormAPITests();
        }

        public static void FormAPITests()
        {
            string input = System.IO.File.ReadAllText("ApplicationForm_example.xml");
            Test(input);
        }
        public static void Test(string input)
        {
            FormManager.InitialiseForm(input);
            var res = FormManager.ExecuteField(0, "L", "2", "Oliver", "DebuggingChampion", "false");
        }
    }
}
