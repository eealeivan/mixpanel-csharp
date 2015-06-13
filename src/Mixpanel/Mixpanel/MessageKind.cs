namespace Mixpanel
{
    public enum MessageKind
    {
        Track,
        Alias,

        PeopleSet,
        PeopleSetOnce,
        PeopleAdd,
        PeopleAppend,
        PeopleUnion,
        PeopleUnset,
        PeopleDelete,
        PeopleTrackCharge,

        Batch
    }
}