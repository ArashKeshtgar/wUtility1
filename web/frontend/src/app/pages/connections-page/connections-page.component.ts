import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ConnectionsService,
  SiteConnectionSummary,
  AddSiteConnection
} from '../../services/connections.service';

// Web port of Connections.xaml.cs — but a real server-side vault instead of
// the original's per-module local encrypted app settings. Connection strings
// are never sent back from the API after being saved; only success/failure
// from the "test" action and metadata (module, display name, date) ever
// reach this page.
@Component({
  selector: 'app-connections-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './connections-page.component.html',
  styleUrl: './connections-page.component.css'
})
export class ConnectionsPageComponent implements OnInit {
  modules: string[] = [];
  selectedModule = '';
  connections: SiteConnectionSummary[] = [];
  loading = false;
  error: string | null = null;

  adding = false;
  newConnection: AddSiteConnection = { moduleName: '', displayName: '', connectionString: '' };

  testResults: Record<number, string> = {};

  constructor(private api: ConnectionsService) {}

  ngOnInit(): void {
    this.api.getModules().subscribe((modules) => {
      this.modules = modules;
      this.selectedModule = modules[0];
      this.load();
    });
  }

  load(): void {
    this.loading = true;
    this.api.getConnections(this.selectedModule).subscribe({
      next: (rows) => {
        this.connections = rows;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'بارگذاری ناموفق بود — ' + (err.error?.title || err.message);
        this.loading = false;
      }
    });
  }

  onModuleChange(): void {
    this.testResults = {};
    this.load();
  }

  startAdd(): void {
    this.adding = true;
    this.newConnection = { moduleName: this.selectedModule, displayName: '', connectionString: '' };
  }

  cancelAdd(): void {
    this.adding = false;
  }

  save(): void {
    this.api.addConnection(this.newConnection).subscribe({
      next: () => {
        this.adding = false;
        this.load();
      },
      error: (err) => {
        this.error = 'ذخیره ناموفق بود — ' + (err.error?.title || err.message);
      }
    });
  }

  test(conn: SiteConnectionSummary): void {
    this.testResults[conn.id] = 'در حال تست…';
    this.api.testConnection(conn.id).subscribe((res) => {
      this.testResults[conn.id] = res.success ? '✅ موفق' : '❌ ' + res.error;
    });
  }

  remove(conn: SiteConnectionSummary): void {
    if (!confirm(`آیا از حذف «${conn.displayName}» مطمئن هستید؟`)) return;
    this.api.deleteConnection(conn.id).subscribe(() => this.load());
  }
}
