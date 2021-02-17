namespace YamlLoader

open Donald

module Db =

  let handleInsert insert =
    match insert with
    | Ok ok ->
        System.Console.WriteLine(sprintf "Inserting into table was successful %A" ok)
    | Error err ->
        System.Console.WriteLine(sprintf "Inserting into table error: %A" err)
        System.Environment.Exit(1)

  let createTableCompany =
    "CREATE TABLE IF NOT EXISTS companies(
      company_id INTEGER PRIMARY KEY,
      company_eid TEXT NOT NULL UNIQUE,
      company_name TEXT NOT NULL,
      active INTEGER NOT NULL,
      business_entity_type TEXT NOT NULL
    );"


  let createTableSubsidiary =
    "CREATE TABLE IF NOT EXISTS subsidiaries(
      subsidiary_id INTEGER PRIMARY KEY,
      subsidiary_eid TEXT NOT NULL UNIQUE,
      subsidiary_name TEXT NOT NULL,
      active INTEGER NOT NULL,
      business_entity_type TEXT NOT NULL,
      company_id INTEGER NOT NULL,
      FOREIGN KEY (company_id) REFERENCES companies (company_id)
    );"


  let createTableSite =
    "CREATE TABLE IF NOT EXISTS sites(
      site_id INTEGER PRIMARY KEY,
      site_eid TEXT NOT NULL UNIQUE,
      site_name TEXT NOT NULL,
      site_image TEXT NOT NULL,
      active INTEGER NOT NULL,
      country TEXT NOT NULL,
      zip_code TEXT NOT NULL,
      subsidiary_id INTEGER NOT NULL,
      FOREIGN KEY (subsidiary_id) REFERENCES subsidiaries (subsidiary_id)
    );"


  let createTablePod =
    "CREATE TABLE IF NOT EXISTS pods(
      pod_id INTEGER PRIMARY KEY,
      pod_eid TEXT NOT NULL UNIQUE,
      pod_name TEXT NOT NULL,
      pod_type TEXT NOT NULL,
      active INTEGER NOT NULL,
      site_id INTEGER NOT NULL,
      FOREIGN KEY (site_id) REFERENCES sites (site_id)
    );"


  let createTable conn (tables:Tables) =

    let createTable =
      match tables with
      | CompanyTable -> createTableCompany
      | SubsidiaryTable -> createTableSubsidiary
      | SiteTable -> createTableSite
      | PodTable -> createTablePod

    try
      let createTable =
        dbCommand conn {
          cmdText  createTable
          cmdParam [ ]
        }
        |> DbConn.exec
      match createTable with
      | Ok ok ->
          System.Console.WriteLine(sprintf "Creating table was successful %A" ok)
      | Error err ->
          System.Console.WriteLine(sprintf "Create table error: %A" err)
          System.Environment.Exit(1)
    with ex ->
      System.Console.WriteLine(sprintf "Create table: %A" ex.Message)
      System.Environment.Exit(1)


  let insertCompany conn (co:Company) =
    try
      let insert =
        dbCommand conn {
          cmdText
            "INSERT OR IGNORE INTO
              companies (company_eid, company_name, active, business_entity_type)
             VALUES
              (@company_eid, @company_name, @active, @business_entity_type)"
          cmdParam [
            "company_eid", SqlType.String co.CompanyEid
            "company_name", SqlType.String co.CompanyName
            "active", SqlType.Boolean co.Active
            "business_entity_type", SqlType.String co.BusinessEntityType
          ]
        }
        |> DbConn.exec
      // return or exit
      handleInsert insert
    with ex ->
      System.Console.WriteLine(sprintf "Inserting into table: %A" ex.Message)
      System.Environment.Exit(1)


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
        let insert =
          dbCommand conn {
            cmdText
              "INSERT OR IGNORE INTO subsidiaries
                (subsidiary_eid, subsidiary_name, active, business_entity_type, company_id)
              VALUES
                (@subsidiary_eid, @subsidiary_name, @active, @business_entity_type, @company_id)"
            cmdParam [
              "subsidiary_eid", SqlType.String subs.SubsidiaryEid
              "subsidiary_name", SqlType.String subs.SubsidiaryName
              "active", SqlType.Boolean subs.Active
              "business_entity_type", SqlType.String subs.BusinessEntityType
              "company_id", SqlType.Int32 coId
            ]
          }
          |> DbConn.exec
        // return or exit
        handleInsert insert
    | Error err ->
        System.Console.WriteLine(sprintf "Db error: %A" err)


  let insertSite conn (site:Site) (subsEid:string) =
    System.Console.WriteLine(sprintf "Db insert %A %A" site subsEid)
    let subsidiaryId =
      dbCommand conn {
        cmdText  "SELECT subsidiary_id
                  FROM    subsidiaries
                  WHERE   subsidiary_eid = @subsidiary_eid"
        cmdParam  [ "subsidiary_eid", SqlType.String subsEid]
      }
      |> DbConn.querySingle (fun s -> s.ReadInt32 "subsidiary_id")
      |> Result.map Option.get

    match subsidiaryId with
    | Ok subsId ->
        let insert =
          dbCommand conn {
            cmdText
              "INSERT OR IGNORE INTO sites
                (site_eid, site_name, site_image, active, country, zip_code, subsidiary_id)
              VALUES
                (@site_eid, @site_name, @site_image, @active, @country, @zip_code, @subsidiary_id)"
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
        // return or exit
        handleInsert insert
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
        let insert =
          dbCommand conn {
            cmdText
              "INSERT OR IGNORE INTO pods
                (pod_eid, pod_name, pod_type, active, site_id)
              VALUES
                (@pod_eid, @pod_name, @pod_type, @active, @site_id)"
            cmdParam [
              "pod_eid", SqlType.String pod.PodEid
              "pod_name", SqlType.String pod.PodName
              "pod_type", SqlType.String pod.PodType
              "active", SqlType.Boolean pod.Active
              "site_id", SqlType.Int32 sId
            ]
          }
          |> DbConn.exec
        // return or exit
        handleInsert insert
    | Error err ->
        System.Console.WriteLine(sprintf "Db error: %A" err)
