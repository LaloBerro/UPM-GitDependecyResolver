#if UNITY_EDITOR
using System.Collections.Generic;

namespace GitDependecyResolvers
{
    [System.Serializable]
    public class Dependencies
    {
        public List<Dependency> dependencies;
    }
}
#endif