import { Routes } from '@angular/router';
import { SchemaSyncPageComponent } from './pages/schema-sync-page/schema-sync-page.component';
import { DoctorsPageComponent } from './pages/doctors-page/doctors-page.component';
import { DoctorPriceGroupsPageComponent } from './pages/doctor-price-groups-page/doctor-price-groups-page.component';
import { DbConfigPageComponent } from './pages/db-config-page/db-config-page.component';
import { ConnectionsPageComponent } from './pages/connections-page/connections-page.component';

export const routes: Routes = [
  { path: '', redirectTo: 'schema-sync', pathMatch: 'full' },
  { path: 'schema-sync', component: SchemaSyncPageComponent },
  { path: 'doctors', component: DoctorsPageComponent },
  { path: 'doctor-price-groups', component: DoctorPriceGroupsPageComponent },
  { path: 'db-config', component: DbConfigPageComponent },
  { path: 'connections', component: ConnectionsPageComponent }
];
