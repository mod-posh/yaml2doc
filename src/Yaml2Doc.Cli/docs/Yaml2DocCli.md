# Yaml2DocCli

Provides the command-line entry point logic for converting YAML to Markdown.

**Remarks**

The runner performs basic argument parsing and enforces safe file path handling: - Resolves user-supplied paths to full paths relative to the current working directory. - Rejects UNC and device paths, and paths resolving outside the working directory. - Blocks traversal through reparse points (symlinks/junctions) within the working tree. - Prevents accidental overwrites by requiring non-existent output targets. This type is public to facilitate unit testing of CLI behavior.

<a id="yaml2doc.cli.yaml2doccli.printusage(system.io.textwriter)"></a>
## Method: PrintUsage(TextWriter)
Writes usage instructions to the specified [TextWriter](System.IO.TextWriter.md).

**Parameters**
- `writer` — The output writer to receive usage text.

<a id="yaml2doc.cli.yaml2doccli.resolveandvalidatepath(string,string,bool,system.io.textwriter)"></a>
## Method: ResolveAndValidatePath(string, string, bool, TextWriter)
Resolves a user-supplied path to a full path and validates it against safety constraints.

**Parameters**
- `path` — The path provided by the user (relative or absolute).
- `baseDirFull` — The allowed base directory full path, typically the current working directory.
- `allowExistingFile` — When, existing files are permitted (e.g., input). When, the target must not already exist (e.g., output) to prevent unintended overwrite.
- `stderr` — Error output writer used to report validation failures.

**Returns**

The validated full path on success; otherwise, with an error message written to `stderr`.

<a id="yaml2doc.cli.yaml2doccli.run(string[],system.io.textwriter,system.io.textwriter)"></a>
## Method: Run(string[], TextWriter, TextWriter)
Entry point for the CLI logic, separated from `Program` to enable unit testing.

**Parameters**
- `args` — Command-line arguments in the form:

yaml2doc <input.yml> yaml2doc <input.yml> <output.md> yaml2doc --dialect <id> <input.yml> [output.md]
- `stdout` — Destination for normal output (Markdown).
- `stderr` — Destination for error output.

**Returns**

Process exit code:

0 on success. 1 for usage or argument errors. 2 when the input path is invalid or the input file is not found. 3 on output path validation failure, conversion error, or I/O failure.

