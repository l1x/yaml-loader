namespace YamlLoader

open System

module Cli =

  let isValidFile f =
    System.IO.File.Exists(f)

  [<StructuredFormatDisplay("YamlFile: {YamlFile}")>]
  type CommandLineOptions =
    { YamlFile: string }


  // create the "helper" recursive function
  let rec private parseCommandLineRec args optionsSoFar =
    //loggerCli.LogInfo(args)
    match args with
    // empty list means we're done.
    | [] ->
        System.Console.WriteLine(sprintf "optionsSoFar %A" optionsSoFar)
        optionsSoFar

    // match yaml
    | "--yaml" :: xs ->
        match xs with
        | yamlFile :: xss ->
            match isValidFile yamlFile with
            | true -> parseCommandLineRec xss { optionsSoFar with YamlFile = yamlFile }
            | false ->
                System.Console.WriteLine(String.Format("Unsupported YamlFile: {0}", yamlFile))
                Environment.Exit 1
                parseCommandLineRec xss optionsSoFar // never reach

        | [] ->
            System.Console.WriteLine(String.Format("YamlFile cannot be empty"))
            Environment.Exit 1
            parseCommandLineRec xs optionsSoFar // never reach
    // handle unrecognized option and keep looping
    | x :: xs ->
        System.Console.WriteLine(String.Format("Option {0} is unrecognized", x))
        parseCommandLineRec xs optionsSoFar

  // create the "public" parse function
  let parseCommandLine args =
    // create the defaults
    let defaultOptions =
      { YamlFile = "hello.yaml" }
    // call the recursive one with the initial options
    parseCommandLineRec args defaultOptions


