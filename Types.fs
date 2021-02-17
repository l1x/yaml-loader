namespace YamlLoader

open YamlDotNet.Serialization
open System.Collections.Generic

type Tables =
  CompanyTable | SubsidiaryTable | SiteTable | PodTable

type Pod() =
    [<YamlMember(Alias = "pod_eid")>]
    member val PodEid = "" with get,set
    [<YamlMember(Alias = "pod_name")>]
    member val PodName = "" with get,set
    [<YamlMember(Alias = "pod_type")>]
    member val PodType = "" with get,set
    [<YamlMember(Alias = "active")>]
    member val Active = true with get,set

  type Site() =
    [<YamlMember(Alias = "site_eid")>]
    member val SiteEid = "" with get,set
    [<YamlMember(Alias = "site_name")>]
    member val SiteName = "" with get,set
    [<YamlMember(Alias = "site_image")>]
    member val SiteImage = "" with get,set
    [<YamlMember(Alias = "active")>]
    member val Active  = true with get,set
    [<YamlMember(Alias = "country")>]
    member val Country = "" with get,set
    [<YamlMember(Alias = "zip_code")>]
    member val ZipCode = "" with get,set
    [<YamlMember(Alias = "pods")>]
    member val Pods = List<Pod>() with get,set


  type Subsidiary() =
    [<YamlMember(Alias = "subsidiary_eid")>]
    member val SubsidiaryEid = "" with get,set
    [<YamlMember(Alias = "subsidiary_name")>]
    member val SubsidiaryName = "" with get,set
    [<YamlMember(Alias = "active")>]
    member val Active  = true with get,set
    [<YamlMember(Alias = "business_entity_type")>]
    member val BusinessEntityType = "" with get,set
    [<YamlMember(Alias = "sites")>]
    member val Sites = List<Site>() with get,set


  type Company() =
    [<YamlMember(Alias = "company_eid")>]
    member val CompanyEid = "" with get,set
    [<YamlMember(Alias = "company_name")>]
    member val CompanyName  = "" with get,set
    [<YamlMember(Alias = "active")>]
    member val Active  = true with get,set
    [<YamlMember(Alias = "business_entity_type")>]
    member val BusinessEntityType = "" with get,set
    [<YamlMember(Alias = "subsidiaries")>]
    member val Subsidiaries = List<Subsidiary>() with get,set

  type Companies() =
    member val Companies = new List<Company>() with get,set

  type Fix() =
    [<YamlMember(Alias = "companies_db")>]
    member val CompaniesDb  = Companies() with get,set
