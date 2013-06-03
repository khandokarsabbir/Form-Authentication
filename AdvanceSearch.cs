using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using RunrentSales.Model;
using System.Web.Mvc;
using System.Linq;

namespace RunrentSales.Domain.Services
{
    public class AdvanceSearch
    {
        private const string ModelNamespace = "RunrentSales.Model.";
        private RunrentDataContext context;
        private string tableName = "";
        private IList<string> columns;
        private List<TableColumn> columnList;
        protected List<NameValueList> mapping;

        #region Constructors

        public AdvanceSearch(RunrentDataContext context, string tableName, IList<string> columns)
        {
            this.context = context;
            Initilize(tableName, columns);
        }
        
        public AdvanceSearch(string tableName, IList<string> columns)
            : this(DatabaseHelper.RunrentDataContext, tableName, columns)
        {
        }
        
        public AdvanceSearch(RunrentDataContext context, string tableName, IList<TableColumn> tableColumns)
        {
            this.context = context;
            IList<string> temp = new List<string>();
            foreach (TableColumn tc in tableColumns)
                temp.Add(tc.ColumnName);
            Initilize(tableName, temp);
        }
        
        public AdvanceSearch(string tableName, IList<TableColumn> tableColumns)
            : this(DatabaseHelper.RunrentDataContext, tableName, tableColumns)
        {
        }
 
        #endregion

        private void Initilize(string tableName, IList<string> columns)
        {
            this.tableName = tableName;
            this.columns = columns;

            mapping = new List<NameValueList>();
            mapping.Add(new NameValueList() { Name = "=", Value = (byte)ExpressionValue.Equals });
            mapping.Add(new NameValueList() { Name = ">", Value = (byte)ExpressionValue.Greater });
            mapping.Add(new NameValueList() { Name = "<", Value = (byte)ExpressionValue.Less });

            columnList = new List<TableColumn>();

            List<TableColumn> tempTableColumn = new List<TableColumn>();
            tempTableColumn.Add(new TableColumn() { ColumnDbType = 0, ColumnName = "--Select--" });

            Type type = Type.GetType(ModelNamespace + tableName);
            System.Reflection.PropertyInfo[] properties = type.GetProperties();

            foreach (System.Reflection.PropertyInfo pi in properties)
            {
                ColumnType temp = GetColumnDbType(pi);
                if (columns.Contains(pi.Name))
                {
                    TableColumn tc = new TableColumn();
                    tc.ColumnName = pi.Name;
                    tc.ColumnDbType = (byte)temp;
                    columnList.Add(tc);
                    tempTableColumn.Add(tc);
                }
            }
            Columns = new SelectList(tempTableColumn, "ColumnName", "ColumnName");
        }

        public List<TableColumn> ColumnList
        {
            get { return columnList; }
        }

        public SelectList Columns { get; private set; }

        public SelectList TextExpressions
        {
            get
            {
                List<NameValueList> textExpression = new List<NameValueList>();
                textExpression.Add(new NameValueList() { Name = "Starts With", Value = (byte)ExpressionValue.StartsWith });
                textExpression.Add(new NameValueList() { Name = "Ends With", Value = (byte)ExpressionValue.EndsWith });
                textExpression.Add(new NameValueList() { Name = "Contains", Value = (byte)ExpressionValue.Contains });
                return new SelectList(textExpression, "Value", "Name");
            }
        }

        public SelectList NumericExpressions
        {
            get
            {
                List<NameValueList> numericExpression = new List<NameValueList>();
                numericExpression.Add(new NameValueList() { Name = "Equals", Value = (byte)ExpressionValue.Equals });
                numericExpression.Add(new NameValueList() { Name = "Less than", Value = (byte)ExpressionValue.Less });
                numericExpression.Add(new NameValueList() { Name = "Greater than", Value = (byte)ExpressionValue.Greater });
                numericExpression.Add(new NameValueList() { Name = "Between", Value = (byte)ExpressionValue.Between });
                return new SelectList(numericExpression, "Value", "Name");
            }
        }

        public SelectList DateExpressions
        {
            get
            {
                List<NameValueList> dateExpression = new List<NameValueList>();
                dateExpression.Add(new NameValueList() { Name = "Equals", Value = (byte)ExpressionValue.Equals });
                dateExpression.Add(new NameValueList() { Name = "Earlier", Value = (byte)ExpressionValue.Less });
                dateExpression.Add(new NameValueList() { Name = "Later", Value = (byte)ExpressionValue.Greater });
                dateExpression.Add(new NameValueList() { Name = "Between", Value = (byte)ExpressionValue.Between });
                return new SelectList(dateExpression, "Value", "Name");
            }
        }

        private ColumnType GetColumnDbType(System.Reflection.PropertyInfo property)
        {
            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal))
                return ColumnType.Numeric;
            else if (property.PropertyType == typeof(DateTime))
                return ColumnType.DateAndTime;
            else
                return ColumnType.Text;
        }

        protected string GetSqlString(IList<TableColumn> tableColumns)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendFormat("SELECT * FROM [{0}] WHERE ", this.tableName);

            foreach (TableColumn tc in tableColumns)
            {
                string clause = "";
                switch (tc.Expression)
                {
                    case (byte)ExpressionValue.StartsWith:

                        clause = String.Format(" LIKE '{0}%' ", tc.ColumnValue);
                        break;

                    case (byte)ExpressionValue.EndsWith:

                        clause = String.Format(" LIKE '%{0}' ", tc.ColumnValue);
                        break;

                    case (byte)ExpressionValue.Contains:

                        clause = String.Format(" LIKE '%{0}%' ", tc.ColumnValue);

                        break;

                    case (byte)ExpressionValue.Between:

                        clause = String.Format(" BETWEEN '{0}' AND '{1}' ", tc.LeftValue, tc.RightValue);

                        break;

                    default:
                        NameValueList nvl = mapping.Single<NameValueList>(p => p.Value == tc.Expression);
                        clause = String.Format(" {0} '{1}' ", nvl.Name, tc.ColumnValue);
                        break;
                }
                clause += tc.ExpressionJoin;
                string tempColumnName = (tc.ColumnDbType == (byte)ColumnType.DateAndTime) ? String.Format(" CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, {0}))) ", tc.ColumnName) : "[" + tc.ColumnName + "]";
                stringBuilder.AppendFormat(" {0} {1}", tempColumnName, clause);
            }
            return stringBuilder.ToString();
        }
        public List<T> GetSearchResult<T>(IList<TableColumn> tableColumns) where T : class
        {
            string sql = GetSqlString(tableColumns);
            try
            {
                return context.ExecuteQuery<T>(sql).ToList<T>();
            }
            catch { return null; }
        }
    }

    public struct NameValueList
    {
        public string Name { get; set; }
        public byte Value { get; set; }
    }

    public enum ColumnType
    {
        Numeric = 1,
        Text = 2,
        DateAndTime = 3
    }

    public enum ExpressionValue
    {
        StartsWith = 1,
        EndsWith = 2,
        Contains = 3,
        Between = 4,
        Greater = 5,
        Less = 6,
        Equals = 7
    }
}
