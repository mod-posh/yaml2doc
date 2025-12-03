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
    }
}
