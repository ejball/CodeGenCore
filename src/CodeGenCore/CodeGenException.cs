namespace CodeGenCore;

/// <summary>
/// Thrown when an error occurs while generating code.
/// </summary>
public sealed class CodeGenException : Exception
{
	/// <summary>
	/// Creates an exception.
	/// </summary>
	/// <param name="message">The exception message.</param>
	/// <param name="innerException">The inner exception.</param>
	public CodeGenException(string message, Exception? innerException = null)
		: base(message, innerException)
	{
	}
}
