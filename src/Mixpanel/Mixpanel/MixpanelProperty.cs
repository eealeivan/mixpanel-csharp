namespace Mixpanel
{
    public sealed class MixpanelProperty
    {
        public const string Event = "event";
        public const string Token = "token";
        public const string DistinctId = "distinct_id";
        public const string Ip = "ip";
        public const string Time = "time";
        public const string IgnoreTime = "ignore_time";
        public const string FirstName = "first_name";
        public const string LastName = "last_name";
        public const string Name = "name";
        public const string Created = "created";
        public const string Email = "email";
        public const string Phone = "phone";
        public const string Alias = "alias";

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
        internal const string PeopleUnset = "$unset";
        internal const string PeopleDelete = "$delete";
        internal const string PeopleTransactions = "$transactions";
    }
}
