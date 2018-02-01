<table width="100%" border="0">
	<tr>
		<th>
			<h1 align="center">Chattel</h1>
			<p align="center">.NET/Mono library for accessing WHIP and CloudFiles based Halcyon assets</p>
		</th>
		<th align="right">
			<a href="https://travis-ci.org/kf6kjg/chattel"><img alt="Travis-CI Build Status" src="https://travis-ci.org/kf6kjg/chattel.svg?branch=master"/></a><br/>
			<a href="https://ci.appveyor.com/project/kf6kjg/chattel"><img alt="Appveyor Build Status" src="https://ci.appveyor.com/api/projects/status/github/kf6kjg/chattel?svg=true&branch=master"/></a><br/>
			<a href="https://www.nuget.org/packages/Chattel"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Chattel.svg?maxAge=2592000"/></a>
			<a href="https://www.codacy.com/app/kf6kjg/chattel?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=kf6kjg/chattel&amp;utm_campaign=Badge_Grade"><img alt="Codacy Badge" src="https://api.codacy.com/project/badge/Grade/872446917ae24afdb205f06c8ee6e4cf"/></a>
		</th>
	</tr>
</table>

Chattel is a library for Mono/.NET programs designed to make accessing assets stored in asset servers such as [WHIP][] or via [Halcyon][]'s CloudFiles integration simple and fast.

[WHIP]: https://github.com/InWorldz/whip-server
[Halcyon]: https://github.com/InWorldz/halcyon
[nugetpackage]: https://www.nuget.org/packages/Chattel
[appveyor]: https://ci.appveyor.com/project/kf6kjg/chattel

Chattel provides:
* The ability to read and write assets.
* Integration with the [WHIP asset server][WHIP].
* Integration with [Halcyon][]'s CloudFiles asset server.
* Asset servers can be set up in a series-parallel array.
* Assets can be disk-cached for faster access.
* Integration with [LibreMetaverse](https://bitbucket.org/cinderblocks/libremetaverse) to allow easy access to assets.
* Logging via the excellent [Apache log4net](http://logging.apache.org/log4net/).
* Direct c'tor or Nini-based configuration.

How is Chattel different from InWorldz' [whip-dotnet-client](https://github.com/InWorldz/whip-dotnet-client)? Simple: whip-dotnet-client is a very low-level library designed to allow connection to a WHIP server, but it cannot decode the resulting stream into something useful - Chattel can.  Chattel also takes this one step further and allows you connect to other asset servers that are not WHIP-based.

# Series-parallel array of asset servers
Chattel allows you to query multiple asset servers at the same time, aka in parallel, with the first one to return the asset winning. It also allows you to specify within this parallel list asset servers that should be queried in series until one returns the requested asset.

The benefit of this arrangement only comes if your grid is growing and you've had to change asset servers.  Let's assume that you started your grid with WHIP, but later spun up a CloudFiles server and ported all your assets to the latter, but your WHIP servers are there just in case some asset didn't get ported.  With this set up you could specify that the CloudFiles servers should be queried first, then the old WHIP servers, by setting the CloudFiles server in series with WHIP.

Alternatively, let's say you had multiple grids with different asset servers, but your application requires that you be able to pull assets from all of them.  Simply set each asset server in parallel.  If some of those have legacy asset servers, set the legacy asset servers in series with their primaries.

There are many possibilities and use-cases enabled by this architecture.

Note that since reading and writing are handled by separate classes, you can configure a different array for writing assets than you use for reading them.

# Installation
Simply use your NuGet package manager to install the latest release straight from the [NuGet.org package][nugetpackage] built by the [Appveyor integration][appveyor].

# Contributing
Open source contributions are very welcome - please follow the style you find in the code.

