using System;
using System.IO;
using System.Text;
using Yaml2Doc.Cli;
using Xunit;

namespace Yaml2Doc.Core.Tests.Cli
{
    public class Yaml2DocCliTests
    {
        [Fact]
        public void Run_WithValidInput_WritesMarkdownToStdout_AndReturnsZero()
        {
            // Arrange: create a temp YAML file in the current working directory
            var yaml = """
                       name: demo-pipeline
                       foo: bar
                       steps:
                         - run: echo "hello"
                       """;

            var baseDir = Directory.GetCurrentDirectory();
            var tempYamlPath = Path.Combine(baseDir, $"yaml2doc-cli-test-{Guid.NewGuid():N}.yml");
            File.WriteAllText(tempYamlPath, yaml);

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            try
            {
                // Act
                var exitCode = Yaml2DocCli.Run(
                    new[] { tempYamlPath },
                    stdout,
                    stderr
                );

                // Assert
                Assert.Equal(0, exitCode);

                var output = stdout.ToString();
                Assert.Contains("# demo-pipeline", output);
                Assert.Contains("## Root Keys", output);
                Assert.Contains("- foo", output);
                Assert.Contains("- steps", output);

                // stderr should be empty on success
                Assert.Equal(string.Empty, stderr.ToString());
            }
            finally
            {
                if (File.Exists(tempYamlPath))
                {
                    File.Delete(tempYamlPath);
                }
            }
        }

        [Fact]
        public void Run_WithOutputFile_WritesMarkdownToFile()
        {
            // Arrange
            var yaml = "name: file-output-test";
            var baseDir = Directory.GetCurrentDirectory();

            var tempYamlPath = Path.Combine(baseDir, $"yaml2doc-cli-test-{Guid.NewGuid():N}.yml");
            var tempMdPath = Path.Combine(baseDir, $"yaml2doc-cli-test-{Guid.NewGuid():N}.md");

            File.WriteAllText(tempYamlPath, yaml);

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            try
            {
                // Act
                var exitCode = Yaml2DocCli.Run(
                    new[] { tempYamlPath, tempMdPath },
                    stdout,
                    stderr
                );

                // Assert
                Assert.Equal(0, exitCode);
                Assert.True(File.Exists(tempMdPath));

                var md = File.ReadAllText(tempMdPath, Encoding.UTF8);
                Assert.Contains("# file-output-test", md);

                // stdout should be empty because we wrote to file instead
                Assert.Equal(string.Empty, stdout.ToString());
            }
            finally
            {
                if (File.Exists(tempYamlPath))
                {
                    File.Delete(tempYamlPath);
                }

                if (File.Exists(tempMdPath))
                {
                    File.Delete(tempMdPath);
                }
            }
        }

        [Fact]
        public void Run_WithMissingInputFile_ReturnsMissingFileExitCode()
        {
            // Arrange: missing file inside the working directory
            var baseDir = Directory.GetCurrentDirectory();
            var missingPath = Path.Combine(baseDir, $"yaml2doc-cli-missing-{Guid.NewGuid():N}.yml");

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            // Act
            var exitCode = Yaml2DocCli.Run(
                new[] { missingPath },
                stdout,
                stderr
            );

            // Assert
            Assert.Equal(2, exitCode);

            var err = stderr.ToString();
            Assert.Contains("Input file not found", err);
        }

        [Fact]
        public void Run_WithNoArguments_ReturnsUsageError()
        {
            // Arrange
            var stdout = new StringWriter();
            var stderr = new StringWriter();

            // Act
            var exitCode = Yaml2DocCli.Run(
                Array.Empty<string>(),
                stdout,
                stderr
            );

            // Assert
            Assert.Equal(1, exitCode);
            var err = stderr.ToString();
            Assert.Contains("Usage:", err);
            Assert.Contains("yaml2doc <input.yml>", err);
        }

