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
      // create company
      Db.createTable conn CompanyTable |> ignore
      // insert company
      Db.insertCompany conn co |> ignore
      for subs in co.Subsidiaries do
        // create table
        Db.createTable conn SubsidiaryTable |> ignore
        // insert subsidiary
        Db.insertSubsidiary conn subs co.CompanyEid |> ignore
        for site in subs.Sites do
          // create table
          Db.createTable conn SiteTable |> ignore
          // insert site
          Db.insertSite conn site subs.SubsidiaryEid |> ignore
          for pod in site.Pods do
            // create table
            Db.createTable conn PodTable |> ignore
            // insert pod
            Db.insertPod conn pod site.SiteEid |> ignore

            let a =
              if isNull pod.Invoices then
                None
              else
                (Some pod.Invoices)

            System.Console.WriteLine(sprintf "%s :: %s :: %s :: %s :: %A" co.CompanyEid subs.SubsidiaryEid site.SiteEid pod.PodEid a)

    0
