# VacationPlanner

Autor: Leon K.
Stand: 31.08.2025

KURZBESCHREIBUNG
----------------
Kleine, vollständige Web-App zur Verwaltung von Urlaubsanträgen in Projektteams.
Fokus der Lösung: klare Domänenmodelle, konfliktbewusste Urlaubslogik, JSON-Import.

TECHNOLOGIE & ARCHITEKTUR (kurz)
--------------------------------
- Stack: ASP.NET Core MVC (.NET 9), Entity Framework Core, SQLite
- Aufbau:
  - MVC-Controller & Razor Views (UI)
  - Domain-Modelle + DbContext (EF Core)
  - Services/ImportService (JSON-Import, Insert-only)
- Datenbank: lokale Datei "vacationplanner.db" (wird automatisch erstellt)
- IDs:
  - Employee: interner int-PK + optionale "ExternalId" (für Importe; UNIQUE gefiltert)
  - Customer & Project: string-IDs (1:1 aus der Importdatei übernehmbar)

DOMÄNENMODELLE
--------------
- Employee(Id:int, ExternalId?:string, Name, JobTitle)
- Customer(Id:string, Name)
- Project(Id:string, CustomerId, Start:DateOnly, End:DateOnly)
- EmployeeProject(EmployeeId:int, ProjectId:string) – Join-Tabelle (PK: EmployeeId+ProjectId)
- VacationRequest(Id:int, EmployeeId, ProjectId, Start:DateOnly, End:DateOnly, Status)

STATUSDEFINITION (VacationRequestStatus)
---------------------------------------
- Submitted  – eingereicht/offen
- Approved   – genehmigt
- Rejected   – abgelehnt
- Cancelled  – storniert

KONFLIKTREGELN
--------------
- Beantragung/Änderung: Vergleiche NUR mit bereits **genehmigten** Anträgen anderer Teammitglieder
  im **gleichen Projekt** und **überlappendem Zeitraum**.
- Genehmigung: Vergleiche mit **allen** Anträgen anderer Teammitglieder im gleichen Projekt
  und überlappendem Zeitraum (egal ob submitted/approved/etc.).
- Überschneidung: A.Start <= B.End && B.Start <= A.End
- Ein Antrag ist nur gültig, wenn der Mitarbeitende dem Projekt zugeordnet ist.

VORAUSSETZUNGEN
---------------
- .NET 9 SDK
- Keine zusätzliche DB-Installation nötig (SQLite-Datei wird erstellt)
- Optional: DB Browser for SQLite (zum Reinschauen)

SCHNELLSTART
------------
1) Build & Run:
    dotnet build
    dotnet ef database update
    dotnet run

    Standard-URL: http://localhost:5075

2) (Falls notwendig) Datenbank neu aufsetzen:
    dotnet ef database drop
    dotnet ef database update


HAUPT-FUNKTIONEN / ROUTEN
-------------------------
- Mitarbeitende:       "/Employees"
- Kunden:              "/Customers"
- Projekte:            "/Projects"
- Urlaubsanträge:      "/VacationRequests"
- Create (Submitted), Approve, Reject, Cancel
- Konfliktprüfungen gemäß Regeln oben
- JSON-Import (UI):    "/Import"

JSON-IMPORT
-----------
- Start: Menü „Import“ oder "/Import" aufrufen und eine JSON-Datei hochladen.
- Verhalten: **Insert-only** (kein Upsert). Duplikate/fehlerhafte Referenzen werden
als Fehler auf der Ergebnis-Seite gelistet; gültige Datensätze werden trotzdem gespeichert.
- Validierungen:
- "project.customerId" muss existieren
- "assignedEmployeeIds" müssen auf importierte Employees verweisen
- "period.start <= period.end"
- "Employee.ExternalId" ist optional, aber wenn gesetzt, eindeutig (UNIQUE gefiltert)

- Beispiel-JSON:

{
   "employees":[
      {
         "id":"EMPLOYEE_ID_1",
         "name":"Max Mustermann",
         "jobTitle":"Developer"
      },
      {
         "id":"EMPLOYEE_ID_2",
         "name":"Anna Schmidt",
         "jobTitle":"Tester"
      }
   ],
   "customers":[
      {
         "id":"CUSTOMER_ID_1",
         "name":"Acme GmbH"
      }
   ],
   "projects":[
      {
         "id":"PROJECT_ID_1",
         "customerId":"CUSTOMER_ID_1",
         "period":{
            "start":"2025-09-01",
            "end":"2025-12-31"
         },
         "assignedEmployeeIds":[
            "EMPLOYEE_ID_1",
            "EMPLOYEE_ID_2"
         ]
      }
   ]
}


PROJEKTSTRUKTUR (vereinfachter Überblick)
-----------------------------------------
- "Controllers/"
- "EmployeesController", "CustomersController", "ProjectsController"
- "VacationRequestsController" (Create + Konfliktprüfung, Approve/Reject/Cancel)
- "ImportController" (Upload-UI → ImportService)
- "Models/"
- Domänenklassen (Employee, Customer, Project, EmployeeProject, VacationRequest, Enum)
- Import-DTOs ("ImportPayload", "ImportEmployee", "ImportCustomer", "ImportProject", "ImportPeriod")
- "Services/"
- "ImportService" (liest JSON, validiert, Insert-only, Fehlerliste)
- "Data/"
- "AppDbContext" (DbSets, Mappings, Indizes/Constraints)
- "Views/"
- Razor Views für oben genannte Controller
- "Views/Import/Index.cshtml" (Upload), "Views/Import/Result.cshtml" (Ergebnis)
- "Migrations/"
- EF Core Migrationen (inkl. gefiltertem UNIQUE-Index auf "Employee.ExternalId")

KONFIGURATION
-------------
- Connection String: in "Program.cs" per
"UseSqlite("Data Source=vacationplanner.db")".
- Standardpfade/Ports: ASP.NET Core Defaults; angepasst über "launchSettings.json" möglich.

TROUBLESHOOTING
---------------
- **„Pending model changes“** beim "database update":
→ neue Migration anlegen: "dotnet ef migrations add <Name>" → dann "dotnet ef database update"
- **UNIQUE constraint auf "Employees.ExternalId"**:
→ "ExternalId" ist optional (nullable) und via gefiltertem Index UNIQUE nur, wenn gesetzt.
  Falls Altbestände mit leerem String existieren: auf NULL migrieren.
- **FK-Fehler beim Anlegen eines Projects**:
→ "CustomerId" muss existieren (Dropdown statt Freitext verwenden).
- **Konfliktmeldung beim Create/Approve**:
→ gewollt: zeigt konfligierende Anträge mit Name und Zeitraum.

GETROFFENE ENTSCHEIDUNGEN 
--------------------------------
- String-IDs für Customer/Project (direkter Import ohne Mapping)
- Import v1 = **Insert-only** (klar, robust; Upsert als Option für spätere Deltas)
- Konfliktlogik in C# (testbar/lesbar), DB bleibt generisch
- Monolithische Web-App (einfacher Start), saubere Services → später erweiterbar

MITARBEIT & LOKALE TESTDATEN
----------------------------
- Für schnelle Tests: JSON-Import unter "/Import" verwenden.
- Alternativ: Entities manuell über die jeweiligen CRUD-Seiten anlegen.

ENDE
----
Diese README beschreibt Teil A vollständig (Implementierung inkl. Import und Konfliktregeln).
