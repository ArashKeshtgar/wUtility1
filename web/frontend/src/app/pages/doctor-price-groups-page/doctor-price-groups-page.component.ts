import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DoctorsService, DoctorPriceGroup, Work, UpsertDoctorPriceGroup } from '../../services/doctors.service';

// Web port of DoctorsPriceGroupsForm.xaml.cs: full CRUD grid of price groups
// for one doctor (RowGD, DrCode, WCode, CenterSharePrice, Title), joined with
// Doctors/Works for display — same table names, same shape as the original.
@Component({
  selector: 'app-doctor-price-groups-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './doctor-price-groups-page.component.html',
  styleUrl: './doctor-price-groups-page.component.css'
})
export class DoctorPriceGroupsPageComponent implements OnInit {
  drCode = 0;
  drName = '';
  priceGroups: DoctorPriceGroup[] = [];
  works: Work[] = [];
  loading = true;
  error: string | null = null;

  editingRowGd: number | null = null; // null = not editing, 0 = adding new
  editForm: UpsertDoctorPriceGroup = { drCode: 0, wCode: null, centerSharePrice: null, title: null };

  constructor(
    private api: DoctorsService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.drCode = Number(params['drCode']);
      this.drName = params['drName'] ?? '';
      if (this.drCode) this.load();
    });
    this.api.getWorks().subscribe((w) => (this.works = w));
  }

  load(): void {
    this.loading = true;
    this.api.getPriceGroups(this.drCode).subscribe({
      next: (rows) => {
        this.priceGroups = rows;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'بارگذاری ناموفق بود — ' + (err.error?.title || err.message);
        this.loading = false;
      }
    });
  }

  startAdd(): void {
    this.editingRowGd = 0;
    this.editForm = { drCode: this.drCode, wCode: null, centerSharePrice: null, title: null };
  }

  startEdit(row: DoctorPriceGroup): void {
    this.editingRowGd = row.rowGd;
    this.editForm = {
      drCode: row.drCode,
      wCode: row.wCode,
      centerSharePrice: row.centerSharePrice,
      title: row.title
    };
  }

  cancelEdit(): void {
    this.editingRowGd = null;
  }

  save(): void {
    const onSuccess = () => {
      this.editingRowGd = null;
      this.load();
    };
    const onError = (err: { error?: { title?: string }; message: string }) => {
      this.error = 'ذخیره ناموفق بود — ' + (err.error?.title || err.message);
    };

    if (this.editingRowGd === 0) {
      this.api.addPriceGroup(this.editForm).subscribe({ next: onSuccess, error: onError });
    } else {
      this.api.updatePriceGroup(this.editingRowGd!, this.editForm).subscribe({ next: onSuccess, error: onError });
    }
  }

  delete(row: DoctorPriceGroup): void {
    if (!confirm('آیا از حذف مطمئن هستید؟')) return;
    this.api.deletePriceGroup(row.rowGd).subscribe({
      next: () => this.load(),
      error: (err) => {
        this.error = 'حذف ناموفق بود — ' + (err.error?.title || err.message);
      }
    });
  }
}
