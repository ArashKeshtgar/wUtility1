import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DbConfigSetting {
  fldId: number;
  fldMainDataBase: string;
  fldDataBaseName: string;
  fldDelphiConnectionName: string | null;
  fldPersianDescDataBase: string | null;
  fldIsActive: boolean;
  fldInstanceName: string | null;
  fldSecondInstance: string | null;
}

const API_BASE = 'http://localhost:5091/api';

@Injectable({ providedIn: 'root' })
export class DbConfigService {
  constructor(private http: HttpClient) {}

  getSettings(): Observable<DbConfigSetting[]> {
    return this.http.get<DbConfigSetting[]>(`${API_BASE}/db-config-settings`);
  }
}
