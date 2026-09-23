import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SchemaSyncService, CompareResponse, ExecuteResult, DIFF_KIND_LABELS } from './schema-sync.service';

@Component({
  selector: 'app-schema-sync-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './schema-sync-page.component.html',
  styleUrl: './schema-sync-page.component.css'
})
export class SchemaSyncPageComponent {
  sourceConnectionString = 'Server=(local);Database=SchemaSyncDemo_Source;Trusted_Connection=True;TrustServerCertificate=True;';
  targetConnectionString = 'Server=(local);Database=SchemaSyncDemo_Target;Trusted_Connection=True;TrustServerCertificate=True;';

  comparing = false;
  executing = false;
  result: CompareResponse | null = null;
  executeResult: ExecuteResult | null = null;
  error: string | null = null;

  diffLabels = DIFF_KIND_LABELS;

  constructor(private api: SchemaSyncService) {}

  compare(): void {
    this.comparing = true;
    this.error = null;
    this.executeResult = null;
    this.api.compare(this.sourceConnectionString, this.targetConnectionString).subscribe({
      next: (res) => {
        this.result = res;
        this.comparing = false;
      },
      error: (err) => {
        this.error = 'مقایسه ناموفق بود — ' + (err.error?.title || err.message);
        this.comparing = false;
      }
    });
  }

  applyScript(): void {
    if (!this.result) return;
    this.executing = true;
    this.api.execute(this.targetConnectionString, this.result.script).subscribe({
      next: (res) => {
        this.executeResult = res;
        this.executing = false;
        if (res.success) this.compare();
      },
      error: (err) => {
        this.error = 'اجرای اسکریپت ناموفق بود — ' + (err.error?.title || err.message);
        this.executing = false;
      }
    });
  }
}
