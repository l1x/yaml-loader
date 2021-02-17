namespace YamlLoader

open YamlDotNet.Serialization
open System.Collections.Generic

type Tables =
  CompanyTable | SubsidiaryTable | SiteTable | PodTable


[<CLIMutable>]
type Pod = {
  PodEid: string
  PodName: string
  PodType: string
  Active: bool
}

[<CLIMutable>]
type Site = {
  SiteEid: string
  SiteName: string
  SiteImage: string
  Active: bool
  Country: string
  ZipCode: string
  Pods: Pod[]
}

[<CLIMutable>]
type Subsidiary = {
  SubsidiaryEid: string
  SubsidiaryName: string
  Active: bool
  BusinessEntityType: string
  Sites: Site[]
}

[<CLIMutable>]
type Company = {
  CompanyEid: string
  CompanyName: string
  Active: bool
  BusinessEntityType: string
  Subsidiaries: Subsidiary[]
}

[<CLIMutable>]
type Companies = {
  Companies: Company[]
}

[<CLIMutable>]
type Fix = {
  CompaniesDb: Companies
}
