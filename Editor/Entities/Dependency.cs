#if UNITY_EDITOR
namespace GitDependecyResolvers
{
    [System.Serializable]
    public class Dependency
    {
        public string name;
        public string gitUrl;
    }
}
#endif