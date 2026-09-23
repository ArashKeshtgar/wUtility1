import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DiffEntry {
  kind: number;
  tableName: string;
  columnName: string | null;
  description: string;
}

export interface CompareResponse {
  diffs: DiffEntry[];
  script: string;
}

export interface ExecuteResult {
  success: boolean;
  executedStatements: string[];
  error: string | null;
}

const API_BASE = 'http://localhost:5091/api';

export const DIFF_KIND_LABELS: Record<number, string> = {
  0: 'جدول موجود نیست',
  1: 'ستون موجود نیست',
  2: 'نوع ستون متفاوت است',
  3: 'nullability متفاوت است'
};

@Injectable({ providedIn: 'root' })
export class SchemaSyncService {
  constructor(private http: HttpClient) {}

  compare(sourceConnectionString: string, targetConnectionString: string): Observable<CompareResponse> {
    return this.http.post<CompareResponse>(`${API_BASE}/compare`, {
      sourceConnectionString,
      targetConnectionString
    });
  }

  execute(targetConnectionString: string, script: string): Observable<ExecuteResult> {
    return this.http.post<ExecuteResult>(`${API_BASE}/execute`, {
      targetConnectionString,
      script
    });
  }
}
