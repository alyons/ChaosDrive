/* This code was found from this blog:
 * http://blogs.msdn.com/b/jaredpar/archive/2008/05/16/switching-on-types.aspx
 * I am using it here giving full credit to Jared Par with any modifications
 * made being attributed to myself. Just trying to makes sure the copyrights
 * and patents are held.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChaosDrive.Utility
{
    public static class TypeSwitch
    {
        public class CaseInfo
        {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Action<object> Action { get; set; }
        }

        public static void Do(object source, params CaseInfo[] cases)
        {
            var type = source.GetType();
            foreach (var entry in cases)
            {
                if (entry.IsDefault || type == entry.Target)
                {
                    entry.Action(source);
                    break;
                }
            }
        }

        public static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo()
            {
                Action = x => action(),
                Target = typeof(T)
            };
        }

        public static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo()
            {
                Action = (x) => action((T)x),
                Target = typeof(T)
            };
        }

        public static CaseInfo Default(Action action)
        {
            return new CaseInfo()
            {
                IsDefault = true,
                Action = x => action()
            };
        }
    }
}
