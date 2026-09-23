import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Doctor {
  code: number;
  fName: string;
  lName: string;
  name: string;
  postNo: string | null;
}

export interface Work {
  code: number;
  name: string;
}

export interface DoctorPriceGroup {
  rowGd: number;
  drCode: number;
  wCode: number | null;
  centerSharePrice: number | null;
  title: string | null;
  drName: string | null;
  wName: string | null;
}

export interface UpsertDoctorPriceGroup {
  drCode: number;
  wCode: number | null;
  centerSharePrice: number | null;
  title: string | null;
}

const API_BASE = 'http://localhost:5091/api';

@Injectable({ providedIn: 'root' })
export class DoctorsService {
  constructor(private http: HttpClient) {}

  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${API_BASE}/doctors`);
  }

  getWorks(): Observable<Work[]> {
    return this.http.get<Work[]>(`${API_BASE}/works`);
  }

  getPriceGroups(drCode?: number): Observable<DoctorPriceGroup[]> {
    const query = drCode ? `?drCode=${drCode}` : '';
    return this.http.get<DoctorPriceGroup[]>(`${API_BASE}/doctor-price-groups${query}`);
  }

  addPriceGroup(req: UpsertDoctorPriceGroup): Observable<{ rowGd: number }> {
    return this.http.post<{ rowGd: number }>(`${API_BASE}/doctor-price-groups`, req);
  }

  updatePriceGroup(rowGd: number, req: UpsertDoctorPriceGroup): Observable<void> {
    return this.http.put<void>(`${API_BASE}/doctor-price-groups/${rowGd}`, req);
  }

  deletePriceGroup(rowGd: number): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/doctor-price-groups/${rowGd}`);
  }
}
