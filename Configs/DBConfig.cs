using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Configs {
    public class DBConfig {

        private String server = "127.0.0.1";
        private String port = "3306";
        private String user = "root";
        private String password = "";
        private String database = "cantina";

        public String Server {
            get { return server; }
            set { server = value; }
        }

        public String Port {
            get { return port; }
            set { port = value; }
        }

        public String User {
            get { return user; }
            set { user = value; }
        }

        public String Password {
            get { return password; }
            set { password = value; }
        }

        public String Database {
            get { return database; }
            set { database = value; }
        }

        public String ConnectionString() {
            return $"server={this.Server}; " +
                $"port={this.Port}; " +
                $"user={this.User}; " +
                $"password={this.Password}; " +
                $"database={this.Database}; " +
                $"SslMode=none";
        }

    }
}
