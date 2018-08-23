using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AInBox.Astove.Core.Model;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Util
{
    public static class ExcelManager
    {
        public static T[] ReadFromExcel<T>(string path)
            where T : class, IBindingModel, new()
        {
            List<T> list = null;

            string fileExtension = System.IO.Path.GetExtension(path);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                string fileLocation = System.Web.Hosting.HostingEnvironment.MapPath(string.Concat("~", path));

                string excelConnectionString = string.Empty;

                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                if (fileExtension == ".xls")
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                else if (fileExtension == ".xlsx")
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();

                DataTable dt = new DataTable();
                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                    return null;
                
                String[] excelSheets = new String[dt.Rows.Count];
                int t = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                excelConnection.Close();

                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                DataSet ds = new DataSet();
                string query = string.Format("Select * from [{0}]", excelSheets[0]);
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                {
                    dataAdapter.Fill(ds);
                }

                list = new List<T>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    T model = new T();
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        try
                        {
                            object value = ds.Tables[0].Rows[i][prop.Name];
                            var requiredAttr = prop.GetCustomAttributes(true).OfType<RequiredAttribute>().FirstOrDefault();
                            if (requiredAttr != null && string.IsNullOrEmpty((string)Convert.ChangeType(value, typeof(string))))
                            {
                                model = null;
                                break;
                            }

                            try
                            {
                                Type propType = prop.PropertyType;
                                if (propType == typeof(bool))
                                    value = (value.Equals("1"));

                                if (propType.GetGenericArguments() != null && propType.GetGenericArguments().Length > 0)
                                    propType = propType.GetGenericArguments()[0];

                                prop.SetValue(model, Convert.ChangeType(value, propType), null);
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {
                        }
                    }

                    if (model != null)
                        list.Add(model);
                }
            }
            else
            {
                return null;
            }

            return list.ToArray();
        }
    }
}
