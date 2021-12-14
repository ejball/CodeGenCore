namespace CodeGenCore;

/// <summary>
/// Settings used when generating code.
/// </summary>
public sealed class CodeGenSettings
{
	/// <summary>
	/// The newline to use. (Defaults to <c>Environment.NewLine</c>.)
	/// </summary>
	public string? NewLine { get; set; }

	/// <summary>
	/// The indentation to use.
	/// </summary>
	/// <remarks>If specified, the indentation in the template is changed to
	/// use the specified indentation. If not, the indentation in the template is
	/// used as is.</remarks>
	public string? IndentText { get; set; }
}
