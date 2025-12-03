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
    /// The runner performs basic argument parsing and enforces safe file path handling:
    /// - Resolves user-supplied paths to full paths relative to the current working directory.
    /// - Rejects UNC and device paths, and paths resolving outside the working directory.
    /// - Blocks traversal through reparse points (symlinks/junctions) within the working tree.
    /// - Prevents accidental overwrites by requiring non-existent output targets.
    /// This type is public to facilitate unit testing of CLI behavior.
    /// </remarks>
    public static class Yaml2DocCli
    {
        /// <summary>
        /// Entry point for the CLI logic, separated from <c>Program</c> to enable unit testing.
        /// </summary>
        /// <param name="args">
        /// Command-line arguments in the form:
        /// <list type="bullet">
        /// <item><description><c>yaml2doc &lt;input.yml&gt;</c></description></item>
        /// <item><description><c>yaml2doc &lt;input.yml&gt; &lt;output.md&gt;</c></description></item>
        /// <item><description><c>yaml2doc --dialect &lt;id&gt; &lt;input.yml&gt; [output.md]</c></description></item>
        /// </list>
        /// </param>
        /// <param name="stdout">Destination for normal output (Markdown).</param>
        /// <param name="stderr">Destination for error output.</param>
        /// <returns>
        /// Process exit code:
        /// <list type="bullet">
        /// <item><description><c>0</c> on success.</description></item>
        /// <item><description><c>1</c> for usage or argument errors.</description></item>
        /// <item><description><c>2</c> when the input path is invalid or the input file is not found.</description></item>
        /// <item><description><c>3</c> on output path validation failure, conversion error, or I/O failure.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="stdout"/> or <paramref name="stderr"/> is <see langword="null"/>.
        /// </exception>
        public static int Run(string[] args, TextWriter stdout, TextWriter stderr)
        {
            // Simple manual arg parsing:
            // yaml2doc [--dialect <id>] <input.yml> [output.md]
            string? dialectId = null;
            string? inputPath = null;
            string? outputPath = null;

            if (stdout is null) throw new ArgumentNullException(nameof(stdout));
            if (stderr is null) throw new ArgumentNullException(nameof(stderr));

            if (args.Length == 0)
            {
                PrintUsage(stderr);
                return 1;
            }

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (string.Equals(arg, "--dialect", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length)
                    {
                        stderr.WriteLine("Error: --dialect option requires an argument.");
                        PrintUsage(stderr);
                        return 1;
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
                    stderr.WriteLine("Error: Too many arguments.");
                    PrintUsage(stderr);
                    return 1;
                }
            }

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                stderr.WriteLine("Error: Input file path is required.");
                PrintUsage(stderr);
                return 1;
            }

            try
            {
                var baseDir = Directory.GetCurrentDirectory();
                var baseDirFull = Path.GetFullPath(baseDir);

                // Validate input path and open safely to avoid TOCTOU.
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

                // Wire up core services for v1:
                var loader = new YamlLoader();
                var standardDialect = new StandardYamlDialect(loader);
                var registry = new Yaml2DocRegistry(new[] { standardDialect });

                var markdownRenderer = new BasicMarkdownRenderer();
                var engine = new Yaml2DocEngine(registry, markdownRenderer);

                // For v1, dialectId is optional and may be ignored by registry/engine
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
                // Clear message for invalid/malformed YAML.
                // Engine message is already "Failed to parse YAML: ..."
                stderr.WriteLine($"Error: {ex.Message}");
                return 3; // conversion error / invalid YAML
            }
            catch (Exception ex)
            {
                stderr.WriteLine("Error: Failed to convert YAML to Markdown.");
                stderr.WriteLine(ex.Message);
                return 3;
            }
        }

        /// <summary>
        /// Writes usage instructions to the specified <see cref="TextWriter"/>.
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
        /// When <see langword="true"/>, existing files are permitted (e.g., input). When <see langword="false"/>, the target
        /// must not already exist (e.g., output) to prevent unintended overwrite.
        /// </param>
        /// <param name="stderr">Error output writer used to report validation failures.</param>
        /// <returns>
        /// The validated full path on success; otherwise, <see langword="null"/> with an error message written to <paramref name="stderr"/>.
        /// </returns>
        /// <remarks>
        /// Validation rules:
        /// - Normalizes to a full path, resolving relative paths against <paramref name="baseDirFull"/>.
        /// - Rejects UNC and device paths (e.g., <c>\\server\share</c>).
        /// - Ensures the resolved path remains within <paramref name="baseDirFull"/>.
        /// - Rejects control characters within the path string.
        /// - Blocks traversal via reparse points (symlinks/junctions).
        /// - For output targets, rejects paths that already exist to avoid clobbering.
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

                // Normalize to full path relative to base dir when necessary.
                var full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(baseDirFull, path));

                // Reject UNC and device-prefixed full paths.
                if (full.StartsWith(@"\\", StringComparison.Ordinal) || full.StartsWith(@"\\?\",
                        StringComparison.Ordinal))
                {
                    stderr.WriteLine("Error: UNC or device paths are not permitted.");
                    return null;
                }

                // Ensure the path stays within the allowed base directory.
                if (!full.StartsWith(baseDirFull, StringComparison.OrdinalIgnoreCase))
                {
                    stderr.WriteLine("Error: Path resolves outside the allowed working directory.");
                    return null;
                }

                // Basic sanity: forbid control characters.
                foreach (var ch in full)
                {
                    if (char.IsControl(ch))
                    {
                        stderr.WriteLine("Error: Path contains invalid characters.");
                        return null;
                    }
                }

                // Block traversal via reparse points in the path.
                if (TraversesReparsePoint(baseDirFull, full))
                {
                    stderr.WriteLine("Error: Path traverses a reparse point (symlink/junction).");
                    return null;
                }

                // When targeting output, avoid overwriting existing files unless allowed.
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
            // Windows reserved device names without extension
            // e.g., "CON", "PRN", "AUX", "NUL", "COM1".."COM9", "LPT1".."LPT9"
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

            var parts = remainder.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);

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
                // If attributes cannot be read, err on the side of caution.
                return true;
            }
        }
    }
}
