using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Scriban;

namespace CodeGenCore;

/// <summary>
/// A template from which code can be generated.
/// </summary>
/// <remarks>The template is implemented with Scriban, a .NET implementation of Liquid templates.</remarks>
public sealed class CodeGenTemplate
{
	/// <summary>
	/// Parses a template.
	/// </summary>
	public static CodeGenTemplate Parse(string text) => new(Template.Parse(text));

	/// <summary>
	/// Generates code using the specified globals and settings.
	/// </summary>
	public IReadOnlyList<CodeGenOutputFile> Generate(CodeGenGlobals? globals = null, CodeGenSettings? settings = null)
	{
		var newLine = settings?.NewLine ?? Environment.NewLine;
		var indentText = settings?.IndentText;
		string? templateIndentText = null;

		var context = new TemplateContext
		{
			StrictVariables = true,
			EnableRelaxedTargetAccess = true,
			MemberRenamer = x => x.Name,
		};
		context.PushCulture(settings?.Culture ?? CultureInfo.InvariantCulture);

		if (globals is not null)
			context.PushGlobal(globals.ScriptObject);

		var text = Template.Render(context);
		using var reader = new StringReader(text);

		// find first file
		var fileStart = "";
		string? line;
		while ((line = reader.ReadLine()) != null)
		{
			var match = Regex.Match(line, @"^==+>");
			if (match.Success)
			{
				fileStart = match.Value;
				break;
			}
		}

		var files = new List<CodeGenOutputFile>();

		while (line != null)
		{
			var fileName = line.Substring(fileStart.Length);

			var fileLines = new List<string>();
			while ((line = reader.ReadLine()) != null && !line.StartsWith(fileStart, StringComparison.Ordinal))
			{
				line = line.TrimEnd();

				if (indentText != null)
				{
					var indentMatch = s_indentRegex.Match(line);
					if (indentMatch.Success)
					{
						templateIndentText ??= indentMatch.Value;
						var indent = indentMatch.Length / templateIndentText.Length;
						var lineBuilder = new StringBuilder();
						for (var i = 0; i < indent; i++)
							lineBuilder.Append(indentText);
						lineBuilder.Append(line.Substring(templateIndentText.Length * indent));
						line = lineBuilder.ToString();
					}
				}

				fileLines.Add(line);
			}

			// skip exactly one blank line to allow file start to stand out
			if (fileLines.Count != 0 && string.IsNullOrWhiteSpace(fileLines[0]))
				fileLines.RemoveAt(0);

			// remove all blank lines at the end
			while (fileLines.Count != 0 && string.IsNullOrWhiteSpace(fileLines[fileLines.Count - 1]))
				fileLines.RemoveAt(fileLines.Count - 1);

			// build text from lines
			using var stringWriter = new StringWriter { NewLine = newLine };
			foreach (var fileLine in fileLines)
				stringWriter.WriteLine(fileLine);
			files.Add(new CodeGenOutputFile(Name: fileName.Trim(), Text: stringWriter.ToString()));
		}

		return files;
	}

	private Template Template { get; }

	private CodeGenTemplate(Template template) => Template = template;

	private static readonly Regex s_indentRegex = new(@"^[ \t]+", RegexOptions.Compiled);
}
