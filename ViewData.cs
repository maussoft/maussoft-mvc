// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Maussoft.Mvc
{
    public class ViewData : DynamicObject
    {
        protected Dictionary<string, object> dictionary;

        public ViewData()
        {
            this.dictionary = new Dictionary<string, object>();
        }

        public ViewData(IDictionary<string, object> dict)
        {
            this.dictionary = new Dictionary<string, object>();

            foreach (var kvp in dict)
            {
                this.dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public ViewData(IDictionary<string, string> dict)
        {
            this.dictionary = new Dictionary<string, object>();

            foreach (var kvp in dict)
            {
                this.dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public ViewData(ViewData viewData)
        {
            this.dictionary = new Dictionary<string, object>();

            foreach (var kvp in viewData.dictionary)
            {
                this.dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dictionary.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = dictionary.ContainsKey(binder.Name) ? dictionary[binder.Name] : null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            return true;
        }

        public bool Empty()
        {
            return dictionary.Keys.Count == 0;
        }
    }
}