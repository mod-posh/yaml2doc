using System;
using Yaml2Doc.Cli;

return Yaml2DocCli.Run(
    Environment.GetCommandLineArgs()[1..],
    Console.Out,
    Console.Error
);
