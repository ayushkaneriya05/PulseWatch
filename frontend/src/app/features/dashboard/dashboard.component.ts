import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService, DashboardSummary } from '../../core/services/dashboard.service';
import { ApiEndpointService, ApiEndpointResponse } from '../../core/services/api-endpoint.service';
import { AlertService, AlertRecord } from '../../core/services/alert.service';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  summary: DashboardSummary | null = null;
  apis: ApiEndpointResponse[] = [];
  alerts: AlertRecord[] = [];
  loading = true;
  userName = '';
  private refreshInterval: ReturnType<typeof setInterval> | null = null;

  constructor(
    private dashboardService: DashboardService,
    private apiEndpointService: ApiEndpointService,
    private alertService: AlertService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const user = this.authService.getUser();
    this.userName = user?.name || 'User';
    this.loadData();
    this.refreshInterval = setInterval(() => this.loadData(), 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
  }

  loadData(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data: DashboardSummary) => {
        this.summary = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
    this.apiEndpointService.getAll().subscribe({
      next: (data: ApiEndpointResponse[]) => {
        this.apis = data;
        this.cdr.detectChanges();
      }
    });
    this.alertService.getAlerts().subscribe({
      next: (data: AlertRecord[]) => {
        this.alerts = data;
        this.cdr.detectChanges();
      }
    });
  }

  getStatusClass(status: string | null): string {
    if (!status) return 'status-pending';
    switch (status.toLowerCase()) {
      case 'healthy': return 'status-healthy';
      case 'unhealthy': return 'status-unhealthy';
      default: return 'status-pending';
    }
  }

  dismissAlert(id: string): void {
    this.alertService.dismiss(id).subscribe({
      next: () => {
        this.alerts = this.alerts.filter(a => a.id !== id);
        this.cdr.detectChanges();
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
