namespace Mixpanel
{
    /// <summary>
    /// Contains predifined property names for Mixpanel that can be used for message building.
    /// </summary>
    public sealed class MixpanelProperty
    {
        /// <summary>
        /// Event for track messages.
        /// </summary>
        public const string Event = "event";

        /// <summary>
        /// Token for both track and engage messages.
        /// </summary>
        public const string Token = "token";

        /// <summary>
        /// Distinct ID for both track and engage messages.
        /// </summary>
        public const string DistinctId = "distinct_id";

        /// <summary>
        /// Ip for both track and engage messages
        /// </summary>
        public const string Ip = "ip";

        /// <summary>
        /// Time for both track and engage messages
        /// </summary>
        public const string Time = "time";

        /// <summary>
        /// Ignore time for engage messages.
        /// </summary>
        public const string IgnoreTime = "ignore_time";

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
        /// Alias for track messages.
        /// </summary>
        public const string Alias = "alias";

        /// <summary>
        /// Duration for track messages.
        /// </summary>
        public const string Duration = "duration";

        internal const string TrackEvent = "event";
        internal const string TrackToken = "token";
        internal const string TrackDistinctId = "distinct_id";
        internal const string TrackTime = "time";
        internal const string TrackIp = "ip";
        internal const string TrackAlias = "alias";

        internal const string TrackProperties = "properties";
        internal const string TrackCreateAlias = "$create_alias";

        internal const string PeopleToken = "$token";
        internal const string PeopleDistinctId = "$distinct_id";
        internal const string PeopleTime = "$time";
        internal const string PeopleIp = "$ip";
        internal const string PeopleIgnoreTime = "$ignore_time";
        internal const string PeopleFirstName = "$first_name";
        internal const string PeopleLastName = "$last_name";
        internal const string PeopleName = "$name";
        internal const string PeopleCreated = "$created";
        internal const string PeopleEmail = "$email";
        internal const string PeoplePhone = "$phone";
        internal const string PeopleAmount = "$amount";
        
        internal const string PeopleSet = "$set";
        internal const string PeopleSetOnce = "$set_once";
        internal const string PeopleAdd = "$add";
        internal const string PeopleAppend = "$append";
        internal const string PeopleUnion = "$union";
        internal const string PeopleUnset = "$unset";
        internal const string PeopleDelete = "$delete";
        internal const string PeopleTransactions = "$transactions";
    }
}
