# VacationPlanner – Bewerbungsaufgabe (.NET/C# Web App)

## Architektur & Annahmen
Die Anwendung ist als klassische ASP.NET Core MVC Web App umgesetzt (ohne getrenntes Frontend).
- **Backend:** .NET 9, ASP.NET Core MVC, Entity Framework Core
- **Datenbank:** SQLite (lokale Datei `vacationplanner.db`)
- **Domänenmodell:**
  - Employee (int PK, plus ExternalId für Importe)
  - Customer (string PK)
  - Project (string PK, Zeitraum Start/Ende)
  - EmployeeProject (Join-Tabelle für Zuordnungen)
  - VacationRequest (Start/End, Status)
- **IDs:** Für Customer/Project werden string-IDs verwendet, um externe Referenzen aus JSON direkt übernehmen zu können.
- **Status-Enum:** VacationRequestStatus = Submitted, Approved, Rejected, Cancelled
- **Konfliktlogik:** 
  - Beantragung → Vergleich nur mit bereits genehmigten Anträgen anderer Teammitglieder im gleichen Projekt.
  - Genehmigung → Vergleich mit allen Anträgen anderer Teammitglieder im gleichen Projekt (egal ob submitted oder approved).
- **Import:** Insert-only. Duplikate oder fehlerhafte Referenzen werden als Fehler gelistet, valide Datensätze trotzdem übernommen.

---

## Voraussetzungen
- .NET 9 SDK
- Keine externe Datenbank nötig (SQLite-Datei wird automatisch erstellt).
- Tools: optional DB Browser for SQLite für manuelles Debuggen.

---

## Build & Run
Projekt lokal starten:

```bash
dotnet build
dotnet run
````

Die Anwendung läuft standardmäßig auf:
[http://localhost:5075](http://localhost:5075)

---

## Features & Nutzung

### Mitarbeitende

* `/Employees` → Mitarbeitende verwalten (Name, Job Title)

### Kunden & Projekte

* `/Customers` → Kunden anlegen
* `/Projects` → Projekte anlegen (inkl. Zeitraum, Kunde)
* Projekte können Mitarbeitenden über `EmployeeProjects` zugeordnet werden (automatisch beim Import)

### Urlaubsanträge

* `/VacationRequests`
* Anlegen von Anträgen mit Zeitraum, Status
* Bei Beantragung: Konfliktprüfung gegen bereits genehmigte Anträge
* Bei Genehmigung: Konfliktprüfung gegen alle anderen Anträge
* Button „Approve“ auf der Index-Seite

---

## JSON-Import

### UI

* Menüpunkt „Import“ oder direkt `/Import`
* Upload einer JSON-Datei nach vorgegebenem Schema

### Beispiel-Datei

```json
{
  "employees": [
    { "id": "EMPLOYEE_ID_1", "name": "Max Mustermann", "jobTitle": "Developer" },
    { "id": "EMPLOYEE_ID_2", "name": "Anna Schmidt", "jobTitle": "Tester" }
  ],
  "customers": [
    { "id": "CUSTOMER_ID_1", "name": "Acme GmbH" }
  ],
  "projects": [
    {
      "id": "PROJECT_ID_1",
      "customerId": "CUSTOMER_ID_1",
      "period": { "start": "2025-09-01", "end": "2025-12-31" },
      "assignedEmployeeIds": [ "EMPLOYEE_ID_1", "EMPLOYEE_ID_2" ]
    }
  ]
}
```

### Verhalten

* **Insert-only:** Duplikate (z. B. gleicher Employee-Id) werden als Fehler gemeldet, Import läuft für valide Datensätze weiter.
* Validierungen:

  * Customer muss existieren, bevor ein Project importiert wird
  * Employees müssen für Assignments existieren
  * Project-Zeiträume: Start <= End
* Ergebnis-Seite zeigt Zähler (Employees, Customers, Projects, Assignments) und Liste der Fehler

---

## Getroffene Annahmen

* Kein Upsert: bestehende Entities werden nicht überschrieben, sondern als Fehler ausgegeben.
* Keine Authentifizierung/Rollen → Fokus liegt auf Kernlogik
* Konfliktprüfung nur über einfache Überschneidungslogik (A.Start <= B.End && B.Start <= A.End)

---



