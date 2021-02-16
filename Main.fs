namespace YamlLoader

open Cli
open System.IO
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions
open Donald
open Microsoft.Data.Sqlite

module Main =

  [<EntryPoint>]
  let main args =
    let parsed = parseCommandLine (Array.toList args)
    System.Console.WriteLine(sprintf "%A" parsed)

    let fix = System.IO.File.ReadAllText "fix.yaml"

    let deserializer =
      DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build()

    let f = deserializer.Deserialize<Fix>(fix)

    use conn = new SqliteConnection("Data Source=hello.db")


    for co in f.CompaniesDb.Companies do
      // insert company
      Db.insertCompany conn co |> ignore
      for subs in co.Subsidiaries do
        // insert subsidiary
        Db.insertSubsidiary conn subs co.CompanyEid |> ignore
        for site in subs.Sites do
          // insert site
          Db.insertSite conn site subs.SubsidiaryEid |> ignore
          for pod in site.Pods do
            // insert pod
            System.Console.WriteLine(sprintf "%s :: %s :: %s :: %s" co.CompanyEid subs.SubsidiaryEid site.SiteEid pod.PodEid)

    0
