# Yaml2DocCli

Provides the command-line entry point logic for converting YAML to Markdown.

**Remarks**

Performs argument parsing and enforces safe file path handling. Rejects UNC/device paths, prevents traversal via reparse points, and avoids overwriting output files. Public for unit testing CLI behavior.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.#ctor(string,string,string,system.nullable[int],string)"></a>
## Method: #ctor(string, string, string, Nullable<int>, string)
Strongly-typed result of CLI argument parsing.

**Parameters**
- `DialectId` — Optional dialect identifier provided via `--dialect <id>`.
- `InputPath` — Input YAML file path as provided on the command line.
- `OutputPath` — Optional output Markdown file path; if `null`, output is written to standard output.
- `ErrorExitCode` — If parsing fails, the corresponding exit code (e.g., `1`); otherwise, `null`.
- `ErrorMessage` — Human-readable error message when parsing fails; otherwise, `null`.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.dialectid"></a>
## Property: DialectId
Optional dialect identifier provided via `--dialect <id>`.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.errorexitcode"></a>
## Property: ErrorExitCode
If parsing fails, the corresponding exit code (e.g., `1`); otherwise.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.errormessage"></a>
## Property: ErrorMessage
Human-readable error message when parsing fails; otherwise.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.inputpath"></a>
## Property: InputPath
Input YAML file path as provided on the command line.

<a id="yaml2doc.cli.yaml2doccli.isdevicepath(string)"></a>
## Method: IsDevicePath(string)
Determines whether the provided path token refers to a Windows device path (e.g., `CON`, `NUL`).

**Parameters**
- `path` — A file or directory path.

**Returns**

if the path resolves to a reserved device name; otherwise,.

<a id="yaml2doc.cli.yaml2doccli.isreparsepoint(string)"></a>
## Method: IsReparsePoint(string)
Determines if the given path refers to a filesystem entry marked as a reparse point.

**Parameters**
- `path` — Full path to test.

**Returns**

if the path is a reparse point; otherwise,.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.outputpath"></a>
## Property: OutputPath
Optional output Markdown file path; if, output is written to standard output.

<a id="yaml2doc.cli.yaml2doccli.parsearguments(string[])"></a>
## Method: ParseArguments(string[])
Parses command-line arguments into dialect, input, and output values without performing filesystem operations.

**Parameters**
- `args` — Raw command-line arguments.

**Returns**

A [ParsedArguments](Yaml2DocCli.ParsedArguments.md) containing parsed values. On error, `ErrorExitCode` and `ErrorMessage` indicate the failure reason and recommended exit code.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown when `args` is.

<a id="yaml2doc.cli.yaml2doccli.printusage(system.io.textwriter)"></a>
## Method: PrintUsage(TextWriter)
Writes usage instructions to the specified output.

**Parameters**
- `writer` — The output writer to receive usage text.

<a id="yaml2doc.cli.yaml2doccli.resolveandvalidatepath(string,string,bool,system.io.textwriter)"></a>
## Method: ResolveAndValidatePath(string, string, bool, TextWriter)
Resolves a user-supplied path to a full path and validates it against safety constraints.

**Parameters**
- `path` — The path provided by the user (relative or absolute).
- `baseDirFull` — The allowed base directory full path, typically the current working directory.
- `allowExistingFile` — When, existing files are permitted (for input). When, the target must not already exist (for output) to prevent unintended overwrite.
- `stderr` — Error output writer used to report validation failures.

**Returns**

The validated full path on success; otherwise,.

<a id="yaml2doc.cli.yaml2doccli.run(string[],system.io.textwriter,system.io.textwriter)"></a>
## Method: Run(string[], TextWriter, TextWriter)
Executes the CLI: validates paths, converts YAML to Markdown, and writes output.

**Parameters**
- `args` — Command-line arguments.
- `stdout` — Destination for normal output (Markdown).
- `stderr` — Destination for error output.

**Returns**

Exit code: `0` success; `1` usage/argument errors; `2` invalid input path or file not found; `3` output path failure, conversion error, or I/O failure.

**Exceptions**
- [ArgumentNullException](System.ArgumentNullException.md) — Thrown if `stdout` or `stderr` is.

<a id="yaml2doc.cli.yaml2doccli.traversesreparsepoint(string,string)"></a>
## Method: TraversesReparsePoint(string, string)
Checks whether any directory segment from `baseDirFull` to `targetFull` is a reparse point (symlink/junction), which could escape the allowed base directory.

**Parameters**
- `baseDirFull` — Base directory full path.
- `targetFull` — Target full path.

**Returns**

if traversal hits a reparse point; otherwise,.

