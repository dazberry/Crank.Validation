using System;

namespace Crank.Validation.Tests.Models
{
    public class SourceModel
    {
        public string AStringValue { get; set; }
        public int AnIntegerValue { get; set; }
    }

    public class AnOtherSourceModel
    {
        public string AStringValue { get; set; }
        public Guid AGuidValue { get; set; }
    }
}
