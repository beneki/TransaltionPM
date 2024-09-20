import { Component, OnInit, inject } from '@angular/core';
import { ProjectService } from '../../core/services/project.service';
import { Project } from '../../core/models/project.model';
import { RouterLink } from '@angular/router'
import { StatusPipe } from '../../utils/status-pipe';

@Component({
    standalone: true,
    selector: 'app-dashboard',
    imports: [RouterLink, StatusPipe],
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
    projects: Project[] = [];
    projectService = inject(ProjectService)

    ngOnInit(): void {
        this.projectService.getProjects().subscribe(data => {
            this.projects = data; //(data as any)['$values'];
        });
    }
}
