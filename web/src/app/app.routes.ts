import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboad/dashboard.component';
import { ProjectDetailComponent } from './components/project-detail/project-detail.component';
import { ReportComponent } from './components/report/report.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import { TranslatorsComponent } from './components/translators/translators.component';

export const routes: Routes = [
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: DashboardComponent },
    { path: 'project/:id', component: ProjectDetailComponent },
    { path: 'report', component: ReportComponent },
    { path: 'task/:taskId', component: TaskFormComponent },
    { path: 'translators', component: TranslatorsComponent, data: { isVisible: true, canChoose: false } }
];
