# wUtility — SQL Server Schema Comparison & Sync Tool

A Windows desktop utility (C#, WPF) built to compare two structurally-similar SQL Server databases and bring one in line with the other — tables, columns and data types, indexes, views, stored procedures and functions.

## Background

This tool was built while working as the developer on a Hospital Information System (HIS) deployment team in Iran. Hospitals running the same HIS product often ended up with databases that had drifted apart in schema over time (custom columns, missing indexes, out-of-date views/SPs). Writing manual migration scripts for every site didn't scale, so this tool automated the comparison and sync step during deployments, driven by SQL Server's metadata rather than hand-written diff scripts.

It also grew a handful of extra forms (doctors, price groups, drugs, lab, radiology, blood bank, etc.) to support site-specific configuration tasks that came up repeatedly during rollouts. These were bespoke, single-purpose forms for that specific HIS product rather than a general-purpose framework.

A companion Windows Service (`wService`) supports background/scheduled operations.

## What it does

- Connects to a source and a destination SQL Server database
- Detects structural differences: tables, columns, data types, indexes, views, stored procedures, functions
- Generates and applies the changes needed to bring the destination schema up to date with the source
- Provides a set of hospital-specific configuration forms used during HIS deployments

## Tech stack

- C# / .NET Framework 4.8
- WPF (with WinForms interop)
- ADO.NET (`System.Data.SqlClient`) for all database access
- DevExpress WPF controls and Telerik UI for WPF for the UI
- AES-based local encryption for stored connection settings

## Building

This project references licensed **DevExpress** and **Telerik UI for WPF** component libraries, which are not included in this repository. You'll need valid installations of both (matching the versions referenced in `wUtility.csproj`) for the project to build.

## Note on data

This repository contains only the application source. No real connection strings, credentials, or hospital data are included — any deployment-specific configuration was excluded on purpose.
