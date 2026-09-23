import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DbConfigService, DbConfigSetting } from '../../services/db-config.service';

// Web port of DatabasesForm.xaml.cs's top grid: a read-only list of
// Tbl_dbConfigSetting rows (registered hospital site databases per module).
// Read-only here too — the original app never writes to this table either;
// see Models/Tbl_dbConfigSetting.cs.
@Component({
  selector: 'app-db-config-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './db-config-page.component.html',
  styleUrl: './db-config-page.component.css'
})
export class DbConfigPageComponent implements OnInit {
  settings: DbConfigSetting[] = [];
  loading = true;
  error: string | null = null;

  constructor(private api: DbConfigService) {}

  ngOnInit(): void {
    this.api.getSettings().subscribe({
      next: (s) => {
        this.settings = s;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'بارگذاری ناموفق بود — ' + (err.error?.title || err.message);
        this.loading = false;
      }
    });
  }
}
