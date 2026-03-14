import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { ApiEndpointService, ApiEndpointResponse } from '../../../core/services/api-endpoint.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-api-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './api-list.component.html',
  styleUrl: './api-list.component.css'
})
export class ApiListComponent implements OnInit {
  apis: ApiEndpointResponse[] = [];
  loading = true;
  userName = '';

  constructor(
    private apiEndpointService: ApiEndpointService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUser()?.name || 'User';
    this.loadApis();
  }

  loadApis(): void {
    this.apiEndpointService.getAll().subscribe({
      next: (data: ApiEndpointResponse[]) => {
        this.apis = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteApi(id: string): void {
    if (confirm('Are you sure you want to delete this API endpoint?')) {
      this.apiEndpointService.delete(id).subscribe({
        next: () => {
          this.apis = this.apis.filter(a => a.id !== id);
          this.cdr.detectChanges();
        }
      });
    }
  }

  getStatusClass(status: string | null): string {
    if (!status) return 'status-pending';
    switch (status.toLowerCase()) {
      case 'healthy': return 'status-healthy';
      case 'unhealthy': return 'status-unhealthy';
      default: return 'status-pending';
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
