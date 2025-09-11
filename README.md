# Jira Connector

Dieses Projekt dient als Schnittstelle zu **Jira Agile Projekten**.  
Es ruft Daten über die Jira REST API ab und bereitet diese in übersichtlicher Form für Dashboards oder Auswertungen auf.  

## Features
- **Abruf von Tickets** aus einem agilen Projekt (Scrum / Kanban) über die Jira API  
- **Filterung nach Status und Typ**  
  - Status: To Do, In Progress, Done  
  - Typ: Story, Task, Bug  
- **Summierung der Story Points** pro Sprint und Status/Typ  
- **Velocity-Berechnung**  
  - Velocity jedes einzelnen Sprints  
  - Durchschnittliche Velocity der letzten drei abgeschlossenen Sprints  
- **Unterstützung für mehrere Projekte**  

## Technologie-Stack
- **Backend:** C# mit ASP.NET Core (API-Endpunkte)  
- **Frontend:** Angular (Dashboard mit Visualisierungen)  
- **Datenquelle:** Jira REST API (mit Token-Authentifizierung)  

## Beispiel-Metriken
- Anzahl der Tickets je Status (To Do, In Progress, Done)  
- Anzahl der Tickets je Typ (Story, Task, Bug)  
- Velocity-Entwicklung über mehrere Sprints  
- Durchschnittliche Velocity der letzten 3 Sprints  

## Installation & Setup
1. Repository klonen:
   ```bash
   git clone https://github.com/<DEIN-USERNAME>/jira-connector.git
