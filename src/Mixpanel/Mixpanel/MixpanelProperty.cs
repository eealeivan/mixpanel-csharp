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
        /// Event for track messages.
        /// </summary>
        public const string Event = "event";

        /// <summary>
        /// Alias for track messages.
        /// </summary>
        public const string Alias = "alias";

        /// <summary>
        /// Ip for both track and engage messages.
        /// </summary>
        public const string Ip = "ip";

        /// <summary>
        /// Time for both track and engage messages.
        /// </summary>
        public const string Time = "time";

        /// <summary>
        /// Duration for track messages.
        /// </summary>
        public const string Duration = "duration";

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
