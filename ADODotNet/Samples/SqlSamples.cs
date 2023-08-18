using System.Diagnostics;

namespace ADODotNet.Samples;

using System.Data;
using Microsoft.Data.SqlClient;

public class SqlSamples
{
    private string connectionStrings =
        "Server=.;Database=OnlineShopDb;Trusted_Connection=true;TrustServerCertificate=True";

    private SqlConnection _sqlConnection;

    public SqlSamples()
    {
        _sqlConnection = new SqlConnection(connectionStrings);
    }

    public void FirstSample()
    {
        _sqlConnection.Open();

        SqlCommand command = _sqlConnection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = "SELECT * FROM Categories";

        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"Id : {reader["Id"]}\tName : {reader["CategoryName"]}");
        }

        Console.WriteLine(_sqlConnection.State);

        _sqlConnection.Close();
        Console.WriteLine(_sqlConnection.State);
        Console.ReadKey();
    }

    public void WorkingWithConnection()
    {
        _sqlConnection.Open();
        Console.WriteLine(_sqlConnection.Database);
        Console.WriteLine(_sqlConnection.DataSource);
        Console.WriteLine(_sqlConnection.CommandTimeout);
        Console.WriteLine(_sqlConnection.ConnectionTimeout);


        _sqlConnection.Close();
    }


    public void ConnectionBuilder()
    {
        /*SqlConnectionStringBuilder connectionStringBuilder = new();
        connectionStringBuilder.InitialCatalog = "OnlineShopDb";
        connectionStringBuilder.DataSource = ".";
        connectionStringBuilder.TrustServerCertificate = true;
        connectionStringBuilder.CommandTimeout = 200;
        connectionStringBuilder.ConnectTimeout = 400;
        
        SqlConnection connection  = new (connectionStringBuilder.ConnectionString);

        connection.Open();
        Console.WriteLine(connection.Database);
        Console.WriteLine(connection.DataSource);
        Console.WriteLine(connection.CommandTimeout);
        Console.WriteLine(connection.ConnectionTimeout);*/
    }

    public void TestCommand()
    {
        SqlCommand sqlCommand = new();

        sqlCommand.Connection = _sqlConnection;
        sqlCommand.CommandType = CommandType.Text;
        sqlCommand.CommandText = "SELECT * FROM Categories";
        _sqlConnection.Open();
        // sqlCommand.Prepare();
        SqlDataReader reader = sqlCommand.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"Id : {reader["Id"]}\tName : {reader["CategoryName"]}");
        }

        Console.WriteLine(_sqlConnection.State);
        sqlCommand.Dispose();
        _sqlConnection.Close();
    }

    public void TestReader()
    {
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            CommandText = "SELECT * FROM Categories",
        };

        _sqlConnection.Open();
        // sqlCommand.Prepare();
        SqlDataReader reader = sqlCommand.ExecuteReader();

        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write(reader.GetName(i));
                Console.Write(":");
                Console.Write(reader.GetValue(i));
                Console.Write("\t");
            }

            Console.WriteLine();
        }

        sqlCommand.Dispose();
        _sqlConnection.Close();
    }

    public void TestReaderMultiple()
    {
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            CommandText = "SELECT * FROM Categories;SELECT * FROM Products",
        };

        _sqlConnection.Open();
        SqlDataReader reader = sqlCommand.ExecuteReader();
        do
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetName(i));
                    Console.Write(":");
                    Console.Write(reader.GetValue(i));
                    Console.Write("\t");
                }

                Console.WriteLine();
            }

            Console.WriteLine("".PadLeft(100, '_'));
        } while (reader.NextResult());

        sqlCommand.Dispose();
        _sqlConnection.Close();
    }

    public void TestReaderMultipleWithProc()
    {
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            CommandText = "MultipeResult",
        };

        _sqlConnection.Open();
        SqlDataReader reader = sqlCommand.ExecuteReader();
        do
        {
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetName(i));
                    Console.Write(":");
                    Console.Write(reader.GetValue(i));
                    Console.Write("\t");
                }

                Console.WriteLine();
            }

            Console.WriteLine("".PadLeft(100, '_'));
        } while (reader.NextResult());

        sqlCommand.Dispose();
        _sqlConnection.Close();
    }

    public void AddProduct(int categoryId, string productName, string description, int price)
    {
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            CommandText =
                $"INSERT INTO Products (CategoryId,ProductName,Description,Price) VALUES ({categoryId},'{productName}','{description}',{price})",
        };

        _sqlConnection.Open();
        int result = sqlCommand.ExecuteNonQuery();
    }

    public void AddProductWithParameter(int categoryId, string productName, string description, int price)
    {
        SqlParameter categoryIdParameter = new()
        {
            ParameterName = "@CategoryId",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Input,
            Value = categoryId
        };
        SqlParameter productNameIdParameter = new()
        {
            ParameterName = "@ProductName",
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            Value = productName
        };
        SqlParameter descriptionParameter = new()
        {
            ParameterName = "@Description",
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            Value = description
        };
        SqlParameter priceParameter = new()
        {
            ParameterName = "@Price",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Input,
            Value = price
        };
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            CommandText =
                $"INSERT INTO Products (CategoryId,ProductName,Description,Price) VALUES (@CategoryId,@ProductName,@Description,@Price)",
            Parameters =
            {
                categoryIdParameter,
                productNameIdParameter,
                descriptionParameter,
                priceParameter
            }
        };

        _sqlConnection.Open();
        int result = sqlCommand.ExecuteNonQuery();
    }

    public void AddTransactional(string categoryName, int categoryId, string productName, string description, int price)
    {
        SqlTransaction sqlTransaction = null;
        SqlParameter categoryNameParameter = new()
        {
            ParameterName = "@CategoryName",
            DbType = DbType.String,
            Value = categoryName
        };

        SqlCommand addCategory = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text,
            Transaction = sqlTransaction,
            CommandText =
                $"INSERT INTO Categories (CategoryName) VALUE (@CategoryName)",
            Parameters =
            {
                categoryNameParameter
            }
        };

        SqlParameter categoryIdParameter = new()
        {
            ParameterName = "@CategoryId",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Input,
            Value = categoryId
        };
        SqlParameter productNameIdParameter = new()
        {
            ParameterName = "@ProductName",
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            Value = productName
        };
        SqlParameter descriptionParameter = new()
        {
            ParameterName = "@Description",
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            Value = description
        };
        SqlParameter priceParameter = new()
        {
            ParameterName = "@Price",
            DbType = DbType.Int32,
            Direction = ParameterDirection.Input,
            Value = price
        };
        SqlCommand addProduct = new()
        {
            Connection = _sqlConnection,
            Transaction = sqlTransaction,
            CommandType = CommandType.Text,
            CommandText =
                $"INSERT INTO Products (CategoryId,ProductName,Description,Price) VALUES (@CategoryId,@ProductName,@Description,@Price)",
            Parameters =
            {
                categoryIdParameter,
                productNameIdParameter,
                descriptionParameter,
                priceParameter
            }
        };

        _sqlConnection.Open();

        try
        {
            sqlTransaction = _sqlConnection.BeginTransaction();
            int result = addCategory.ExecuteNonQuery();
            result += addProduct.ExecuteNonQuery();
            //   sqlTransaction.Commit();
            Console.WriteLine($"Effected Row {result}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            //     sqlTransaction.Rollback();
            throw;
        }
    }


    public void AddBulkInsert()
    {
        SqlCommand sqlCommand = new()
        {
            Connection = _sqlConnection,
            CommandType = CommandType.Text
        };

        Stopwatch stopwatch = Stopwatch.StartNew();
        _sqlConnection.Open();
        for (int i = 0; i < 1_000_000; i++)
        {
            sqlCommand.CommandText =
                $"INSERT INTO BulkNames(FirstName,LastName) VALUES ('FirstName {i}','LastName {i}')";
            sqlCommand.ExecuteNonQuery();
        }

        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }

    public void AddBulkCopyInsert()
    {
        SqlBulkCopy sqlBulkCopy = new(_sqlConnection);
        sqlBulkCopy.ColumnMappings.Add("FirstName", "FirstName");
        sqlBulkCopy.ColumnMappings.Add("LastName", "LastName");
        sqlBulkCopy.DestinationTableName = "BulkNames";
        _sqlConnection.Open();
        Stopwatch stopwatch = Stopwatch.StartNew();

        DataTable? names = new DataTable();
        names.Columns.Add("FirstName", typeof(string));
        names.Columns.Add("LastName", typeof(string));

        for (int i = 0; i < 1_000_000; i++)
        {
            var dataRow = names.NewRow();
            dataRow["FirstName"] = $"FirstName {i}";
            dataRow["LastName"] = $"LastName {i}";
            names.Rows.Add(dataRow);
        }
        sqlBulkCopy.WriteToServer(names);
        stopwatch.Stop();
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }
}