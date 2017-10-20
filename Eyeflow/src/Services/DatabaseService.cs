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

        public void writeGazeRecord(GazeRecord gaze)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            this.connection.Insert(gaze);
            GazeLib.logProf(timestamp, "writeGazeRecord");
        }

        public void writeDwmRecord(DwmRecord dwm)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            this.connection.Insert(dwm);
            GazeLib.logProf(timestamp, "writeDwmRecord");
        }

        public void writeWindowRecord(WindowRecord windowRecord)
        {
            long timestamp = GazeLib.getTimestamp();
            checkDbCreated();
            this.connection.Insert(windowRecord);
            GazeLib.logProf(timestamp, "writeWindowRecord");
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
