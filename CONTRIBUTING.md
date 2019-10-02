# Contributing

## Reporting Issues

If an issue is found, and you would like to report it, please include detailed steps as to how the issue was encountered. Which feature were you using when you ran into the issue? What version of android does your device run? What model is the device? Are you rooted? Are you running custom firmware? Anything else you think might be relevant. If you can get a log that could be helpful as well, but is not required.

## Contributing Code

Code can be submit via a pull request. Code must conform to the following style guidelines. This list is not exhaustive, but in general if code closely follows the existing style found within the app it should be fine.


### Curly braces should be on their own line. 
Good:
```c#
private MyClass Create(int id, string prop1)
{
    // some code
}
```
Bad:
```c#
private MyClass Create(int id, string prop1) {
    // some code
}
```
Worse:
```c#
private MyClass Create(int id, string prop1) {
    /* some code */ }
``` 
An exception to this is short lines when using object initialization. If the line becomes too long the second version should be used. The following are both acceptable:
```c#
var myObject = new MyClass { MyProp = "value", Id = 1 };
var myObject2 = new MyClass
{
    MyProp = "value",
    Id = 1
}
```
### Use of `var` is preferred
### Never include multiple statements on a single line
Bad:
```c#
if (someCondition) DoTheThing()
```
### Single Line loops, if/else statements, etc do not use braces.
_But but but, what if someone adds a line and suddenly something conditional is always executed??!?_
If your IDE doesn't keep you from doing that you need a new IDE. 
The only exception is if blocks with an else block where one of them requires braces. If one requires them use them on both.
Good:
```c#
if (someCondition)
    DoTheThing();
```
Also Good:
```c#
if (someCondition)
{
    DoTheThing();
}
else
{
    LogConditionMiss();
    DoTheOtherThing();
}
```
Bad:
```c#
foreach (var item in list)
{
    DoTheThing(item);
}
```
### Use of LINQ to Objects is encouraged.
If you're looping over a collection, chances are you can achieve the same results using LINQ. Please do so.
### Avoid mutating state
This is more of a suggestion than a hard rule.
### Methods should be short.
Very short. If the body of your method does not fit on my screen (40-50) lines it is _absolutely_ too long. Even then if it's longer than about 20 lines it's _probably_ too long. Break logical sections into their own methods and give that process a descriptive name
### Comments in method bodies should rarely be used
If you follow the method length points, comments shouldn't be needed inside method bodies because your code will be self explanatory. There are rare exceptions for strange quirks in a library that could potentially cause confusion, or a justification for not doing something in what might be considered the "normal" way. If a comment is needed it should explain _WHY_ not _WHAT_ the code is doing.
Acceptable:
```c#
//Using toolbar.InflateMenu() instead of overriding OnCreateOptionsMenu() to avoid the overflow menu
toolbar.InflateMenu(Resource.Menu.main_menu);
```
Any of the following are bad, and based on things I've seen actual developers do:
```c#
// Load the entity from the repository by Id
var entity = _entityRepository.GetEntity(entityId);

// START: Update the entity

entity.Prop1 = newProp1Val;
entity.Prop2 = newProp2Val;

// START: Save

_entityRepository.SaveEntity(entity);

// END: Save

// END : Update the entity
```
### Use XML style doc comments on all public methods
Any public method should have a doc comment on it explaining briefly what the function does. While the name of the function should give a general idea of the purpose, additional details can be provided in the doc comment.
XML comments on private and internal methods are encouraged, but not required.
### Use auto-properties when possible.
In any classes with public fields use auto-properties if possible. If no validation needs to occur no backing property is need and the `set` can be public. If the class has special validation that needs to happen when the property is set, making the `set` private and creating a separate method with validation is preferred to adding a backing field but either is acceptable.
### Consider the life of a variable when naming it
The longer the life of a variable, the better it's name needs to be. There's no need to stress about a name for a string that is going to be forgotten one line later. Class level variables, constants, or a variable that is used frequently within a single method should have better names so the code is clearer
```c#
private Dictionary<string, string> weatherNameToIconNameMap; //initialized in the constructor

/* more code */

/// <summary>
/// Gets the weather icon file name based on the name of the weather provided.
/// </summary>
public string GetWeatherIconName(string weather)
{
    return weatherNameToFileNameMap.TryGetValue(weather, out var s)
      ? s
      : string.Empty;
}
```
