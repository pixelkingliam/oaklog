# oaklog

C# Flexible Logger.

Simple yet flexible logger for .NET 6+, made to be easy to setup.
# Compilation
Specificy the .NET version in  oaklog.csproj and use the `dotnet` cli utility to compile the library,
# Installation

## Manually
Download the DLL from the releases section or build the library.

Add a reference to the dll in the csproj file of your project.

Example of a csproj project with a reference to a local dll of OakLog
```xml
  <ItemGroup>
    <Reference Include="myproject/oaklog.dll"/>
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

# Usage

## Basic usage
Like any library you need to include it's namespace in your source.

Include the `OakLogger` namespace

``` cs
using OakLogger;
```

You will need to create one or more `LogOutput` objects, these point to a TextWriter stream for output, along side optional customization options, such as seperation and color. (see `LogOutput`'s documentation) 


``` cs
LogOutput MyLogOutput = new LogOutput(Console.Out) // new LogOutput object pointing to the standard output.
```
or for a file.
``` cs
LogOutput MyLogOutput = new LogOutput(StreamWriter("/tmp/ousepak.log")) // new LogOutput object pointing to the /tmp/iuseoak.log file.
// The constructor for StreamWriter(String) will create a new file everytime it's called, so you do not need to worry about making one yourself.
```

After that's done you'll want to make a `OLog` object.
``` cs
OLog MyLogger = new OLog() // new LogOutput object pointing to the standard output.
```

Add all the `LogOuput` objects you have made to the newly created `OLog` object
```cs
MyLogger.Outputs.Add(MyLogOutput); // makes MyLogger use MyLogOutput as an output
``` 
and your done! you've made a basic OLog object and you should be able to ues it via the `OLog.Print(object)` function

```cs
MyLogger.Print("Hello World!");
```
This will write "Hello World!" to all `LogOutput` in `OLog.Outputs`!

## Customizing your output
### Customizing `Olog`
The `OLog` class contains many members that helps to customize the output.

**`Color`**

The `OLog.Color` member is a `Tuple<byte,byte,byte>` each byte represent a color value, the first one being red, the second green and the third blue forming a RGB value. This rgb value will be used to color `LogItem`s other then `LogItem.Text` if the specified `LogOutput` object supports color.


Example:
```cs
var ToSTD = new LogOutput(Console.Out, useColor : true);
Logger.Color = new Tuple<byte,byte,byte>(205,0,26);
Logger.LogItems.Insert(0, LogItem.Type);
Logger.Outputs.Add(ToSTD);
Logger.Print("Hello World!");
```
Output: <t style="background-color: rgb(205,0,26)">[DEFAULT]</t> Hello World!


See `LogOutput` documentation for more information

**`LogItems`**

The `OLog.LogItems` member is the most direct way of customizing your `OLog` object, it is a `List` of `LogItem`, by default it only contains `LogItem.Text` but you can easily add more `LogItem`s by using the `OLog.LogItems.Add(LogItem.ExampleLogItem)` function to add one at the end of the List, or with the `OLog.LogItems.Insert(0, LogItem.ExampleLogItem)` function at the start.

Example:
```cs
var ToSTD = new LogOutput(Console.Out);
Logger.LogItems.Insert(0, LogItem.TimeSinceStartup);
Logger.Outputs.Add(ToSTD);
Logger.Print("Hello World!");
```
Output:
\[0.26s\] Hello World!

See the documentation for the `LogItem` type for more information.

**`LogType`**

The `OLog.LogType` member is a string used to show what kind of log message it is, usually use to show if a message is an error, a warning or some other kind of log messages. For the `OLog.LogType` string to be displayed the `OLog.LogItems` of the same object must contain `LogItem.Type`.

Example
```cs
var ToSTD = new LogOutput(Console.Out);
Logger.Outputs.Add(ToSTD);
Logger.LogItems.Insert(0, LogItem.Type);
Logger.LogType = "SUCCESS";
Logger.Print("Hello World!");
```

Output: \[SUCCESS\] Hello World!

See documentation for `LogItem`.

**`ImplicitNewLine`**

The `Olog.ImplicitNewLine` member is a boolean. If it is  true then `OLog.Print(object)` will finish with a newline character.  The default of this member is `true`.

Example:
```cs
var ToSTD = new LogOutput(Console.Out);
Logger.Outputs.Add(ToSTD);
Logger.ImplicitNewLine = false;
Logger.Print("Hello World!");
Logger.Print("I Love Cake!");
```

Output: Hello World!I Love Cake!

do note, that if you are outputting to a shell, some do not like it when the standard output does not end with a newline
### Customizing `LogOutput`

The `LogOutput` class contains a few members used in customizing the output on a per-output basis.

**`Seperator`**

The `Seperator` member represents the character(s) used to seperate the `LogItem.Text` 
from the other `LogItem`s. The default is a singular space. 

Example:
```cs
var ToSTD = new LogOutput(Console.Out, seperator : "-");
Logger.LogItems.Insert(0, LogItem.Type);
Logger.Outputs.Add(ToSTD);
Logger.Print("Hello World!");
```
Output: \[DEFAULT\]-Hello World!

**`SeperateLogItems`**

The `SeperateLogItems` member is a bool that controls wheter `LogItem`s should be seperated by the `Seperator`. The default is false.

Example:
```cs
var ToSTD = new LogOutput(Console.Out, seperateLogItems : true, seperator : "-");
Logger.LogItems.Insert(0, LogItem.TimeSinceStartup);
Logger.LogItems.Insert(0, LogItem.Type);
Logger.Outputs.Add(ToSTD);
Logger.Print("Hello World!");
```
Output: \[DEFAULT\]-\[0.16s\]-Hello World!

**`UseColor`**

The `UseColor` member is a bool that controls if the `LogItem`s of the output will have a colored background, this uses the `OLog.Color` tuple for the RGB values. This variable defaults to false and should not be set to the true if the output does not supoort ANSI color codes.

Example:
```cs
var ToSTD = new LogOutput(Console.Out, useColor : true);
Logger.Color = new Tuple<byte,byte,byte>(205,0,26);
Logger.LogItems.Insert(0, LogItem.Type);
Logger.Outputs.Add(ToSTD);
Logger.Print("Hello World!");
```
Output: <t style="background-color: rgb(205,0,26)">[DEFAULT]</t> Hello World!

See `OLog`'s documentation for more information.
# Types
## LogOutput Class

The `LogOutput` class serves as directions and rules for outputting text
by the `OLog` class.


### Constructor
Initializes a new instance of the `LogOutput` class for the specified TextWriter stream.
``` cs
public LogOutput(TextWriter, string seperator = " ", bool seperateLogItems = false, bool useColor = false)
```

#### Parameters

-   `TextWriter` outputStream :The `TextWriter` to which log messages will be written. The text writer to which log messages will be written.
-   `String` seperator : The string used to separate log items. Defaults to a single space.
-   `Boolean` seperateLogItems : Sets the `SeperateLogItems` variable. Default is
    `false`.
-   `Boolean` useColor : Whether to use color when writing log messages. Defaults to false.

### Public Members

|  Variable Name   |  Type | Description                                                                                             |
|:----------------:|:-------------:|:--------------------------------------------------------------------------------------------------------:|
|   OutputStream   | `TextWriter`  | This stream will be used as output for the `OLog.Print` function                                         |
|    Seperator     |   `String`    | Seperator for the Log text and Log information.                                                         |
| SeperateLogItems |   `Boolean`   | Seperate all the LogItems with Seperator. If false this will only place a seperator before the Log text |
|     UseColor     |   `Boolean`   | Ouput with color? only set this to `true` on outputs supporting ANSI color escape codes                  |

## LogItem Enum

The `LogItem` Enum contains the various possible components of a log
message, such as the log type, current time and the message itself

### Enum Members

|   Member Name    | Index | Description                                                |          Example Output           |
|:----------------:|:-----:|:------------------------------------------------------------:|:---------------------------------:|
|       Text       |  `0`  | The string argument of `OLog.Print()`.                     |           Hello, World!           |
| TimeSinceStartup |  `1`  | The time in seconds since the program has started          |              \[43s\]              |
|    SystemTime    |  `2`  | The current system time in ISO 8601 format                |       \[1970/01/01 12:00\]        |
|    StackFrame    |  `3`  | The current stack frame at which `OLog.Print` was called   | \[Program.Main(String\[\] args)\] |
|       Type       |  `4`  | The `OLog.LogType` string                                  |[ERROR\]                           |
|     ThreadID     |  `5`  | The thread that `OLog.Print`                               |              \[#1\]               |


Note that all `LogItem` output will be surrounded by \[brackets\], the only exception to this is `LogItem.Text`.
## OLog Class

The `OLog` class is used to output text with it’s `Print` function.

### Constructor

``` csharp
OLog Logger = new OLog()
```

This constructor sets the `LogItems` member to a
`new List<LogItem>(){ LogItem.Text }` and the `Outputs` to a
`new List<LogOutput>()`.

### Public Members

|   Member Name   |           Type            | Description                                                                                                                              |
|:---------------:|:-------------------------:|:----------------------------------------------------------------------------------------------------------------------------------------:|
|      Color      | `Tuple<byte, byte, byte>` | The color value in RGB, requires a `LogOutput` object with `UseColor` enabled.                                                           |
|    LogItems     |      `List<LogItem>`      | The order of LogItem to display, it’s best to have `LogItem.Text` last, see `LogItem`’s documentation for a list of possible `LogItem`s. |
| ImplicitNewLine |         `Boolean`         | Implicitly add a newline after a `Print` call to the output.                                                                             |
|     Outputs     |      `List<Output>`       | List of outputs that the logger should write to. See `LogOutput`’s documentation.                                                        |

### Public Methods

| Method Name | Arguments        | Description                                                                                                      |
|:-------------:|:------------------:|:------------------------------------------------------------------------------------------------------------------:|
| Print       | `object` toPrint | Print to the specified outputs (see `OLog.Outputs`) a stylized (see `OLog.LogItems`) message (`toPrint.ToString`) |

# Contributing
Contributions in the forms of new `LogItem`s, bug fixes and documentation are generally welcomed! Do make sure to make a pull request!