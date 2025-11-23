using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLModel_ConsoleApp1.Temp
{
    internal class UsingDynamicView
    {
        public void Execute()
        {
            var rows = new List<Dictionary<string, object>>
            {
                new() { ["Id"] = 1, ["Score"] = 3.2f, ["Active"] = true },
                new() { ["Id"] = 2, ["Score"] = 5.1f, ["Active"] = false }
            };

            var types = new Dictionary<string, Type>
            {
                ["Id"] = typeof(int),
                ["Score"] = typeof(float),
                ["Active"] = typeof(bool)
            };

            IDataView view = new DynamicDataView(rows, types);
        }
    }
}
