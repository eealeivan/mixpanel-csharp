namespace Mixpanel
{
    /// <summary>
    /// Kind (type) of <see cref="MixpanelMessage"/>.
    /// </summary>
    public enum MessageKind
    {
        /// <summary>
        /// Track message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        Track,

        /// <summary>
        /// Alias message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        Alias,

        /// <summary>
        /// PeopleSet message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleSet,

        /// <summary>
        /// PeopleSetOnce message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleSetOnce,

        /// <summary>
        /// PeopleAdd message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleAdd,

        /// <summary>
        /// PeopleAppend message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleAppend,

        /// <summary>
        /// PeopleUnion message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleUnion,

        /// <summary>
        /// PeopleRemove message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleRemove,

        /// <summary>
        /// PeopleUnset message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleUnset,

        /// <summary>
        /// PeopleDelete message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleDelete,

        /// <summary>
        /// PeopleTrackCharge message. If you manually create a <see cref="MixpanelMessage"/>, please check 
        /// corresponding Test method for a correct message structure.
        /// </summary>
        PeopleTrackCharge,

        /// <summary>
        /// Batch message. Used internally. If you manually create a message of this kind and try to 
        /// send it, it will be ignored.
        /// </summary>
        Batch
    }
}