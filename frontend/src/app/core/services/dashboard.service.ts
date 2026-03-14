import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardSummary {
  totalApis: number;
  healthyApis: number;
  unhealthyApis: number;
  averageResponseTimeMs: number;
}

export interface ApiDetail {
  apiId: string;
  name: string;
  url: string;
  uptimePercentage: number;
  avgResponseTimeMs: number;
  lastStatus: string | null;
  lastCheckedAt: string | null;
  recentLogs: MonitoringLog[];
}

export interface MonitoringLog {
  statusCode: number;
  responseTimeMs: number;
  isSuccess: boolean;
  errorMessage: string | null;
  checkedAt: string;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private apiUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.apiUrl}/summary`);
  }

  getApiDetail(id: string): Observable<ApiDetail> {
    return this.http.get<ApiDetail>(`${this.apiUrl}/api/${id}`);
  }

  getApiLogs(id: string, hours: number = 24): Observable<MonitoringLog[]> {
    return this.http.get<MonitoringLog[]>(`${this.apiUrl}/api/${id}/logs?hours=${hours}`);
  }
}
