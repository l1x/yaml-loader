namespace YamlLoader

open Donald

module Db =

  let createTableCompany  =
    1

  let insertCompany conn (co:Company) =
    dbCommand conn {
      cmdText  "INSERT INTO companies (company_eid, company_name, active, business_entity_type)"
      cmdParam [
        "company_eid", SqlType.String co.CompanyEid
        "company_name", SqlType.String co.CompanyName
        "active", SqlType.Boolean co.Active
        "business_entity_type", SqlType.String co.BusinessEntityType
      ]
    }
    |> DbConn.exec


  let insertSubsidiary conn (subs:Subsidiary) (coEid:string) =

    let companyId =
      dbCommand conn {
        cmdText  "SELECT company_id
                  FROM    companies
                  WHERE   company_eid = @company_eid"
        cmdParam  [ "company_eid", SqlType.String coEid]
      }
      |> DbConn.querySingle (fun s -> s.ReadInt32 "company_id")
      |> Result.map Option.get

    match companyId with
    | Ok coId ->
        dbCommand conn {
          cmdText  "INSERT INTO subsidiaries (subsidiary_eid, subsidiary_name, active, business_entity_type, company_id)"
          cmdParam [
            "subsidiary_eid", SqlType.String subs.SubsidiaryEid
            "subsidiary_name", SqlType.String subs.SubsidiaryName
            "active", SqlType.Boolean subs.Active
            "business_entity_type", SqlType.String subs.BusinessEntityType
            "company_id", SqlType.Int32 coId
          ]
        }
        |> DbConn.exec
        |> ignore
        System.Console.WriteLine(sprintf "Db insert :ok")
    | Error err ->
        System.Console.WriteLine(sprintf "Db error: %A" err)


  let insertSite conn (site:Site) (subsEid:string) =

    let subsidiaryId =
      dbCommand conn {
        cmdText  "SELECT subsidiary_id
                  FROM    subsidiaries
                  WHERE   subsidiary_eid = @subsidiary_eid"
        cmdParam  [ "subsidiary_eid", SqlType.String subsEid]
      }
      |> DbConn.querySingle (fun s -> s.ReadInt32 "company_id")
      |> Result.map Option.get

    match subsidiaryId with
    | Ok subsId ->
        dbCommand conn {
          cmdText  "INSERT INTO sites (site_eid, site_name, site_image, active, country, zip_code, subsidiary_id)"
          cmdParam [
            "site_eid", SqlType.String site.SiteEid
            "site_name", SqlType.String site.SiteName
            "site_image", SqlType.String site.SiteImage
            "active", SqlType.Boolean site.Active
            "country", SqlType.String site.Country
            "zip_code", SqlType.String site.ZipCode
            "subsidiary_id", SqlType.Int32 subsId
          ]
        }
        |> DbConn.exec
        |> ignore
        System.Console.WriteLine(sprintf "Db insert :ok")
    | Error err ->
        System.Console.WriteLine(sprintf "Db error: %A" err)

  let insertPod conn (pod:Pod) (siteEid:string) =

    let siteId =
      dbCommand conn {
        cmdText  "SELECT site_id
                  FROM    sites
                  WHERE   site_eid = @site_eid"
        cmdParam  [ "site_eid", SqlType.String siteEid]
      }
      |> DbConn.querySingle (fun s -> s.ReadInt32 "site_id")
      |> Result.map Option.get

    match siteId with
    | Ok sId ->
        dbCommand conn {
          cmdText  "INSERT INTO sites (pod_eid, pod_name, pod_type, active, site_id)"
          cmdParam [
            "pod_eid", SqlType.String pod.PodEid
            "pod_name", SqlType.String pod.PodName
            "pod_type", SqlType.String pod.PodType
            "active", SqlType.Boolean pod.Active
            "site_id", SqlType.Int32 sId
          ]
        }
        |> DbConn.exec
        |> ignore
        System.Console.WriteLine(sprintf "Db insert :ok")
    | Error err ->
        System.Console.WriteLine(sprintf "Db error: %A" err)
