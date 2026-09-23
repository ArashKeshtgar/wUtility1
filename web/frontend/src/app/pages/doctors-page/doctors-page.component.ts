import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DoctorsService, Doctor } from '../../services/doctors.service';

// Web port of DoctorsForm.xaml.cs: lists doctors (Code, fName, lName, Name,
// PostNo) from the Doctors table. The original's double-click-to-open-price-
// groups behavior becomes a route navigation here.
@Component({
  selector: 'app-doctors-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctors-page.component.html',
  styleUrl: './doctors-page.component.css'
})
export class DoctorsPageComponent implements OnInit {
  doctors: Doctor[] = [];
  loading = true;
  error: string | null = null;

  constructor(private api: DoctorsService, private router: Router) {}

  ngOnInit(): void {
    this.api.getDoctors().subscribe({
      next: (d) => {
        this.doctors = d;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'بارگذاری پزشکان ناموفق بود — ' + (err.error?.title || err.message);
        this.loading = false;
      }
    });
  }

  openPriceGroups(doctor: Doctor): void {
    this.router.navigate(['/doctor-price-groups'], {
      queryParams: { drCode: doctor.code, drName: doctor.name }
    });
  }
}
