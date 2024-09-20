import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { TranslatorService } from '../../core/services/translator.service';
import { Task } from '../../core/models/task.model';
import { Translator } from '../../core/models/translator.model';
import { FormsModule } from '@angular/forms';
import { StatusPipe } from '../../utils/status-pipe';
import { CommonModule } from '@angular/common';
import { TranslatorsTableComponent } from './../translators-table/translators-table.component'

@Component({
    standalone: true,
    imports: [FormsModule, CommonModule, StatusPipe, TranslatorsTableComponent],
    selector: 'app-task-form',
    templateUrl: './task-form.component.html'
})
export class TaskFormComponent implements OnInit {
    task!: Task;
    translators!: Translator[];
    taskService = inject(TaskService);
    translatorService = inject(TranslatorService);
    route = inject(ActivatedRoute);
    router = inject(Router);
    projectId: string | null = null;
    id: string | null = null;
    mode: string | null = 'edit';
    isVisible = false; // Track visibility

    toggleTable() {
        this.isVisible = !this.isVisible;
    }

    ngOnInit(): void {
        this.task = {
            "title": "",
            "description": "",
            "dueDate": "",
            "status": 0,
            "translator": {
                "firstName": "",
                "lastName": "",
                "email": ""
            }
        } as Task;

        this.route.queryParamMap.subscribe(queryParams => {
            this.projectId = queryParams.get('projectId');
        });

        if (!this.projectId) {  // if is editing a task
            this.id = this.route.snapshot.paramMap.get('taskId');
            if (this.id) {
                this.taskService.getTaskById(this.id).subscribe(data => {
                    this.task = data;
                    this.task.dueDate = this.formatDate(this.task.dueDate)
                });

            }
        }

    }

    updateTrasnlator(translotorId: string): void {
        if (this.projectId) { // if is creating a new task
            this.task.translator.id = translotorId;
        } else {
            this.taskService.updatetaskTranslator(this.task.id, translotorId).subscribe((data) => {
                if (data.isSuccess) {
                    this.task.translator = data.translator;
                }
            });
        }
    }

    postSubmit(data: { isSuccess: boolean, task: Task }) {
        if (data.isSuccess) {
            if (this.projectId) { // if is creating a new task
                this.router.navigate([`project/${this.projectId}`]);
            }
            this.task = data.task;
            this.task.dueDate = this.formatDate(this.task.dueDate);
        }
    }

    onSubmit(): void {
        if (!this.task) return

        if (this.projectId) {  // if is creating a new task
            this.taskService.createTask(this.task, this.projectId, this.task.translator.id).subscribe((data) => {
                this.postSubmit(data);
            });
        } else {
            this.taskService.updateTask(this.task).subscribe((data) => {
                this.postSubmit(data);
            });
        }

    }

    formatDate(dateString: string): string {
        return dateString.split('T')[0]; // Returns YYYY-MM-DD
    }

}
