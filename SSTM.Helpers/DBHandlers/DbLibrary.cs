using System;
using System.Data;
using System.Data.SqlClient;

namespace SSTM.Helpers.DBHandlers
{
    public class DbLibrary : DbInteractionBase
    {
        private long _MainCourseId, _MasterCoursId;
        private string _type="";
        private bool _MasterCourse;
        public DataTable GetDataTable(string spName, params SqlParameter[] parameters)
        {
            SqlCommand cmdToExecute = new SqlCommand();
            cmdToExecute.Connection = _mainConnection;
            cmdToExecute.CommandType = CommandType.StoredProcedure;
            cmdToExecute.CommandTimeout = 0;
            cmdToExecute.CommandText = spName;

            DataTable toReturn = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmdToExecute);

            if (parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    cmdToExecute.Parameters.Add(param);
                }
            }

            try
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Open();
                else
                {
                    if (_mainConnectionProvider.IsTransactionPending)
                        cmdToExecute.Transaction = _mainConnectionProvider.CurrentTransaction;
                }

                // Execute query.
                adapter.Fill(toReturn);

                return toReturn;
            }
            catch (Exception ex)
            {
                // some error occured. Bubble it to caller and encapsulate Exception object
                throw ex;
            }
            finally
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Close();

                cmdToExecute.Dispose();
                adapter.Dispose();
            }
        }

        public DataTable GetQueryDataTable(string query, params SqlParameter[] parameters)
        {
            SqlCommand cmdToExecute = new SqlCommand();
            cmdToExecute.Connection = _mainConnection;
            cmdToExecute.CommandType = CommandType.Text;
            cmdToExecute.CommandTimeout = 0;
            cmdToExecute.CommandText = query;

            DataTable toReturn = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmdToExecute);

            if (parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    cmdToExecute.Parameters.Add(param);
                }
            }

            try
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Open();
                else
                {
                    if (_mainConnectionProvider.IsTransactionPending)
                        cmdToExecute.Transaction = _mainConnectionProvider.CurrentTransaction;
                }

                // Execute query.
                adapter.Fill(toReturn);

                return toReturn;
            }
            catch (Exception ex)
            {
                // some error occured. Bubble it to caller and encapsulate Exception object
                throw ex;
            }
            finally
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Close();

                cmdToExecute.Dispose();
                adapter.Dispose();
            }
        }

        public bool ExecuteSQLCommand(string spName, params SqlParameter[] parameters)
        {
            SqlCommand cmdToExecute = new SqlCommand();
            cmdToExecute.Connection = _mainConnection;
            cmdToExecute.CommandType = CommandType.StoredProcedure;
            cmdToExecute.CommandTimeout = 0;
            cmdToExecute.CommandText = spName;

            if (parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    cmdToExecute.Parameters.Add(param);
                }
            }

            try
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Open();
                else
                {
                    if (_mainConnectionProvider.IsTransactionPending)
                        cmdToExecute.Transaction = _mainConnectionProvider.CurrentTransaction;
                }

                cmdToExecute.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                // some error occured. Bubble it to caller and encapsulate Exception object
                throw ex;
            }
            finally
            {
                if (_mainConnectionIsCreatedLocal)
                    _mainConnection.Close();

                cmdToExecute.Dispose();
            }
        }
        public static string CheckMacAddressQuery
        {
            get
            {
                return "SELECT u.Id FROM sstmo.[User] u WHERE u.isDeleted = 0 AND u.isActive = 1 AND MacAddress = @MacAddress or MacAddress1 = @MacAddress";
            }
        }
        public static string checkTrainnerMacAddress
        {
            get
            {
                return "SELECT Id FROM sstmo.[TrainnerMacAddress]  WHERE  MacAddress = @MacAddress";
            }
        }
        public long MainCourseId { get { return _MainCourseId; } set { _MainCourseId = value; } }
        public bool MasterCourse { get { return _MasterCourse; } set { _MasterCourse = value; } }
        public long MasterCoursId { get { return _MasterCoursId; } set { _MasterCoursId = value; } }

        public string type { get { return _type; } set { _type = value; } }

        
        public SqlParameter[] MainCourseToCourseParameters
        {
            get
            {
                return new SqlParameter[1] {
                    new SqlParameter("@mainCoureid", MainCourseId)
                };
            }
        }

        public SqlParameter[] GrvFilldataCoursewithCourseandSubCourse
        {
            get
            {
                return new SqlParameter[3] {
                    new SqlParameter("@MasterCourse", MasterCourse), new SqlParameter("@MasterCoursId", MasterCoursId),
                    new SqlParameter("@type", _type)
                };
            }
        }
    }
}