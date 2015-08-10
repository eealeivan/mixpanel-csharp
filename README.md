#mixpanel-csharp [![NuGet Version](http://img.shields.io/nuget/v/mixpanel-csharp.svg?style=flat)](https://www.nuget.org/packages/mixpanel-csharp/)
[Mixpanel](https://mixpanel.com/) is a great analitics platform, but unfortunetally there are no official integration library for .NET. So if you are writing code on .NET and want to use Mixpanel, then ```mixpanel-csharp``` can be an excellent choise. ```mixpanel-csharp``` main idea is to hide most api details (you don't need to remember what time formatting to use, or in which cases you should prefix properties with ```$```) and concentrate on data that you want to analize.

##Features
- Supports full [Mixpanel HTTP Tracking API](https://mixpanel.com/help/reference/http)
- [Send tracking messages](https://github.com/eealeivan/mixpanel-csharp/wiki/Sending-messages) synchronously or asynchronously, pack them into batches, save messages to send them later
- Pass [message data](https://github.com/eealeivan/mixpanel-csharp/wiki/Message-data) in form that you prefer: `IDictionary<string, object>`, dynamic type, anonymous type or _normal_ class
- Great [configurability](https://github.com/eealeivan/mixpanel-csharp/wiki/Configuration). For example you can provide your own JSON serializer or function that will make HTTP requests
- No dependencies
- Runs on many platforms: NET35, NET40, NET45 (WinRT and Portable comming soon)
- Good [documentation](https://github.com/eealeivan/mixpanel-csharp/wiki)

##Copyright
Copyright © 2015 Aleksandr Ivanov

##Licence
```mixpanel-csharp``` is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php). Refer to license.txt for more information.
