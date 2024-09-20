import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project } from '../models/project.model';

@Injectable({
    providedIn: 'root'
})
export class ProjectService {
    http = inject(HttpClient);

    getProjects(): Observable<Project[]> {
        return this.http.get<Project[]>('Projects');
    }

    getProject(id: string): Observable<Project> {
        return this.http.get<Project>(`Projects/${id}`);
    }

    updateProject(project: Project): Observable<Project> {
        return this.http.put<Project>(`Projects/${project.id}`, project);
    }
}
