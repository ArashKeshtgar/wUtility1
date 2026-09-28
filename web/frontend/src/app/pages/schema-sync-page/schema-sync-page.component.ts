import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SchemaSyncService, CompareResponse, ExecuteResult, DIFF_KIND_LABELS } from './schema-sync.service';
import { DemoService, DemoInfo } from '../../services/demo.service';

@Component({
  selector: 'app-schema-sync-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './schema-sync-page.component.html',
  styleUrl: './schema-sync-page.component.css'
})
export class SchemaSyncPageComponent implements OnInit {
  sourceConnectionString = 'Server=(local);Database=SchemaSyncDemo_Source;Trusted_Connection=True;TrustServerCertificate=True;';
  targetConnectionString = 'Server=(local);Database=SchemaSyncDemo_Target;Trusted_Connection=True;TrustServerCertificate=True;';

  comparing = false;
  executing = false;
  resetting = false;
  demo: DemoInfo | null = null;
  result: CompareResponse | null = null;
  executeResult: ExecuteResult | null = null;
  error: string | null = null;

  diffLabels = DIFF_KIND_LABELS;

  constructor(private api: SchemaSyncService, private demoApi: DemoService) {}

  ngOnInit(): void {
    this.demoApi.info$.subscribe((info) => (this.demo = info));
  }

  // Demo only: puts the target back to its drifted starting schema so the
  // compare -> apply flow can be tried again.
  resetDemo(): void {
    this.resetting = true;
    this.error = null;
    this.executeResult = null;
    this.demoApi.reset().subscribe({
      next: () => {
        this.resetting = false;
        this.compare();
      },
      error: (err) => {
        this.error = 'بازنشانی ناموفق بود — ' + errorText(err);
        this.resetting = false;
      }
    });
  }

  // After a successful apply the page re-compares to show the target is now
  // in line; that re-compare keeps the apply result on screen.
  compare(keepExecuteResult = false): void {
    this.comparing = true;
    this.error = null;
    if (!keepExecuteResult) this.executeResult = null;
    this.api.compare(this.sourceConnectionString, this.targetConnectionString).subscribe({
      next: (res) => {
        this.result = res;
        this.comparing = false;
      },
      error: (err) => {
        this.error = 'مقایسه ناموفق بود — ' + errorText(err);
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
        if (res.success) this.compare(true);
      },
      error: (err) => {
        this.error = 'اجرای اسکریپت ناموفق بود — ' + errorText(err);
        this.executing = false;
      }
    });
  }
}

// API errors come as { error } (ours), { title } (ASP.NET problem details),
// or plain HTTP failures such as 429 from the rate limiter.
function errorText(err: any): string {
  if (err.status === 429) return 'درخواست‌ها زیاد بود، یک دقیقه بعد دوباره امتحان کنید.';
  return err.error?.error || err.error?.title || err.message;
}
