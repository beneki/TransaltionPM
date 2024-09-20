import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task } from '../models/task.model';
import { Translator } from '../models/translator.model';

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    http = inject(HttpClient);
    domain: string = 'Tasks';

    createTask(task: Task, projectId: string, translatorId: string): Observable<{ isSuccess: boolean, task: Task }> {
        return this.http.post<{ isSuccess: boolean, task: Task }>(this.domain, { task: { ...task, projectId }, translatorId });
    }

    getTasks(taskId: string): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.domain}?${taskId}`);
    }

    getTaskById(id: string): Observable<Task> {
        return this.http.get<Task>(`${this.domain}/${id}`);
    }

    updateTask(task: Task): Observable<{ isSuccess: boolean, task: Task }> {
        return this.http.put<{ isSuccess: boolean, task: Task }>(`${this.domain}/${task.id}`, task);
    }

    updatetaskTranslator(taskId: string, translatorId: string): Observable<{ isSuccess: boolean, translator: Translator }> {
        return this.http.put<{ isSuccess: boolean, translator: Translator }>(`${this.domain}/${taskId}/translator/${translatorId}`, {});
    }
}
