import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ApiEndpointResponse {
  id: string;
  name: string;
  url: string;
  httpMethod: string;
  checkIntervalSeconds: number;
  expectedStatusCode: number;
  description: string | null;
  createdAt: string;
  lastStatus: string | null;
  lastResponseTimeMs: number | null;
  lastCheckedAt: string | null;
}

export interface CreateApiRequest {
  name: string;
  url: string;
  httpMethod: string;
  checkIntervalSeconds: number;
  expectedStatusCode: number;
  description?: string;
}

export interface UpdateApiRequest {
  name: string;
  url: string;
  httpMethod: string;
  checkIntervalSeconds: number;
  expectedStatusCode: number;
  description?: string;
}

@Injectable({ providedIn: 'root' })
export class ApiEndpointService {
  private apiUrl = `${environment.apiUrl}/endpoints`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiEndpointResponse[]> {
    return this.http.get<ApiEndpointResponse[]>(this.apiUrl);
  }

  getById(id: string): Observable<ApiEndpointResponse> {
    return this.http.get<ApiEndpointResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateApiRequest): Observable<ApiEndpointResponse> {
    return this.http.post<ApiEndpointResponse>(this.apiUrl, request);
  }

  update(id: string, request: UpdateApiRequest): Observable<ApiEndpointResponse> {
    return this.http.put<ApiEndpointResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
