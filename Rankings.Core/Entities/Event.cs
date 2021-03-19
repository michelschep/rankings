using System;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Event : BaseEntity
    {
        public string Identifier { get; set; }
        public string AggregateId { get; set; }
        public int Index { get; set; }
        public DateTime CreationDate { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
    }
}