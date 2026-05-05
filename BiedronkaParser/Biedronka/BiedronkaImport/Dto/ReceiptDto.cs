using System;
using System.Collections.Generic;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class ReceiptDto
    {
        public string ProtoVersion { get; set; }
        public string IDZ { get; set; }
        public int DeviceType { get; set; }
        public bool Printed { get; set; }
        public string Data { get; set; }

        public List<HeaderItem> Header { get; set; }

        public List<BodyItem> Body { get; set; }

        public string Sign { get; set; }
    }
}
