using System;
using System.Collections.Generic;
using System.Text;

namespace leo.Asset
{
    public class MaterialAsset
    {
        public string EffectName { get; set; }

        private Dictionary<string, object> _bindvalues = new Dictionary<string, object>();

        public IReadOnlyDictionary<string, object> BindValues => _bindvalues;

        public object this[string key]
        {
            set
            {
                _bindvalues[key] = value;
            }
        }
    }
}
