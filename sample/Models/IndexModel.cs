using SampleApp.Data;
using System.Collections.Generic;

namespace SampleApp.Models
{
    public class IndexModel
    {
        public IEnumerable<Iteration> Iterations { get; set; }

        public IEnumerable<Status> Statuses { get; set; }

        public IEnumerable<Area> Areas { get; set; }
    }
}