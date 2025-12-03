using System;
using System.IO;
using Yaml2Doc.Core;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Engine;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Markdown;

namespace Yaml2Doc.Cli
{
    /// <summary>
    /// Provides the command-line entry point logic for converting YAML to Markdown.
    /// </summary>
    /// <remarks>
    /// Performs argument parsing and enforces safe file path handling.
    /// Rejects UNC/device paths, prevents traversal via reparse points, and avoids overwriting output files.
    /// Public for unit testing CLI behavior.
    /// </remarks>
    public static class Yaml2DocCli
    {
        /// <summary>
        /// Strongly-typed result of CLI argument parsing.
        /// </summary>
        /// <param name="DialectId">Optional dialect identifier provided via <c>--dialect &lt;id&gt;</c>.</param>
        /// <param name="InputPath">Input YAML file path as provided on the command line.</param>
        /// <param name="OutputPath">Optional output Markdown file path; if <see langword="null"/>, output is written to standard output.</param>
        /// <param name="ErrorExitCode">If parsing fails, the corresponding exit code (e.g., <c>1</c>); otherwise <see langword="null"/>.</param>
        /// <param name="ErrorMessage">Human-readable error message when parsing fails; otherwise <see langword="null"/>.</param>
        public sealed record ParsedArguments(
            string? DialectId,
            string? InputPath,
            string? OutputPath,
            int? ErrorExitCode,
            string? ErrorMessage
        );

        /// <summary>
        /// Parses command-line arguments into dialect, input, and output values without performing filesystem operations.
        /// </summary>
        /// <param name="args">Raw command-line arguments.</param>
        /// <returns>
        /// A <see cref="ParsedArguments"/> containing parsed values. On error, <c>ErrorExitCode</c> and <c>ErrorMessage</c>
        /// indicate the failure reason and recommended exit code.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Supported forms:
        /// - <c>yaml2doc &lt;input.yml&gt;</c>
        /// - <c>yaml2doc &lt;input.yml&gt; &lt;output.md&gt;</c>
        /// - <c>yaml2doc --dialect &lt;id&gt; &lt;input.yml&gt; [output.md]</c>
        /// </remarks>
        public static ParsedArguments ParseArguments(string[] args)
        {
            if (args is null) throw new ArgumentNullException(nameof(args));

            string? dialectId = null;
            string? inputPath = null;
            string? outputPath = null;

            if (args.Length == 0)
            {
                return new ParsedArguments(
                    DialectId: null,
                    InputPath: null,
                    OutputPath: null,
                    ErrorExitCode: 1,
                    ErrorMessage: "Error: No arguments provided."
                );
            }

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (string.Equals(arg, "--dialect", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length)
                    {
                        return new ParsedArguments(
                            DialectId: null,
                            InputPath: null,
                            OutputPath: null,
                            ErrorExitCode: 1,
                            ErrorMessage: "Error: --dialect option requires an argument."
                        );
                    }

                    dialectId = args[++i];
                }
                else if (inputPath is null)
                {
                    inputPath = arg;
                }
                else if (outputPath is null)
                {
                    outputPath = arg;
                }
                else
                {
                    return new ParsedArguments(
                        DialectId: null,
                        InputPath: null,
                        OutputPath: null,
                        ErrorExitCode: 1,
                        ErrorMessage: "Error: Too many arguments."
                    );
                }
            }

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return new ParsedArguments(
                    DialectId: null,
                    InputPath: null,
                    OutputPath: null,
                    ErrorExitCode: 1,
                    ErrorMessage: "Error: Input file path is required."
                );
            }

