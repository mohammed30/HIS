import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { PatientService, PatientServicesReportDto, PatientLookupDto } from '@proxy/patients';
import { FormsModule } from '@angular/forms';
import { CoreModule, RestService } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-patient-services-report',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, CoreModule, ThemeSharedModule],
  templateUrl: './patient-services-report.html',
  styleUrl: './patient-services-report.scss'
})
export class PatientServicesReport implements OnInit {
  patientService = inject(PatientService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  restService = inject(RestService);

  patientId: string = '';
  reportData?: PatientServicesReportDto;
  showUnpaidOnly: boolean = false;
  isLoading: boolean = false;

  // Search
  searchQuery = '';
  searchResults: PatientLookupDto[] = [];
  isSearching = false;
  hasSearched = false;

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.patientId) {
      this.loadReport();
    }
  }

  searchPatients(): void {
    if (!this.searchQuery) return;
    this.isSearching = true;
    this.hasSearched = true;
    this.patientService.search(this.searchQuery).subscribe({
      next: (res) => {
        this.searchResults = res;
        this.isSearching = false;
      },
      error: () => {
        this.isSearching = false;
      }
    });
  }

  selectPatient(patientId: string): void {
    this.patientId = patientId;
    // Update URL without reloading component
    this.router.navigate(['/patients', patientId, 'services-report'], { replaceUrl: true });
    this.loadReport();
  }

  loadReport(): void {
    this.isLoading = true;
    this.patientService.getPatientServicesReport(this.patientId, this.showUnpaidOnly).subscribe({
      next: (res) => {
        this.reportData = res;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  onToggleUnpaid(): void {
    this.loadReport();
  }

  print(): void {
    if (!this.patientId) return;
    this.isLoading = true;

    this.restService.request<any, Blob>({
      method: 'GET',
      url: `/api/app/patient/patient-services-report-pdf/${this.patientId}`,
      params: { showUnpaidOnly: this.showUnpaidOnly },
      responseType: 'blob'
    }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        document.body.appendChild(a);
        a.style.display = 'none';
        a.href = url;
        a.download = `PatientServicesReport_${this.patientId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        // Optionally show toaster error here
      }
    });
  }
}

