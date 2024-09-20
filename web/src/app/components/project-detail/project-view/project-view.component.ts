import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Project } from '../../../core/models/project.model';

@Component({
    standalone: true,
    selector: 'app-project-view',
    templateUrl: './project-view.component.html'
})
export class ProjectViewComponent {
    @Input() project: Project | undefined;
    @Output() edit = new EventEmitter<void>();

    onEdit() {
        this.edit.emit(); // Trigger edit mode
    }
}