        [Fact]
        public void Run_WithInvalidYaml_ReturnsConversionErrorAndWritesParseError()
        {
            // Arrange
            var cwd = Directory.GetCurrentDirectory();
            var inputPath = Path.Combine(cwd, "invalid-cli.yml");

            // Malformed YAML: missing closing bracket on the flow sequence
            const string invalidYaml = "root:\n  list: [1, 2, 3\n";

            File.WriteAllText(inputPath, invalidYaml);

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            try
            {
                // Act
                var exitCode = Yaml2DocCli.Run(
                    new[] { inputPath },
                    stdout,
                    stderr
                );

                // Assert
                Assert.Equal(3, exitCode); // conversion / parse error

                var errorText = stderr.ToString();
                var outputText = stdout.ToString();

                // From Yaml2DocEngine: "Failed to parse YAML: ..."
                Assert.Contains("Failed to parse YAML", errorText, StringComparison.OrdinalIgnoreCase);

                // Should not have produced any Markdown on stdout
                Assert.True(string.IsNullOrEmpty(outputText) || !outputText.Contains("#"));
            }
            finally
            {
                if (File.Exists(inputPath))
                {
                    File.Delete(inputPath);
                }
            }
        }

        [Fact]
        public void Run_WithEmptyYamlFile_ReturnsConversionErrorAndReportsEmptyInput()
        {
            // Arrange
            var cwd = Directory.GetCurrentDirectory();
            var inputPath = Path.Combine(cwd, "empty-cli.yml");

            // Empty file → engine throws Yaml2DocParseException("YAML input is empty.")
            File.WriteAllText(inputPath, string.Empty);

            var stdout = new StringWriter();
            var stderr = new StringWriter();

            try
            {
                // Act
                var exitCode = Yaml2DocCli.Run(
                    new[] { inputPath },
                    stdout,
                    stderr
                );

                // Assert
                Assert.Equal(3, exitCode);

                var errorText = stderr.ToString();
                var outputText = stdout.ToString();

                Assert.Contains("YAML input is empty", errorText, StringComparison.OrdinalIgnoreCase);
                Assert.True(string.IsNullOrEmpty(outputText) || !outputText.Contains("#"));
            }
            finally
            {
                if (File.Exists(inputPath))
                {
                    File.Delete(inputPath);
                }
            }
        }
    }

    public class Yaml2DocCliDialectTests
    {
        [Fact]
        public void ParseArguments_NoDialectFlag_LeavesDialectNull()
        {
            var parsed = Yaml2DocCli.ParseArguments(new[] { "input.yml" });

            Assert.Null(parsed.DialectId);
            Assert.Equal("input.yml", parsed.InputPath);
            Assert.Null(parsed.OutputPath);
            Assert.Null(parsed.ErrorExitCode);
        }

        [Fact]
        public void ParseArguments_WithDialectStandard_SetsDialectId()
        {
            var parsed = Yaml2DocCli.ParseArguments(new[] { "--dialect", "standard", "input.yml" });

            Assert.Equal("standard", parsed.DialectId);
            Assert.Equal("input.yml", parsed.InputPath);
            Assert.Null(parsed.OutputPath);
            Assert.Null(parsed.ErrorExitCode);
        }

        [Fact]
        public void ParseArguments_WithDialectGha_SetsDialectIdToGha()
        {
            var parsed = Yaml2DocCli.ParseArguments(new[] { "--dialect", "gha", "workflow.yml" });

            Assert.Equal("gha", parsed.DialectId);
            Assert.Equal("workflow.yml", parsed.InputPath);
            Assert.Null(parsed.OutputPath);
            Assert.Null(parsed.ErrorExitCode);
        }

        [Fact]
        public void ParseArguments_WithDialectAdo_SetsDialectIdToAdo()
        {
            var parsed = Yaml2DocCli.ParseArguments(new[] { "--dialect", "ado", "azure-pipelines.yml" });

            Assert.Equal("ado", parsed.DialectId);
            Assert.Equal("azure-pipelines.yml", parsed.InputPath);
            Assert.Null(parsed.OutputPath);
            Assert.Null(parsed.ErrorExitCode);
        }

        [Fact]
        public void ParseArguments_DialectWithoutValue_ReturnsError()
        {
            var parsed = Yaml2DocCli.ParseArguments(new[] { "--dialect" });

            Assert.NotNull(parsed.ErrorExitCode);
            Assert.Equal(1, parsed.ErrorExitCode);
            Assert.Contains("--dialect option requires an argument", parsed.ErrorMessage);
        }
    }
}
