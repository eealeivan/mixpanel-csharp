## Description
An open source Mixpanel .NET integration library that supports complete Mixpanel API.

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

## Project URL
https://github.com/eealeivan/mixpanel-csharp

## License
```mixpanel-csharp``` is licensed under [MIT](https://www.opensource.org/licenses/mit-license.php). Refer to [LICENSE](https://github.com/eealeivan/mixpanel-csharp/blob/master/LICENSE) for more information.
