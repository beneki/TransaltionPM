import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Translator } from '../models/translator.model';

@Injectable({
    providedIn: 'root'
})
export class TranslatorService {
    http = inject(HttpClient);

    getTranslators(): Observable<Translator[]> {
        return this.http.get<Translator[]>('Translators');
    }

    getTranslatorByTaskId(taskId: string): Observable<Translator> {
        return this.http.get<Translator>(`Translators/task/${taskId}`); // Make HTTP GET request to fetch tasks
    }
}
