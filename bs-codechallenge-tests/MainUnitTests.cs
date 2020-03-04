//-------------------------------------------------------------------------------------------------
// Project:    bs-codechallenge-tests
// File:       MainUnitTests.cs
//
// Task:        Provides unit tests for solution.
//
//     Date        Developer		Modifications
//      2020/03/04  R K             Started with unit tests.
//
//-------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bs_codechallenge;

namespace bs_codechallenge_tests
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   XXX Class for main program parts. Only class so far. </summary>
    ///
    /// <remarks>   R K, 2020/03/04. </remarks>
    ///-------------------------------------------------------------------------------------------------
    [TestClass]
    public class MainUnitTests
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a file, specified by <paramref name="path"/> is a CSV file. 
        ///             </summary>
        /// 
        /// <remarks>   R K, 2020/03/04 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void IsCsvFile_ValidPath_ReturnsTrue()
        {
            // Arrange
            String validPath = "blablablubb.csv";

            // Act
            bool actual = Program.IsCsvFile(validPath);

            // Assert
            Assert.AreEqual(true, actual, "CSV file not correctly recognized.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a file, specified by <paramref name="path"/> is a CSV file. 
        ///             </summary>
        /// 
        /// <remarks>   R K, 2020/03/04 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void IsCsvFile_InvalidPath_ReturnsFalse()
        {
            // Arrange
            String validPath = "./hallo/blablablubbcsv";

            // Act
            bool actual = Program.IsCsvFile(validPath);

            // Assert
            Assert.AreEqual(false, actual, "Invalid path incorrectly recognized as valid one.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a file, specified by <paramref name="path"/> is a CSV file. 
        ///             </summary>
        /// 
        /// <remarks>   R K, 2020/03/04 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void IsCsvFile_DirectoryPath_ReturnsFalse()
        {
            // Arrange
            String validPath = "./hallo/";

            // Act
            bool actual = Program.IsCsvFile(validPath);

            // Assert
            Assert.AreEqual(false, actual, "Directory path incorrectly recognized as CSV file.");
        }
    }
}
