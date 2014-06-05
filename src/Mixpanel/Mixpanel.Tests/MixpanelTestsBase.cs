using System;

namespace Mixpanel.Tests
{
    public abstract class MixpanelTestsBase
    {
        #region Data

        protected const string Event = "TestEvent";
        protected const string Token = "1234";
        protected const string DistinctId = "456";
        protected const int DistinctIdInt = 456;
        protected const string Ip = "111.111.111.111";
        protected static readonly DateTime Time = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc);
        protected const long TimeUnix = 1385769600L;
        protected const string TimeFormat = "2013-11-30T00:00:00";

        protected const bool IgnoreTime = true;
        protected const string FirstName = "Darth";
        protected const string LastName = "Vader";
        protected const string Name = "Darth Vader";
        protected static readonly DateTime Created = new DateTime(2014, 10, 22, 0, 0, 0, DateTimeKind.Utc);
        protected const string CreatedFormat = "2014-10-22T00:00:00";
        protected const string Email = "darth.vader@mail.com";
        protected const string Phone = "589741";
        protected const string Alias = "999";

        protected const string StringPropertyName = "StringProperty";
        protected const string StringPropertyValue = "Tatooine";
        protected const string DecimalPropertyName = "DecimalProperty";
        protected const decimal DecimalPropertyValue = 2.5m; 
        protected const string IntPropertyName = "IntProperty";
        protected const int IntPropertyValue = 3; 
        protected const string DoublePropertyName = "DoubleProperty";
        protected const double DoublePropertyValue = 4.5d;
        protected static readonly string[] StringPropertyArray = {"Prop1", "Prop2"};
        protected static readonly decimal[] DecimalPropertyArray = {5.5m, 6.6m};

        #endregion Data
    }
}