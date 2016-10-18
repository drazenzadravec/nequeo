/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
 * 
 *  File :          User.cs
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

namespace Nequeo.DataAccess.NequeoCompany.Data.Extension
{
    /// <summary>
    /// Override user insert operations.
    /// </summary>
    internal class UserInsertOverride : InsertDataGenericBase<Data.User> 
    {
        /// <summary>
        /// Override user insert operations.
        /// </summary>
        /// <param name="insert">The current insert data generic provider to override.</param>
        public UserInsertOverride(IInsertDataGenericBase<Data.User> insert)
            : base(insert.ConfigurationDatabaseConnection, insert.ConnectionType, insert.ConnectionDataType, insert.DataAccessProvider)
        {
        }

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="user">The user data.</param>
        /// <param name="encode">Encode the user.</param>
        private void EncodePassword(Data.User user, bool encode = true)
        {
            Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();

            if (encode)
            {
                // Encode password.
                user.LoginPassword = encoder.Encode(user.LoginPassword, encoder.PasswordFormat);
            }
            else
            {
                // Decode password.
                user.LoginPassword = encoder.Decode(user.LoginPassword, encoder.PasswordFormat);
            }
        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override List<object> InsertDataEntity(Data.User dataEntity)
        {
            return InsertDataEntity<Data.User>(dataEntity);
        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity)
        {
            return InsertDataEntity<TypeDataEntity>(dataEntity, "");
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="identitySqlQuery"></param>
        /// <returns></returns>
        public override List<object> InsertDataEntity(Data.User dataEntity, string identitySqlQuery)
        {
            return InsertDataEntity<Data.User>(dataEntity, identitySqlQuery);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <param name="identitySqlQuery"></param>
        /// <returns></returns>
        public override List<object> InsertDataEntity<TypeDataEntity>(TypeDataEntity dataEntity, string identitySqlQuery)
        {
            if (dataEntity is Data.User)
            {
                // Get the user.
                Data.User insertUser = dataEntity as Data.User;
                EncodePassword(insertUser);

                // Return the data.
                return base.InsertDataEntity<Data.User>(insertUser, identitySqlQuery);
            }
            else
                return base.InsertDataEntity<TypeDataEntity>(dataEntity, identitySqlQuery);
        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override bool InsertItem(Data.User dataEntity)
        {
            return InsertItem<Data.User>(dataEntity);
        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override bool InsertItem<TypeDataEntity>(TypeDataEntity dataEntity)
        {
            return InsertDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity });
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="dataEntities"></param>
        /// <returns></returns>
        public override bool InsertDataEntities(Data.User[] dataEntities)
        {
            return InsertDataEntities<Data.User>(dataEntities);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntities"></param>
        /// <returns></returns>
        public override bool InsertDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
        {
            if (dataEntities[0] is Data.User)
            {
                // Get the user.
                List<Data.User> insertUsers = new List<Data.User>();
                foreach (TypeDataEntity user in dataEntities)
                {
                    // Get the user.
                    Data.User insertUser = user as Data.User;
                    EncodePassword(insertUser);
                    insertUsers.Add(insertUser);
                }

                // Return the data.
                return base.InsertDataEntities<Data.User>(insertUsers.ToArray());
            }
            else
                return base.InsertDataEntities<TypeDataEntity>(dataEntities);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int InsertCommandItem(
            ref System.Data.Common.DbCommand sqlCommand, string commandText, System.Data.CommandType commandType, params System.Data.Common.DbParameter[] values)
        {
            return base.InsertCommandItem(ref sqlCommand, commandText, commandType, values);
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="queryText"></param>
        /// <param name="commandType"></param>
        /// <param name="getSchemaTable"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override System.Data.Common.DbCommand InsertQueryItem(
            ref System.Data.DataTable dataTable, string queryText, System.Data.CommandType commandType, bool getSchemaTable, params System.Data.Common.DbParameter[] values)
        {
            return base.InsertQueryItem(ref dataTable, queryText, commandType, getSchemaTable, values);
        }
    }

    /// <summary>
    /// Override user update operations.
    /// </summary>
    internal class UserUpdateOverride : UpdateDataGenericBase<Data.User>
    {
        /// <summary>
        /// Override user update operations.
        /// </summary>
        /// <param name="update">The current update data generic provider to override.</param>
        public UserUpdateOverride(IUpdateDataGenericBase<Data.User> update)
            : base(update.ConfigurationDatabaseConnection, update.ConnectionType, update.ConnectionDataType, update.DataAccessProvider)
        {
        }

        /// <summary>
        /// Encode the password.
        /// </summary>
        /// <param name="user">The user data.</param>
        /// <param name="encode">Encode the user.</param>
        private void EncodePassword(Data.User user, bool encode = true)
        {
            Nequeo.Cryptography.IPasswordEncryption encoder = PasswordAuthorisationCode.GetEncoder();

            // Get the current user.
            Data.User current = new Data.DataContext().Users.First(u => u.UserID == user.UserID);
            string currentPasswordEncoded = current.LoginPassword;
            string passwordEncoded = encoder.Encode(user.LoginPassword, encoder.PasswordFormat);

            // If password is different.
            if (user.LoginPassword != currentPasswordEncoded)
            {
                // If the passwords do not match.
                if (currentPasswordEncoded != passwordEncoded)
                {
                    // Encode password.
                    user.LoginPassword = passwordEncoded;
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override bool UpdateItem(Data.User dataEntity)
        {
            return UpdateItem<Data.User>(dataEntity);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="useRowVersion"></param>
        /// <returns></returns>
        public override bool UpdateItem(Data.User dataEntity, bool useRowVersion)
        {
            return UpdateItem<Data.User>(dataEntity, useRowVersion);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        public override bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity)
        {
            return UpdateItem<TypeDataEntity>(dataEntity, false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <param name="useRowVersion"></param>
        /// <returns></returns>
        public override bool UpdateItem<TypeDataEntity>(TypeDataEntity dataEntity, bool useRowVersion)
        {
            return UpdateDataEntities<TypeDataEntity>(new TypeDataEntity[] { dataEntity }, useRowVersion);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntities"></param>
        /// <returns></returns>
        public override bool UpdateDataEntities(Data.User[] dataEntities)
        {
            return UpdateDataEntities<Data.User>(dataEntities);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntities"></param>
        /// <param name="useRowVersion"></param>
        /// <returns></returns>
        public override bool UpdateDataEntities(Data.User[] dataEntities, bool useRowVersion)
        {
            return UpdateDataEntities<Data.User>(dataEntities, useRowVersion);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntities"></param>
        /// <returns></returns>
        public override bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities)
        {
            return UpdateDataEntities<TypeDataEntity>(dataEntities, false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntities"></param>
        /// <param name="useRowVersion"></param>
        /// <returns></returns>
        public override bool UpdateDataEntities<TypeDataEntity>(TypeDataEntity[] dataEntities, bool useRowVersion)
        {
            if (dataEntities[0] is Data.User)
            {
                // Get the user.
                List<Data.User> updateUsers = new List<Data.User>();
                foreach (TypeDataEntity user in dataEntities)
                {
                    // Get the user.
                    Data.User updateUser = user as Data.User;
                    EncodePassword(updateUser);
                    updateUsers.Add(updateUser);
                }

                // Return the data.
                return base.UpdateDataEntities<Data.User>(updateUsers.ToArray(), useRowVersion);
            }
            else
                return base.UpdateDataEntities<TypeDataEntity>(dataEntities, useRowVersion);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public override bool UpdateItemKey(Data.User dataEntity, object keyValue, string keyName)
        {
            return UpdateItemKey<Data.User>(dataEntity, keyValue, keyName);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="rowVersionData"></param>
        /// <param name="rowVersionName"></param>
        /// <returns></returns>
        public override bool UpdateItemKey(Data.User dataEntity, object keyValue, string keyName, object rowVersionData, string rowVersionName)
        {
            return UpdateItemKey<Data.User>(dataEntity, keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public override bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity, object keyValue, string keyName)
        {
            if (dataEntity is Data.User)
            {
                // Get the user.
                Data.User updateUser = dataEntity as Data.User;
                EncodePassword(updateUser);

                // Return the data.
                return base.UpdateItemKey<Data.User>(updateUser, keyValue, keyName);
            }
            else
                return base.UpdateItemKey<TypeDataEntity>(dataEntity, keyValue, keyName);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <param name="keyValue"></param>
        /// <param name="keyName"></param>
        /// <param name="rowVersionData"></param>
        /// <param name="rowVersionName"></param>
        /// <returns></returns>
        public override bool UpdateItemKey<TypeDataEntity>(TypeDataEntity dataEntity, object keyValue, string keyName, object rowVersionData, string rowVersionName)
        {
            if (dataEntity is Data.User)
            {
                // Get the user.
                Data.User updateUser = dataEntity as Data.User;
                EncodePassword(updateUser);

                // Return the data.
                return base.UpdateItemKey<Data.User>(updateUser, keyValue, keyName, rowVersionData, rowVersionName);
            }
            else
                return base.UpdateItemKey<TypeDataEntity>(dataEntity, keyValue, keyName, rowVersionData, rowVersionName);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override bool UpdateItemPredicate(Data.User dataEntity, string predicate, params object[] values)
        {
            return UpdateItemPredicate<Data.User>(dataEntity, predicate, values);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override bool UpdateItemPredicate(Data.User dataEntity, System.Linq.Expressions.Expression<Threading.FunctionHandler<bool, Data.User>> predicate)
        {
            // Get the user.
            Data.User updateUser = dataEntity as Data.User;
            EncodePassword(updateUser);

            // Return the data.
            return base.UpdateItemPredicate(updateUser, predicate);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TypeDataEntity"></typeparam>
        /// <param name="dataEntity"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override bool UpdateItemPredicate<TypeDataEntity>(TypeDataEntity dataEntity, string predicate, params object[] values)
        {
            if (dataEntity is Data.User)
            {
                // Get the user.
                Data.User updateUser = dataEntity as Data.User;
                EncodePassword(updateUser);

                // Return the data.
                return base.UpdateItemPredicate<Data.User>(updateUser, predicate, values);
            }
            else
                return base.UpdateItemPredicate<TypeDataEntity>(dataEntity, predicate, values);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override int UpdateCommandItem(
            ref System.Data.Common.DbCommand sqlCommand, string commandText, System.Data.CommandType commandType, params System.Data.Common.DbParameter[] values)
        {
            return base.UpdateCommandItem(ref sqlCommand, commandText, commandType, values);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="queryText"></param>
        /// <param name="commandType"></param>
        /// <param name="getSchemaTable"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override System.Data.Common.DbCommand UpdateQueryItem(
            ref System.Data.DataTable dataTable, string queryText, System.Data.CommandType commandType, bool getSchemaTable, params System.Data.Common.DbParameter[] values)
        {
            return base.UpdateQueryItem(ref dataTable, queryText, commandType, getSchemaTable, values);
        }
    }
}
