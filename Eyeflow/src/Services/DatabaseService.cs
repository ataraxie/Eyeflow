using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Eyeflow.Entities;
using Eyeflow.Util;

namespace Eyeflow.Services
{
    class DatabaseService
    {
        private static Logger log = Logger.get(typeof(DatabaseService));

        private static DatabaseService instance;

        private SQLiteConnection connection;

        private DatabaseService()
        {

        }

        public static DatabaseService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseService();
                }
                return instance;
            }
        }

        public void createDatabase()
        {
            if (this.connection != null)
            {
                throw new Exception("Database already created");
            }
            this.connection = new SQLiteConnection(Config.Instance.databaseFilePath);
            this.connection.CreateTable<GazeRecord>();
            this.connection.CreateTable<DwmRecord>();
            this.connection.CreateTable<WindowRecord>();
        }

        public void disconnect()
        {
            checkDbCreated();
            this.connection.Close();
        }

        public void writeGazeRecord(GazeRecord gazeRecord)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            log.debug("Writing GazeRecord: " + gazeRecord.ToString());
            insertSafe(gazeRecord);


            GazeLib.logProf(timestamp, "writeGazeRecord");
        }

        public void writeDwmRecord(DwmRecord dwmRecord)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            log.debug("Writing DwmRecord: " + dwmRecord.ToString());
            insertSafe(dwmRecord);
            GazeLib.logProf(timestamp, "writeDwmRecord");
        }

        public void writeWindowRecord(WindowRecord windowRecord)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            log.debug("Writing windowRecord: " + windowRecord.ToString());
            insertSafe(windowRecord);
            GazeLib.logProf(timestamp, "writeWindowRecord");
        }

        private void insertSafe(Object obj)
        {
            try
            {
                this.connection.Insert(obj);
            }
            catch (Exception e)
            {
                log.warn("Failed to write record of type {0}", obj.GetType());
            }
        }

        private void checkDbCreated()
        {
            if (this.connection == null)
            {
                throw new Exception("No database was created");
            }
        }

    }
}
