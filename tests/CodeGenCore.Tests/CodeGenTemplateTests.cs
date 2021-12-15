using System.Globalization;

namespace CodeGenCore.Tests
{
	internal class CodeGenTemplateTests
	{
		[Test]
		public void EmptyTemplate()
		{
			var template = CodeGenTemplate.Parse("");
			template.Generate().Should().BeEmpty();
		}

		[Test]
		public void OneFile()
		{
			var template = CodeGenTemplate.Parse("==> a.txt");
			template.Generate().Should().Equal(new CodeGenOutputFile("a.txt", ""));
		}

		[Test]
		public void TwoFile()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n==> b.txt");
			template.Generate().Should().Equal(new CodeGenOutputFile("a.txt", ""), new CodeGenOutputFile("b.txt", ""));
		}

		[Test]
		public void RemoveBlankLines()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n\n\nline 1\n\n\n==> b.txt\n\nline 1\n\n");
			template.Generate(settings: s_lfSettings).Should().Equal(new CodeGenOutputFile("a.txt", "\nline 1\n"), new CodeGenOutputFile("b.txt", "line 1\n"));
		}

		[TestCase("\t")]
		[TestCase("  ")]
		[TestCase("    ")]
		public void PreserveIndent(string indent)
		{
			var template = CodeGenTemplate.Parse($"==> a.txt\nline 1\n{indent}line 2\n{indent}{indent}line 3");
			template.Generate(settings: s_lfSettings).Should().Equal(new CodeGenOutputFile("a.txt", $"line 1\n{indent}line 2\n{indent}{indent}line 3\n"));
		}

		[TestCase("\t", "\t")]
		[TestCase("  ", "  ")]
		[TestCase("\t", "  ")]
		[TestCase("  ", "\t")]
		[TestCase("    ", "  ")]
		[TestCase("  ", "    ")]
		public void ConvertIndent(string indentBefore, string indentAfter)
		{
			var template = CodeGenTemplate.Parse($"==> a.txt\nline 1\n{indentBefore}line 2\n{indentBefore}{indentBefore}line 3");
			template.Generate(settings: new() { IndentText = indentAfter, NewLine = "\n" })
				.Should().Equal(new CodeGenOutputFile("a.txt", $"line 1\n{indentAfter}line 2\n{indentAfter}{indentAfter}line 3\n"));
		}

		[Test]
		public void BasicMath()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n1 + 2 = {{ 1 + 2 }}\n");
			template.Generate(settings: s_lfSettings).Should().Equal(new CodeGenOutputFile("a.txt", "1 + 2 = 3\n"));
		}

		[Test]
		public void UseGlobals()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n{{ Number }}\n{{ Triple Number }}");
			var globals = CodeGenGlobals.Create(new Globals());
			template.Generate(settings: s_lfSettings, globals: globals).Should().Equal(new CodeGenOutputFile("a.txt", "42\n74088\n"));
		}

		[TestCase(null, "3.14")]
		[TestCase("", "3.14")]
		[TestCase("en-NZ", "3.14")]
		[TestCase("de-DE", "3,14")]
		public void Culture(string? culture, string pi)
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n{{ 3.14 }}\n");
			template.Generate(settings: new() { NewLine = "\n", Culture = culture is null ? null : CultureInfo.CreateSpecificCulture(culture) })
				.Should().Equal(new CodeGenOutputFile("a.txt", $"{pi}\n"));
		}

		[Test]
		public void UseGlobalsSnakeCase()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n{{ number }}\n{{ triple number }}");
			var globals = CodeGenGlobals.Create(new Globals());
			template.Generate(settings: new CodeGenSettings { NewLine = "\n", UseSnakeCase = true }, globals: globals).Should().Equal(new CodeGenOutputFile("a.txt", "42\n74088\n"));
		}

		[Test]
		public void MissingGlobal()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n{{ number }}\n");
			var globals = CodeGenGlobals.Create(new Globals());
			Assert.Throws<CodeGenException>(() => template.Generate());
		}

		[Test]
		public void BlankFileName()
		{
			var template = CodeGenTemplate.Parse("==>\nA\n");
			Assert.Throws<CodeGenException>(() => template.Generate());
		}

		[Test]
		public void RepeatedFileName()
		{
			var template = CodeGenTemplate.Parse("==> a.txt\n==> a.txt");
			Assert.Throws<CodeGenException>(() => template.Generate());
		}

		[Test]
		public void IgnorePrologue()
		{
			var template = CodeGenTemplate.Parse("before\n==> a.txt\nafter\n");
			template.Generate(settings: s_lfSettings).Should().Equal(new CodeGenOutputFile("a.txt", "after\n"));
		}

		[Test]
		public void SingleFileName()
		{
			var template = CodeGenTemplate.Parse("before\n==> a.txt\nafter\n");
			template.Generate(settings: new CodeGenSettings { NewLine = "\n", SingleFileName = "single.txt" }).Should().Equal(new CodeGenOutputFile("single.txt", "before\n==> a.txt\nafter\n"));
		}

		public sealed class Globals
		{
			public int Number => 42;

			public double Triple(int value) => value * value * value;
		}

		private static readonly CodeGenSettings s_lfSettings = new() { NewLine = "\n" };
	}
}
