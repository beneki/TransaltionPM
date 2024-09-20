import { Component, OnInit, inject, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TaskService } from '../../../core/services/task.service'; // Service to fetch tasks
import { Task } from '../../../core/models/task.model'; // Task model
import { StatusPipe } from '../../../utils/status-pipe';


@Component({
    standalone: true,
    selector: 'app-project-tasks',
    imports: [RouterLink, StatusPipe],
    templateUrl: './project-tasks.component.html'
})
export class ProjectTasksComponent implements OnInit {
    tasks: Task[] = []; // Array to hold task data
    taskService = inject(TaskService); // Inject the task service to fetch tasks

    @Input() projectId!: string; // Decorate projectId as an @Input()

    ngOnInit(): void {
        if (this.projectId) this.fetchAssignedTasks(this.projectId);
    }

    fetchAssignedTasks(projectId: string): void {
        this.taskService.getTasks(projectId).subscribe((data: Task[]) => {
            this.tasks = data; // Store tasks data from API in the tasks array
        });
    }

    trackByTaskId(index: number, task: Task): string {
        return task.id; // Track tasks by unique id
    }
}

