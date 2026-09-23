import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SiteConnectionSummary {
  id: number;
  moduleName: string;
  displayName: string;
  createdAt: string;
}

export interface AddSiteConnection {
  moduleName: string;
  displayName: string;
  connectionString: string;
}

export interface TestConnectionResult {
  success: boolean;
  error: string | null;
}

const API_BASE = 'http://localhost:5091/api';

@Injectable({ providedIn: 'root' })
export class ConnectionsService {
  constructor(private http: HttpClient) {}

  getModules(): Observable<string[]> {
    return this.http.get<string[]>(`${API_BASE}/modules`);
  }

  getConnections(module: string): Observable<SiteConnectionSummary[]> {
    return this.http.get<SiteConnectionSummary[]>(`${API_BASE}/connections?module=${module}`);
  }

  addConnection(req: AddSiteConnection): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${API_BASE}/connections`, req);
  }

  testConnection(id: number): Observable<TestConnectionResult> {
    return this.http.post<TestConnectionResult>(`${API_BASE}/connections/${id}/test`, {});
  }

  deleteConnection(id: number): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/connections/${id}`);
  }
}