            return new ParsedArguments(
                DialectId: dialectId,
                InputPath: inputPath,
                OutputPath: outputPath,
                ErrorExitCode: null,
                ErrorMessage: null
            );
        }

        /// <summary>
        /// Executes the CLI: validates paths, converts YAML to Markdown, and writes output.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <param name="stdout">Destination for normal output (Markdown).</param>
        /// <param name="stderr">Destination for error output.</param>
        /// <returns>
        /// Exit code: <c>0</c> success; <c>1</c> usage/argument errors;
        /// <c>2</c> invalid input path or file not found; <c>3</c> output path failure, conversion error, or I/O failure.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stdout"/> or <paramref name="stderr"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Delegates argument parsing to <see cref="ParseArguments(string[])"/> before path validation, YAML parsing, and rendering.
        /// </remarks>
        public static int Run(string[] args, TextWriter stdout, TextWriter stderr)
        {
            if (stdout is null) throw new ArgumentNullException(nameof(stdout));
            if (stderr is null) throw new ArgumentNullException(nameof(stderr));

            args ??= Array.Empty<string>();

            var parsed = ParseArguments(args);
            if (parsed.ErrorExitCode is not null)
            {
                if (!string.IsNullOrEmpty(parsed.ErrorMessage))
                {
                    stderr.WriteLine(parsed.ErrorMessage);
                }
                PrintUsage(stderr);
                return parsed.ErrorExitCode.Value;
            }

            var dialectId = parsed.DialectId;
            var inputPath = parsed.InputPath!;
            var outputPath = parsed.OutputPath;

            try
            {
                var baseDir = Directory.GetCurrentDirectory();
                var baseDirFull = Path.GetFullPath(baseDir);

                var inputFullPath = ResolveAndValidatePath(inputPath, baseDirFull, allowExistingFile: true, stderr);
                if (inputFullPath is null)
                {
                    return 2;
                }

                string yamlText;
                try
                {
                    using var fs = new FileStream(inputFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new StreamReader(fs);
                    yamlText = reader.ReadToEnd();
                }
                catch (FileNotFoundException)
                {
                    stderr.WriteLine($"Error: Input file not found: '{inputFullPath}'.");
                    return 2;
                }

                var loader = new YamlLoader();
                var standardDialect = new StandardYamlDialect(loader);
                var registry = new Yaml2DocRegistry(new[] { standardDialect });

                var markdownRenderer = new BasicMarkdownRenderer();
                var engine = new Yaml2DocEngine(registry, markdownRenderer);

                var markdown = engine.Convert(yamlText, dialectId);

                if (!string.IsNullOrWhiteSpace(outputPath))
                {
                    var outputFullPath = ResolveAndValidatePath(outputPath!, baseDirFull, allowExistingFile: false, stderr);
                    if (outputFullPath is null)
                    {
                        return 3;
                    }

                    var outputDir = Path.GetDirectoryName(outputFullPath)!;
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    using var ofs = new FileStream(outputFullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    using var writer = new StreamWriter(ofs);
                    writer.Write(markdown);
                }
                else
                {
                    stdout.Write(markdown);
                }

                return 0;
            }
            catch (Yaml2DocParseException ex)
            {
                stderr.WriteLine($"Error: {ex.Message}");
                return 3;
            }
            catch (Exception ex)
            {
                stderr.WriteLine("Error: Failed to convert YAML to Markdown.");
                stderr.WriteLine(ex.Message);
                return 3;
            }
        }

        /// <summary>
        /// Writes usage instructions to the specified output.
        /// </summary>
        /// <param name="writer">The output writer to receive usage text.</param>
        private static void PrintUsage(TextWriter writer)
        {
            writer.WriteLine("Usage:");
            writer.WriteLine("  yaml2doc <input.yml>");
            writer.WriteLine("  yaml2doc <input.yml> <output.md>");
            writer.WriteLine("  yaml2doc --dialect <id> <input.yml> [output.md]");
        }

        /// <summary>
        /// Resolves a user-supplied path to a full path and validates it against safety constraints.
        /// </summary>
        /// <param name="path">The path provided by the user (relative or absolute).</param>
        /// <param name="baseDirFull">The allowed base directory full path, typically the current working directory.</param>
        /// <param name="allowExistingFile">
        /// When <see langword="true"/>, existing files are permitted (for input). When <see langword="false"/>, the target
        /// must not already exist (for output) to prevent unintended overwrite.
        /// </param>
        /// <param name="stderr">Error output writer used to report validation failures.</param>
        /// <returns>The validated full path on success; otherwise, <see langword="null"/>.</returns>
        /// <remarks>
        /// Normalizes to a full path, rejects UNC/device paths, ensures containment within the working directory,
        /// forbids control characters, blocks traversal via reparse points, and rejects existing output targets.
        /// </remarks>
        private static string? ResolveAndValidatePath(string path, string baseDirFull, bool allowExistingFile, TextWriter stderr)
        {
            try
            {
                if (IsDevicePath(path))
                {
                    stderr.WriteLine("Error: Device paths are not permitted.");
                    return null;
                }

                var full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(baseDirFull, path));

                if (full.StartsWith(@"\\", StringComparison.Ordinal) || full.StartsWith(@"\\?\", StringComparison.Ordinal))
                {
                    stderr.WriteLine("Error: UNC or device paths are not permitted.");
                    return null;
                }

                if (!full.StartsWith(baseDirFull, StringComparison.OrdinalIgnoreCase))
                {
                    stderr.WriteLine("Error: Path resolves outside the allowed working directory.");
                    return null;
                }

                foreach (var ch in full)
                {
                    if (char.IsControl(ch))
                    {
                        stderr.WriteLine("Error: Path contains invalid characters.");
                        return null;
                    }
                }

                if (TraversesReparsePoint(baseDirFull, full))
                {
                    stderr.WriteLine("Error: Path traverses a reparse point (symlink/junction).");
                    return null;
                }

                if (!allowExistingFile && File.Exists(full))
                {
                    stderr.WriteLine($"Error: Output file already exists: '{full}'.");
                    return null;
                }

                return full;
            }
            catch (Exception ex)
            {
                stderr.WriteLine($"Error: Invalid path '{path}'. {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Determines whether the provided path token refers to a Windows device path (e.g., <c>CON</c>, <c>NUL</c>).
        /// </summary>
        /// <param name="path">A file or directory path.</param>
        /// <returns><see langword="true"/> if the path resolves to a reserved device name; otherwise, <see langword="false"/>.</returns>
        private static bool IsDevicePath(string path)
        {
            var fileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var upper = fileName.ToUpperInvariant();
            return upper is "CON" or "PRN" or "AUX" or "NUL"
                || (upper.StartsWith("COM") && upper.Length == 4 && char.IsDigit(upper[3]))
                || (upper.StartsWith("LPT") && upper.Length == 4 && char.IsDigit(upper[3]));
        }

        /// <summary>
        /// Checks whether any directory segment from <paramref name="baseDirFull"/> to <paramref name="targetFull"/> 
        /// is a reparse point (symlink/junction), which could escape the allowed base directory.
        /// </summary>
        /// <param name="baseDirFull">Base directory full path.</param>
        /// <param name="targetFull">Target full path.</param>
        /// <returns><see langword="true"/> if traversal hits a reparse point; otherwise, <see langword="false"/>.</returns>
        private static bool TraversesReparsePoint(string baseDirFull, string targetFull)
        {
            var baseTrim = baseDirFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (!targetFull.StartsWith(baseTrim, StringComparison.OrdinalIgnoreCase))
            {
                return true; // already outside
            }

            var remainder = targetFull.Substring(baseTrim.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (string.IsNullOrEmpty(remainder))
            {
                return IsReparsePoint(baseTrim);
            }

            var parts = remainder.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            var current = baseTrim;
            foreach (var part in parts)
            {
                current = Path.Combine(current, part);
                if (Directory.Exists(current) && IsReparsePoint(current))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the given path refers to a filesystem entry marked as a reparse point.
        /// </summary>
        /// <param name="path">Full path to test.</param>
        /// <returns><see langword="true"/> if the path is a reparse point; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// On non-Windows platforms, reparse points may not be reported uniformly; failures to read attributes are treated as reparse points.
        /// </remarks>
        private static bool IsReparsePoint(string path)
        {
            try
            {
                var attrs = File.GetAttributes(path);
                return (attrs & FileAttributes.ReparsePoint) != 0;
            }
            catch
            {
                return true;
            }
        }
    }
}
