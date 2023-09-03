using SSTM.Helpers.Helpers;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SSTM.Helpers.DBHandlers
{
    /// <summary>
	/// Purpose: Error Enums used by this LLBL library.
	/// </summary>
	public enum LLBLError
    {
        AllOk
        // Add more here (check the comma's!)
    }

    public abstract class DbInteractionBase : IDisposable
    {
        #region Class Member Declarations
        protected SqlConnection _mainConnection;

        protected DbConnectionProvider _mainConnectionProvider;

        protected int _rowsAffected;

        protected bool _mainConnectionIsCreatedLocal, _isDisposed;
        #endregion

        #region Class Properties Deinitions
        public DbConnectionProvider MainConnectionProvider
        {
            set
            {
                if (value == null)
                {
                    // Invalid value
                    throw new ArgumentNullException("MainConnectionProvider", "Null passed as value to this property which is not allowed.");
                }

                // A connection provider object is passed to this class.
                // Retrieve the SqlConnection object, if present and create a
                // reference to it. If there is already a MainConnection object
                // referenced by the membervar, destroy that one or simply 
                // remove the reference, based on the flag.
                if (_mainConnection != null)
                {
                    // First get rid of current connection object. Caller is responsible
                    if (_mainConnectionIsCreatedLocal)
                    {
                        // Is local created object, close it and dispose it.
                        _mainConnection.Close();
                        _mainConnection.Dispose();
                    }
                    // Remove reference.
                    _mainConnection = null;
                }
                _mainConnectionProvider = value;
                _mainConnection = _mainConnectionProvider.DBConnection;
                _mainConnectionIsCreatedLocal = false;
            }
        }

        /// <summary>
        /// Purpose: Class constructor.
        /// </summary>
        public DbInteractionBase()
        {
            // Initialize the class' members.
            InitClass();
        }
        #endregion

        /// <summary>
		/// Purpose: Initializes class members.
		/// </summary>
		private void InitClass()
        {
            // create all the objects and initialize other members.
            _mainConnection = new SqlConnection();
            _mainConnectionIsCreatedLocal = true;
            _mainConnectionProvider = null;

            AppSettingsReader _configReader = new AppSettingsReader();

            // Set connection string of the sqlconnection object
            _mainConnection.ConnectionString =Utility.dbConnectionString;
            //_configReader.GetValue("ConnectionString", typeof(string)).ToString();
            _isDisposed = false;
        }

        /// <summary>
        /// Purpose: Implements the IDispose' method Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Purpose: Implements the Dispose functionality.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
            // Check to see if Dispose has already been called.
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    // Dispose managed resources.
                    if (_mainConnectionIsCreatedLocal)
                    {
                        // Object is created in this class, so destroy it here.
                        _mainConnection.Close();
                        _mainConnection.Dispose();
                        _mainConnectionIsCreatedLocal = false;
                    }
                    _mainConnectionProvider = null;
                    _mainConnection = null;
                }
            }
            _isDisposed = true;
        }
    }
}