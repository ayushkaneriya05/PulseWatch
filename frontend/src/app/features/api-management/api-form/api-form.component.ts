import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiEndpointService, ApiEndpointResponse } from '../../../core/services/api-endpoint.service';

@Component({
  selector: 'app-api-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './api-form.component.html',
  styleUrl: './api-form.component.css'
})
export class ApiFormComponent implements OnInit {
  isEdit = false;
  apiId = '';
  name = '';
  url = '';
  httpMethod = 'GET';
  checkIntervalSeconds = 60;
  expectedStatusCode = 200;
  description = '';
  error = '';
  loading = false;

  httpMethods = ['GET', 'POST', 'PUT', 'DELETE', 'PATCH', 'HEAD'];
  intervals = [
    { label: '30 seconds', value: 30 },
    { label: '1 minute', value: 60 },
    { label: '5 minutes', value: 300 },
    { label: '10 minutes', value: 600 },
    { label: '30 minutes', value: 1800 }
  ];

  constructor(
    private apiEndpointService: ApiEndpointService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.apiId = id;
      this.apiEndpointService.getById(id).subscribe({
        next: (api: ApiEndpointResponse) => {
          this.name = api.name;
          this.url = api.url;
          this.httpMethod = api.httpMethod;
          this.checkIntervalSeconds = api.checkIntervalSeconds;
          this.expectedStatusCode = api.expectedStatusCode;
          this.description = api.description || '';
          this.cdr.detectChanges();
        }
      });
    }
  }

  onSubmit(): void {
    this.loading = true;
    this.error = '';

    const request = {
      name: this.name,
      url: this.url,
      httpMethod: this.httpMethod,
      checkIntervalSeconds: this.checkIntervalSeconds,
      expectedStatusCode: this.expectedStatusCode,
      description: this.description || undefined
    };

    const action = this.isEdit
      ? this.apiEndpointService.update(this.apiId, request)
      : this.apiEndpointService.create(request);

    action.subscribe({
      next: () => this.router.navigate(['/api-management']),
      error: (err: { error?: { message?: string } }) => {
        this.error = err.error?.message || 'Operation failed. Please try again.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
