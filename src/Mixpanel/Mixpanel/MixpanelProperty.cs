namespace Mixpanel
{
    /// <summary>
    /// Contains predefined property names for Mixpanel that can be used for message building.
    /// </summary>
    public static class MixpanelProperty
    {
        /// <summary>
        /// Token for both track and engage messages.
        /// </summary>
        public const string Token = "token";

        /// <summary>
        /// Distinct ID for both track and engage messages.
        /// </summary>
        public const string DistinctId = "distinct_id";

        /// <summary>
        /// Time for both track and engage messages.
        /// </summary>
        public const string Time = "time";

        /// <summary>
        /// Ip for both track and engage messages.
        /// </summary>
        public const string Ip = "ip";

        /// <summary>
        /// Duration for track messages.
        /// </summary>
        public const string Duration = "duration";

        /// <summary>
        /// Operating system for track messages.
        /// </summary>
        public const string Os = "os";

        /// <summary>
        /// Screen width for track messages.
        /// </summary>
        public const string ScreenWidth = "screen_width";

        /// <summary>
        /// Screen height for track messages.
        /// </summary>
        public const string ScreenHeight = "screen_height";

        /// <summary>
        /// First name for engage messages.
        /// </summary>
        public const string FirstName = "first_name";

        /// <summary>
        /// Last name for engage messages.
        /// </summary>
        public const string LastName = "last_name";

        /// <summary>
        /// Name for engage messages.
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// Created for engage messages.
        /// </summary>
        public const string Created = "created";

        /// <summary>
        /// Email for engage messages.
        /// </summary>
        public const string Email = "email";

        /// <summary>
        /// Phone for engage messages.
        /// </summary>
        public const string Phone = "phone";

        /// <summary>
        /// Ignore time for engage messages.
        /// </summary>
        public const string IgnoreTime = "ignore_time";

        /// <summary>
        /// Ignore alias for engage messages.
        /// </summary>
        public const string IgnoreAlias = "ignore_alias";
    }
}
