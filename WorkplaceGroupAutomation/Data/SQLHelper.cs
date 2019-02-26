using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using WorkplaceGroupAutomation.Models;
using Dapper;

namespace WorkplaceGroupAutomation.Data
{

    /// <summary>
    /// Performs buffered bulk inserts into a sql server table using objects instead of DataRows. :)
    /// </summary>
    public static class SQLHelper
    {
        #region Properties
        
        enum LogTypes
        {
            Log = 1,
            Error = 2
        }

        #endregion

        public static bool BulkInsert<T>(string connectionString, IEnumerable<T> list, int batchSize = 0, string table = null)
        {
            var result = true;
            try
            {

                //LogMessage(connectionString, 0, "BulkInsert", (int)LogTypes.Log, "Bulk Insert Started for " + table ?? ""  , 0, "");
                using (var bulkCopy = new SqlBulkCopy(connectionString))
                {
                    var type = typeof(T);

                    var tableName = type.Name;
                    var tableAttribute = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();
                    if (tableAttribute != null)
                    {
                        var schemaName = String.IsNullOrWhiteSpace(tableAttribute.Schema) ? tableAttribute.Schema + "." : "";
                        tableName = schemaName + tableAttribute.Name;
                    }
                    if (table != null) tableName = table;

                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tableName;

                    var dataTable = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))
                                               //Only have system data types 
                                               //i.e. filter out the relationships/collections
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .Where(i => !i.Attributes.OfType<DatabaseGeneratedAttribute>().Any())
                                               .Where(i => !i.Attributes.OfType<NotMappedAttribute>().Any())
                                               .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        var colName = propertyInfo.Name;

                        var colAttribute = propertyInfo.Attributes.OfType<ColumnAttribute>().FirstOrDefault();
                        if (colAttribute != null) colName = colAttribute.Name;

                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, colName);
                        dataTable.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        dataTable.Rows.Add(values);
                    }
                    //ClearTable(connectionString, table);
                    bulkCopy.WriteToServer(dataTable);
                }

                //LogMessage(connectionString, 0, "BulkInsert", (int)LogTypes.Log, "Bulk Insert Completed for " + table ?? "", 0, "");
            }
            catch(Exception ex)
            {
                //LogMessage(connectionString,0, "BulkInsert", (int)LogTypes.Error, (table ?? "") + " - " + ex.Message, 0, "");
                result = false;
            }

            return result;
        }
        

        public static List<dynamic> RetrieveDataFromStoredProcedure(string connectionString, string sp)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<dynamic>(sp, commandType: CommandType.StoredProcedure).ToList();
            }
        }
        
    }
}