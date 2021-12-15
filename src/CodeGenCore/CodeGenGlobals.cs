using System.Linq.Expressions;
using System.Reflection;
using Scriban.Runtime;

namespace CodeGenCore;

/// <summary>
/// The available globals when generating code from a template.
/// </summary>
public sealed class CodeGenGlobals
{
	/// <summary>
	/// Creates globals from the specified object.
	/// </summary>
	/// <remarks>Each public property and method becomes a global (transformed to snake_case if
	/// <see cref="CodeGenSettings.UseSnakeCase" /> is <c>true</c>). The properties and methods can
	/// be static or instance.</remarks>
	public static CodeGenGlobals Create(object source) => new CodeGenGlobals(source);

	internal ScriptObject CreateScriptObject(CodeGenSettings? settings)
	{
		var scriptObject = new ScriptObject();

		var sourceType = m_source.GetType();
		var useSnakeCase = settings?.UseSnakeCase ?? false;

		foreach (var (name, methodInfo) in sourceType.GetProperties().Select(x => (x.Name, x.GetMethod))
			.Concat(sourceType.GetMethods().Where(IsValidMethod).Select(x => (x.Name, x))))
		{
			scriptObject.Import(
				member: useSnakeCase ? StandardMemberRenamer.Rename(name) : name,
				function: methodInfo.CreateDelegate(
					Expression.GetDelegateType(
						methodInfo.GetParameters()
							.Select(parameter => parameter.ParameterType)
							.Append(methodInfo.ReturnType)
							.ToArray()),
					target: methodInfo.IsStatic ? null : m_source));
		}

		return scriptObject;

		static bool IsValidMethod(MethodInfo method) =>
			(method.Attributes & MethodAttributes.SpecialName) == 0 &&
			method.DeclaringType != typeof(object);
	}

	private CodeGenGlobals(object source) => m_source = source;

	private readonly object m_source;
}
