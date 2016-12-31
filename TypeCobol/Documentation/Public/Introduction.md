# TypeCobol

## Overview

### TypeCobol is a modern and open set of tools to analyze, edit and transform IBM Cobol source code.

* TypeCobol implements the full syntax of [IBM Enterprise Cobol 5.1 for zOS](http://publibfp.boulder.ibm.com/epubs/pdf/igy5lr11.pdf)
* TypeCobol exposes a comprehensive object model covering all aspects of a Cobol program
* TypeCobol doesn't generate executable machine code : it is focused on code analysis and transformation
* TypeCobol improves the productivity of Cobol developpers via a rich editing experience and code exploration tools
* TypeCobol is an Open Source library written in portable C# (multiplatform with [.NET core](https://www.microsoft.com/net/core/platform))
* TypeCobol source code is [available on Github](https://www.microsoft.com/net/core/platform) and all contributions are welcome
* TypeCobol can be freely reused in any context, in compliance with a [LGPL-like license](https://www.microsoft.com/net/core/platform)

### TypeCobol also defines a new language : a typed superset of Cobol which compiles down to IBM Cobol.

* TypeCobol aims to bring the same benefits to Cobol developers that [TypeScript](https://www.typescriptlang.org/) brought to Javascript developers
* TypeCobol extended syntax can be optionally enabled or disabled when using the TypeCobol compiler
* TypeCobol defines a strict superset of Cobol : all Cobol programs are already valid TypeCobol programs
* TypeCobol developers can gradually improve existing Cobol programs through types, functions and interfaces definitions
* TypeCobol extended syntax is compiled down to standard IBM Cobol syntax in human readable programs
* TypeCobol language definition is still a work in progress, the current syntax is incomplete and experimental

## Motivation

> Cobol is a dead language : why would anybody spend time building modern development tools for this platform ?

The core TypeCobol team originates from a big financial company in which more than 1000 developers are dedicated to maintain and expand a mission critical system built with more than 200 millions lines of IBM Cobol code. This system is not a legacy : several millions of new Cobol lines are added to the system each year to implement new and improved features.

In many big companies like this one, Cobol is still at the heart of the information system and will be for years to come. Cobol systems are very efficient and the underlying hardware platform is constantly improved by IBM, but the productivity of Cobol developers is hindered 

> Why not progressively migrate Cobol programs to a more modern platform ?

The mainframe platform encourages a centralized development model. In the context of a single transaction, all Cobol programs share the same memory space. There is no native concept in the language to enforce encapsulation and isolation between components. Calling and called components are free to read and write any content in their shared memory segment.

This is a big performance and efficiency advantage to process huge amounts of data across a very complex system, but it makes it almost impossible to isolate a small part of the system to migrate it to another platform. Because the same memory segment is shared from the top to the bottom of the call chain, any kind of change in the list of parameters or data representation impacts all programs at once.

Moving a few programs to another platform would also introduce asynchronous external calls in a system designed from the ground up to run synchronously on a single machine : this proves to be an incredibly challenging and time-intensive task, because it transitively impacts all the call chains leading to these programs.

Some of the existing features simply can't be reimplemented in a distributed computing model : the latency introduced between the components would make them way too slow with the current design.

> How can TypeCobol help with this situation ?

It would literally cost billions, take several years, and involve an unreasonable amount of risk to manually migrate such Cobol systems to a new platform : the only realistic path is to gradually improve the existing Cobol programs.
* Group Cobol programs into components and enhance them with additional 
* Define TypeCobol types, functions and interfaces to properly document the data exchanged between components
* Replace direct Cobol calls (with shared memory) by decoupled TypeCobol service calls
* 

> The recent versions of IBM Cobol

## Use cases

### Cobol syntax training

### Cobol source code analysis

### Cobol interactive editing

### Cobol programs modernization

## Unique design goals



## Features

## Similar projects

## Gettign started

## Documentation

## Contributing

