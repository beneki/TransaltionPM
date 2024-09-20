import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../core/services/project.service';
import { Project } from '../../core/models/project.model';
import { ProjectViewComponent } from './project-view/project-view.component';
import { ProjectEditComponent } from './project-edit/project-edit.component';
import { ProjectTasksComponent } from './project-tasks/project-tasks.component';

@Component({
    standalone: true,
    imports: [ProjectViewComponent, ProjectEditComponent, ProjectTasksComponent],
    selector: 'app-project-detail',
    templateUrl: './project-detail.component.html'
})
export class ProjectDetailComponent implements OnInit {
    id: string | null = null;
    project: Project | undefined;
    isEditing: boolean = false;
    isCreateAJobMode: boolean = false;
    projectService = inject(ProjectService);
    route = inject(ActivatedRoute);

    ngOnInit(): void {
        this.id = this.route.snapshot.paramMap.get('id');
        if (this.id) {
            this.projectService.getProject(this.id).subscribe(data => {
                this.project = data;
            });
        }
    }

    toggleEditMode() {
        this.isEditing = !this.isEditing;
    }


}
