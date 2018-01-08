using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectStorageSharp.Response {
    public class KeyStoneResult {
        public Access access { get; set; }
    }

    public class Access {
        public Token token { get; set; }
        public Servicecatalog[] serviceCatalog { get; set; }
        public User user { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Token {
        public DateTime issued_at { get; set; }
        public DateTime expires { get; set; }
        public string id { get; set; }
        public Tenant tenant { get; set; }
        public string[] audit_ids { get; set; }
    }

    public class Tenant {
        public string domain_id { get; set; }
        public string description { get; set; }
        public bool enabled { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class User {
        public string username { get; set; }
        public object[] roles_links { get; set; }
        public string id { get; set; }
        public Role[] roles { get; set; }
        public string name { get; set; }
    }

    public class Role {
        public string name { get; set; }
    }

    public class Metadata {
        public int is_admin { get; set; }
        public string[] roles { get; set; }
    }

    public class Servicecatalog {
        public Endpoint[] endpoints { get; set; }
        public object[] endpoints_links { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Endpoint {
        public string region { get; set; }
        public string publicURL { get; set; }
    }
}
