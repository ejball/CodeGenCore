using System.Linq.Expressions;
using System.Reflection;
using Scriban.Runtime;

namespace CodeGenCore;

public sealed class CodeGenGlobals
{
	public static CodeGenGlobals Create(object source)
	{
		var scriptObject = new ScriptObject();

		var sourceType = source.GetType();

		foreach (var (name, methodInfo) in sourceType.GetProperties().Select(x => (x.Name, x.GetMethod))
			.Concat(sourceType.GetMethods().Where(IsValidMethod).Select(x => (x.Name, x))))
		{
			scriptObject.Import(member: name,
				function: methodInfo.CreateDelegate(
					Expression.GetDelegateType(
						methodInfo.GetParameters()
							.Select(parameter => parameter.ParameterType)
							.Append(methodInfo.ReturnType)
							.ToArray()),
					target: methodInfo.IsStatic ? null : source));
		}

		return new CodeGenGlobals(scriptObject);

		static bool IsValidMethod(MethodInfo method) =>
			(method.Attributes & MethodAttributes.SpecialName) == 0 &&
			method.DeclaringType != typeof(object);
	}

	internal ScriptObject ScriptObject { get; }

	private CodeGenGlobals(ScriptObject scriptObject) => ScriptObject = scriptObject;
}
