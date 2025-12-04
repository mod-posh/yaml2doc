# ParsedArguments

Strongly-typed result of CLI argument parsing.

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.#ctor(string,string,string,system.nullable[int],string)"></a>
## Method: #ctor(string, string, string, Nullable<int>, string)
Strongly-typed result of CLI argument parsing.

**Parameters**
- `DialectId` — Optional dialect identifier provided via `--dialect <id>`.
- `InputPath` — Input YAML file path as provided on the command line.
- `OutputPath` — Optional output Markdown file path; if, output is written to standard output.
- `ErrorExitCode` — If parsing fails, the corresponding exit code (e.g., `1`); otherwise.
- `ErrorMessage` — Human-readable error message when parsing fails; otherwise.

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

<a id="yaml2doc.cli.yaml2doccli.parsedarguments.outputpath"></a>
## Property: OutputPath
Optional output Markdown file path; if, output is written to standard output.

