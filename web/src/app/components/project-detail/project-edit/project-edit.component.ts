import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { Project } from '../../../core/models/project.model';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../core/services/project.service';

@Component({
    standalone: true,
    imports: [FormsModule],
    selector: 'app-project-edit',
    templateUrl: './project-edit.component.html'
})
export class ProjectEditComponent {
    @Input() project!: Project;
    @Output() save = new EventEmitter<Project>();
    projectService = inject(ProjectService);

    saveProject() {
        console.log(this.project.name)
        this.projectService.updateProject(this.project).subscribe(() => {
            this.save.emit();
        });
    }
}
