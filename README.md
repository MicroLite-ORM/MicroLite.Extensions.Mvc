# MicroLite.Extensions.Mvc

MicroLite.Extensions.Mvc is a .NET 4.5 library which adds an extension for the MicroLite ORM Framework to integrate with ASP.NET MVC.

![Nuget](https://img.shields.io/nuget/dt/MicroLite.Extensions.Mvc)

|Branch|Status|
|------|------|
|/develop|![GitHub last commit (branch)](https://img.shields.io/github/last-commit/MicroLite-ORM/MicroLite.Extensions.Mvc/develop) [![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite.Extensions.Mvc?branchName=develop)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=23&branchName=develop) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/MicroLite.Extensions.Mvc)|
|/master|![GitHub last commit](https://img.shields.io/github/last-commit/MicroLite-ORM/MicroLite.Extensions.Mvc/master) [![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite.Extensions.Mvc?branchName=master)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=23&branchName=master) ![Nuget](https://img.shields.io/nuget/v/MicroLite.Extensions.Mvc) ![GitHub Release Date](https://img.shields.io/github/release-date/MicroLite-ORM/MicroLite.Extensions.Mvc)|

## Installation

Install the nuget package `Install-Package MicroLite.Extensions.Mvc`

## Configuration

It is easy to use MicroLite with ASP.NET MVC, simply supply your controller with a Session `ISession` or `IReadOnlySession` and use it in your controller actions. However, using the MVC extension for MicroLite makes it even easier and contains some useful extras.

## Supported .NET Versions

The NuGet Package contains binaries compiled against (dependencies indented):

* .NET Framework 4.5
  * MicroLite 7.0.0
  * Microsoft.AspNet.Mvc 5.2.7

To find out more, head over to the [Wiki](https://github.com/MicroLite-ORM/MicroLite.Extensions.Mvc/wiki), or check out the [MVC](http://microliteorm.wordpress.com/tag/mvc/) tag on the MicroLite Blog.
