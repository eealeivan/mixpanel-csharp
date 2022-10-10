# mixpanel-csharp [![NuGet](http://img.shields.io/nuget/v/mixpanel-csharp.svg)](https://www.nuget.org/packages/mixpanel-csharp/) [![Downloads](https://img.shields.io/nuget/dt/mixpanel-csharp.svg)](https://www.nuget.org/packages/mixpanel-csharp/) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[Mixpanel](https://mixpanel.com/) is a great analitics platform, but unfortunetally there is no official integration library for .NET. So if you are writing code on .NET and want to use Mixpanel, then ```mixpanel-csharp``` can be an excellent choice. ```mixpanel-csharp``` main idea is to hide most api details (you don't need to remember what time formatting to use, or in which cases you should prefix properties with ```$```) and concentrate on data that you want to analyze.

## Features
- Supports the following [Mixpanel Ingestion API's](https://developer.mixpanel.com/reference/ingestion-api):
    - [Track Events](https://developer.mixpanel.com/reference/track-event)
    - [User Profiles](https://developer.mixpanel.com/reference/user-profiles)
- [Send messages simultaneously](https://github.com/eealeivan/mixpanel-csharp/wiki/Sending-messages) or just create a message instance and send it later
- Pass [the message data](https://github.com/eealeivan/mixpanel-csharp/wiki/Message-data) in form that you prefer: predefined contract, `IDictionary<string, object>`, anonymous type or dynamic
- Add properties globally to all messages with super properties. Usable for properties such as `distinct_id`
- Great [configurability](https://github.com/eealeivan/mixpanel-csharp/wiki/Configuration). For example you can provide your own JSON serializer or function that will make HTTP requests
- No dependencies. Keeps your project clean
- Runs on .NET 4.6.1 and [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- Detailed [documentation](https://github.com/eealeivan/mixpanel-csharp/wiki)

## Sample usage for track message
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
await mc.TrackAsync("Level Complete", new {
    DistinctId = "12345",
    LevelNumber = 5,
    Duration = TimeSpan.FromMinutes(1)
});
```
This will send the following JSON to `https://api.mixpanel.com/track/`:
```json
{
  "event": "Level Complete",
  "properties": {
    "token": "e3bc4100330c35722740fb8c6f5abddc",
    "distinct_id": "12345",
    "LevelNumber": 5,
    "$duration": 60
  }
}
```

## Sample usage for profile message
```csharp
var mc = new MixpanelClient("e3bc4100330c35722740fb8c6f5abddc");
await mc.PeopleSetAsync(new {
    DistinctId = "12345",   
    Name = "Darth Vader",    
    Kills = 215
});
```
This will send the following JSON to `https://api.mixpanel.com/engage/`:
```json
{
    "$token": "e3bc4100330c35722740fb8c6f5abddc",
    "$distinct_id": "12345",    
    "$set": {       
        "$name": "Darth Vader",      
        "Kills": 215
    }
}
```

## Copyright
Copyright © 2022 Aleksandr Ivanov

## License
```mixpanel-csharp``` is licensed under [MIT](https://www.opensource.org/licenses/mit-license.php). Refer to [LICENSE](https://github.com/eealeivan/mixpanel-csharp/blob/master/LICENSE) for more information.
