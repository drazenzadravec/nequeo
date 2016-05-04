using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Unique table name comparer.
    /// </summary>
    internal class ToUpperComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToUpper() == y.ToUpper();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Unique table name comparer.
    /// </summary>
    internal class ToLowerComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Unique table name comparer.
    /// </summary>
    internal class UniqueTableNameComparer : IEqualityComparer<TablesResult>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(TablesResult x, TablesResult y)
        {
            return x.TableName == y.TableName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(TablesResult obj)
        {
            return obj.TableName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique procedures name comparer.
    /// </summary>
    internal class UniqueProcedureNameComparer : IEqualityComparer<ProceduresResult>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(ProceduresResult x, ProceduresResult y)
        {
            return x.ProcedureName == y.ProcedureName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(ProceduresResult obj)
        {
            return obj.ProcedureName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique function name comparer.
    /// </summary>
    internal class UniqueFunctionOverloadNameComparer : IEqualityComparer<FunctionResult>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(FunctionResult x, FunctionResult y)
        {
            return x.OverloadName == y.OverloadName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(FunctionResult obj)
        {
            return obj.OverloadName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique function name comparer.
    /// </summary>
    internal class UniqueFunctionNameComparer : IEqualityComparer<FunctionResult>
    {
        /// <summary>
        /// Compares the table name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(FunctionResult x, FunctionResult y)
        {
            return x.FunctionName == y.FunctionName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(FunctionResult obj)
        {
            return obj.FunctionName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueColumnNameComparer : IEqualityComparer<TableColumnsResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            TableColumnsResult x,
            TableColumnsResult y)
        {
            return x.ColumnName == y.ColumnName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(TableColumnsResult obj)
        {
            return obj.ColumnName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueProcedureColumnNameComparer : IEqualityComparer<ProcedureColumnsResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            ProcedureColumnsResult x,
            ProcedureColumnsResult y)
        {
            return x.ColumnName == y.ColumnName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(ProcedureColumnsResult obj)
        {
            return obj.ColumnName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueFunctionColumnNameComparer : IEqualityComparer<FunctionColumnsResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            FunctionColumnsResult x,
            FunctionColumnsResult y)
        {
            return x.ColumnName == y.ColumnName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(FunctionColumnsResult obj)
        {
            if (obj.ColumnName != null)
                return obj.ColumnName.GetHashCode();
            else
                return 0;
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueColumnNameComparerFk : IEqualityComparer<ForeignKeyTableResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            ForeignKeyTableResult x,
            ForeignKeyTableResult y)
        {
            return x.ColumnName == y.ColumnName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(ForeignKeyTableResult obj)
        {
            return obj.ColumnName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniquePrimaryKeyNameComparer : IEqualityComparer<PrimaryKeyColumnsResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            PrimaryKeyColumnsResult x,
            PrimaryKeyColumnsResult y)
        {
            return x.PrimaryKeyName == y.PrimaryKeyName;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(PrimaryKeyColumnsResult obj)
        {
            return obj.PrimaryKeyName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueTableNameComparerRef : IEqualityComparer<ForeignKeyTableResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            ForeignKeyTableResult x,
            ForeignKeyTableResult y)
        {
            return (x.TableName == y.TableName);
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(ForeignKeyTableResult obj)
        {
            return obj.TableName.GetHashCode();
        }
    }

    /// <summary>
    /// Unique column name comparer.
    /// </summary>
    internal class UniqueColumnIndictaorComparer : IEqualityComparer<ColumnValuesResult>
    {
        /// <summary>
        /// Compares the column name of the two objects and
        /// returns the unique collection of the type.
        /// </summary>
        /// <param name="x">The first object to match.</param>
        /// <param name="y">The second object to match.</param>
        /// <returns>True if a match has been found.</returns>
        public bool Equals(
            ColumnValuesResult x,
            ColumnValuesResult y)
        {
            return x.ColumnIndicator == y.ColumnIndicator;
        }

        /// <summary>
        /// Gets the hash code for the comparered object.
        /// </summary>
        /// <param name="obj">The object containing the member.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(ColumnValuesResult obj)
        {
            return obj.ColumnIndicator.GetHashCode();
        }
    } 
}
