# tin-man

[![Build status](https://ci.appveyor.com/api/projects/status/64jrna1i8ate0xb5?svg=true)](https://ci.appveyor.com/project/drewnoakes/tin-man) [![TinMan NuGet version](https://img.shields.io/nuget/v/TinMan.svg)](https://www.nuget.org/packages/TinMan/)

> Program your own RoboCup 3D soccer playing agents in .NET

![RoboViz Screenshot](https://raw.githubusercontent.com/drewnoakes/tin-man/master/Documentation/game-screenshot.png)

Who doesn't dream of leading a horde of ball-kicking robots to glorious international victory through the sheer power of their programming skill?

_TinMan_ is a great base upon which to build such a team.  It takes care of low level details, leaving you to add the [_heart_](http://www.youtube.com/watch?v=4SsykCXL6_4).

------

### Features

* A simple and powerful object-oriented API
* Machine learning support via [trainer commands](../../wiki/Wizard)
* Geometry API with polar/cartesian vectors and transformation matrices
* [PID control](../../wiki/PIDControl)
* Support for [RoboViz](../../wiki/RoboViz) monitor drawing
* Great performance
* Extensive API documentation
* Idiomatic use of .NET paradigms
* Supports any .NET language: C#, VB.NET, [F#](../../wiki/FSharp), Boo, C++, etc...
* Core types used via interfaces, allowing mock objects for unit testing
* Runs on MS CLR (Windows) or Mono (Linux/Mac OSX/Windows)

_TinMan_ provides the right level of abstraction to make simple things easy and advanced things possible.  It has been designed with the [Pit Of Success](https://blog.codinghorror.com/falling-into-the-pit-of-success/) in mind.

### Used by

* [Karachi Koalas](https://twitter.com/karachikoalas) a joint team between IBA in Karachi, Pakistan and UTS in Sydney, Australia
