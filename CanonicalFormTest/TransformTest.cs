using Microsoft.VisualStudio.TestTools.UnitTesting;

using CanonicalForm;
using System.IO;
using System.Linq;
using CanonicalFormExceptions;

namespace CanonicalFormTest
{
    [TestClass]
    public class TransformTest
    {
        Parser parser = new Parser();

        [TestMethod]
        public void GivenExamplesTest()
        {
            string equation = "x^2 + 3.5xy + y = y^2 - xy + y";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "x^2 + 4.5xy - y^2 = 0");
            equation = "x = 1";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "x - 1 = 0");
            equation = "x - (y^2 - x) = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "2x - y^2 = 0");
            equation = "x - (0 - (0 - x)) = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "0 = 0");
        }

        [TestMethod]
        public void DefaultTest()
        {
            string equation = "(x + y) * (x + y) = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "x^2 + 2xy + y^2 = 0");
            equation = "x + (5 + (2 - 2x)) = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "-x + 7 = 0");
            equation = "x - (23 - (0 - x) + (x - 500)) = (y - 3)";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "-x + 480 - y = 0");
            equation = "5 - (xyx * (42.30xy^25 - 2.0)) = (z - 5)";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "10 + 2x^2y - 42.3x^3y^26 - z = 0");
        }

        [TestMethod]
        public void AddingExponentsTest()
        {
            string equation = "3x^2 + 4x^2 - 8x^2 - x^2 = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "-2x^2 = 0");
        }

        [TestMethod]
        public void MultiplyingExponentsTest()
        {
            string equation = "x^2 * yx * xy * y^2 = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "x^4y^4 = 0");
        }

        [TestMethod]
        public void LeadingNegativeTest()
        {
            string equation = "-x + 2xy = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "-x + 2xy = 0");
        }

        [TestMethod]
        public void SameVariablesInSummandTest()
        {
            
            string equation = "10zx10z = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "100xz^2 = 0");
        }

        [TestMethod]
        public void ZeroTest()
        {
            
            string equation = "2x = 2x";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "0 = 0");
            equation = "0 = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "0 = 0");
            equation = "0z - (xyx * 0) = 0";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "0 = 0");
        }

        [TestMethod]
        public void TrailingZeroTest()
        { 
            string equation = "5 - (xyx * (42.30xy^25 - 2.0)) = (z - 5)";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "10 + 2x^2y - 42.3x^3y^26 - z = 0");
        }

        [TestMethod]
        public void SpacesTest()
        { 
            string equation = "5 - (xyx *   (42.3 0xy^   25  - 2.0 ))   = (z - 5)";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "10 + 2x^2y - 42.3x^3y^26 - z = 0");
        }

        [TestMethod]
        public void BracketsTest()
        {
            string equation = "x - [0 - (0 - x * {x - 5})] = x";
            Assert.AreEqual(parser.TransformEquationToCanonical(equation), "5x - x^2 = 0");
        }

        [TestMethod]
        public void FileTest()
        {
            string inputFilename = "inputTest.txt";
            string outputTest = "expectedOutput.txt.out";
            string outputFilename = parser.TransformFileToCanonical(inputFilename);
            Assert.IsTrue(File.ReadAllBytes(outputFilename).SequenceEqual(File.ReadAllBytes(outputTest)));
        }

        [ExpectedException(typeof(InvalidEquationException))]
        public void MismatchedParenthesesTest()
        {
            string equation = "x - ((0) = y";
            string result = parser.TransformEquationToCanonical(equation);
            equation = "x - (0)) = y";
            result = parser.TransformEquationToCanonical(equation);
            equation = "x = y - (";
            result = parser.TransformEquationToCanonical(equation);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEquationException))]
        public void InvalidOperatorsTest()
        {
            string equation = "x ++ y^2 = x";
            string result = parser.TransformEquationToCanonical(equation);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidEquationException))]
        public void InvalidEqualsTest()
        {

            string equation = "x + y^2 = = x";
            string result = parser.TransformEquationToCanonical(equation);
            equation = "x +  = y^2 = x";
            result = parser.TransformEquationToCanonical(equation);
            equation = "= x";
            result = parser.TransformEquationToCanonical(equation);
        }

        [ExpectedException(typeof(InvalidEquationException))]
        public void InvalidExponentTest()
        {

            string equation = "x^4.3 = y";
            string result = parser.TransformEquationToCanonical(equation);
        }

        [ExpectedException(typeof(InvalidEquationException))]
        public void UnknownTokenTest()
        {

            string equation = "x @ 5 = x";
            string result = parser.TransformEquationToCanonical(equation);
        }
    }
}