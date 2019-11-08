# Introduction to C# - Exercises

## Goal

These exercises aim to introduce newcomers to the programming language C# from Microsoft. It is
expected that you already have some experience with other (object oriented) programming languages.
The focus of the introduction is the programming language, not the runtime / framework .NET.

## How to complete the tasks

The exercises are grouped by topic, where each topic has its own folder. For each topic a second
folder labeled _x-2 Solution_ exists. As the name suggests, this folder contains one of the many
possible solutions. I highly recommend not to look at the solutions in advance, but only if you
either struggle with solving a particular task or have already come up with a solution and wish to
compare it to mine. The exercises themselfes are set up as small, independent scenarios. This means
you can complete them in any order you like or skip them entirely. The instructions for each task
are written as a comment at the top of the _*.cs_ files themselfes. The files also contian further
tips directly in the code. Some of the files might contian a larger portion of code that is not
relevant for the exercise itselft, but is needed for the application to run. Such code is placed
inside a region named "SupportingCode". Your editor or IDE should allow you to hide this section so
it won't distract you from what's important, if you wish. Also, for most exercise exists a test
file called _Tests-xxx.cs_. These should help you along the way, as they give you feedback about
your progress. You can find out how to run and test the applicaitons by reading the next section.

## How to run and test the applications

Your editor or IDE most likely already provides convenient functions for running and testing the
exercies. However, some applications require you to provide command line parameters when starting
the program. In these cases it might be easier to start them from a command prompt as described
below.

### Run a program

1. Open a terminal application an navigate inside the folder of the exercise: `cd /path/to/exercise`
2. Run `dotnet run`. If the folder contains more than one _*.csproj_ files, you need to specify the
    one belonging to the exercise you wish to run: `dotnet run -p XYZ.csproj`

In case the `dotnet run` command does not work as expected you can perform the same tasks in two
separate steps: `dotnet build` and `dotnet bin/Debug/netcoreapp3.0/<project-name>.[exe|dll]`. The
build artifact will have an _.exe_ ending if compiled on a windows machine and a _.dll_ ending 
otherwise.

### Test a program

1. Open a terminal application an navigate inside the folder of the exercise: `cd /path/to/exercise`
2. Run `dotnet test`. If the folder contains more than one _*.csproj_ files, you need to specify the
    one belonging to the exercise you wish to run: `dotnet test XYZ.csproj`

## How to contribute

Do you have an awesome idea for an exercise that would help newcomers to learn the language? Or have
you found an issue with one of the existing exercises? Then please feel free to change anything you
like in a separate feature branch and create a pull request.