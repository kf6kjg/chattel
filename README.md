<table width="100%" border="0">
	<tr>
		<th>
			<h1 align="center">Chattel</h1>
			<p align="center">.NET/Mono library for acessing WHIP- and CF-based Halcyon assets</p>
		</th>
		<th align="right" width="100">
			<a href="https://travis-ci.org/kf6kjg/chattel"><img alt="Travis-CI Build Status" src="https://travis-ci.org/kf6kjg/chattel.svg?branch=master"/></a><br/>
			<a href="https://ci.appveyor.com/project/kf6kjg/chattel"><img alt="Appveyor Build Status" src="https://ci.appveyor.com/api/projects/status/github/kf6kjg/chattel?svg=true&branch=master"/></a><br/>
			<a href="https://www.nuget.org/packages/Chattel"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Chattel.svg?maxAge=2592000"/></a>
		</th>
	</tr>
</table>

Chattel is a library for Mono/.NET programs designed to make accessing assets stored in asset servers such as [WHIP][] or via [Halcyon][]'s CloudFiles integration simple and fast.

[WHIP]: https://github.com/InWorldz/whip-server
[Halcyon]: https://github.com/InWorldz/halcyon
[nugetpackage]: https://www.nuget.org/packages/Chattel
[appveyor]: https://ci.appveyor.com/project/kf6kjg/chattel

Chattel provides:
* Read assets from the [WHIP asset server][WHIP].
* Read assets from [Halcyon][]'s CloudFiles asset server integration.
* Asset servers can be set up in a series-parallel array.
* Assets can be disk-cached for faster access.
* Integration with [LibreMetaverse](https://bitbucket.org/cinderblocks/libremetaverse) to allow easy access to assets.
* Logging via the excellent [Apache log4net](http://logging.apache.org/log4net/).
* Direct c'tor or Nini-based configuration.

How is Chattel different from InWorldz' [whip-dotnet-client](https://github.com/InWorldz/whip-dotnet-client)? Simple: whip-dotnet-client is a very low-level library designed to allow connection to a WHIP server, but it cannot decode the resulting stream into something useful - Chattel can.  Chattel also takes this one step further and allows you connect to other asset servers that are not WHIP-based.

# Installation
Simply use your NuGet package manager to install the lastest release straight from the [NuGet.org package][nugetpackage] built by the [Appveyor integration][appveyor].

# Contributing
Open source contributions are very welcome - please follow the style you find in the code.

