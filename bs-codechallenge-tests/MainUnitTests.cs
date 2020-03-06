//-------------------------------------------------------------------------------------------------
// Project:    bs-codechallenge-tests
// File:       MainUnitTests.cs
//
// Task:        Provides unit tests for the solution.
//
//     Date        Developer		Modifications
//      2020/03/04  R K             Started with unit tests for IsCsvFile().
//      2020/03/05  R K             Continued with tests for CheckPathForCsv().
//      2020/03/06  R K             Finished comments on tests.
//-------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bs_codechallenge;

namespace bs_codechallenge_tests
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Class for unit tests. </summary>
    ///
    /// <remarks>   R K, 2020/03/04. </remarks>
    ///-------------------------------------------------------------------------------------------------
    [TestClass]
    public class MainUnitTests
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests IsCsvFile() on a file path with a correct CSV file ending. </summary>
        /// 
        /// <remarks>   R K, 2020/03/04 </remarks>
        ///-------------------------------------------------------------------------------------------------
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
        /// <summary>   Tests IsCsvFile() on an invalid file path. </summary>
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
        /// <summary>   Tests IsCsvFile() on an (invalid) directory path. </summary>
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
        /// <summary>   Tests CheckPathForCsv() on a valid directory path. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_ValidDirectoryPath_ReturnsDirectory()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge";

            // Act
            Program.PathType actual = Program.CheckPathForCsv(validPath);

            // Assert
            Assert.AreEqual(Program.PathType.Directory, actual, 
                "Directory path with CSV files not recognized.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests CheckPathForCsv() on a valid directory path without CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_PathWithNoCsv_ReturnsInvalid()
        {
            // Arrange
            String validPath = ".";

            // Act
            Program.PathType actual = Program.CheckPathForCsv(validPath);

            // Assert
            Assert.AreEqual(Program.PathType.Invalid, actual, 
                "Directory path without CSV files is not rejected correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests CheckPathForCsv() on an invalid path. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_InvalidPath_ReturnsInvalid()
        {
            // Arrange
            String validPath = "./blablebla";

            // Act
            Program.PathType actual = Program.CheckPathForCsv(validPath);

            // Assert
            Assert.AreEqual(Program.PathType.Invalid, actual, 
                "Invalid directory path is not rejected correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests CheckPathForCsv() on a valid file path. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_ValidFilePath_ReturnsFile()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge/test.csv";

            // Act
            Program.PathType actual = Program.CheckPathForCsv(validPath);

            // Assert
            Assert.AreEqual(Program.PathType.File, actual, 
                "Valid directory path is not recognized correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests CheckPathForCsv() on an invalid file path. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public void CheckPathForCsv_InvalidFilePath_ReturnsInvalid()
        {
            // Arrange
            String validPath = "../../../bs-codechallenge/testx.csv";

            // Act
            Program.PathType actual = Program.CheckPathForCsv(validPath);

            // Assert
            Assert.AreEqual(Program.PathType.Invalid, actual, 
                "Invalid directory path is not rejected correctly.");
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Tests CreateSqlTable() for its reaction without database access. </summary>
        /// 
        /// <remarks>   R K, 2020/03/05 </remarks>
        ///-----------------------------------------------  --------------------------------------------------
        [TestMethod]
        public async Task CreateSqlTable_NoDataBase_ReturnsNull()
        {
            // Arrange
            String tableName = "Test1";
            String[] columns = new String[] { "ID", "Name", "Birthday", "Town" };

            // Act
            String[] actual = await Program.CreateSqlTable(tableName, columns);

            // Assert
            Assert.AreEqual(null, actual,
                "CreateSqlTable() returns column names without database being there.");
        }
    }
}
