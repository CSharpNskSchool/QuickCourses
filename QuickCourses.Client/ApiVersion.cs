namespace QuickCourses.Client
{
    public sealed class ApiVersion
    {
        public readonly static ApiVersion V0 = new ApiVersion(0);

        private ApiVersion(int version)
        {
            Version = version;
        }

        public int Version { get; }

        public override string ToString()
        {
            return $"V{Version}";
        }
    }
}
