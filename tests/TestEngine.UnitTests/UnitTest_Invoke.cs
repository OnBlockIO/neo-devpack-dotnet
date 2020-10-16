using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using Neo.VM;
using Neo.VM.Types;
using System.IO;
using TestingEngine;
using Compiler = Neo.Compiler.Program;

namespace TestEngine.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke
    {
        [TestInitialize]
        public void Init()
        {
            string path = Directory.GetCurrentDirectory();
            var option = new Compiler.Options()
            {
                File = path + "/TestClasses/Contract1.cs"
            };
            Compiler.Compile(option);

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);
            Engine.Instance.Reset();
        }

        [TestMethod]
        public void Test_Missing_Arguments()
        {
            var args = new string[] {
                "./TestClasses/Contract1.nef"
            };
            var result = Program.Run(args);

            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.AreEqual(result["error"].AsString(), "One or more arguments are missing");
        }

        [TestMethod]
        public void Test_Method_Without_Parameters_Void()
        {
            var args = new string[] {
                "./TestClasses/Contract1.nef",
                "testVoid"
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // test result
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 0);
        }

        [TestMethod]
        public void Test_Method_Without_Parameters_With_Return()
        {
            var args = new string[] {
                "./TestClasses/Contract1.nef",
                "unitTest_001"
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // test result
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());
        }

        [TestMethod]
        public void Test_Method_With_Parameters()
        {
            StackItem arguments = 16;
            var args = new string[] {
                "./TestClasses/Contract1.nef",
                "testArgs1",
                arguments.ToJson().ToString()
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // test result
            StackItem wantresult = new byte[] { 1, 2, 3, 16 };
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());
        }

        [TestMethod]
        public void Test_Method_With_Parameters_Missing()
        {
            var args = new string[] {
                "./TestClasses/Contract1.nef",
                "testArgs1"
            };
            var result = Program.Run(args);

            // mustn have an error
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNotNull(result["error"]);

            // vm state must've faulted
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.FAULT.ToString());

            // result stack must be empty
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 0);
        }
    }
}
