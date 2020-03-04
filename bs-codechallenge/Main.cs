//-------------------------------------------------------------------------------------------------
// Project:    bs-codechallenge
// File:       Main.cs
//
// Task:        Transforms one or more given CSV files into an SQL data base.
//
//     Date        Developer		Modifications
//		2020/03/01	R K			    Started development. Introduced Main() and csv path checking 
//			                        function.
//      2020/03/03  R K             Finished csv path checking and started with file parsing
//      2020/03/04  R K             Continued with file parsing and did SQL insertion
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace bs_codechallenge
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Class for main program parts. Only class so far. </summary>
    ///
    /// <remarks>   R K, 2020/03/01. </remarks>
    ///-------------------------------------------------------------------------------------------------
    class Program
    {
        /// <summary> Contains all the information for the SQL connection. </summary>
        private static SqlConnectionStringBuilder sqlConString;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Main function for CSV to SQL migration. </summary>
        /// 
        /// <remarks>   
        ///     Initializes the SQL connection string builder and the default CSV file. Checks then the 
        ///     suitability of a given file or directory and starts the migration if applicable.
        /// 
        ///     R K, 2020/03/01
        /// </remarks>
        ///
        /// <param name="args">     Command line input for the function. Can only be a CSV file path for now. 
        ///                         </param>
        ///-------------------------------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            sqlConString = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                UserID = "SA",
                Password = "k28nJTvof3UTL5E",
                InitialCatalog = "MainDB"
            };

            String inputPath = @"../../test.csv";
            if (args.Length > 1)
            {
                inputPath = args[0];
            }

            if (PathContainsCsv(inputPath))
            {
                ConvertCsvToSql(inputPath);
            }
            else
            {
                Console.WriteLine("ERROR: The specified path does not lead to (a) CSV file(s). Please enter a valid path!");
            }

            return;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file(s), 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool PathContainsCsv(String path)
        {
            if (File.Exists(path) && IsCsvFile(path))
            {
                return true;
            }
            else if (Directory.Exists(path))
            {
                String[] filePaths = Directory.GetFiles(path);
                foreach (String filePath in filePaths)
                {
                    if (IsCsvFile(filePath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Function for checking if a file, specified by <paramref name="path"/> is a CSV file. 
        ///             </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="path">    Path leading to the file in question. </param>
        ///
        /// <returns>   Returns <see langword="true"/> if <paramref name="path"/> leads to CSV file, 
        ///             returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool IsCsvFile(String path)
        {
            String fileEnding = path.Substring(path.Length - 3, 3);
            return fileEnding.ToLower() == "csv";
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Converts a given CSV file into an SQL data table. </summary>
        /// 
        /// <remarks>   
        ///     Extracts the table name from the path, creates the data table on the SQL server if possible
        ///     and then writes the records in the data table. Empty parts of the records are substituted
        ///     with NULLs. If the data table could not be created, the method aborts.
        /// 
        ///     R K, 2020/03/03 
        /// </remarks>
        /// 
        /// <param name="csvPath">    Path leading to the CSV file. </param>
        ///-----------------------------------------------  --------------------------------------------------
        private static void ConvertCsvToSql(String csvPath)
        {
            String tableName = csvPath.Split('/').ToArray().Last();
            tableName = tableName.Split('.')[0].ToUpper();

            using (StreamReader csvFile = new StreamReader(csvPath))
            {
                String columnLine = csvFile.ReadLine();
                String[] columns = columnLine.Split(new char[] { ';' });

                if (!CreateSqlTable(tableName, columns))
                {
                    Console.WriteLine("ERROR: Could not create SQL data table. " + 
                    "A table '{0}' might already exist.", tableName);
                    return;
                }

                while(!csvFile.EndOfStream)
                {
                    String newLine = csvFile.ReadLine();
                    String[] newRecord = newLine.Split(new char[] { ';' });
                    for (int i = 0; i < newRecord.Length; i++)
                    {
                        if (newRecord[i] == "")
                        {
                            newRecord[i] = "NULL";
                        }
                    }

                    if (!InsertRecord(tableName, columns, newRecord))
                    {
                        Console.WriteLine("ERROR: Could not insert record no. {0}.", newRecord[0]);
                    }
                }
            }
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Creates an SQL data table based on the column descriptions given in 
        ///             <paramref name="columns"/>. </summary>
        /// 
        /// <remarks>   R K, 2020/03/03 </remarks>
        /// 
        /// <param name="tableName">    Name for the new data table. </param>
        /// <param name="columns">      Array with column names and data types, separated by "$$". </param>
        /// 
        /// <returns>   Returns <see langword="true"/> if table has been created successfully, returns 
        ///             <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool CreateSqlTable(String tableName, String[] columns)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    dbConnection.Open();

                    StringBuilder sqlCmdText = new StringBuilder("CREATE TABLE " + tableName + " (");
                    foreach (String column in columns)
                    {
                        sqlCmdText.Append(column.Replace("$$", " ") + ", ");
                    }
                    sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                    sqlCmdText.Append(");");

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText.ToString(), dbConnection))
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Inserts the record <paramref name="data"/> into the SQL table 
        ///             <paramref name="tableName"/>. </summary>
        /// 
        /// <remarks>   
        ///     Data for CHAR/VARCHAR or TEXT columns is surrounded by inverted commas.
        ///     
        ///     R K, 2020/03/03
        /// </remarks>
        /// 
        /// <param name="data">    Path leading to the CSV file. </param>
        /// 
        /// <param name="tableName">    Name for the new data table. </param>
        /// <param name="columns">      Array with column names and data types, separated by "$$". </param>
        /// <param name="data">         Array with record data. </param>
        /// 
        /// <returns>   Returns <see langword="true"/> if record has been inserted successfully into the table
        ///             <paramref name="tableName"/>, returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private static bool InsertRecord(String tableName, String[] columns, String[] data)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    dbConnection.Open();

                    StringBuilder sqlCmdText = new StringBuilder("INSERT INTO " + tableName + " VALUES (");
                    for (int i = 1; i < data.Length; i++)
                    {
                        if (columns[i].ToUpper().Contains("CHAR") || columns[i].ToUpper().Contains("TEXT"))
                        {
                            sqlCmdText.Append("'" + data[i] + "', ");
                        }
                        else
                        {
                            sqlCmdText.Append(data[i] + ", ");
                        }
                    }
                    sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                    sqlCmdText.Append(");");

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText.ToString(), dbConnection))
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }
    }
}
