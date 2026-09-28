import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of, shareReplay } from 'rxjs';

export interface DemoInfo {
  enabled: boolean;
  sourceDatabase: string | null;
  targetDatabase: string | null;
}

const API_BASE = '/api';

// The public Azure deployment runs the API in demo mode (see api/DemoMode.cs):
// schema sync is fixed to two sandbox databases, only the generated script
// can be applied, and the connection vault is read-only. Pages ask once and
// adapt their UI; the API enforces the same rules regardless.
@Injectable({ providedIn: 'root' })
export class DemoService {
  readonly info$: Observable<DemoInfo>;

  constructor(private http: HttpClient) {
    this.info$ = this.http.get<DemoInfo>(`${API_BASE}/demo`).pipe(
      catchError(() => of({ enabled: false, sourceDatabase: null, targetDatabase: null })),
      shareReplay(1)
    );
  }

  reset(): Observable<void> {
    return this.http.post<void>(`${API_BASE}/demo/reset`, {});
  }
}
