import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AlertRecord {
  id: string;
  apiEndpointId: string;
  alertType: string;
  message: string;
  isDismissed: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class AlertService {
  private apiUrl = `${environment.apiUrl}/alerts`;

  constructor(private http: HttpClient) {}

  getAlerts(): Observable<AlertRecord[]> {
    return this.http.get<AlertRecord[]>(this.apiUrl);
  }

  dismiss(id: string): Observable<{message: string}> {
    return this.http.post<{message: string}>(`${this.apiUrl}/${id}/dismiss`, {});
  }
}
