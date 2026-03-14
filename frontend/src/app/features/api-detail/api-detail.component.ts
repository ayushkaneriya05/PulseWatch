import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DashboardService, ApiDetail, MonitoringLog } from '../../core/services/dashboard.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-api-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './api-detail.component.html',
  styleUrl: './api-detail.component.css'
})
export class ApiDetailComponent implements OnInit {
  apiDetail: ApiDetail | null = null;
  loading = true;
  apiId = '';

  private responseChart: Chart | null = null;
  private successChart: Chart | null = null;

  constructor(
    private route: ActivatedRoute,
    private dashboardService: DashboardService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.apiId = this.route.snapshot.paramMap.get('id') || '';
    this.loadData();
  }

  loadData(): void {
    this.dashboardService.getApiDetail(this.apiId).subscribe({
      next: (data: ApiDetail) => {
        this.apiDetail = data;
        this.loading = false;
        this.cdr.detectChanges();
        setTimeout(() => this.renderCharts(), 100);
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  getStatusClass(): string {
    if (!this.apiDetail?.lastStatus) return 'status-pending';
    return this.apiDetail.lastStatus.toLowerCase() === 'healthy' ? 'status-healthy' : 'status-unhealthy';
  }

  getUptimeClass(): string {
    if (!this.apiDetail) return '';
    if (this.apiDetail.uptimePercentage >= 99) return 'uptime-good';
    if (this.apiDetail.uptimePercentage >= 95) return 'uptime-warn';
    return 'uptime-bad';
  }

  private renderCharts(): void {
    if (!this.apiDetail || this.apiDetail.recentLogs.length === 0) return;

    const logs = [...this.apiDetail.recentLogs].reverse().slice(-30);

    const rtCanvas = document.getElementById('responseTimeChart') as HTMLCanvasElement;
    if (rtCanvas) {
      if (this.responseChart) this.responseChart.destroy();
      this.responseChart = new Chart(rtCanvas, {
        type: 'line',
        data: {
          labels: logs.map((l: MonitoringLog) => new Date(l.checkedAt).toLocaleTimeString()),
          datasets: [{
            label: 'Response Time (ms)',
            data: logs.map((l: MonitoringLog) => l.responseTimeMs),
            borderColor: '#818cf8',
            backgroundColor: 'rgba(129, 140, 248, 0.1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4,
            pointRadius: 3,
            pointBackgroundColor: '#818cf8'
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            x: { ticks: { color: '#64748b', maxTicksLimit: 8, font: { size: 11 } }, grid: { color: 'rgba(255,255,255,0.04)' } },
            y: { ticks: { color: '#64748b', font: { size: 11 } }, grid: { color: 'rgba(255,255,255,0.04)' } }
          }
        }
      });
    }

    const sfCanvas = document.getElementById('successFailChart') as HTMLCanvasElement;
    if (sfCanvas) {
      const successCount = this.apiDetail.recentLogs.filter((l: MonitoringLog) => l.isSuccess).length;
      const failCount = this.apiDetail.recentLogs.length - successCount;

      if (this.successChart) this.successChart.destroy();
      this.successChart = new Chart(sfCanvas, {
        type: 'doughnut',
        data: {
          labels: ['Success', 'Failed'],
          datasets: [{
            data: [successCount, failCount],
            backgroundColor: ['#34d399', '#f87171'],
            borderWidth: 0,
            hoverOffset: 8
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: {
              position: 'bottom',
              labels: { color: '#94a3b8', padding: 16, font: { size: 12 } }
            }
          }
        }
      });
    }
  }
}
