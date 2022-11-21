using System;

namespace ObjectStorageSharp.Response
{
    public class ObjectInfo {
        public string hash { get; set; }
        public DateTime last_modified { get; set; }
        public int bytes { get; set; }
        public string name { get; set; }
        public string content_type { get; set; }
    }
}
