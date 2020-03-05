//-------------------------------------------------------------------------------------------------
// Project:    bs-codechallenge-tests
// File:       MainUnitTests.cs
//
// Task:        Provides unit tests for solution.
//
//     Date        Developer		Modifications
//      2020/03/04  R K             Started with unit tests for IsCsvFile().
//      2020/03/05  R K             Continued with tests for CheckPathForCsv().
//
//-------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows.Forms;
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


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_ValidDirectoryPath_ReturnsTrue()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge";

            //MessageBox.Show(Directory.GetCurrentDirectory());

            // Act
            bool actual = Program.PathContainsCsv(validPath);

            // Assert
            Assert.AreEqual(true, actual, "Directory path with CSV files not recognized.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_PathWithNoCsv_ReturnsFalse()
        {
            // Arrange
            String validPath = ".";

            //MessageBox.Show(Directory.GetCurrentDirectory());

            // Act
            bool actual = Program.PathContainsCsv(validPath);

            // Assert
            Assert.AreEqual(false, actual, "Directory path without CSV files is not rejected correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_InvalidPath_ReturnsFalse()
        {
            // Arrange
            String validPath = "./blablebla";

            // Act
            bool actual = Program.PathContainsCsv(validPath);

            // Assert
            Assert.AreEqual(false, actual, "Invalid directory path is not rejected correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_ValidFilePath_ReturnsTrue()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge/test.csv";

            // Act
            bool actual = Program.PathContainsCsv(validPath);

            // Assert
            Assert.AreEqual(true, actual, "Valid directory path is not recognized correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   XXX Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_InvalidFilePath_ReturnsFalse()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge/testx.csv";

            // Act
            bool actual = Program.PathContainsCsv(validPath);

            // Assert
            Assert.AreEqual(false, actual, "Invalid directory path is not rejected correctly.");
        }
    }
}
