# CodeGenSettings class

Settings used when generating code.

```csharp
public sealed class CodeGenSettings
```

## Public Members

| name | description |
| --- | --- |
| [CodeGenSettings](CodeGenSettings/CodeGenSettings.md)() | The default constructor. |
| [Culture](CodeGenSettings/Culture.md) { get; set; } | The culture to use. (Defaults to the "invariant" culture.) |
| [IndentText](CodeGenSettings/IndentText.md) { get; set; } | The indentation to use. |
| [NewLine](CodeGenSettings/NewLine.md) { get; set; } | The newline to use. (Defaults to `Environment.NewLine`.) |
| [SingleFileName](CodeGenSettings/SingleFileName.md) { get; set; } | The name of the single file to generate from the template. |
| [UseSnakeCase](CodeGenSettings/UseSnakeCase.md) { get; set; } | True to make properties and methods available using snake case names. (Default false.) |

## See Also

* namespace [CodeGenCore](../CodeGenCore.md)
* [CodeGenSettings.cs](https://github.com/ejball/CodeGenCore/tree/master/src/CodeGenCore/CodeGenSettings.cs)

<!-- DO NOT EDIT: generated by xmldocmd for CodeGenCore.dll -->
