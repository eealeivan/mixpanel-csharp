using System;
using System.Collections.Generic;

namespace Mixpanel.Tests
{
    public abstract class MixpanelTestsBase
    {
        protected const string Event = "TestEvent";
        protected const string Token = "1234";
        protected const string DistinctIdPropertyName = "DistinctId";
        protected const string DistinctId = "456";
        protected const string SuperDistinctId = "789";
        protected const int DistinctIdInt = 456;
        protected const string Ip = "192.168.0.136";
        protected static readonly DateTime Time = new DateTime(2013, 11, 30, 0, 0, 0, DateTimeKind.Utc);
        protected static readonly DateTimeOffset TimeOffset = new DateTimeOffset(2013, 11, 30, 0, 0, 0, TimeSpan.Zero);
        protected const long TimeUnix = 1385769600L;
        protected const string TimeFormat = "2013-11-30T00:00:00";
        protected const double DurationSeconds = 2.34D;
        protected static readonly TimeSpan Duration = TimeSpan.FromSeconds(DurationSeconds);
        protected const string Os = "Windows";
        protected const int ScreenWidth = 1920;
        protected const int ScreenHeight = 1200;
        protected const bool IgnoreTime = true;
        protected const bool IgnoreAlias = true;

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
        protected const string StringPropertyName2 = "StringProperty2";
        protected const string StringPropertyValue2 = "Tatooine 2";  
        protected const string StringSuperPropertyName = "StringSuperProperty";
        protected const string StringSuperPropertyValue = "Super Tatooine";
        protected const string DecimalPropertyName = "DecimalProperty";
        protected const decimal DecimalPropertyValue = 2.5m;         
        protected const string DecimalPropertyName2 = "DecimalProperty2";
        protected const decimal DecimalPropertyValue2 = 4.6m; 
        protected const string DecimalSuperPropertyName = "DecimalSuperProperty";
        protected const decimal DecimalSuperPropertyValue = 10.67m; 
        protected const string IntPropertyName = "IntProperty";
        protected const int IntPropertyValue = 3;
        protected const string DateTimePropertyName = "DateTimeProperty";
        protected static readonly DateTime DateTimePropertyValue = new DateTime(2015, 07, 19, 22, 30, 50, DateTimeKind.Utc);
        protected const string DoublePropertyName = "DoubleProperty";
        protected const double DoublePropertyValue = 4.5d;
        protected const string InvalidPropertyName = "InvalidProperty";
        protected static readonly object InvalidPropertyValue = new object(); 
        protected const string InvalidPropertyName2 = "InvalidProperty2";
        protected static readonly object InvalidPropertyValue2 = new object();
        protected static readonly string[] StringPropertyArray = {"Prop1", "Prop2"};
        protected static readonly decimal[] DecimalPropertyArray = {5.5m, 6.6m};
        protected static readonly Dictionary<string, object> DictionaryWithStringProperty =
            new Dictionary<string, object>
            {
                {StringPropertyName, StringPropertyValue}
            };
    }
}