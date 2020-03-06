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
//      2020/03/04  R K             Continued with file parsing and did SQL insertion.
//      2020/03/05  R K             Introduced batch processing of files, started UPSERT option.
//      2020/03/06  R K             Finished UPSERT option, checked comments.
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace bs_codechallenge
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Class for main program parts. Only class (apart from testing). </summary>
    ///
    /// <remarks>   R K, 2020/03/01. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public static class Program
    {
        /// <summary> Contains all the information for the SQL connection. </summary>
        public readonly static SqlConnectionStringBuilder sqlConString = new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            UserID = "SA",
            Password = "k28nJTvof3UTL5E",
            InitialCatalog = "MainDB"
        };

        /// <summary> Enumeration type for CheckPathForCsv return variable. </summary>
        public enum PathType
        {
            Directory,
            File,
            Invalid
        };

        /// <summary> <see langword="true"/> if upsert flag has been activated by user. </summary>
        private static bool upsertActive = false;


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Main function for CSV to SQL migration. Processes arguments and paths passed by 
        ///             user. </summary>
        /// 
        /// <remarks>   Flags have to be specified first. CSV path(s) come second.
        ///             R K, 2020/03/01. </remarks>
        ///
        /// <param name="args">     Command line input for the function. Can contain nothing, HELP or UPSERT 
        ///                         flag, and paths to CSV files or directories. </param>
        /// 
        /// <returns>   Returns 0. </returns>
        ///-------------------------------------------------------------------------------------------------
        public static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                ProcessFlags("--help");
                return 0;
            }

            int flagCount = 0;
            while (args.Length > flagCount && args[flagCount].Contains("--"))
            {
                ProcessFlags(args[flagCount]);
                flagCount++;
            }

            for (int i = args.Length - flagCount; i > 0; i--)
            {
                String inputPath = args[args.Length - i];
                await ProcessInputPath(inputPath);
            }

            return 0;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Method for processing flag arguments to main function. </summary>
        /// 
        /// <remarks>   
        ///     Flag options are either "--upsert" or anything else (with two leading dashes), which is 
        ///     treated like a "--help" flag. This in turn produces commentarial output.
        /// 
        ///     R K, 2020/03/05
        /// </remarks>
        ///
        /// <param name="flag">    Flag argument from command line. </param>
        ///-------------------------------------------------------------------------------------------------
        private static void ProcessFlags(String flag)
        {
            switch(flag)
            {
                case "--upsert":
                    upsertActive = true;
                    break;

                default:
                        Console.WriteLine("");
                        Console.WriteLine("usage: bs-codechallenge <path>");
                        Console.WriteLine("");
                        Console.WriteLine("The path can either lead directly to a CSV file or to a directory with "
                            + "one or more CSV files. Also, multiple paths can be provided.");
                        break;
            }
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Method for processing a path passed by the user which is supposed to lead to one or 
        ///             more CSV files. </summary>
        /// 
        /// <remarks>   
        ///     If a given path is a valid file path, it is directly forwarded to the CSV-SQL converter. If
        ///     it is a valid directory path, all CSV files in this directory (none from the subdirectories)
        ///     are forwarded to the CSV-SQL converter. In any other case, only an explainatory output is
        ///     produced.
        /// 
        ///     R K, 2020/03/05
        /// </remarks>
        ///
        /// <param name="inputPath">    Given path potentially leading to CSV file(s). </param>
        /// 
        /// <returns>   Returns 0. </returns>
        ///-------------------------------------------------------------------------------------------------
        public static async Task<int> ProcessInputPath(string inputPath)
        {
            switch (CheckPathForCsv(inputPath))
            {
                case PathType.File:
                    await ConvertCsvToSql(inputPath);
                    break;

                case PathType.Directory:
                    Console.WriteLine("Batch migration active:");

                    String[] filePaths = Directory.GetFiles(inputPath);
                    foreach (String filePath in filePaths)
                    {
                        if (IsCsvFile(filePath))
                        {
                            await ConvertCsvToSql(filePath);
                        }
                    }
                    Console.WriteLine("Batch migration of directory {0} done.", inputPath);
                    break;

                default:
                    Console.WriteLine("ERROR: The specified path {0} does not lead to (a) CSV file(s). "
                        + "Hence, the path is skipped.", inputPath);
                    break;
            }

            return 0;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Function for checking if a specified <paramref name="path"/> leads to either a CSV 
        ///             file or a directory with one or more CSV files. </summary>
        /// 
        /// <remarks>   R K, 2020/03/01 </remarks>
        /// 
        /// <param name="path">    Path in question. </param>
        ///
        /// <returns>   Returns a PathType variable, specifying if the paths leads to a valid file or 
        ///             directory, or if it is invalid. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        public static PathType CheckPathForCsv(String path)
        {
            if (File.Exists(path) && IsCsvFile(path))
            {
                return PathType.File;
            }
            else if (Directory.Exists(path))
            {
                String[] filePaths = Directory.GetFiles(path);
                foreach (String filePath in filePaths)
                {
                    if (IsCsvFile(filePath))
                    {
                        return PathType.Directory;
                    }
                }
            }

            return PathType.Invalid;
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
        public static bool IsCsvFile(String path)
        {
            String fileEnding = path.Substring(path.Length - 4, 4);
            return fileEnding.ToLower() == ".csv";
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Inserts data from CSV file into an SQL data table. </summary>
        /// 
        /// <remarks>   
        ///     Extracts the table name from the path, creates the data table on the SQL server if possible
        ///     and then writes the records in the data table. Empty parts of the records are substituted
        ///     with NULLs. Depending on whether an UPSERT flag has been set, the data is either inserted or
        ///     upserted.
        /// 
        ///     R K, 2020/03/03 
        /// </remarks>
        /// 
        /// <param name="csvPath">    Path leading to the CSV file. </param>
        /// 
        /// <returns>   Returns 0. </returns>
        ///-------------------------------------------------------------------------------------------------
        private static async Task<int> ConvertCsvToSql(String csvPath)
        {
            String tableName = csvPath.Split('/').ToArray().Last();
            tableName = tableName.Split('.')[0].ToUpper();

            Console.WriteLine("File {0} is migrated to new SQL table {1}.", csvPath, tableName);

            using (StreamReader csvFile = new StreamReader(csvPath))
            {
                String columnLine = csvFile.ReadLine();
                String[] columns = columnLine.Split(new char[] { ';' });
                String[] columnNames = await CreateSqlTable(tableName, columns);

                if (columnNames == null)
                {
                    if (upsertActive)
                    {
                        Console.WriteLine("ERROR: Could not create SQL data table. "
                            + "An unknown error occured.");
                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Could not create SQL data table. "
                            + "A table '{0}' might already exist. " 
                            + "Potentially use UPSERT flag.", tableName);
                        return 0;
                    }
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

                    if (upsertActive)
                    {
                        if (!await UpsertRecord(tableName, columnNames, newRecord))
                        {
                            Console.WriteLine("ERROR: Could not upsert record no. {0}.", newRecord[0]);
                        }
                    }
                    else
                    {
                        if (!await InsertRecord(tableName, columnNames, newRecord))
                        {
                            Console.WriteLine("ERROR: Could not insert record no. {0}.", newRecord[0]);
                        }
                    }
                }
            }

            return 0;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Creates an SQL data table based on the column descriptions given in 
        ///             <paramref name="columns"/>. </summary>
        /// 
        /// <remarks>   
        ///     If in INSERTION mode, the method returns NULL if the new table could not be created (might
        ///     already exist). If in UPSERTION mode, the method returns NULL if something went wrong, but 
        ///     not, if the SQLException only indicated, that the table already exists.
        ///     
        ///     R K, 2020/03/03
        /// </remarks>
        /// 
        /// <param name="tableName">    Name for the new data table. </param>
        /// <param name="columns">      Array with column names and data types, separated by "$$". </param>
        /// 
        /// <returns>   Returns string array with column names after successful execution. If something went
        ///             wrong, NULL is returned. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private async static Task<String[]> CreateSqlTable(String tableName, String[] columns)
        {
            String[] columnNames = new String[columns.Length];

            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    StringBuilder sqlCmdText = new StringBuilder("CREATE TABLE " + tableName + " (");
                    for (int i = 0; i < columns.Length; i++)
                    {
                        // PARAMETER INSERT?
                        sqlCmdText.Append(columns[i].Replace("$$", " ") + ", ");

                        columnNames[i] = columns[i].Split(new String[] { "$$" }, StringSplitOptions.None).First();
                    }
                    sqlCmdText.Remove(sqlCmdText.Length - 2, 2);
                    sqlCmdText.Append(");");

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText.ToString(), dbConnection))
                    {
                        await sqlCmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (upsertActive && ex.Number == 2714)
                {
                    Console.WriteLine("Table {0} has been found in database.", tableName);
                    return columnNames;
                }
                else
                {
                    return null;
                }
            }

            return columnNames;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Inserts the record <paramref name="data"/> into the SQL table 
        ///             <paramref name="tableName"/>. </summary>
        /// 
        /// <remarks>   
        ///     Activates the IDENTITY_INSERT option for the data table in order to convert all data 
        ///     (including the ID values). The SQL command is constructed using the two StringBuilder
        ///     sqlCmdFields and sqlCmdValues.
        ///     
        ///     R K, 2020/03/03
        /// </remarks>
        /// 
        /// <param name="tableName">    Name for the new data table. </param>
        /// <param name="columnNames">  Array with column names. </param>
        /// <param name="data">         Array with record data. </param>
        /// 
        /// <returns>   Returns <see langword="true"/> if record has been inserted successfully into the table
        ///             <paramref name="tableName"/>, returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private async static Task<bool> InsertRecord(String tableName, String[] columnNames, String[] data)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    StringBuilder sqlCmdFields = new StringBuilder("SET IDENTITY_INSERT " 
                        + tableName + " ON; INSERT INTO " + tableName + " (");
                    StringBuilder sqlCmdValues = new StringBuilder(") VALUES (");

                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        sqlCmdFields.Append(columnNames[i] + ", ");
                        sqlCmdValues.Append("@" + columnNames[i] + ", ");
                    }

                    sqlCmdFields.Remove(sqlCmdFields.Length - 2, 2);
                    sqlCmdValues.Remove(sqlCmdValues.Length - 2, 2);
                    sqlCmdValues.Append(");");

                    sqlCmdFields.Append(sqlCmdValues.ToString());

                    //Console.WriteLine(sqlCmdFields.ToString());

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdFields.ToString(), dbConnection))
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            sqlCmd.Parameters.AddWithValue("@" + columnNames[i], data[i]);
                        }

                        await sqlCmd.ExecuteNonQueryAsync();
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
        /// <summary>   Upserts the record <paramref name="data"/> into the SQL table 
        ///             <paramref name="tableName"/>. </summary>
        /// 
        /// <remarks>   
        ///     Activates the IDENTITY_INSERT option for the data table in order to convert all data 
        ///     (including the ID values) when inserting the data. When updating the data, the ID column is 
        ///     left out as this cannot be updated. The SQL command is constructed using the three 
        ///     StringBuilder sqlCmdInsertFields, sqlCmdInsertValues and sqlCmdUpdate.
        ///     
        ///     R K, 2020/03/06
        /// </remarks>
        /// 
        /// <param name="tableName">    Name for the new data table. </param>
        /// <param name="columnNames">  Array with column names. </param>
        /// <param name="data">         Array with record data. </param>
        /// 
        /// <returns>   Returns <see langword="true"/> if record has been upserted successfully into the table
        ///             <paramref name="tableName"/>, returns <see langword="false"/> otherwise. </returns>
        ///-----------------------------------------------  --------------------------------------------------
        private async static Task<bool> UpsertRecord(String tableName, String[] columnNames, String[] data)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(sqlConString.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    StringBuilder sqlCmdInsertFields = new StringBuilder("SET IDENTITY_INSERT "
                        + tableName + " ON; "
                        + "IF NOT EXISTS (SELECT * FROM " + tableName + " WHERE " + columnNames[0] + "=@" + columnNames[0] + ") "
                        + "INSERT INTO " + tableName + " (");
                    StringBuilder sqlCmdInsertValues = new StringBuilder(") VALUES (");

                    StringBuilder sqlCmdUpdate = new StringBuilder(") ELSE "
                        + "UPDATE " + tableName
                        + " SET ");

                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        sqlCmdInsertFields.Append(columnNames[i] + ", ");
                        sqlCmdInsertValues.Append("@" + columnNames[i] + ", ");

                        if (i > 0)
                        {
                            sqlCmdUpdate.Append(columnNames[i] + "=@" + columnNames[i] + ", ");
                        }
                    }

                    sqlCmdInsertFields.Remove(sqlCmdInsertFields.Length - 2, 2);
                    sqlCmdInsertValues.Remove(sqlCmdInsertValues.Length - 2, 2);
                    sqlCmdUpdate.Remove(sqlCmdUpdate.Length - 2, 2);
                    sqlCmdUpdate.Append(" WHERE " + columnNames[0] + "=@" + columnNames[0] + ";");

                    sqlCmdInsertFields.Append(sqlCmdInsertValues.ToString());
                    sqlCmdInsertFields.Append(sqlCmdUpdate.ToString());

                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdInsertFields.ToString(), dbConnection))
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            sqlCmd.Parameters.AddWithValue("@" + columnNames[i], data[i]);
                        }

                        await sqlCmd.ExecuteNonQueryAsync();
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
