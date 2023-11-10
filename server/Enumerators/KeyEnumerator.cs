using System.Collections;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace AclManager.Server.Enumerators
{
    /// <summary>
    /// Enumerator class that gets all kvp keys from a specified prefix
    /// </summary>
    internal class KeyEnumerator : IEnumerable<string>
    {
        private string _prefix;

        public KeyEnumerator(string prefix)
        {
            _prefix = prefix;
        }

        public IEnumerator<string> GetEnumerator()
        {
            var kvpHandle = StartFindKvp(_prefix);

            try
            {
                if (kvpHandle != -1)
                {
                    string key;

                    do
                    {
                        key = FindKvp(kvpHandle);

                        if (key != null)
                        {
                            yield return key;
                        }
                    } while (key != null);
                }
            }
            finally
            {
                EndFindKvp(kvpHandle);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
