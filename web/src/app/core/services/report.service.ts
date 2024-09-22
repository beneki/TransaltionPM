import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task } from '../models/task.model';

interface ProjectCountsByMonth {
    countsFirstYear: number[];
    countsSecondYear: number[];
}

@Injectable({
    providedIn: 'root'
})
export class ReportService {
    http = inject(HttpClient);

    getProjectCountsByYears(firstYear: number, secondYear: number, status: number): Observable<ProjectCountsByMonth> {
        return this.http.get<ProjectCountsByMonth>(`Reports/getProjectCountsByYears/${firstYear}/to/${secondYear}/status/${status}`);
    }
    getProjectCountsByYearsPerTranslator(firstYear: number, secondYear: number, status: number, translatorId: string): Observable<ProjectCountsByMonth> {
        return this.http.get<ProjectCountsByMonth>(`Reports/getProjectCountsByYearsPerTranslator/${firstYear}/to/${secondYear}/status/${status}/translator/${translatorId}`);
    }
}
