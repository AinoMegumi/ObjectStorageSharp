using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectStorageSharp.Response {
    public class ContainerInfo {
        public int count { get; set; }
        public int bytes { get; set; }
        public string name { get; set; }
    }
}
